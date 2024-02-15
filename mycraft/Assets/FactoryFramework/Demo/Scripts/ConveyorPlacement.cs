using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FactoryFramework;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
//using MyCraft;
using System.Linq;

public class ConveyorPlacement : IPlacement
{
	private Conveyor current;

	private Vector3 startPos;
	private float startHeight;
	private Vector3 endPos;
	private Vector3 flatEndPos;
	private Vector2 shiftMousePos;

	private Socket startSocket;
	private Socket endSocket;

	[Header("Visual Feedback Materials")]
	public Material originalFrameMat;
	public Material originalBeltMat;
	public Material greenGhostMat;
	public Material redGhostMat;


	//OnDestroy에서 호출된다.
	//ConveyorPlacement에서 Pool_Conveyor를 먼저 정리하고(belt의 아이템을 날리기 위해)
	//BuildingPlacement에서 나머지 Pool_xxx을 정리하게 됩니다.
	protected override void OnfnDestroy()
	{
		Debug.Log("ConveyorPlacement 종료를 진행합니다.");
		//Debug.Log($"-  {this.name}");
		Transform go = this.transform.Find("Pool_Conveyor");
		if(null != go)
		{
			//Debug.Log($"+  {go.name}");

			//뒤에서 부터 삭제해야함.
			for (int i = go.childCount - 1; i >= 0; --i)
			{
				//Debug.Log($"++  {go.GetChild(i).name}");
				Conveyor conveyor = go.GetChild(i).GetComponent<Conveyor>();
				if (null == conveyor) continue;
				
				conveyor.OnDeleted(false); //false:밸트위 아이템을 인벤으로 회수하지 않고 날린다.
				MyCraft.Managers.Resource.Destroy(conveyor.gameObject);
			}
		}
	}

	//buildingPlace의 ForceCancel()와 흡사(conveyor에서 centerGrid을 적용할때 buildingPlace와 통합을 고려할 것)
	public override void ForceCancel()
	{
		Debug.Log("ForceCancel ConveyorPlacement");
		if (current != null)
		{
			////재사용시(불러오기) material이 녹색으로 노출되거나 collider가 disable된 경우가 있어서
			////  우선 취소할때 설정해 준다.
			////  Load()할떄 처리해 주면 좋겠는데, 꼭 처리할 것.
			//current.SetMaterials(originalFrameMat, originalBeltMat);
			//current.SetEnable_2(true);

			MyCraft.Managers.Resource.Destroy(current.gameObject);
		}
		current = null;
		startSocket = null;
		endSocket = null;
		base.state = State.None;
	}

	//public override void DestroyBuilding(GameObject target)
	//{
	//	if (false == target.TryGetComponent<Conveyor>(out Conveyor conveyor))
	//		return;

	//	Debug.Log($"{conveyor.name} 철거");
	//	conveyor.OnDeleted(true);
	//	MyCraft.Managers.Resource.Destroy(conveyor.gameObject);
	//}

	protected override void OnKeyDown()
	{
		if (Input.GetKeyDown(base.cancelKey))
		{
			cancelPlacementEvent?.Raise();
			return;
		}
		//base.OnKeyDown();	//BuildingPlacement와 중복으로 2번호출되어진다.
	}

	//protected override void OnMouseEvent(Define.MouseEvent evt)
	//{
	//	//UI위를 클릭했을때...무시
	//	if (true == base.IsPointerOverGameObject()) return;

	//	switch (evt)
	//	{
	//		case Define.MouseEvent.R_Press:
	//			{
	//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	//				RaycastHit hit = Physics.RaycastAll(ray, MyCraft.Common.MAX_RAY_DISTANCE).OrderBy(h => h.distance).FirstOrDefault();
	//				//foreach (RaycastHit hit in Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE))
	//				if (null == hit.collider || null == hit.collider.transform.parent)
	//					break;
	//				if (hit.collider.transform.parent.TryGetComponent<Conveyor>(out Conveyor conveyor))
	//				{
	//					//DestroyBuilding(conveyor.gameObject);
	//					MyCraft.Managers.Game.DestoryProcess.SetProgress(this, conveyor.gameObject);
	//					MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(true);
	//					return;
	//				}
	//			}
	//			break;

	//		case Define.MouseEvent.R_Click:
	//			{
	//				if (MyCraft.Managers.Game.DestoryProcess.gameObject.activeSelf)
	//				{
	//					MyCraft.Managers.Game.DestoryProcess.SetProgress(this, null);
	//					MyCraft.Managers.Game.DestoryProcess.gameObject.SetActive(false);
	//				}
	//			}
	//			break;
	//	}//..switch (evt)
	//}

