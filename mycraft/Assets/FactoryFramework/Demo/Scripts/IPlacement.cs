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

	//�ǹ��� ��ġ�� ���� ã�� ������
	//conveyor ������ġ�� ã�� ������
	protected virtual void HandleStartState() { }
	//�ǹ��� ������ �����Ҷ�
	//conveyor ���κ� ������ ��
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
				//�ǹ��� ������ġ�� ã�� ������
				//conveyor ������ġ�� ã�� ������
				HandleStartState();
				break;
			case State.End:
				//�ǹ��� ������ �����Ҷ�
				//conveyor ���κ� ������ ��
				HandleEndState();
				break;
		}
	}

	//��ġ������ collider�� disable ���ѵд�.(ī�޶� �Դٰ��� ����)
	public virtual void SetEnable_1(LogisticComponent logistic, bool enable)
	{
		//		logistic.enabled = enable;
		//logistic.GetComponent<BoxCollider>().enabled = enable;
		logistic.SetEnable_2(enable);
	}


	protected bool GetRayCast(Vector3 pos, out RaycastHit hit, float maxDistance)
	{
		Ray ray = Camera.main.ScreenPointToRay(pos);//���� ���콺�� Ŭ�� ��ġ
		return Physics.Raycast(ray, out hit, maxDistance);
	}

	//UI���� Ŭ��������...����
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
