using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class InvenItemData : ItemData
	{
		//public ItemInvenBase owner;
		//public ItemBase itembase;
		//public int amount;
		//public int panel;
		//public int slot;

		//private Inventory inven;
		//private Tooltip tooltip;
		//private Vector2 offset;

		//public Text textAmount;

		//public override void OnPointerEnter(PointerEventData eventData)
		//{
		//    Debug.Log("OnPointerEnter");
		//    if (false == InvenBase.choiced_item)
		//        Managers.Game.Tooltips.Activate((base.database));
		//}

		//public override void OnPointerExit(PointerEventData eventData)
		//{
		//    //Debug.Log("point exit");
		//    Managers.Game.Tooltips.Deactivate();
		//}

		//public override void OnPointerDown(PointerEventData eventData)
		//{
		//    base.OnPointerDown(eventData);

		//    //L button
		//    if (Input.GetMouseButtonDown(0)) OnMouseLButtonDown();
		//    //R button
		//    else if (Input.GetMouseButtonDown(1)) OnMouseRButtonDown();

		//    if (false == InvenBase.choiced_item)    Managers.Game.Tooltips.Activate((base.database));
		//    else                                    Managers.Game.Tooltips.Deactivate();
		//}

		protected override void OnMouseLButtonDown()
		{
			//pickAll의 경우에 교체시에는 pickup이 마지막으로 처리되어 0이 되는 결과가 발생
			bool noti2block = true;

			//들고 있다면...
			if (null != InvenBase.choiced_item)
			{
				//같은 아이템을 들고 있다면, 겹치기
				if (this.database.id == InvenBase.choiced_item.database.id)
				{
					//겹치고 남은 개수를 리턴합니다.
					int amount = InvenBase.choiced_item.SetStackCount(this.CheckOverlapItem(InvenBase.choiced_item.amount), false);
					if (amount <= 0)
					{
						Managers.Resource.Destroy(InvenBase.choiced_item.gameObject);
						InvenBase.choiced_item = null;
						Managers.Game.DestoryBuilding();    //선택된 건물이있다면 내려놓는다.
					}
					return;
				}


				//내려놓을 수 있는지 확인한다.
				if (false == this.owner.CheckPutdownGoods(this.panel, this.slot, InvenBase.choiced_item.database.id))
					return;


				//들고 있던건 내려놓고(다른아이템인 경우)
				//this.owner.slots[this.slot].AddItem(InvenBase.choiced_item);
				Slot s = this.owner.GetInvenSlot(this.panel, this.slot);
				if (null != s) s.AddItem(InvenBase.choiced_item);
				InvenBase.choiced_item = null;
				Managers.Game.DestoryBuilding();    //선택된 건물이있다면 내려놓는다.
				noti2block = false;//교체시에는 Pickup을 block으로 전달하지 않는다.
			}

			//들 수 없다.
			if (false == this.owner.CheckPickupGoods())
			{
				Debug.Log(this.owner.ToString() + "은 pickup 할 수 없습니다.");
				return;
			}

			//모두 집어든다.(아이템이 있다면)
			if (0 != this.database.id)
				InvenBase.choiced_item = this.PickupAll(this.transform.parent.parent.parent.parent, noti2block);

		}

		protected override void OnMouseRButtonDown()
		{
			//들 수 없다.
			if (false == this.owner.CheckPickupGoods())
			{
				Debug.Log(this.owner.ToString() + "은 pickup 할 수 없습니다.");
				return;
			}

			int pickCnt = 0; //인벤에서는 개수만큼 빼준다.

			//들고 있는 것이 없다면...
			if (null == InvenBase.choiced_item)
			{
				//모두 잡는다.(한번에 잡을 수 있는 수 보다 작다)
				if (this.amount <= 1)
				{
					InvenBase.choiced_item = this.PickupAll(this.transform.parent.parent.parent.parent, true);
					return;
				}

				//인벤에서는 개수만큼 빼준다.
				pickCnt = (int)(this.amount * 0.5f);    //절반을 가져온다.
				this.AddStackCount(-pickCnt, true);

				//일부만 가져옵니다(새로 만들어서)
				InvenBase.choiced_item = (InvenItemData)this.owner.CreateItemData(null, this.transform.parent.parent.parent.parent, this.panel, this.slot, this.database, ref pickCnt, false);
				InvenBase.choiced_item.GetComponent<CanvasGroup>().blocksRaycasts = false;
				return;
			}

			//다른 아이템(ID)을 들고 있으면 pickup 불가능
			if (InvenBase.choiced_item.database.id != this.database.id)
				return;

			//주울 수 있는 최대수는 아이템 개수를 초과할 수 없다.
			if (this.amount <= 1)   pickCnt = this.amount;
			else                    pickCnt = (int)(this.amount * 0.5f);    //절반을 가져온다.
			//인벤에서는 개수만큼 빼준다.
			this.AddStackCount(-pickCnt, true);

			//겹쳐 들어 줍니다.
			pickCnt = InvenBase.choiced_item.CheckOverlapItem(pickCnt);

			this.AddStackCount(pickCnt, true);//남은거는 더해준다.
			//남은 것이 없다면...삭제
			if (this.amount <= 0) Managers.Resource.Destroy(this.gameObject);

		}

		//RETURN : 겹치고 남은 아이템 개수를 리턴합니다.
		public virtual int CheckOverlapItem(int add)
		{
			//cnt가 (+)이면 최대개수 초과를 의미합니다.
			int cnt = this.amount + add - ((ItemBase)this.database).Stackable;
			if (cnt <= 0)
			{
				this.AddStackCount(add, true);
				return 0;
			}

			this.amount = ((ItemBase)this.database).Stackable;
			this.AddStackCount(0, true);
			return cnt;//겹치고 남은 개수
		}


		//bool CheckOverlapPickup(InvenItemData target, InvenItemData additem)
		//{
		//    //들고 있는것이 없으니 pickup가능
		//    if (null == target)
		//        return true;

		//    //잡을 아이템이 없으면 pickup 불가능
		//    if (0 == additem.itembase.id)
		//        return false;

		//    //다른 아이템(ID)을 들고 있으면 pickup 불가능
		//    if (target.itembase.id != additem.itembase.id)
		//        return false;

		//    //HG_TODO : stack count 를 초과해서는 더이상 pickup 불가능
		//    if (target.itembase.Stackable <= target.amount)
		//        return false;

		//    return true;//pickup 가능
		//}

		public InvenItemData PickupAll(Transform parent, bool noti)
		{
			this.transform.SetParent(parent);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
			//UI
			if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, 0);//모두 집어들었기때문에 인벤은 amount=0 입니다.
			return this;
		}

		//겹치고 남은 아이템의 개수를 리턴합니다.
		//this.amount <= 0 일때의 처리는 별도로 진행해 주셔야 됩니다.
		public override int AddStackCount(int add, bool noti)
		{
			//if (0 == add) return add;
			this.amount += add;

			//add가 음수(-)인 경우에 대한 처리.
			if (this.amount <= 0)
			{
				add = this.amount;
				this.amount = 0;
				this.textAmount.text = "";
                //Debug.Log("inven slot amount: " + this.amount);
                //UI
                if (true == noti)//block으로 전달합니다.
					this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount);
				return add;
			}

			//겹친다.(최대를 넘지않는다.)
			if (this.amount <= ((ItemBase)this.database).Stackable)
			{
				//Debug.Log("inven slot amount: " + this.amount);
				this.textAmount.text = this.amount.ToString();
				//UI
				if (true == noti)//block으로 전달합니다.
					this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount);
				return 0;
			}

			//겹치고 남은 수...
			add = this.amount - ((ItemBase)this.database).Stackable;
			this.amount = ((ItemBase)this.database).Stackable;
			this.textAmount.text = this.amount.ToString();
			//Debug.Log("inven slot amount: " + this.amount);
			//UI
			if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount);
			return add;
		}

		public int SetStackCount(int amount, bool noti)
		{
			this.amount = amount;
			//Debug.Log("inven slot amount: " + this.amount);
			if (this.amount <= 0)	this.textAmount.text = "";
            else					this.textAmount.text = this.amount.ToString();

            //UI
            if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount);
			return this.amount;
		}

		//ChestInven에서 변경된 아이템정보를 ChestScript에 반영합니다.
		public void SetInven2Block(int panel, int slot, int id, int amount)
		{
			if (null == this.owner)
				return;
			this.owner.SetInven2Block(panel, slot, id, amount);
		}

	}

}