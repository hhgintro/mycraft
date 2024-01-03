using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryFramework;
//using MyCraft;
using UnityEngine.EventSystems;

public class IPlacement : MonoBehaviour
{
	[Header("Placement Events")]
	public VoidEventChannel_SO startPlacementEvent;
	public VoidEventChannel_SO finishPlacementEvent;
	public VoidEventChannel_SO cancelPlacementEvent;

	[Header("Controls")]
	protected KeyCode cancelKey = KeyCode.Escape;

	protected enum State
	{
		None,
		Start,
		End
	}
	protected State state;

	protected virtual void fnStart() { }
	protected virtual void OnfnDestroy() { }
	public virtual void ForceCancel() { }
	public virtual void DestroyBuilding(GameObject target) { }
	protected virtual void OnKeyDown() { }
	protected virtual void OnMouseEvent(Define.MouseEvent evt) { }

	//건물이 위치할 곳을 찾고 있을때
	//conveyor 시작위치를 찾고 있을때
	protected virtual void HandleStartState() { }
	//건물의 방향을 결정할때
	//conveyor 끝부분 지정할 때
	protected virtual void HandleEndState() { }


	private void Start()
	{
		this.fnStart();

		MyCraft.Managers.Input.KeyAction -= OnKeyDown;
		MyCraft.Managers.Input.KeyAction += OnKeyDown;

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
	private void OnDestroy()
	{
		OnfnDestroy();
	}

	public void Update()
	{
		switch (this.state)
		{
			case State.None:
				//HandleIdleState();
				break;
			case State.Start:
				//건물의 시작위치를 찾고 있을때
				//conveyor 시작위치를 찾고 있을때
				HandleStartState();
				break;
			case State.End:
				//건물의 방향을 결정할때
				//conveyor 끝부분 지정할 때
				HandleEndState();
				break;
		}
	}

	//설치전에는 collider를 disable 시켜둔다.(카메라 왔다갔다 현상)
	public virtual void SetEnable_1(LogisticComponent logistic, bool enable)
	{
		//		logistic.enabled = enable;
		//logistic.GetComponent<BoxCollider>().enabled = enable;
		logistic.SetEnable_2(enable);
	}


	protected bool GetRayCast(Vector3 pos, out RaycastHit hit, float maxDistance)
	{
		Ray ray = Camera.main.ScreenPointToRay(pos);//현재 마우스의 클릭 위치
		return Physics.Raycast(ray, out hit, maxDistance);
	}

	//UI위를 클릭했을때...무시
	public virtual bool IsPointerOverGameObject()
	{
		//if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#if UNITY_ANDROID // or iOS 
			if (EventSystem.current.IsPointerOverGameObject(0) == false)
				return true;
#else
		if (true == EventSystem.current.IsPointerOverGameObject())
		{
			GameObject go = EventSystem.current.currentSelectedGameObject;
			return true;
		}
#endif
		return false;
	}


}
