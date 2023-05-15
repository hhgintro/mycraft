using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FactoryFramework
{
    public class Conveyor : LogisticComponent, IInput, IOutput
    {
        public ConveyorData data;
        private float Length { get { return (p == null) ? 0f : p.GetTotalLength(); } }

        [SerializeField, SerializeReference] public IPath p; // ? maybe should be included in serialization data
        public BeltMeshSO frameBM;
        public BeltMeshSO beltBM;
        private bool _validMesh = true;
        public bool ValidMesh { get { return _validMesh; } }

        // pool for gameobjects (models) on belts
        protected ObjectPool<Transform> beltObjectPool;
        
        public InputSocket inputSocket;
        public OutputSocket outputSocket;

        public string InputSocketGuid { get { return inputSocket.outputConnection?._logisticComponent.GUID.ToString() ?? null; } }
        public string OutputSocketGuid { get { return outputSocket.inputConnection?._logisticComponent.GUID.ToString() ?? null; } }

        [SerializeField] private MeshFilter frameFilter;
        [SerializeField] private MeshFilter beltFilter;
        [SerializeField, HideInInspector] private MeshRenderer beltMeshRenderer;

        private int capacity;
        public int Capacity { get { return capacity; } }
        public List<ItemOnBelt> items = new List<ItemOnBelt>();
        //private List<Transform> _transforms = new List<Transform>();
        private ItemOnBelt LastItem { get { return items[items.Count - 1]; } }
        // private int currentLoad; 

        private ConveyorJob _conveyorJob;
        private JobHandle _jobHandle;
        private NativeArray<float> _itemPositionsArray;
        private TransformAccessArray _transformsArray;

        // events
        public UnityEvent<Conveyor> OnConveyorDestroyed;

        private void Awake()
        {
            _powerGridComponent ??= GetComponent<PowerGridComponent>();

            beltMeshRenderer = beltFilter.gameObject.GetComponent<MeshRenderer>();

            p = PathFactory.GeneratePathOfType(data.start, data.startDir, data.end, data.endDir, settings.PATHTYPE);
            CalculateCapacity();
        }

        public void Reset()
        {
            frameFilter.sharedMesh?.Clear();
            beltFilter.sharedMesh?.Clear();

            if (Application.isEditor && Application.isPlaying)
            {
                Destroy(frameFilter.sharedMesh);
                Destroy(beltFilter.sharedMesh);
            }
            else
            {
                DestroyImmediate(frameFilter.sharedMesh);
                DestroyImmediate(beltFilter.sharedMesh);
            }

            beltObjectPool?.Clear();
            items?.Clear();
        }

        private void OnDestroy()
        {
            //_itemPositionsArray.Dispose();
            //_transformsArray.Dispose();
            for (int x = 0; x < items.Count; x++)
            {
                Destroy(items[0].model.gameObject);
            }
            items.Clear();
            Reset();
            p?.CleanUp();
            Disconnect();
            OnConveyorDestroyed?.Invoke(this);
        }

        public void CalculateCapacity(int _capacity=-1)
        {
            // this function also serves as a sort of Init()

            // capcity is a simple calculation that depends on belt length and belt_spacing
            capacity = (_capacity == -1) ? Mathf.Max(1,Mathf.FloorToInt(Length / settings.BELT_SPACING)) : _capacity;

            // create a pool of gameobjects with rendering capabilities
            // pool => CreateFunc, DestroyFunc, GetFunc, ReleaseFunc, capacity
            beltObjectPool?.Clear();
            beltObjectPool = new ObjectPool<Transform>(
                createFunc:() => { 
                    GameObject item = new GameObject();
                    item.name = "PooledItem";
                    item.transform.parent = transform;
                    item.AddComponent<MeshFilter>();
                    item.AddComponent<MeshRenderer>();
                    return item.transform;
                },
                actionOnGet:(Transform t) => { t.gameObject.SetActive(true); },
                actionOnRelease:(Transform t) => { t.gameObject.SetActive(false); },
                actionOnDestroy:(Transform t) => { Destroy(t.gameObject); },
                defaultCapacity: capacity);

            //_itemPositionsArray = new NativeArray<float>(capacity, Allocator.Persistent);
        }

        #region INPUTS_AND_OUTPUTS
        public bool CanGiveOutput(Item filter = null)
        {
            if (filter != null) Debug.LogWarning("Conveyor Belt Does not Implement Item Filter Output");
            if (items.Count == 0) return false;
            return items[0].position == Length;
        }
        public bool CanTakeInput(Item item)
        {
            if (items.Count == 0) return true;
            else if (capacity == 0) return false;
            // make sure the previous item on the belt is far enough away to leave room for a new item
            return LastItem.position >= settings.BELT_SPACING;
        }
        public void TakeInput(Item item)
        {
            if (!CanTakeInput(item))
                Debug.LogError($"Belt is trying to accept input {item} when it is unable to.");
            
          
            ItemOnBelt iob = new ItemOnBelt()
            {
                item = item,
                position = 0f
            };
         
            Transform newItemModel = beltObjectPool.Get();
            if (newItemModel == null)
            {
                Debug.LogError($"error with {gameObject.name} no items left to grab");
            }

            newItemModel.GetComponent<MeshFilter>().sharedMesh = iob.item.prefab.GetComponent<MeshFilter>().sharedMesh;
            newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob.item.prefab.GetComponent<MeshRenderer>().sharedMaterial;
            iob.model = newItemModel;
            items.Add(iob);
            capacity -= 1;
            return;
        }

        public Item OutputType()
        {
            if (items.Count == 0) return null;
            if (items[0].position < Length) return null;
            return items[0].item;
        }
        public Item GiveOutput(Item filter = null)
        {
            if (!CanGiveOutput())
                Debug.LogError($"Belt is trying to GiveOutput when it is unable to.");
            if (filter != null) Debug.LogWarning("Conveyor Belt Does not Implement Item Filter Output");

            ItemOnBelt firstItem = items[0];
            // return item model to pool
            Transform model = firstItem.model;
            firstItem.model = null;
            beltObjectPool.Release(model);
            // actually remove this item
            items.RemoveAt(0);
            // add 1 to  remaining capacity
            capacity += 1;
            return firstItem.item;
        }
        #endregion

        public override void ProcessLoop()
        {
            MoveItems();
            // FIXME because IPath is not a struct
            //MoveItemsJob(); // this will be the Jobs/Burst way to do things
        }

        public void SetItemOnBelt(string itemPath, float position)
        {
            if (capacity <= 0)
            {
                Debug.LogError("Tring to place new item on a belt at full capacity");
            }
            Item item = Resources.Load<Item>(itemPath);
            ItemOnBelt iob = new ItemOnBelt()
            {
                item = item,
                position = position
            };

            Transform newItemModel = beltObjectPool.Get();
            if (newItemModel == null)
            {
                Debug.LogError($"error with {gameObject.name} no items left to grab");
            }

            newItemModel.GetComponent<MeshFilter>().sharedMesh = iob.item.prefab.GetComponent<MeshFilter>().sharedMesh;
            newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob.item.prefab.GetComponent<MeshRenderer>().sharedMaterial;
            iob.model = newItemModel;
            items.Add(iob);
            capacity -= 1;
            
        }

        public void MoveItems()
        {
            // this can be done with Jobs
            float cumulativeMaxPos = Length;

            for (int x = 0; x < items.Count; x++)
            {
                float position = items[x].position;
                position += data.speed * Time.deltaTime;
                position = math.clamp(position, 0f, cumulativeMaxPos);


                ItemOnBelt item = items[x];
                item.position = position;
                items[x] = item;


                Transform t = item.model;
                float pos = item.position;
                float percent = pos / p.GetTotalLength();
                Vector3 worldPos = p.GetWorldPointFromPathSpace(percent);
                Quaternion worldRotation = p.GetRotationAtPoint(percent);
                t.SetPositionAndRotation(worldPos, worldRotation);
                

                // update max cumulative position
                cumulativeMaxPos -= 1f * settings.BELT_SPACING;
            }
        }

        private void Update()
        { 
            ProcessLoop();

            // FIXME maybe don't need to do this every update
            beltMeshRenderer?.material.SetFloat("_Speed", data.speed);

            IsWorking = items.Count > 0;

        }

        //private void LateUpdate()
        //{
        //    _jobHandle.Complete();

        //    for (int i = 0; i < _conveyorJob.itemPositions.Length; i++)
        //    {
        //        ItemOnBelt iob = items[i];
        //        iob.position = _conveyorJob.itemPositions[i];
        //        items[i] = iob;
        //    }
        //}

        //void MoveItemsJob()
        //{
        //    if (items.Count == 0) return;


        //    Transform[] _transforms = new Transform[items.Count];
        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        _transforms[i] = items[i].model;
        //        _itemPositionsArray[i] = items[i].position;
        //    }
        //    _transformsArray = new TransformAccessArray(_transforms);


        //    _conveyorJob = new ConveyorJob()
        //    {
        //        //path=p,
        //        itemPositions = _itemPositionsArray,
        //        speed = data.speed,
        //        spacing = settings.BELT_SPACING,
        //        length = Length,
        //        deltatime = Time.deltaTime
        //    };
        //    _jobHandle = _conveyorJob.Schedule(_transformsArray);
        //}

        #region MESH_AND_VISUALS
        public void UpdateMesh(bool finalize = false, Collider[] ignored = null, int startskip = 0, int endskip = 0)
        {
            _validMesh = true;
            p?.CleanUp();
            p = PathFactory.GeneratePathOfType(data.start, data.startDir, data.end, data.endDir, settings.PATHTYPE);

            if (!p.IsValid)
            {
                if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to path");
                _validMesh = false;
            }
            int length = Mathf.Max(1,(int)(p.GetTotalLength() * settings.BELT_SEGMENTS_PER_UNIT));
            //Debug.Log($"Length is {length}"); // development debug

            bool collision = PathFactory.CollisionAlongPath(p, 0.5f, ConveyorLogisticsUtils.settings.BELT_SCALE/2f, ~0, ignored, startskip: startskip, endskip: endskip); //only collide belt collideable layer
            if (collision)
            {
                if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to collision");
                _validMesh = false;
            }

            frameFilter.sharedMesh?.Clear();
            beltFilter.sharedMesh?.Clear();

            if (Application.isEditor && Application.isPlaying)
            {
                Destroy(frameFilter.sharedMesh);
                Destroy(beltFilter.sharedMesh);
            } else
            {
                DestroyImmediate(frameFilter.sharedMesh);
                DestroyImmediate(beltFilter.sharedMesh);
            }
            

            frameFilter.mesh = BeltMeshGenerator.Generate(p, frameBM, length, ConveyorLogisticsUtils.settings.BELT_SCALE);
            beltFilter.mesh = BeltMeshGenerator.Generate(p, beltBM, length, ConveyorLogisticsUtils.settings.BELT_SCALE, 1f, true);

            beltMeshRenderer = beltFilter.gameObject.GetComponent<MeshRenderer>();

            // position the sockets!
            inputSocket.transform.position = data.start;
            inputSocket.transform.forward = data.startDir;
            outputSocket.transform.position = data.end;
            outputSocket.transform.forward = data.endDir;

            if (finalize) 
                CalculateCapacity();
        }

        public void SetMaterials(Material frameMat, Material beltMat)
        {
            frameFilter.gameObject.GetComponent<MeshRenderer>().material = frameMat;
            beltFilter.gameObject.GetComponent<MeshRenderer>().material = beltMat;
        }
        public void AddCollider()
        {
            frameFilter.gameObject.AddComponent(typeof(MeshCollider));
            beltFilter.gameObject.AddComponent(typeof(MeshCollider));
        }
#endregion


        #region SOCKETS
        public void ConnectToInput(InputSocket connectionTo)
        {
            this.outputSocket.Connect(connectionTo);
            if (connectionTo._logisticComponent is Building)
            {
                Building b = connectionTo._logisticComponent as Building;
                this.data.inputSocketIndex = b.GetInputIndexBySocket(connectionTo);
            } else
            {
                this.data.inputSocketIndex = 0;
            }
            this.data.end = connectionTo.transform.position;
            this.data.endDir = connectionTo.transform.forward;
        }
        public void ConnectToOutput(OutputSocket connectionFrom)
        {
            this.inputSocket.Connect(connectionFrom);
            if (connectionFrom._logisticComponent is Building)
            {
                Building b = connectionFrom._logisticComponent as Building;
                this.data.outputSocketIndex = b.GetOutputIndexBySocket(connectionFrom);
            }
            else
            {
                this.data.outputSocketIndex = 0;
            }
            this.data.start = connectionFrom.transform.position;
            this.data.startDir = connectionFrom.transform.forward;
        }
        public void Disconnect()
        {
            this.inputSocket.outputConnection?.Disconnect();
            this.outputSocket.inputConnection?.Disconnect();

            this.inputSocket.Disconnect();
            this.outputSocket.Disconnect();
        }
        
        #endregion


        #region Connect and Disconnect Helpers
        public void Connect(LogisticComponent obj)
        {
            throw new System.NotImplementedException();
        }

        public void Disconnect(LogisticComponent obj)
        {
            throw new System.NotImplementedException();
        }
        #endregion

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // doesnt matter item type
            if (!CanGiveOutput() && CanTakeInput(null))
            {
                Gizmos.color = Color.green;
            }
            else if (CanGiveOutput() && !CanTakeInput(null))
            {
                Gizmos.color = Color.red;
            }
            else if (!CanGiveOutput() && !CanTakeInput(null))
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            Handles.matrix = transform.localToWorldMatrix;

            foreach (ItemOnBelt i in items)
            {
                Gizmos.color = i.item.DebugColor;

                float pos = i.position;
                float percent = pos / p.GetTotalLength();
                Vector3 worldPos = p.GetWorldPointFromPathSpace(percent);

                Gizmos.DrawWireSphere(worldPos, settings.BELT_SPACING / 2f);

            }
        }
#endif
    }

    [BurstCompile]
    public struct ConveyorJob : IJobParallelForTransform
    {
        public NativeArray<float> itemPositions;
        //public IPath path;
        public float speed;
        public float spacing;
        public float length;
        public float deltatime;

        public void Execute(int index, TransformAccess transform)
        {
            float maxPos = length - ((float)index * spacing);
            float position = itemPositions[index];
            position = math.clamp(position + speed * deltatime, 0f, maxPos);
            itemPositions[index] = position;

            //float percent = position / path.GetTotalLength();
            //Vector3 worldPos = path.GetWorldPointFromPathSpace(percent);
            //Quaternion worldRotation = path.GetRotationAtPoint(percent);
            //transform.position = worldPos;
            //transform.rotation = worldRotation;
        }
    }
    [System.Serializable]
    public struct ConveyorData
    {
        public Vector3 start;
        public Vector3 end;
        public Vector3 startDir;
        public Vector3 endDir;
        public float speed;

        //public InputSocket inputSocket;
        public int inputSocketIndex;
        //public OutputSocket outputSocket;
        public int outputSocketIndex;
    }
}