	public override void SetEnable_1(LogisticComponent logistic, bool enable)
	{
		//재사용시(불러오기) material이 녹색으로 노출되거나 collider가 disable된 경우가 있어서
		//  우선 취소할때 설정해 준다.
		//  Load()할떄 처리해 주면 좋겠는데, 꼭 처리할 것.
		logistic.SetMaterials(originalFrameMat, originalBeltMat);
		base.SetEnable_1(logistic, enable);
	}

	//private bool TryChangeState(State desiredState)
	//{
	//	base.state = desiredState;
	//	return true;
	//}

	//private bool ValidLocation()
	//{
	//	if (current == null) return false;
		
	//		foreach (Collider c in Physics.OverlapSphere(startPos, 1f))
	//		{
	//			//if (c.CompareTag("Building") && c.gameObject != current.gameObject)
	//			if (c.transform.gameObject.layer == LayerMask.NameToLayer("Building")
	//				&& c.gameObject != current.gameObject)
	//			{
	//				// colliding something!
	//				if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
	//					Debug.LogWarning($"Invalid placement: {current.gameObject.name} collides with {c.gameObject.name} at the start");
	//				//ChangeMatrerial(redGhostMat);
	//				return false;
	//			}
	//		}
	//		foreach (Collider c in Physics.OverlapSphere(endPos, 1f))
	//		{
	//			//if (c.CompareTag("Building") && c.gameObject != current.gameObject)
	//			if (c.transform.gameObject.layer == LayerMask.NameToLayer("Building")
	//				&& c.gameObject != current.gameObject)
	//			{
	//				// colliding something!
	//				if (ConveyorLogisticsUtils.settings.SHOW_DEBUG_LOGS)
	//					Debug.LogWarning($"Invalid placement: {current.gameObject.name} collides with {c.gameObject.name} at the end");
	//				//ChangeMatrerial(redGhostMat);
	//				return false;
	//			}
	//		}

	//	//ChangeMatrerial(greenGhostMat);
	//	return true;
	//}

	public GameObject StartPlacingConveyor(GameObject prefab)
	{
		//cancel any placement currently happening
		cancelPlacementEvent?.Raise();
		// instantiate a belt to place
		current = MyCraft.Managers.Resource.Instantiate(prefab).GetComponent<Conveyor>();    //HG[2023.06.01]테스트필요
		//current.transform.parent = this.transform.Find($"Pool_{prefab.name}") ?? (new GameObject($"Pool_{prefab.name}") { transform = { parent = this.transform } }).transform;
		current.transform.parent = MyCraft.Common.ParentPool(this.transform, prefab.name);
		if (base.TryChangeState(State.Start))
		{
			//b.enabled = false;
			base.SetEnable_1(current, false);

			//재시작할때 들고 있던 아이템들 날린다.(false:회수안함)
			//current.OnDeleted(false); //<--여기서 처리할 것이 아니라. BuildingPlacement와 ConveyorPlacement를 통합후에 Destroy할때 초기화 하도록 합니다.

			startSocket = null;
			endSocket = null;
		}
		// trigger event
		startPlacementEvent?.Raise();
		return current.gameObject;
	}

	//void HandleIdleState()
	//{
	//}

	private bool IsSafeFooting(RaycastHit hit, ref Vector3 worldPos, ref Vector3 worldDir)
	{
		if (hit.transform.tag != "Safe-Footing")  //안전발판
			return false;

		worldPos = MyCraft.Common.Floor(hit.point + Vector3.up * 0.3f);   //시작높이	//건물위치간격보간
		worldDir = currentRotation * Vector3.forward;
		return true;
	}

	private bool IsTerrain(RaycastHit hit, ref Vector3 worldPos, ref Vector3 worldDir)
	{
		if (false == hit.collider.gameObject.TryGetComponent<Terrain>(out Terrain t))
			return false;

		worldPos = MyCraft.Common.Floor(hit.point + Vector3.up);	//건물위치간격보간
		worldDir = currentRotation * Vector3.forward;
		return true;
	}

