using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FactoryFramework;
using MyCraft;
using UnityEngine.EventSystems;

public class IPlacement : MonoBehaviour
{
	public virtual void DestroyBuilding(GameObject target) { }

	protected bool GetRayCast(Vector3 pos, out RaycastHit hit, float maxDistance)
	{
		Ray ray = Camera.main.ScreenPointToRay(pos);//���� ���콺�� Ŭ�� ��ġ
		return Physics.Raycast(ray, out hit, maxDistance);
	}
}
