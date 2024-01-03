using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryFramework;
//using MyCraft;
using UnityEngine.EventSystems;
using Unity.Burst.CompilerServices;
using MyCraft;
using System;

public class BuildingPlacement : IPlacement
{
	//[Header("Building Prefabs")]
	private GameObject current; //설치하기전 잡고있는 building.
	//건물위치설정할때 "r"눌렀을때 회전하는 각도.
	private Quaternion currentRotation = Quaternion.identity;

	//#if UNITY_STANDALONE
	//	private static int index=0;//debug용 
	//#endif//..UNITY_STANDALONE

	[Header("Visual Feedback Building Materials")]
	public Material originalMaterial;
	public Material greenPlacementMaterial;
	public Material redPlacementMaterial;

	// list of valid materials so we don't change any other materials
	private Material[] validMaterials { get { return new Material[3] { originalMaterial, greenPlacementMaterial, redPlacementMaterial }; } }


	//drill의 경우 아래 자원(철광석,돌,구리광석)이 있어야 설치가 가능합니다.
	private bool _requiresResourceDepoist = false;

	// building placement variables to track
	private Vector3 mouseDownPos;
	private float mouseHeldTime = 0f;
	private float secondsHoldToRotate = .333f;

	// we'll want to trigger connect whenever we place a new thing nearby
	private List<PowerGridComponent> aoePowerConnections = new List<PowerGridComponent>();

    protected override void fnStart()
	{
		Managers.CenterGrid.Init(this.transform);
	}

	//OnDestroy에서 호출된다.
	//ConveyorPlacement에서 Pool_Conveyor를 먼저 정리하고(belt의 아이템을 날리기 위해)
	//BuildingPlacement에서 나머지 Pool_xxx을 정리하게 됩니다.
	protected override void OnfnDestroy()
	{
		Debug.Log("BuildingPlacement 종료를 진행합니다.");
		//Debug.Log($"-  {this.name}");
		foreach (Transform go in this.transform)
		{
			//Debug.Log($"+  {go.name}");

			//뒤에서 부터 삭제해야함.
			for (int i = go.childCount - 1; i >= 0; --i)
			{
				//파괴시 (인벤에)가지고 있는 아이템을 모두 날린다.(재시작후 새로지어진 건물에 기존아이템이 그대로 있던 버그가 있었다.)
				if(go.GetChild(i).TryGetComponent<Building>(out Building building))
					building.OnDeleted(false);

				//Debug.Log($"++  {go.GetChild(i).name}");
				Managers.Resource.Destroy(go.GetChild(i).gameObject);
			}
		}
	}

	public override void ForceCancel()
	{
		Debug.Log("ForceCancel");
		if (current != null)
			MyCraft.Managers.Resource.Destroy(current.gameObject);

		current = null;
		Managers.CenterGrid.Stop();
		base.state = State.None;

        //foreach(var item in rook.Keys)
        //    Debug.Log($"GUID:[{item}]");
    }

	//DestroyProcess에 의해 철거될때 호출
	public override void DestroyBuilding(GameObject target)
	{
		if (false == target.TryGetComponent<LogisticComponent>(out LogisticComponent logistic))
			return;

		//Debug.Log($"{building.name} 철거");
		logistic.OnDeleted(true);
		MyCraft.Managers.Resource.Destroy(target);
	}


	protected override void OnKeyDown()
	{
		if (Input.GetKeyDown(base.cancelKey))
		{
			Debug.Log("Key_Down ESC");
			cancelPlacementEvent?.Raise();
			return;
		}
		if(Input.GetKeyDown(KeyCode.R))
		{
			float angle = Common.BUILDING_ROTATION_KEYDOWN_ANGLE;	//회전각도(45도-시계방향)
			if (Input.GetKey(KeyCode.LeftShift)) angle *= -1;//반시계방향
			currentRotation = Quaternion.Euler(Vector3.up * angle) * currentRotation;
		}
	}