	//conveyor 시작위치를 찾고 있을때
	protected override void HandleStartState()
	{
		Debug.Assert(current != null, "Not currently placing a conveyor.");
		startSocket = null;
		Vector3 worldPos = Vector3.zero;
		Vector3 worldDir = Vector3.forward;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, MyCraft.Common.MAX_RAY_DISTANCE).OrderBy(h => h.distance).ToArray();
		foreach (RaycastHit hit in hits)
		{
			// skip objects/colliders in the conveyor we're currently placing
			//if (hit.transform.root == current.transform) continue;  //여기때문에 conveyor의 parent을 설정하면. 설치가 안되는 현상나타남
			if (hit.transform.parent == current.transform) continue;
			// try to find an open socket
			if (hit.collider.gameObject.TryGetComponent(out OutputSocket socket))
			{
				// Socket already Occupied
				if (!socket.IsOpen()) continue;

				startSocket = socket;
				break;
			}

			//안전발판
			if (IsSafeFooting(hit, ref worldPos, ref worldDir)) break;
			if (IsTerrain(hit, ref worldPos, ref worldDir))		break;
		}
		// override placement if we found a valid socket
		if (startSocket)
		{
			worldPos = startSocket.transform.position;
			worldDir = startSocket.transform.forward;
		}

		startPos                = worldPos;
		// setup the start and end vectors to solve for path building
		current.data.start      = worldPos;
		current.data.startDir   = worldDir;
		current.data.end        = worldPos + worldDir;
		current.data.endDir     = worldDir;
		// get the relative height
		if (startSocket == null)	startHeight = 0f; // Terrain.activeTerrain.SampleHeight(worldPos);
		else						startHeight = startSocket.transform.position.y - Terrain.activeTerrain.SampleHeight(worldPos);

		//ignore colliders from start and end points
		List<Collider> collidersToIgnore = new List<Collider>();
		// add colliders associated with the connected start socket
		if (startSocket != null)
		{
			collidersToIgnore.AddRange(startSocket.transform.root.GetComponentsInChildren<Collider>());
			collidersToIgnore.Remove(startSocket.transform.root.GetComponent<Collider>());
		}
		else
		{
			collidersToIgnore.Add(Terrain.activeTerrain.GetComponent<TerrainCollider>());
		}
		
		//if (collidersToIgnore.Count > 0)
		current.UpdateMesh(ignored: collidersToIgnore.ToArray(), startskip:1, endskip: 1);
		//else
		//    current.UpdateMesh();

