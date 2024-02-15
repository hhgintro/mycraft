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
using Dreamteck.Splines;
using Dreamteck.Splines.Examples;
using MyCraft;


//using MyCraft;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FactoryFramework
{
	public class Rail : LogisticComponent
	{
		//public ConveyorData data;
		//private float Length { get { return (_path == null) ? 0f : _path.GetTotalLength(); } }

		//[SerializeField, SerializeReference] public IPath _path; // ? maybe should be included in serialization data
		//public BeltMeshSO frameBM;
		//public BeltMeshSO beltBM;
		//private bool _validMesh = true;
		//public bool ValidMesh { get { return _validMesh; } }

		//// pool for gameobjects (models) on belts
		//[SerializeField]
		//protected ObjectPool<Transform> beltObjectPool;
		
		//public InputSocket inputSocket;
		//public OutputSocket outputSocket;

		////HG_TODO: 아래에 if문 추가했는데. storage에서 belt에서 연계가 안된다.
		//public string InputSocketGuid {
		//	get { return inputSocket.outputConnection?._logisticComponent.GUID.ToString() ?? null; }
		//	//set { /*if(inputSocket.outputConnection)*/ inputSocket.outputConnection._logisticComponent.GUID = Guid.Parse(value); }
		//}
		//public string OutputSocketGuid {
		//	get { return outputSocket.inputConnection?._logisticComponent.GUID.ToString() ?? null; }
		//	//set { /*if(outputSocket.inputConnection)*/ outputSocket.inputConnection._logisticComponent.GUID = Guid.Parse(value); }
		//}
		//public string tmpInputGuid;
		//public string tmpOutputGuid;

		//[SerializeField] private MeshFilter frameFilter;
		//[SerializeField] private MeshFilter beltFilter;
		//[SerializeField, HideInInspector] private MeshRenderer beltMeshRenderer;

		//private int _capacity;
		//public int Capacity { get { return _capacity; } }
		//public List<ItemOnBelt> items = new List<ItemOnBelt>();
		////private List<Transform> _transforms = new List<Transform>();
		//private ItemOnBelt LastItem { get { return items[items.Count - 1]; } }
		//// private int currentLoad; 
		//private Vector3 _distanceAboveBelt = Vector3.up * 0.1f;    //밸트위 아이템이 뭍히지 않게 조금 띄운다

		//private ConveyorJob _conveyorJob;
		//private JobHandle _jobHandle;
		//private NativeArray<float> _itemPositionsArray;
		//private TransformAccessArray _transformsArray;

		//// events
		////public UnityEvent<Conveyor> OnConveyorDestroyed;

		public override void fnStart()
		{
			//_powerGridComponent ??= GetComponent<PowerGridComponent>();

			//beltMeshRenderer = beltFilter.gameObject.GetComponent<MeshRenderer>();

			////_path = PathFactory.GeneratePathOfType(data.start, data.startDir, data.end, data.endDir, settings.PATHTYPE);
			////CalculateCapacity();
		}

		public void Reset()
		{
			Debug.Log("Rail RESET을 (종료시에)호출합니다.");
			//frameFilter.sharedMesh?.Clear();
			//beltFilter.sharedMesh?.Clear();

			//if (Application.isEditor && Application.isPlaying)
			//{
			//	Destroy(frameFilter.sharedMesh);
			//	Destroy(beltFilter.sharedMesh);
			//}
			//else
			//{
			//	DestroyImmediate(frameFilter.sharedMesh);
			//	DestroyImmediate(beltFilter.sharedMesh);
			//}
		}

		//파괴될 때.(bReturn:true이면 인벤으로 회수, 게임재시작할때에는 false이어야 합니다.)
		public override void OnDeleted(bool bReturn)
		{
			//base.GUID = Guid.Empty; //GUID중복:save파일과 Mem에 동일할 GUID가 존재한다.
			//Disconnect();
			////Load할때 Reset()을 호출하지 않는다.(frameFilter/beltFilter에서 오류확인됨)
			////if(false == bReturn) Reset();	//Destroy에서 호출될때에 실행한다.
			////_path?.CleanUp();		//Load할떄 중복으로 처리되어 문제가 있는거 같다.

			////ItemOnBelt 회수
			//for (int i = 0; i < items.Count; i++)
			//{
			//	if (bReturn) MyCraft.Managers.Game.AddItem(this.items[i]._itembase.id, 1, MyCraft.Global.FILLAMOUNT_DEFAULT);
			//	MyCraft.Managers.Resource.Destroy(items[i]._model.gameObject);
			//}
			//items.Clear();
			//beltObjectPool?.Clear();

			////conveyor(자신) 회수
			//if (bReturn) MyCraft.Managers.Game.AddItem(base._itembase.id, this.Capacity, MyCraft.Global.FILLAMOUNT_DEFAULT);
			////OnConveyorDestroyed?.Invoke(this);
		}

		//설치전에는 collider를 disable 시켜둔다.(카메라 왔다갔다 현상)
		public override void SetEnable_2(bool enable)
		{
			////this.enabled = enable;
			////MeshCollider는 enable이 true일 때 추가(AddCollider())되므로 여기에서 처리할 필요없다.
			////this.transform.GetChild(0).GetComponent<MeshCollider>().enabled = enable;

			//inputSocket.GetComponent<BoxCollider>().enabled = enable;
			//outputSocket.GetComponent<BoxCollider>().enabled = enable;
		}

		#region MESH_AND_VISUALS
		//public void UpdateMesh(bool finalize = false, Collider[] ignored = null, int startskip = 0, int endskip = 0)
		//{
  //          _validMesh = true;
  //          _path?.CleanUp();
  //          _path = PathFactory.GeneratePathOfType(data.start, data.startDir, data.end, data.endDir, settings.PATHTYPE);

  //          if (!_path.IsValid)
		//	{
		//		if (settings.SHOW_DEBUG_LOGS) Debug.Log("Invalid Conveyor due to path");
		//		_validMesh = false;
		//	}
		//	//0.05f: _path의 길이가 0.9999로 찍히는 경우에 대한 보정값
  //          int length = Math.Max(1,(int)(_path.GetTotalLength() * settings.BELT_SEGMENTS_PER_UNIT + 0.05f));
		//	//Debug.Log($"Conveyor Length is {length}"); // development debug

		//	bool collision = PathFactory.CollisionAlongPath(_path, 0.5f, ConveyorLogisticsUtils.settings.BELT_SCALE/2f, ~0, ignored, startskip, endskip); //only collide belt collideable layer
		//	if (collision)
		//	{
		//		//충돌인지를 위해 _validMesh 설정은 그대로 두고, 오류메시지는 노출할 필요없다.
		//		//if (settings.SHOW_DEBUG_LOGS) Debug.LogError("Invalid Conveyor due to collision");
		//		_validMesh = false;   //HG_TEST:(주석처리하면) 모든 collider의 충돌을 무시한다.
		//	}

		//	frameFilter.sharedMesh?.Clear();
		//	beltFilter.sharedMesh?.Clear();

		//	if (Application.isEditor && Application.isPlaying)
		//	{
		//		//Destroy(frameFilter.sharedMesh);
		//		//Destroy(beltFilter.sharedMesh);
		//		DestroyImmediate(frameFilter.sharedMesh, true);
		//		DestroyImmediate(beltFilter.sharedMesh, true);
		//	}
		//	else
		//	{
		//		DestroyImmediate(frameFilter.sharedMesh);
		//		DestroyImmediate(beltFilter.sharedMesh);
		//	}
			

		//	frameFilter.mesh = BeltMeshGenerator.Generate(_path, frameBM, length, ConveyorLogisticsUtils.settings.BELT_SCALE);
		//	beltFilter.mesh = BeltMeshGenerator.Generate(_path, beltBM, length, ConveyorLogisticsUtils.settings.BELT_SCALE, 1f, true);

		//	beltMeshRenderer = beltFilter.gameObject.GetComponent<MeshRenderer>();

		//	// position the sockets!
		//	inputSocket.transform.position = data.start;
		//	inputSocket.transform.forward = data.startDir;
		//	outputSocket.transform.position = data.end;
		//	outputSocket.transform.forward = data.endDir;

		//	if (finalize) 
		//		CalculateCapacity();
		//}

		public override void SetMaterials(Material frameMat, Material beltMat = null)
		{
			////conveyor는 ItemOnBelt때문에 base.SetMaterials()를 사용할 수 없습니다.

			//frameFilter.gameObject.GetComponent<MeshRenderer>().material = frameMat;
			//beltFilter.gameObject.GetComponent<MeshRenderer>().material = beltMat;
		}
		public void AddCollider()
		{
			////Debug.Log($"문제가있는 {this.GUID}/{data.start}/{data.startDir}/{data.end}/{data.endDir}/{data.speed}/{data.inputSocketIndex}/{data.outputSocketIndex}");
			////frameFilter.gameObject.AddComponent<MeshCollider>();
			////beltFilter.gameObject.AddComponent<MeshCollider>();

			//var frame = frameFilter.gameObject.GetComponent<MeshCollider>();
			//if (frame)
			//{
			//	Debug.Log("frame collider 삭제후 재생성");
			//	//다시불러오기시에 frame의 MeshCollider의 Mesh가 깨져서 지우고 다시생성하고 있다.
			//	//HG_TODO: 다시 세팅해줄수 있는지 체크할 것
			//	Destroy(frame);
			//}
			//frameFilter.gameObject.AddComponent(typeof(MeshCollider));
			//var belt = beltFilter.gameObject.GetComponent<MeshCollider>();
			//if (belt)
			//{
			//	Debug.Log("belt collider 삭제후 재생성");
   //             //다시불러오기시에 belt의 MeshCollider의 Mesh가 깨져서 지우고 다시생성하고 있다.
   //             //HG_TODO: 다시 세팅해줄수 있는지 체크할 것
   //             Destroy(belt);
			//}
			//beltFilter.gameObject.AddComponent(typeof(MeshCollider));

			////if (null == frameFilter.gameObject.GetComponent<MeshCollider>())
			////	frameFilter.gameObject.AddComponent(typeof(MeshCollider));
			////if (null == beltFilter.gameObject.GetComponent<MeshCollider>())
			////	beltFilter.gameObject.AddComponent(typeof(MeshCollider));
		}

#if ITEM_MESH_ON_BELT  //override GetSharedMesh()                             
		public override Mesh GetSharedMesh()
		{
			//Vector3 dir = Vector3.right;
			//data.start = Vector3.zero - dir * 0.5f;
			//data.startDir = dir;
			//data.end = data.start + dir * 0.5f;
			//data.endDir = data.startDir;

			////ignore colliders from start and end points
			//List<Collider> collidersToIgnore = new List<Collider>();
			////// add colliders associated with the connected start socket
			////if (startSocket != null)
			////{
			////	collidersToIgnore.AddRange(startSocket.transform.root.GetComponentsInChildren<Collider>());
			////	collidersToIgnore.Remove(startSocket.transform.root.GetComponent<Collider>());
			////}
			////else
			////{
			////	collidersToIgnore.Add(Terrain.activeTerrain.GetComponent<TerrainCollider>());
			////}
			////if (collidersToIgnore.Count > 0)
			//this.UpdateMesh(ignored: collidersToIgnore.ToArray(), startskip: 1, endskip: 1);

			return this.transform.GetChild(1).GetComponent<MeshFilter>().sharedMesh;
		}
		public override Material GetSharedMaterial() { return this.transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial; }
		//public virtual float GetLocalScale() { return this._itembase.scaleOnBelt; }
#else
#endif //..ITEM_MESH_ON_BELT

		#endregion

		#region SAVE
		public override void Save(BinaryWriter writer)
		{
			base.Save(writer);

			//data.Save(writer);

			////items
			//writer.Write(items.Count);
			//for(int i=0; i < items.Count; ++i)
			//{
			//	writer.Write(items[i]._itembase.id);
			//	writer.Write(items[i]._position);
			//	//Debug.Log($"SAVE:{i}-{items.Count}/{items[i]._itembase.id}/{items[i]._position}");
			//}

			//writer.Write(this.InputSocketGuid??"");
			//writer.Write(this.OutputSocketGuid??"");
			////Debug.Log($"SAVE:{this.InputSocketGuid ?? ""}/{this.OutputSocketGuid ?? ""}");
		}
		public override void Load(BinaryReader reader)
		{
			base.Load(reader);
   //         data.Load(reader);

   //         this.inputSocket = this.GetComponentInChildren<InputSocket>();
			//this.outputSocket = this.GetComponentInChildren<OutputSocket>();
   //         //Debug.Log($" + before: {this.GUID}");	//테스트용:에러가발생할때 체크(before,after)
   //         //if(Guid.Parse("3ee88e14-db2d-4a59-baa3-e055a9a89cb8") == this.GUID)
   //         //{
   //         //	int a = 0;
   //         //	a = 0;
   //         //}

   //         // draw
   //         this.UpdateMesh(true);
   //         this.AddCollider();

   //         //items
   //         int itemcount = reader.ReadInt32();
   //         for (int i = 0; i < itemcount; ++i)
			//{
			//	int itemid = reader.ReadInt32();
			//	float position = reader.ReadSingle();
			//	////Debug.Log($"LOAD:{i}-{itemcount}/{itemid}/{position}");
			//	//HGK_TEST: 디버깅중
			//	SetItemOnBelt(itemid, position);
			//}

   //         //this.InputSocketGuid = reader.ReadString();
   //         //this.OutputSocketGuid = reader.ReadString();
   //         this.tmpInputGuid = reader.ReadString();
			//this.tmpOutputGuid = reader.ReadString();
   //         //Debug.Log($"LOAD:{this.tmpInputGuid}/{this.tmpOutputGuid}");
   //         //this.SetEnable(true);
   //         //Debug.Log($" +++ after: {this.GUID}");	//테스트용:에러가발생할때 체크(before,after)
		}
		#endregion //..SAVE

	}
}