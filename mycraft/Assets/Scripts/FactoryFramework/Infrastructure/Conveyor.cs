using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Windows;
using MyCraft;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FactoryFramework
{
	public class Conveyor : LogisticComponent, IInput, IOutput
	{
		public ConveyorData data;
		private float Length { get { return (_path == null) ? 0f : _path.GetTotalLength(); } }

		[SerializeField, SerializeReference] public IPath _path; // ? maybe should be included in serialization data
		public BeltMeshSO frameBM;
		public BeltMeshSO beltBM;
		private bool _validMesh = true;
		public bool ValidMesh { get { return _validMesh; } }

		// pool for gameobjects (models) on belts
		protected ObjectPool<Transform> beltObjectPool;
		
		public InputSocket inputSocket;
		public OutputSocket outputSocket;

		//HG_TODO: 아래에 if문 추가했는데. storage에서 belt에서 연계가 안된다.
		public string InputSocketGuid {
			get { return inputSocket.outputConnection?._logisticComponent.GUID.ToString() ?? null; }
			//set { /*if(inputSocket.outputConnection)*/ inputSocket.outputConnection._logisticComponent.GUID = Guid.Parse(value); }
		}
		public string OutputSocketGuid {
			get { return outputSocket.inputConnection?._logisticComponent.GUID.ToString() ?? null; }
			//set { /*if(outputSocket.inputConnection)*/ outputSocket.inputConnection._logisticComponent.GUID = Guid.Parse(value); }
		}
		public string tmpInputGuid;
		public string tmpOutputGuid;

		[SerializeField] private MeshFilter frameFilter;
		[SerializeField] private MeshFilter beltFilter;
		[SerializeField, HideInInspector] private MeshRenderer beltMeshRenderer;

		private int _capacity;
		public int Capacity { get { return _capacity; } }
		public List<ItemOnBelt> items = new List<ItemOnBelt>();
		//private List<Transform> _transforms = new List<Transform>();
		private ItemOnBelt LastItem { get { return items[items.Count - 1]; } }
		// private int currentLoad; 
		private Vector3 _distanceAboveBelt = Vector3.up * 0.1f;    //밸트위 아이템이 뭍히지 않게 조금 띄운다

		private ConveyorJob _conveyorJob;
		private JobHandle _jobHandle;
		private NativeArray<float> _itemPositionsArray;
		private TransformAccessArray _transformsArray;

		// events
		public UnityEvent<Conveyor> OnConveyorDestroyed;

		private void Start()
		{
			_powerGridComponent ??= GetComponent<PowerGridComponent>();

			beltMeshRenderer = beltFilter.gameObject.GetComponent<MeshRenderer>();

			_path = PathFactory.GeneratePathOfType(data.start, data.startDir, data.end, data.endDir, settings.PATHTYPE);
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

        public virtual void OnDeleted()
		{
            for (int i = 0; i < items.Count; i++)
            {
                MyCraft.Managers.Game.AddItem(this.items[i]._itembase.id, 1);
                MyCraft.Managers.Resource.Destroy(items[i]._model.gameObject);
            }
            items.Clear();
            this.Disconnect();
            MyCraft.Managers.Game.AddItem(base._itembase.id, this.Capacity);
        }

        private void OnDestroy()
		{
			for (int i = 0; i < items.Count; i++)
				MyCraft.Managers.Resource.Destroy(items[i]._model.gameObject);

			items.Clear();
			this.Disconnect();
			Reset();
			_path?.CleanUp();
			Disconnect();
			OnConveyorDestroyed?.Invoke(this);
		}


		private void Update()
		{
			ProcessLoop();

			// FIXME maybe don't need to do this every update
			beltMeshRenderer?.material.SetFloat("_Speed", data.speed);

			_IsWorking = items.Count > 0;

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

		public override void ProcessLoop()
		{
			MoveItems();
			// FIXME because IPath is not a struct
			//MoveItemsJob(); // this will be the Jobs/Burst way to do things
		}

		public void CalculateCapacity(int capacity=-1)
		{
			// this function also serves as a sort of Init()

			// capcity is a simple calculation that depends on belt length and belt_spacing
			//capacity = (_capacity == -1) ? Mathf.Max(1,Mathf.FloorToInt(Length / settings.BELT_SPACING)) : _capacity;
			_capacity = (capacity == -1) ? Mathf.FloorToInt(Length/settings.BELT_SPACING)+1 : capacity;

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
				defaultCapacity: _capacity);

			//_itemPositionsArray = new NativeArray<float>(capacity, Allocator.Persistent);
		}

		public void SetItemOnBelt(string itemPath, float position)
		{
			SetItemOnBelt(40, position);
		}
		public void SetItemOnBelt(int itemid, float position)
		{
			if (_capacity <= 0)
			{
				Debug.LogError("Tring to place new item on a belt at full capacity");
			}
			//HG[2023.06.09] Item -> MyCraft.ItemBase
			//Item item = Resources.Load<Item>(itemPath);
			MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid);//철광석
			ItemOnBelt iob = new ItemOnBelt()
			{
				_itembase = itembase,
				_position = position
			};

			Transform newItemModel = beltObjectPool.Get();
			if (newItemModel == null)
			{
				Debug.LogError($"error with {gameObject.name} no items left to grab");
			}

			//건물을 conveyor에 태우기 위해.
			if (0 < iob._itembase.prefab.transform.childCount)
			{
				newItemModel.GetComponent<MeshFilter>().sharedMesh = iob._itembase.prefab.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
				newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob._itembase.prefab.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
				newItemModel.localScale = Vector3.one * iob._itembase.scaleOnBelt;
				//newItemModel.Rotate(90f, 0, 0);	//회전이 안되는군.
			}
			else
			{
				newItemModel.GetComponent<MeshFilter>().sharedMesh = iob._itembase.prefab.GetComponent<MeshFilter>().sharedMesh;
				newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob._itembase.prefab.GetComponent<MeshRenderer>().sharedMaterial;
			}
			iob._model = newItemModel;
			items.Add(iob);
			_capacity -= 1;			
		}

		public void MoveItems()
		{
			// this can be done with Jobs
			float cumulativeMaxPos = Length;

			for (int x = 0; x < items.Count; x++)
			{
				float position = items[x]._position;
				position += data.speed * Time.deltaTime;
				position = math.clamp(position, 0f, cumulativeMaxPos);


				ItemOnBelt item = items[x];
				item._position = position;
				items[x] = item;


				Transform t = item._model;
				float pos = item._position;
				float percent = pos / _path.GetTotalLength();
				Vector3 worldPos = _path.GetWorldPointFromPathSpace(percent) + _distanceAboveBelt;
				Quaternion worldRotation = _path.GetRotationAtPoint(percent);
				t.SetPositionAndRotation(worldPos, worldRotation);


				// update max cumulative position
				cumulativeMaxPos -= 1f * settings.BELT_SPACING;
			}
		}

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
			_path?.CleanUp();
			_path = PathFactory.GeneratePathOfType(data.start, data.startDir, data.end, data.endDir, settings.PATHTYPE);

			if (!_path.IsValid)
			{
				if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to path");
				_validMesh = false;
			}
			int length = Mathf.Max(1,(int)(_path.GetTotalLength() * settings.BELT_SEGMENTS_PER_UNIT));
			//Debug.Log($"Length is {length}"); // development debug

			bool collision = PathFactory.CollisionAlongPath(_path, 0.5f, ConveyorLogisticsUtils.settings.BELT_SCALE/2f, ~0, ignored, startskip: startskip, endskip: endskip); //only collide belt collideable layer
			if (collision)
			{
				if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to collision");
				_validMesh = false;   //HG_TEST:(주석처리하면) 모든 collider의 충돌을 무시한다.
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
			

			frameFilter.mesh = BeltMeshGenerator.Generate(_path, frameBM, length, ConveyorLogisticsUtils.settings.BELT_SCALE);
			beltFilter.mesh = BeltMeshGenerator.Generate(_path, beltBM, length, ConveyorLogisticsUtils.settings.BELT_SCALE, 1f, true);

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
			//connectionTo.Connect(this.outputSocket);
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
			//connectionFrom.Connect(this.inputSocket);
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


		#region GIVE_OUTPUT
		//HG[2023.06.09] Item -> MyCraft.ItemBase
		//public bool CanGiveOutput(Item filter = null)
		//{
		//    if (filter != null) Debug.LogWarning("Conveyor Belt Does not Implement Item Filter Output");
		//    if (items.Count == 0) return false;
		//    return items[0].position == Length;
		//}
		public bool CanGiveOutput()
		{
			//if (filter != null) Debug.LogWarning("Conveyor Belt Does not Implement Item Filter Output");
			if (items.Count == 0) return false;
			return items[0]._position == Length;
		}
		//public Item OutputType()
		//{
		//    if (items.Count == 0) return null;
		//    if (items[0].position < Length) return null;
		//    return items[0].item;
		//}
		public int OutputType()
		{
			if (items.Count == 0) return 0;
			if (items[0]._position < Length) return 0;
			return items[0]._itembase.id;
		}

		//public Item GiveOutput(Item filter = null)
		//{
		//    if (!CanGiveOutput())
		//        Debug.LogError($"Belt is trying to GiveOutput when it is unable to.");
		//    if (filter != null) Debug.LogWarning("Conveyor Belt Does not Implement Item Filter Output");

		//    ItemOnBelt firstItem = items[0];
		//    // return item model to pool
		//    Transform model = firstItem.model;
		//    firstItem.model = null;
		//    beltObjectPool.Release(model);
		//    // actually remove this item
		//    items.RemoveAt(0);
		//    // add 1 to  remaining capacity
		//    _capacity += 1;
		//    return firstItem.item;
		//}
		public int GiveOutput()
		{
			if (!CanGiveOutput())
				Debug.LogError($"Belt is trying to GiveOutput when it is unable to.");
			//if (filter != null) Debug.LogWarning("Conveyor Belt Does not Implement Item Filter Output");

			ItemOnBelt firstItem = items[0];
			// return item model to pool
			Transform model = firstItem._model;
			firstItem._model = null;
			beltObjectPool.Release(model);
			// actually remove this item
			items.RemoveAt(0);
			// add 1 to  remaining capacity
			_capacity += 1;
			return firstItem._itembase.id;
		}
		//..//HG[2023.06.09] Item -> MyCraft.ItemBase

		#endregion //..GIVE_OUTPUT

		#region TAKE_INPUT
		//public bool CanTakeInput(Item item)
		//{
		//    if (items.Count == 0) return true;
		//    else if (_capacity == 0) return false;
		//    // make sure the previous item on the belt is far enough away to leave room for a new item
		//    return LastItem.position >= settings.BELT_SPACING;
		//}
		public bool CanTakeInput(int itemid)
		{
			if (0 == itemid) return false;
			if (items.Count == 0) return true;
			else if (_capacity == 0) return false;
			// make sure the previous item on the belt is far enough away to leave room for a new item
			return LastItem._position >= settings.BELT_SPACING;
		}
		//public void TakeInput(Item item)
		//{
		//    if (!CanTakeInput(item))
		//        Debug.LogError($"Belt is trying to accept input {item} when it is unable to.");


		//    ItemOnBelt iob = new ItemOnBelt()
		//    {
		//        item = item,
		//        position = 0f
		//    };

		//    Transform newItemModel = beltObjectPool.Get();
		//    if (newItemModel == null)
		//    {
		//        Debug.LogError($"error with {gameObject.name} no items left to grab");
		//    }

		//    newItemModel.GetComponent<MeshFilter>().sharedMesh = iob.item.prefab.GetComponent<MeshFilter>().sharedMesh;
		//    newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob.item.prefab.GetComponent<MeshRenderer>().sharedMaterial;
		//    iob.model = newItemModel;
		//    items.Add(iob);
		//    _capacity -= 1;
		//    return;
		//}
		public void TakeInput(int itemid)
		{
			if (!CanTakeInput(itemid))
				Debug.LogError($"Belt is trying to accept input {itemid} when it is unable to.");


			ItemOnBelt iob = new ItemOnBelt()
			{
				_itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(itemid),
				_position = 0f
			};

			Transform newItemModel = beltObjectPool.Get();
			if (newItemModel == null)
			{
				Debug.LogError($"error with {gameObject.name} no items left to grab");
			}

			//건물을 conveyor에 태우기 위해.
			if(0 < iob._itembase.prefab.transform.childCount)
			{
				newItemModel.GetComponent<MeshFilter>().sharedMesh = iob._itembase.prefab.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh;
				newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob._itembase.prefab.transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
				newItemModel.localScale = Vector3.one * iob._itembase.scaleOnBelt;
				//newItemModel.Rotate(90f, 0, 0);	//회전이 안되는군.
			}
			else
			{
				newItemModel.GetComponent<MeshFilter>().sharedMesh = iob._itembase.prefab.GetComponent<MeshFilter>().sharedMesh;
				newItemModel.GetComponent<MeshRenderer>().sharedMaterial = iob._itembase.prefab.GetComponent<MeshRenderer>().sharedMaterial;
			}
			//if((int)LAYER_TYPE.BELT_COLLIDEABLE == iob.item.prefab.layer)
			//iob.item.prefab.CompareTag("")
			iob._model = newItemModel;
			items.Add(iob);
			_capacity -= 1;
			return;
		}
		#endregion //..TAKE_INPUT

		#region SAVE
		public override void Save(BinaryWriter writer)
		{
			base.Save(writer);

			data.Save(writer);

			//items
			writer.Write(items.Count);
			for(int i=0; i < items.Count; ++i)
			{
				writer.Write(items[i]._itembase.id);
				writer.Write(items[i]._position);
				//Debug.Log($"SAVE:{i}-{items.Count}/{items[i]._itembase.id}/{items[i]._position}");
			}

			writer.Write(this.InputSocketGuid??"");
			writer.Write(this.OutputSocketGuid??"");
			//Debug.Log($"SAVE:{this.InputSocketGuid ?? ""}/{this.OutputSocketGuid ?? ""}");
		}
		public override void Load(BinaryReader reader)
		{
			base.Load(reader);

			data.Load(reader);

			this.inputSocket = this.GetComponentInChildren<InputSocket>();
			this.outputSocket = this.GetComponentInChildren<OutputSocket>();
			// draw
			this.UpdateMesh(true);
			this.AddCollider();

			//items
			int itemcount = reader.ReadInt32();
			for(int i=0; i < itemcount; ++i)
			{
				int itemid = reader.ReadInt32();
				float position = reader.ReadSingle();
				//Debug.Log($"LOAD:{i}-{itemcount}/{itemid}/{position}");
				SetItemOnBelt(itemid, position);
			}

			//this.InputSocketGuid = reader.ReadString();
			//this.OutputSocketGuid = reader.ReadString();
			this.tmpInputGuid = reader.ReadString();
			this.tmpOutputGuid = reader.ReadString();
			//Debug.Log($"LOAD:{this.tmpInputGuid}/{this.tmpOutputGuid}");
		}
		#endregion //..SAVE


#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			// doesnt matter item type
			if (!CanGiveOutput() && CanTakeInput(0))
			{
				Gizmos.color = Color.green;
			}
			else if (CanGiveOutput() && !CanTakeInput(0))
			{
				Gizmos.color = Color.red;
			}
			else if (!CanGiveOutput() && !CanTakeInput(0))
			{
				Gizmos.color = Color.yellow;
			}

			Gizmos.matrix = transform.localToWorldMatrix;
			Handles.matrix = transform.localToWorldMatrix;

			foreach (ItemOnBelt i in items)
			{
				//HG[2023.06.09] Item -> MyCraft.ItemBase
				//Gizmos.color = i.item.DebugColor;
				Gizmos.color = Color.cyan;

				float pos = i._position;
				float percent = pos / _path.GetTotalLength();
				Vector3 worldPos = _path.GetWorldPointFromPathSpace(percent) + _distanceAboveBelt;

				Gizmos.DrawWireSphere(worldPos, settings.BELT_SPACING/2f);
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

		#region SAVE
		public void Save(BinaryWriter writer)
		{
			MyCraft.Common.WriteVector3(writer, start);
			MyCraft.Common.WriteVector3(writer, end);
			MyCraft.Common.WriteVector3(writer, startDir);
			MyCraft.Common.WriteVector3(writer, endDir);
			writer.Write(speed);
			
			writer.Write(inputSocketIndex);
			writer.Write(outputSocketIndex);
			//Debug.Log($"SAVE(conveyor):{speed}/{inputSocketIndex}/{outputSocketIndex}");

		}
		public void Load(BinaryReader reader)
		{
			start		= MyCraft.Common.ReadVector3(reader);
			end			= MyCraft.Common.ReadVector3(reader);
			startDir	= MyCraft.Common.ReadVector3(reader);
			endDir		= MyCraft.Common.ReadVector3(reader);
			speed		= reader.ReadSingle();

			inputSocketIndex	= reader.ReadInt32();
			outputSocketIndex	= reader.ReadInt32();
			//Debug.Log($"LOAD(conveyor):{speed}/{inputSocketIndex}/{outputSocketIndex}");
		}
		#endregion //..SAVE

	}
}