	protected override void OnMouseEvent(Define.MouseEvent evt)
	{
		//UI위를 클릭했을때...무시
		if (true == base.IsPointerOverGameObject()) return;

		switch (evt)
		{
			case Define.MouseEvent.L_Click:
				{
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit = Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE).OrderBy(h => h.distance).FirstOrDefault();
					//foreach (RaycastHit hit in Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE))
					if (null == hit.collider) break;
					if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
					{
						//Debug.Log($"클릭:{building.name}");
						building.OnClicked();
					}
				}
				break;

			case Define.MouseEvent.R_Press:
				{
					LayerMask m;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					int layerMask = LayerMask.GetMask(new string[] { "Building", "Conveyor", "Floor"});
					RaycastHit hit = Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE, layerMask).OrderBy(h => h.distance).FirstOrDefault();
					if (null == hit.collider)
					{
						//etc
						if (MyCraft.Managers.Game.DestoryProcess.gameObject.activeSelf)
						{
							MyCraft.Managers.Game.DestoryProcess.SetProgress(this, null);
							MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(false);
						}
						break;
					}
					////powerGrid
					//if (hit.collider.TryGetComponent(out PowerGridComponent pgc))
					//{
					//	//MyCraft.Managers.Resource.Destroy(pgc.gameObject);
					//	//break;
					//}
					//building
					if (hit.collider.gameObject.TryGetComponent<Building>(out Building building))
					{
						//DestroyBuilding(building.gameObject);
						MyCraft.Managers.Game.DestoryProcess.SetProgress(this, building.gameObject);
						MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(true);
						break;
					}
					//conveyor
					if(hit.collider.transform.parent
						&& hit.collider.transform.parent.TryGetComponent<Conveyor>(out Conveyor conveyor))
					{
						MyCraft.Managers.Game.DestoryProcess.SetProgress(this, conveyor.gameObject);
						MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(true);
						break;
					}
				}
				break;

