using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryFramework;
//using MyCraft;
using UnityEngine.EventSystems;
using Unity.Burst.CompilerServices;

public class BuildingPlacement : IPlacement
{
	[Header("Event Channels to Handle State")]
	public VoidEventChannel_SO startPlacementEvent;
	public VoidEventChannel_SO finishPlacementEvent;
	public VoidEventChannel_SO cancelPlacementEvent;
	
	//[Header("Building Prefabs")]
	//public GameObject Miner;
	//public GameObject Processor;
	//public GameObject Factory;
	//public GameObject Storage;
	//public GameObject Splitter;
	//public GameObject Merger;
	//public GameObject Assembler;
	private GameObject current; //설치하기전 잡고있는 building.
#if UNITY_STANDALONE
	private static int index=0;//debug용 
#endif//..UNITY_STANDALONE

	[Header("Visual Feedback Building Materials")]
	public Material originalMaterial;
	public Material greenPlacementMaterial;
	public Material redPlacementMaterial;

	// list of valid materials so we don't change any other materials
	private Material[] validMaterials { get { return new Material[3] { originalMaterial, greenPlacementMaterial, redPlacementMaterial }; } }

	[Header("Controls")]
	public KeyCode CancelKey = KeyCode.Escape;

	private enum State
	{
		None,
		PlaceBuilding,		//설치위치 선정
		RotateBuilding		//방향 선정
	}
	private State state;
	private bool RequiresResourceDepoist = false;

	// building placement variables to track
	private Vector3 mouseDownPos;
	private float mouseHeldTime = 0f;
	private float secondsHoldToRotate = .333f;

	// we'll want to trigger connect whenever we place a new thing nearby
	private List<PowerGridComponent> aoePowerConnections = new List<PowerGridComponent>();

	private void Start()
	{
		MyCraft.Managers.Input.KeyAction -= OnKeyDown_BuildingPlacement;
		MyCraft.Managers.Input.KeyAction += OnKeyDown_BuildingPlacement;

		MyCraft.Managers.Input.MouseAction -= OnMouseEvent;
		MyCraft.Managers.Input.MouseAction += OnMouseEvent;
	}
	private void OnEnable()
	{
		// listen to the cancel event to force cancel placement from elsewhere in the code
		cancelPlacementEvent.OnEvent += ForceCancel;
	}
	private void OnDisable()
	{
		// stop listening
		cancelPlacementEvent.OnEvent -= ForceCancel;
	}

	public void ForceCancel()
	{
		if (current != null)
			MyCraft.Managers.Resource.Destroy(current.gameObject);

		current = null;
		this.state = State.None;
	}

	//public void PlaceMiner()        => StartPlacingBuilding(Miner, true);
	//public void PlaceProcessor()    => StartPlacingBuilding(Processor);
	//public void PlaceFactory()      => StartPlacingBuilding(Factory);
	//public void PlaceAssembler()    => StartPlacingBuilding(Assembler);
	//public void PlaceStorage()      => StartPlacingBuilding(Storage);
	//public void PlaceSplitter()     => StartPlacingBuilding(Splitter);
	//public void PlaceMerger()       => StartPlacingBuilding(Merger);

	//requireDeposit: 자원위에 설치가능
	public GameObject StartPlacingBuilding(GameObject prefab, bool requireDeposit=false)
	{
		cancelPlacementEvent?.Raise();
		RequiresResourceDepoist = requireDeposit;
		// spawn a prefab and start placement
		if (!TryChangeState(State.PlaceBuilding))
			return null;

		//current = Instantiate(prefab);
		current = MyCraft.Managers.Resource.Instantiate(prefab);    //HG[2023.06.01]테스트필요
		current.transform.parent = this.transform.Find($"Pool_{prefab.name}") ?? (new GameObject($"Pool_{prefab.name}") { transform = { parent = this.transform } }).transform;
#if UNITY_STANDALONE
		if(false == current.name.Contains("_"))
			current.name += string.Format($"_{(++index).ToString()}");
#endif//..UNITY_STANDALONE

		// don't let building "work" until placement is finished
		if (current.TryGetComponent(out Building b))
		{
			//b.enabled = false;
			base.SetEnable_1(b, false);
		}
		if (current.TryGetComponent(out PowerGridComponent pgc))
		{
			pgc.enabled = false;
		}

		// init material to ghost
		ChangeMatrerial(greenPlacementMaterial);
		return current;
	}

	private void ChangeMatrerial(Material mat)
	{
		foreach (MeshRenderer mr in current?.GetComponentsInChildren<MeshRenderer>())
		{
			// dont change materials that shouldn't be changed!
			if (validMaterials.Contains(mr.sharedMaterial))
				mr.sharedMaterial = mat;
		}
	}

	public override void DestroyBuilding(GameObject target)
	{
		if (false == target.TryGetComponent<Building>(out Building building))
			return;

		foreach (Socket socket in building.gameObject.GetComponentsInChildren<Socket>())
		{
			// FIXME remove this from sockets
		}

		//Debug.Log($"{building.name} 철거");
		building.OnDeleted();
		MyCraft.Managers.Resource.Destroy(building.gameObject);
	}