		// startSocket != null prevents belt from starting disconnected
		if (current.ValidMesh) // && startSocket != null
		{
			//current.SetMaterials(originalFrameMat, originalBeltMat);
			current.SetMaterials(greenGhostMat, greenGhostMat);
			if (Input.GetMouseButtonDown(0)) base.TryChangeState(State.End);
		}
		else
		{
			current.SetMaterials(redGhostMat, redGhostMat);
		}		
	}
	//conveyor 끝부분 지정할 때
	protected override void HandleEndState()
	{
		Debug.Assert(current != null, "Not currently placing a conveyor.");
		Vector3 worldPos = Vector3.zero;
		Vector3 worldDir = Vector3.forward;

		//Ray mousedownRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		//foreach (RaycastHit hit in Physics.RaycastAll(mousedownRay, Common.MAX_RAY_DISTANCE))
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, MyCraft.Common.MAX_RAY_DISTANCE).OrderBy(h => h.distance).ToArray();
		foreach (RaycastHit hit in hits)
		{
			//if (hit.collider.transform.root == current.transform) continue;	//이거때문에 문제가 발생한건 아니지만 혹시나 해서 HandleStartState와 같이 맞춰줌.
			if (hit.transform.parent == current.transform) continue;
			// want to specifically connect to a conveyor socket, not a belt bridge
			if (hit.collider.gameObject.TryGetComponent<InputSocket>(out InputSocket socket))
			{
				if (!socket.IsOpen())
				{
					// Socket already Occupied
					continue;
				}
				worldPos    = hit.collider.transform.position;
				worldDir    = hit.collider.transform.forward;
				endSocket   = socket;
				break;
			}

			if (hit.transform.tag == "Safe-Footing")  //안전발판(위)
			{
				//worldPos = hit.point;
				//// stay same level if this is the terrain
				//worldPos.y = Terrain.activeTerrain.SampleHeight(worldPos) + startHeight;
				worldPos = MyCraft.Common.Floor(hit.point + Vector3.up * 0.301f);	//건물위치간격보간

				//Vector3 camForward = Camera.main.transform.forward;
				//camForward.y = 0f;
				//camForward.Normalize();
				//worldDir = camForward;
				worldDir = currentRotation * Vector3.forward;

				// reset socket
				endSocket = null;
				break;
			}

			if (hit.collider.gameObject.TryGetComponent<Terrain>(out Terrain t))
			{
				// handle height offset when holding shift
				if (Input.GetKey(KeyCode.LeftShift))
				{
					// find the intersection of the camera mouse ray plane and the endPos->Vector.Up line
					Vector3 planeNormal = Camera.main.transform.up;
					Vector3 lineStart  = flatEndPos;
					Vector3 lineVector = Vector3.up;

					float dotNumerator   = Vector3.Dot((hit.point - lineStart), planeNormal);
					float dotDenominator = Vector3.Dot(lineVector, planeNormal);
					if (dotDenominator == 0.0f)
						worldPos = flatEndPos;
					else
					{
						var length = dotNumerator / dotDenominator;
						Vector3 vec = Vector3.up * length;
						worldPos = lineStart + vec;
;                   }
					worldPos.y = Mathf.Max(Terrain.activeTerrain.SampleHeight(worldPos), worldPos.y);
				} else
				{
					worldPos = hit.point;
					//// stay same level if this is the terrain
					//float sampleHeight = Terrain.activeTerrain.SampleHeight(worldPos);
					//worldPos.y = sampleHeight + startHeight;// + 1;	//+1: 땅에 뭍혀서
				}

				worldPos = MyCraft.Common.Floor(worldPos);	//건물위치간격보간
				worldDir = currentRotation * Vector3.forward;
				// reset socket
				endSocket = null;
				break;
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			flatEndPos = endPos;
			shiftMousePos = Input.mousePosition;
		}
		worldPos.x = worldPos.x + 0.0001f;	//1f딱 떨어지는 수를 넣지 마라. LOOK at Zero라는 메시지를 보게된다.(우클릭으로 삭제도 안됨)
		endPos = worldPos;
		current.data.end = worldPos;
		current.data.endDir = worldDir;
		List<Collider> collidersToIgnore = new List<Collider>();
		//add colliders associated with the connected start and end sockets
		//THIS IS NOT A GREAT WAY TO DO THIS - CONSIDER USING LAYERMASKS
		//if (startSocket == null)
		//    collidersToIgnore.AddRange(FindObjectsOfType<TerrainCollider>());
		if (startSocket) collidersToIgnore.AddRange(startSocket.GetComponentsInChildren<Collider>());
		if (endSocket) collidersToIgnore.AddRange(endSocket.GetComponentsInChildren<Collider>());
		// add self
		OutputSocket outputSocket = current.GetComponentInChildren<OutputSocket>();
		if (outputSocket) collidersToIgnore.Add(outputSocket.GetComponent<Collider>());
		InputSocket inputSocket = current.GetComponentInChildren<InputSocket>();
		if (inputSocket) collidersToIgnore.Add(inputSocket.GetComponent<Collider>());
		// add connected sockets
		if (startSocket) collidersToIgnore.Add(startSocket.GetComponent<Collider>());
		if (endSocket) collidersToIgnore.Add(endSocket.GetComponent<Collider>());

		current.UpdateMesh(
			startskip: 1, //startSocket != null ? 1 : 0, 
			endskip: 1,
			ignored: collidersToIgnore.Count > 0 ? collidersToIgnore.ToArray() : null
		);

		//아이템개수가 부족하다.
		bool ValidMesh = current.ValidMesh;
		if (MyCraft.InvenBase.choiced_item.amount <= current.CalculateCapacity_1())
			ValidMesh = false;

		if (ValidMesh)	current.SetMaterials(greenGhostMat, greenGhostMat);
		else			current.SetMaterials(redGhostMat, redGhostMat);

		//coordinate
		//MyCraft.Managers.Game.Coordinates.DrawCoordinate(worldPos);

		if (Input.GetMouseButtonDown(0) && ValidMesh)
		{
			////b.enabled = true;
			//base.SetEnable(current, true);

			// change the sockets!
			if (startSocket != null)
			{
				//startSocket.Connect(current);
				current.ConnectToOutput(startSocket as OutputSocket);
			}
			if (endSocket != null)
			{
				//endSocket.Connect(current);
				current.ConnectToInput(endSocket as InputSocket);
			}
			// finalize the conveyor
			current.UpdateMesh(true);
			current.SetMaterials(originalFrameMat, originalBeltMat);
			current.AddCollider();

			//지급
			int amount = MyCraft.InvenBase.choiced_item._SubStackCount(current.Capacity, false);
			if(0 < amount)	//HG_TODO: 남은 개수는 인벤에서 빼준다.
			{
				//Managers.Game.SubItem(InvenBase.choiced_item);

			}


			// stop placing conveyor
			current = null;
			startSocket = null;
			endSocket = null;

			base.TryChangeState(State.None);
			finishPlacementEvent?.Raise();
			//finishPlacementEvent?.Raise(current.Capacity);
		}
	}
}
