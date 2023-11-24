using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class TechCancel : MonoBehaviour, IPointerDownHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
            Slot slot = this.transform.parent.GetComponent<Slot>();
			slot.owner.GetComponent<TechInven>().OnResearchCancel(slot._slot);
		}
	}
}