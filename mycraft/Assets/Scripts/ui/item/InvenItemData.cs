using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
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

		public float _fillAmount;	//아이템(물약) 소진률(0~1.0)
		//public MyCraft.Progress _progress;  //물약은 남은시간(ms), 탄창은 남은 탄수(count)
		GameObject _progressFill;	//아이템(물약) 소진률(0~1.0) : 물약은 남은시간(ms), 탄창은 남은 탄수(count)

		bool isTest = false;

		public override void InitStart()
		{
			//필요한 경우만 progress를 노출한다.
			if (null == _progressFill && 1 < this.transform.childCount)
			{
				_progressFill = this.transform.GetChild(1).gameObject;
				ItemBase itembase = (ItemBase)this.database;
				if (itembase.progress)
				{
					SetFillAmount(MyCraft.Global.FILLAMOUNT_DEFAULT);
					_progressFill?.SetActive(true);
				}
				else
				{
					_progressFill?.SetActive(false);
				}
			}
		}

		protected override void OnMouseLButtonDown()
		{
			//pickAll의 경우에 교체시에는 pickup이 마지막으로 처리되어 0이 되는 결과가 발생
			bool noti2block = true;

			//디버깅용
			if (true == isTest)
			{
				isTest = false;
				SetFillAmount(0.4f);
			}

			//pickup할 수 없다.
			if (false == this.owner.CheckPickupGoods())
			{
				Debug.Log(this.owner.ToString() + "은 pickup 할 수 없습니다.");
				return;
			}

			//들고 있다면...
			if (null != InvenBase.choiced_item)
			{
				//같은 아이템을 들고 있다면, 겹치기(내려놓는다)
				if (this.database.id == InvenBase.choiced_item.database.id)
				{
					//겹치고(내려놓고) 남은 개수를 리턴합니다.
					float fillAmount = InvenBase.choiced_item.GetFillAmount();
					int amount = InvenBase.choiced_item._SetStackCount(this._AddStackCount(InvenBase.choiced_item.amount, ref fillAmount, true), fillAmount, false);
					if (amount <= 0)
					{
						Managers.Resource.Destroy(InvenBase.choiced_item.gameObject);
						InvenBase.choiced_item = null;
						Managers.Game.DestoryBuilding();
					}
					return;
				}

				//내려놓을 수 있는지 확인한다.
				if (false == this.owner.CheckPutdownGoods(this.panel, this.slot, InvenBase.choiced_item.database.id))
					return;

				//들고 있던건 내려놓고(다른아이템인 경우)
				//this.owner.slots[this.slot].AddItem(InvenBase.choiced_item);
				Slot s = this.owner.GetInvenSlot(this.panel, this.slot);
				s?.PutdownChoicedItem();
				noti2block = false;//교체시에는 Pickup을 block으로 전달하지 않는다.
			}

			//모두 집어든다.(아이템이 있다면)
			if (0 != this.database.id)
			{
				if(this.amount <= 0)
				{
					Managers.Resource.Destroy(this.gameObject);
					return;
				}
				InvenBase.choiced_item = this.PickupAll(this.transform.parent.parent.parent.parent, noti2block);
			}
		}

		protected override void OnMouseRButtonDown()
		{
			//들 수 없다.
			if (false == this.owner.CheckPickupGoods())
			{
				Debug.Log(this.owner.ToString() + "은 pickup 할 수 없습니다.");
				return;
			}

			float fillAmount = this.GetFillAmount();
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
				pickCnt = (int)(this.amount * 0.5f + 0.5f);    //절반을 가져온다.
				this._SubStackCount(pickCnt, true);

				//일부만 가져옵니다(새로 만들어서)
				InvenBase.choiced_item = (InvenItemData)this.owner.CreateItemData(null, this.transform.parent.parent.parent.parent, this.panel, this.slot, this.database, ref pickCnt, ref fillAmount, false);
				InvenBase.choiced_item.GetComponent<CanvasGroup>().blocksRaycasts = false;
				return;
			}

			//다른 아이템(ID)을 들고 있으면 pickup 불가능
			if (InvenBase.choiced_item.database.id != this.database.id)
				return;

			//주울 수 있는 최대수는 아이템 개수를 초과할 수 없다.
			//if (this.amount <= 1)   pickCnt = this.amount;
			//else                    pickCnt = (int)(this.amount * 0.5f);    //절반을 가져온다.
			pickCnt = (int)(this.amount * 0.5f + 0.5f);    //절반을 가져온다.
			this._SetStackCount(InvenBase.choiced_item._AddStackCount(pickCnt, ref fillAmount, true), fillAmount, true);//남은거는 더해준다.
			//남은 것이 없다면...삭제
			if (this.amount <= 0) Managers.Resource.Destroy(this.gameObject);
		}

		public InvenItemData PickupAll(Transform parent, bool noti)
		{
			this.transform.SetParent(parent);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
			//UI
			if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, 0, 0.0f);//모두 집어들었기때문에 인벤은 amount=0 입니다.
			return this;
		}

		//RETURN : 겹치고 남은 아이템 개수를 리턴합니다.
		public virtual int CheckOverlapItem(int add, float fillAmount)
		{
			float fillTotal = this.GetFillAmount() + fillAmount;
			if (fillTotal < MyCraft.Global.FILLAMOUNT_DEFAULT) add -= 1;
			fillTotal = Math.Min(fillTotal, MyCraft.Global.FILLAMOUNT_DEFAULT);

			//cnt가 (+)이면 최대개수 초과를 의미합니다.
			int cnt = this.amount + add - ((ItemBase)this.database).Stackable;
			if (cnt <= 0)
			{
				this._AddStackCount(add, ref fillTotal, true);
				return 0;
			}

			this.SetStackCount(((ItemBase)this.database).Stackable, fillTotal, true);
			return cnt;//겹치고 남은 개수
		}

		//겹치고 남은 아이템의 개수를 리턴합니다.
		//this.amount <= 0 일때의 처리는 별도로 진행해 주셔야 됩니다.
		public override int AddStackCount(int add, float fillAmount, bool noti)
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

				this.SetFillAmount(MyCraft.Global.FILLAMOUNT_DEFAULT);

				//UI
				if (true == noti)//block으로 전달합니다.
					this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount, this.GetFillAmount());
				return add;
			}

			add = this.amount - ((ItemBase)this.database).Stackable;
			if(0 <= add) this.amount = ((ItemBase)this.database).Stackable;
			//Debug.Log("inven slot amount: " + this.amount);
			this.textAmount.text = this.amount.ToString();

			this.SetFillAmount(fillAmount);

			//UI
			if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount, this.GetFillAmount());
			return Mathf.Max(0, add);
		}

		public int SetStackCount(int amount, float fillAmount, bool noti)
		{
			this.amount = amount;
			//Debug.Log("inven slot amount: " + this.amount);
			if (this.amount <= 0)	this.textAmount.text = "";
			else					this.textAmount.text = this.amount.ToString();

			this.SetFillAmount(fillAmount);

			//UI
			if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount, this.GetFillAmount());
			return this.amount;
		}

		//추가하고 남은 개수를 리턴한다.
		public override int _AddStackCount(int add, ref float fillAmount, bool noti)
		{
			if (add <= 0) return 0;
			if (((ItemBase)this.database).Stackable <= this.amount) return add;  //가득참

			//아이템 소진률의 합이 "MyCraft.Global.FILLAMOUNT_DEFAULT" 보다 작으면...( 1(0.4) + 1(0.4) = 1(0.8) 이 됩니다.)
			float fillTotal = this.GetFillAmount() + fillAmount;
			//디폴드값 미만은 그값그대로(개수만 -1), 디폴드보다 크면 디폴트값을 뺀값(1.4 -> 0.4)
			if (fillTotal < MyCraft.Global.FILLAMOUNT_DEFAULT)	add -= 1;
			else												fillTotal -= MyCraft.Global.FILLAMOUNT_DEFAULT;
			fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;	//리셋

			int total = this.amount + add;
			//최대개수 초과
			if (((ItemBase)this.database).Stackable < total)
			{
				add = total - ((ItemBase)this.database).Stackable;
				this._SetStackCount(((ItemBase)this.database).Stackable, fillTotal, true);
				return add; //추가하고 남은 개수
			}

			//충분한 경우
			this._SetStackCount(total, fillTotal, true);
			return 0;
		}

		//빼고 모자른 개수를 리턴한다.
		public override int _SubStackCount(int sub, bool noti)
		{
			if (sub <= 0) return 0;
			float fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;	//어짜피 빼면, 리셋된다.

			//부족한 경우
			if (this.amount <= sub)
			{
				sub -= this.amount; //뺀후 부족한 수량.
				_SetStackCount(0, fillAmount, noti);
				return sub; //빼고 모자른 개수
			}

			//충분한 경우
			_SetStackCount(this.amount - sub, fillAmount, noti);
			return 0;
		}

		public int _SetStackCount(int amount, float fillAmount, bool noti)
		{
			this.amount = amount;
			//Debug.Log("inven slot amount: " + this.amount);
			if (this.amount <= 0) this.textAmount.text = "";
			else
			{
				//아이템(물약) 소진률(0~1.0)
				this.SetFillAmount(fillAmount);
				this.textAmount.text = this.amount.ToString();
			}

			if (true == noti)//block으로 전달합니다.
				this.SetInven2Block(this.panel, this.slot, this.database.id, this.amount, fillAmount);
			return this.amount;
		}

		//ChestInven에서 변경된 아이템정보를 ChestScript에 반영합니다.
		public void SetInven2Block(int panel, int slot, int itemid, int amount, float fillAmount)
		{
			if (null == this.owner)	return;
			this.owner.SetInven2Block(panel, slot, itemid, amount, fillAmount);
		}

		public float GetFillAmount()
		{
			if (null == _progressFill) return MyCraft.Global.FILLAMOUNT_DEFAULT;
			return _fillAmount;
		}
		public float SetFillAmount(float fillAmount)
		{
			if (null == _progressFill) return MyCraft.Global.FILLAMOUNT_DEFAULT;
			_fillAmount = fillAmount;
			_progressFill.transform.GetChild(0).GetComponent<Image>().fillAmount = _fillAmount;
			return fillAmount;
		}
	}

}