			case Define.MouseEvent.R_Click:
				{
					if (MyCraft.Managers.Game.DestoryProcess.gameObject.activeSelf)
					{
						MyCraft.Managers.Game.DestoryProcess.SetProgress(this, null);
						MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(false);
					}
				}
				break;
		}//..switch (evt)
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

	public void ResetAOEPowerConnectins()
	{
		// must be performed on load
		aoePowerConnections = new List<PowerGridComponent>();
		foreach (var aoe in FindObjectsOfType<PowerGridComponent>())
		{
			if (aoe.useConnectionRadius) aoePowerConnections.Add(aoe);
		}
	}

	private bool TryChangeState(State desiredState)
	{
		switch (desiredState)
		{
			case State.Start:
				{
					if (current != null || base.state != State.None)
					{
						// if currently placing a building, cancel it
						MyCraft.Managers.Resource.Destroy(current);
						base.state = State.None;
						cancelPlacementEvent?.Raise();
					}
					mouseHeldTime = 0f;
					base.state = desiredState;
					// trigger event
					startPlacementEvent?.Raise();
					return true;
				}
				break;

			case State.End:
				{
					base.state = desiredState;
					return true;
				}
				break;

			case State.None:
				{
					// if we weren't placing a building, ignore
					if (current == null)
					{
						base.state = desiredState;
						return true;
					}

					// make sure building placement and rotation is valid
					if (false == ValidLocation())
					{
						base.state = State.Start;
						mouseHeldTime = 0f;
						return false;
					}

					// finish placing building and enable it
					base.state = desiredState;
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
					MyCraft.InvenBase.choiced_item._SubStackCount(1, false);
					current = null;
					Managers.CenterGrid.Stop();
					return true;

					// trigger event
					finishPlacementEvent?.Raise();
					return true;

				}
				break;
		}//..switch(desiredState)
		return false;
	}

	private bool ValidLocation()
	{
		if (current == null) return false;
		// this only works with box xcolliders because thats an assumption we made with the demo prefabs!
		if (current.TryGetComponent<BoxCollider>(out BoxCollider col))
		{
			bool onResourceDeposit = false;
			Vector3 extents = new Vector3(current.transform.localScale.x * col.size.x
				, current.transform.localScale.y * col.size.y
				, current.transform.localScale.z * col.size.z);
            //foreach (Collider c in Physics.OverlapBox(col.transform.TransformPoint(col.center), col.size / 2f, col.transform.rotation))
            foreach (Collider c in Physics.OverlapBox(col.transform.TransformPoint(col.center), extents / 2f, col.transform.rotation))
            {
                if (c.gameObject == current.gameObject) continue;
				//Debug.Log($"collider: {c.tag}");
				//if (c.CompareTag("Building"))
				if(c.transform.gameObject.layer == LayerMask.NameToLayer("Building")
					|| c.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
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
			//drill의 경우 아래 자원(철광석,돌,구리광석)이 있어야 설치가 가능합니다.
			if (_requiresResourceDepoist && !onResourceDeposit)
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

	//requireDeposit: 자원위에 설치가능
	public GameObject StartPlacingBuilding(GameObject prefab, bool requireDeposit = false)
	{
		cancelPlacementEvent?.Raise();
		_requiresResourceDepoist = requireDeposit;
		// spawn a prefab and start placement
		if (!TryChangeState(State.Start))
			return null;

		//current = Instantiate(prefab);
		current = MyCraft.Managers.Resource.Instantiate(prefab);    //HG[2023.06.01]테스트필요
		current.transform.parent = this.transform.Find($"Pool_{prefab.name}") ?? (new GameObject($"Pool_{prefab.name}") { transform = { parent = this.transform } }).transform;
		//#if UNITY_STANDALONE
		//	번호를 부여했더니, 이름이 달라져서 개체가 (pool에 저장되지 못하고)삭제되어 버린다.
		//	디버깅용도로만 사용할 것
		//	pool에 들어갈깨 "_"를 비교하면서 까지 번호를 부어할 필요는 없지.
		//		if(false == current.name.Contains("_"))
		//			current.name += string.Format($"_{(++index).ToString()}");
		//#endif//..UNITY_STANDALONE

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

	//private void HandleIdleState()
	//{}

	float y = 0;
	//건물이 위치할 곳을 찾고 있을때
	protected override void HandleStartState()
	{
		// move building with mouse pos
		Vector3 groundPos = Vector3.zero;
		Vector3 groundDir = currentRotation * Vector3.forward;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE).OrderBy(h => h.distance).ToArray();
		//RaycastHit[] hits = Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE);
		foreach (RaycastHit hit in hits)
		{
			//자신은...무시
			if (current == hit.collider.gameObject) continue;
			if (false == current.TryGetComponent(out Building building)) continue;

			//============================================
			//	위치보정(socket에 의한)
			if (true == building.LocationCorrectForSocket(hit, ref groundPos, ref groundDir))
				break;


			//============================================================================
			// *** 분배기 / 병합기 / 안전발판 / Terrain은 centerGrid의 영향을 받지 않는다. *** //

			//Debug.Log($"tag:{hit.transform.tag}");
			if (hit.transform.tag == "Splitter")  //분배기 / 병합기
			{
				if (current.transform.tag == "Splitter")		groundPos = hit.transform.position + Vector3.up * 1.6f;
				Managers.CenterGrid.Stop();
				break;
			}

			if (hit.transform.tag == "Safe-Footing")  //안전발판
			{
				//안전발판위에 안전발판(current) 설치 위치
				if (current.transform.tag == "Safe-Footing")	groundPos = hit.point + Vector3.up * 4f;
				//안전발판위에 건물(current) 설치 위치
				else											groundPos = hit.point;// + Vector3.up * 0.5f;
				Managers.CenterGrid.Stop();
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

			//건물에만 적용(안전발판,계단은 제외)
			if(LayerMask.NameToLayer("Building") == current.layer)
				Managers.CenterGrid.Place(current, ref groundPos);
		}

		//Debug.Log($"{current.name}:({groundPos})");
		current.transform.position = groundPos;
		current.transform.forward = groundDir;


		bool valid = ValidLocation();
		// left mouse button to try to place building
		if (Input.GetMouseButtonDown(0) && valid)
		{
			// try to change state to rotate the building
			if (TryChangeState(State.End))
				mouseDownPos = groundPos;
		}
	}
	//건물의 방향을 결정할때
	protected override void HandleEndState()
	{
		// wait for mouse to be held for X seconds until building rotation is allowed
		// this prevents quick clicks resulting in seemingly random building rotations
		mouseHeldTime += Time.deltaTime;
		if (mouseHeldTime > secondsHoldToRotate)
		{
			// rotate the building!
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			foreach (RaycastHit hit in Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE))
			{
				// thios demo script will only use a terrain object as the "ground"
				if (hit.collider.TryGetComponent<Terrain>(out Terrain _))
				{
					Vector3 targetDirection = mouseDownPos - new Vector3(hit.point.x, mouseDownPos.y, hit.point.z);
					float angle = Vector3.SignedAngle(current.transform.forward, targetDirection, Vector3.up);
					//Debug.Log($"회전각:({angle})");
					if (Common.BUILDING_ROTATION_MOUSE_ANGLE < angle)		//+:시계방향
					{
						Quaternion currentRotation = current.transform.rotation;
						Quaternion targetRotation = Quaternion.Euler(Vector3.up * Common.BUILDING_ROTATION_MOUSE_ANGLE) * currentRotation;
						current.transform.rotation = targetRotation;
					}
					else if(angle < -Common.BUILDING_ROTATION_MOUSE_ANGLE) //-:반시계방향
					{
						Quaternion currentRotation = current.transform.rotation;
						Quaternion targetRotation = Quaternion.Euler(Vector3.up * -Common.BUILDING_ROTATION_MOUSE_ANGLE) * currentRotation;
						current.transform.rotation = targetRotation;
					}
				}
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
}
