using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class ItemData : MonoBehaviour
		, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
		public InvenBase owner;
		//public TechBase itembase;
		public JSonDatabase database;
		public int amount;
		public int panel;
		public int slot;
		//public float fillAmount;

		//private Inventory inven;
		//private Tooltip tooltip;
		//private Vector2 offset;

		public Text textAmount;

		void Awake()
		{
			fnAwake();


			//inven = GameManager.GetInventory();
			//tooltip = GameManager.GetTooltip();
		}

		void Start()
		{
			fnStart();
		}
		//void OnDisable()
		//{
		//	//Debug.Log("OnDisable");
		//	//Managers.Game.Tooltips.Deactivate();
		//}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (false == InvenBase.choiced_item)
				Managers.Game.Tooltips.Activate((this.database));
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			//Debug.Log("OnPointerExit");
			Managers.Game.Tooltips.Deactivate();
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if (Input.GetMouseButtonDown(0))		this.OnMouseLButtonDown();	//L button
			else if (Input.GetMouseButtonDown(1))	this.OnMouseRButtonDown();	//R button
		}

		public virtual void fnAwake()
		{
			if (0 < this.transform.childCount)
				textAmount = this.transform.GetChild(0).GetComponent<Text>();
		}
		public virtual void fnStart() { }

		protected virtual void OnMouseLButtonDown() { }
		protected virtual void OnMouseRButtonDown() { }


		//겹치고 남은 아이템의 개수를 리턴합니다.
		//this.amount <= 0 일때의 처리는 별도로 진행해 주셔야 됩니다.
		public virtual int AddStackCount(int add, float fillAmount, bool noti) { return 0; }
		//추가하고 남은 개수를 리턴한다.
		public virtual int _AddStackCount(int add, ref float fillAmount, bool noti) { return 0; }
		//빼고 모자른 개수를 리턴한다.
		public virtual int _SubStackCount(int add, bool noti) { return 0; }


		#region SAVE
		//public virtual void Save(BinaryWriter bw)
		//{
		//	bw.Write(this.slot);    //slot
		//	bw.Write(this.database.id); //item id
		//	bw.Write(this.amount);  //amount
		//}

		//public virtual void Load(BinaryReader reader)
		//{
		//	int panel			= 0;
		//	int slot			= reader.ReadInt32();
		//	int id				= reader.ReadInt32();
		//	int amount			= reader.ReadInt32();
		//	float fillAmount	= MyCraft.Global.FILLAMOUNT_DEFAULT;    //reader.ReadSingle();
		//	//Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

		//	SetItem(panel, slot, id, amount, fillAmount, false);
		//}
		#endregion //..SAVE
	}

}