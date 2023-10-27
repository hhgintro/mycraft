using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryFramework;
using MyCraft;
using UnityEngine.EventSystems;

public class IPlacement : MonoBehaviour
{
	//��ġ������ collider�� disable ���ѵд�.(ī�޶� �Դٰ��� ����)
	public virtual void SetEnable_1(LogisticComponent logistic, bool enable)
	{
		//		logistic.enabled = enable;
		//logistic.GetComponent<BoxCollider>().enabled = enable;
		logistic.SetEnable_2(enable);
	}

	public virtual void DestroyBuilding(GameObject target) { }

	protected bool GetRayCast(Vector3 pos, out RaycastHit hit, float maxDistance)
	{
		Ray ray = Camera.main.ScreenPointToRay(pos);//���� ���콺�� Ŭ�� ��ġ
		return Physics.Raycast(ray, out hit, maxDistance);
	}

	//UI���� Ŭ��������...����
	public virtual bool IsPointerOverGameObject()
	{
#if UNITY_EDITOR
		if (true == EventSystem.current.IsPointerOverGameObject())
		{
			GameObject go = EventSystem.current.currentSelectedGameObject;
			return true;
		}
		//if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
			if (EventSystem.current.IsPointerOverGameObject(0) == false)
				return true;
#endif
		return false;
	}


}