	//private void HandleIdleState()
	//{}

	//건물이 위치할 곳을 찾고 있을때
	private void HandlePlaceBuildingState()
	{
		// move building with mouse pos
		Vector3 groundPos = transform.position;
		Vector3 groundDir = transform.forward;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, 100).OrderBy(h => h.distance).ToArray();
		//RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
		foreach (RaycastHit hit in hits)
		{
			//자신은...무시
			if (current == hit.collider.gameObject) continue;
			if (false == current.TryGetComponent(out Building socket)) continue;

			//============================================
			//	위치보정(socket에 의한)
			if (true == current.GetComponent<Building>().LocationCorrectForSocket(hit, ref groundPos, ref groundDir))
				break;
			//if (current.transform.tag == "Safe-Footing")
			//{
			//	if (hit.collider.gameObject.TryGetComponent(out Socket socket))
			//	{
			//		if (current == socket._logisticComponent.gameObject) continue; //자신의 socket이면 무시
			//		if (socket.IsOpen())
			//		{
			//			groundPos = socket._logisticComponent.transform.position
			//				+ socket.transform.forward * current.transform.localScale.z;
			//			groundDir = socket.transform.forward;
			//			break;
			//		}
			//	}
			//}
			//============================================

			//Debug.Log($"tag:{hit.transform.tag}");
			if (hit.transform.tag == "Safe-Footing")  //안전발판
			{
				//안전발판위에 안전발판(current) 설치 위치
				if (current.transform.tag == "Safe-Footing")	groundPos = hit.point + Vector3.up * 4f;
				//안전발판위에 건물(current) 설치 위치
				else											groundPos = hit.point;// + Vector3.up * 0.5f;
				break;
			}

			// this will only place buildings on terrain. feel free to change this!
			if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
			{
				//Terrain위에 안전발판(current) 설치 위치
				if (current.transform.tag == "Safe-Footing")	groundPos = hit.point + Vector3.up * 2.5f;
				//Terrain위에 건물(current) 설치 위치
				else											groundPos = hit.point + Vector3.up;
			}
		}


