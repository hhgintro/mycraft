using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryFramework;
using MyCraft;
using UnityEngine.EventSystems;
using Unity.Burst.CompilerServices;
using static UnityEditor.PlayerSettings;

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
		PlaceBuilding,
		RotateBuilding
	}
	private State state;
	private bool RequiresResourceDepoist = false;

	// building placement variables to track
	private Vector3 mouseDownPos;
	private float mouseHeldTime = 0f;
	private float secondsHoldToRotate = .333f;

	// we'll want to trigger connect whenever we place a new thing nearby
	private List<PowerGridComponent> aoePowerConnections = new List<PowerGridComponent>();

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
		// don't let building "work" until placement is finished
		if (current.TryGetComponent(out Building b))
		{
			b.enabled = false;
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

		Debug.Log($"{building.name} 철거");
		building.OnDeleted();
		MyCraft.Managers.Game.AddItem(building._itembase.id, 1);
		MyCraft.Managers.Resource.Destroy(building.gameObject);
	}


	private void HandleIdleState()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			foreach(RaycastHit hit in Physics.RaycastAll(ray, 100f))
			{
				if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
				{
					Debug.Log($"클릭:{building.name}");
					building.OnClicked();
				}
			}
		}

		// right click to delete
		if (Input.GetMouseButtonDown(1))
		{
#if UNITY_EDITOR
			//UI위를 클릭했을때...무시
			if (true == EventSystem.current.IsPointerOverGameObject())
			{
				GameObject go = EventSystem.current.currentSelectedGameObject;
				return;
			}
			//if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
						if (EventSystem.current.IsPointerOverGameObject(0) == false)
							return;
#endif
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
			{
				if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
				{
					DestroyBuilding(building.gameObject);
					//Managers.Game.DestoryProcess.SetProgress(this, null);
					//Managers.Game.DestoryProcess.gameObject.SetActive(true);
					return;
				}

				if (hit.collider.TryGetComponent(out PowerGridComponent pgc))
				{
					Debug.Log($"{building.name} 철거");
					MyCraft.Managers.Resource.Destroy(pgc.gameObject);
					return;
				}
			}
		}
//        if (Input.GetMouseButton(1))
//        {
//#if UNITY_EDITOR
//			//UI위를 클릭했을때...무시
//			if (true == EventSystem.current.IsPointerOverGameObject())
//			{
//				GameObject go = EventSystem.current.currentSelectedGameObject;
//				return;
//			}
//			//if (EventSystem.current.IsPointerOverGameObject(-1) == false)
//#elif UNITY_ANDROID // or iOS 
//						if (EventSystem.current.IsPointerOverGameObject(0) == false)
//							return;
//#endif

//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
//			{
//				if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
//				{
//					Managers.Game.DestoryProcess.SetProgress(this, building.gameObject, 5f);
//					Managers.Game.DestoryProcess.gameObject.SetActive(true);
//					//foreach (Socket socket in building.gameObject.GetComponentsInChildren<Socket>())
//					//{
//					//	// FIXME remove this from sockets
//					//}
//					//building.OnDeleted();
//					//MyCraft.Managers.Game.AddItem(building._itembase.id, 1);
//					//MyCraft.Managers.Resource.Destroy(building.gameObject);
//					return;
//				}

//				Managers.Game.DestoryProcess.SetProgress(this, null);
//				Managers.Game.DestoryProcess.gameObject.SetActive(false);
//			}
//		}
		return;
	}


	//건물이 위치할 곳을 찾고 있을때
	private void HandlePlaceBuildingState()
	{
		// move building with mouse pos
		Vector3 groundPos = transform.position;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		foreach (RaycastHit hit in Physics.RaycastAll(ray, 100f))
		{
			// this will only place buildings on terrain. feel free to change this!
			if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
			{
				groundPos = hit.point;	//초기위치
			}
		}


		current.transform.position = groundPos;
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
				if (c.CompareTag("Building") && c.gameObject != current.gameObject)
				{
					// colliding something!
					if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
						Debug.LogWarning($"Invalid placement: {current.gameObject.name} collides with {c.gameObject.name}");
					ChangeMatrerial(redPlacementMaterial);
					return false;
				}
				// check for resources
				if (c.CompareTag("Resources"))
				{
					onResourceDeposit = true;
				}
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
		if (desiredState == State.PlaceBuilding)
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
		}
		if (desiredState == State.RotateBuilding)
		{
			this.state = desiredState;
			return true;
		}
		if (desiredState == State.None)
		{   
			// if we weren't placing a building, ignore
			if (current == null)
			{
				this.state = desiredState;
				return true;
			}

			// make sure building placement and rotation is valid
			if (ValidLocation())
			{
				// finish placing building and enable it
				this.state = desiredState;
				ChangeMatrerial(originalMaterial);
				if (current.TryGetComponent(out Building b))
				{
					b.enabled = true;
				}
				if (current.TryGetComponent(out PowerGridComponent pgc))
				{
					pgc.enabled = true;
					// check if this is a AOE power grid
					if (pgc.useConnectionRadius)
					{
						aoePowerConnections.Add(pgc);
					}

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
			}
			else
			{
				this.state = State.PlaceBuilding;
				mouseHeldTime = 0f;
				return false;
			}
		}
		return false;
	}

	private void Update()
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

		switch (state)
		{
			case State.RotateBuilding:
				HandleRotateBuildingState();    //건물의 방향을 결정할때
				break;
			case State.None:
				HandleIdleState();
				break;
			case State.PlaceBuilding:
				HandlePlaceBuildingState(); //건물이 위치할 곳을 찾고 있을때
				break;
		}

	}

}