		current.transform.position = groundPos;
		current.transform.forward = groundDir;
		bool valid = ValidLocation();
		// left mouse button to try to place building
		if (Input.GetMouseButtonDown(0) && valid)
		{
			// try to change state to rotate the building
			if (TryChangeState(State.RotateBuilding))
				mouseDownPos = groundPos;
		}

	}
	//건물의 방향을 결정할때
	private void HandleRotateBuildingState()
	{
		// wait for mouse to be held for X seconds until building rotation is allowed
		// this prevents quick clicks resulting in seemingly random building rotations
		mouseHeldTime += Time.deltaTime;
		if (mouseHeldTime > secondsHoldToRotate)
		{
			bool valid = ValidLocation();
			// get new ground position to rotate towards
			Vector3 dir = current.transform.forward;
			// rotate the building!
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
			{
				// thios demo script will only use a terrain object as the "ground"
				if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
					current.transform.forward = (mouseDownPos - new Vector3(hit.point.x,mouseDownPos.y,hit.point.z)).normalized;
			}
			current.transform.position = mouseDownPos;
		}

		if (Input.GetMouseButtonUp(0))
		{
			TryChangeState(State.None);
			//손에 들고있는 아이템의 수량이 남아 있다면, prefab을 생성해 줍니다.
			MyCraft.Managers.Game.PlaceBuilding(MyCraft.InvenBase.choiced_item);
			finishPlacementEvent?.Raise();
		}
	}

	private bool ValidLocation()
	{
		if (current == null) return false;
		// this only works with box xcolliders because thats an assumption we made with the demo prefabs!
		if (current.TryGetComponent<BoxCollider>(out BoxCollider col))
		{
			bool onResourceDeposit = false;
			foreach (Collider c in Physics.OverlapBox(col.transform.TransformPoint(col.center), col.size/2f, col.transform.rotation))
			{
				if (c.gameObject == current.gameObject) continue;
				//Debug.Log($"collider: {c.tag}");
				if (c.CompareTag("Building"))
				{
					// colliding something!
					if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
						Debug.LogWarning($"Invalid placement: {current.gameObject.name} collides with {c.gameObject.name}");
					ChangeMatrerial(redPlacementMaterial);
					return false;
				}
				// check for resources
				if (c.CompareTag("Resources"))
					onResourceDeposit = true;
			}
			if (RequiresResourceDepoist && !onResourceDeposit)
			{
				if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
					Debug.LogWarning($"Invalid placement: {current.gameObject.name} requries placement near Resource Deposit");
				ChangeMatrerial(redPlacementMaterial);
				return false;
			}
		}
		ChangeMatrerial(greenPlacementMaterial);
		return true;
	}

	public void ResetAOEPowerConnectins()
	{
		// must be performed on load
		aoePowerConnections = new List<PowerGridComponent>();
		foreach(var aoe in FindObjectsOfType<PowerGridComponent>())
		{
			if (aoe.useConnectionRadius) aoePowerConnections.Add(aoe);
		}
	}

	private bool TryChangeState(State desiredState)
	{
		switch(desiredState)
		{
			case State.PlaceBuilding:
			{
				if (state != State.None || current != null)
				{
					// if currently placing a building, cancel it
					MyCraft.Managers.Resource.Destroy(current);
					state = State.None;
					cancelPlacementEvent?.Raise();
				}
				mouseHeldTime = 0f;
				this.state = desiredState;
				// trigger event
				startPlacementEvent?.Raise();
				return true;
			} break;

			case State.RotateBuilding:
			{
				this.state = desiredState;
				return true;
			} break;

			case State.None:
			{   
				// if we weren't placing a building, ignore
				if (current == null)
				{
					this.state = desiredState;
					return true;
				}

				// make sure building placement and rotation is valid
				if (false == ValidLocation())
				{
					this.state = State.PlaceBuilding;
					mouseHeldTime = 0f;
					return false;
				}

				// finish placing building and enable it
				this.state = desiredState;
				ChangeMatrerial(originalMaterial);
				if (current.TryGetComponent(out Building b))
				{
					////b.enabled = true;
					//base.SetEnable(b, true);
				}
				if (current.TryGetComponent(out PowerGridComponent pgc))
				{
					pgc.enabled = true;
					// check if this is a AOE power grid
					if (pgc.useConnectionRadius)
						aoePowerConnections.Add(pgc);

					// reconnect all aoe's so we know anything in radius has been found
					foreach (var aoe in aoePowerConnections)
						aoe.Connect();
				}

				//지급
				MyCraft.InvenBase.choiced_item.AddStackCount(-1, false);
				current = null;
				return true;

				// trigger event
				finishPlacementEvent?.Raise();

			} break;
		}//..switch(desiredState)
		return false;
	}

	private void OnKeyDown_BuildingPlacement()
	{
		if (Input.GetKeyDown(CancelKey))
		{
			cancelPlacementEvent?.Raise();
			//HG_TEST:아래코드 막고, Event로만 처리가 가능할까?
			//if (current != null)
			//{
			//    MyCraft.Managers.Resource.Destroy(current.gameObject);
			//    cancelPlacementEvent?.Raise();
			//}
			//current = null;
			//state = State.None;
		}
	}

	private void OnMouseEvent(Define.MouseEvent evt)
	{
		//UI위를 클릭했을때...무시
		if (true == base.IsPointerOverGameObject()) return;

		switch (evt)
		{
			case Define.MouseEvent.L_Click:
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit = Physics.RaycastAll(ray, 100f).OrderBy(h => h.distance).FirstOrDefault();
				//foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
				if (null == hit.collider) break;
				if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
				{
					//Debug.Log($"클릭:{building.name}");
					building.OnClicked();
				}
			} break;

			case Define.MouseEvent.R_Press:
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				//RaycastHit[] raycastall = Physics.RaycastAll(ray, 10).OrderBy(h => h.distance).ToArray();
				//int layerMask = 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Conveyor");
				//foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
				//RaycastHit hit = Physics.RaycastAll(ray, 100f).OrderBy(h => h.distance).FirstOrDefault();
				//RaycastHit[] hits = Physics.RaycastAll(ray, 100).OrderBy(h => h.distance).ToArray();
				//foreach (RaycastHit hit in hits)
				RaycastHit hit = Physics.RaycastAll(ray, 100f).OrderBy(h => h.distance).FirstOrDefault();
				if (hit.collider)
				{
					if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
					{
						if (MyCraft.Managers.Game.DestoryProcess.gameObject.activeSelf)
						{
							MyCraft.Managers.Game.DestoryProcess.SetProgress(this, null);
							MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(false);
						}
						break;
					}

					if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
					{
						//DestroyBuilding(building.gameObject);
						MyCraft.Managers.Game.DestoryProcess.SetProgress(this, building.gameObject);
						MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(true);
						break;
					}

					if (hit.collider.TryGetComponent(out PowerGridComponent pgc))
					{
						Debug.Log($"{building.name} 철거");
						MyCraft.Managers.Resource.Destroy(pgc.gameObject);
						break;
					}
				}
			} break;

			case Define.MouseEvent.R_Click:
			{
				if (MyCraft.Managers.Game.DestoryProcess.gameObject.activeSelf)
				{
					MyCraft.Managers.Game.DestoryProcess.SetProgress(this, null);
					MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(false);
				}
			} break;
		}//..switch (evt)
	}

	private void Update()
	{
		switch (state)
		{
			case State.None:
				//HandleIdleState();
				break;
			case State.RotateBuilding:
				HandleRotateBuildingState();    //건물의 방향을 결정할때
				break;
			case State.PlaceBuilding:
				HandlePlaceBuildingState(); //건물이 위치할 곳을 찾고 있을때
				break;
		}

	}

}
