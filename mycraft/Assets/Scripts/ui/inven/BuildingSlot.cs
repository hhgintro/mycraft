using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCraft
{
	public class BuildingSlot
	{
		public int _panel;  //owner의 번호
		public int _slot;   //자신의 구분자

		//public int _itemid;//item id
		//public int _amount;
		//public float _fillAmount;   //아이템 소진률(0~1.0)
		public BuildingItem _item = new BuildingItem();

		public BuildingSlot(int panel, int slot, int itemid, int amount, float fillAmount)
		{
			this._panel = panel;
			this._slot  = slot;

			_item._itemid		= itemid;
			_item._amount		= amount;
			_item._fillAmount	= fillAmount;
		}

		public void Clear()
		{
			//_itemid = 0;
			//_amount = 0;
			//_fillAmount = 0.0f;
			_item.Clear();
		}
		//amount만큼 있는면 true
		public int GetItemAmount()
		{
			//itemid
			if (0 == this._item._itemid) return 0;
			//amount
			return this._item._amount;
		}

		//public bool OnOverlapItem(int itemid, int amount, float fillAmount, int stackable)
		//{
		//	//itemID가 다르면...무시
		//	if (this._item._itemid != itemid) return false;
		//	//가득차면...무시
		//	if (stackable <= this._item._amount) return false;

		//	this._item._amount += amount;
		//	this._item._fillAmount = fillAmount;
		//	return true;
		//}

		//public bool OnCreateItemData(int itemid)
		//{
		//	if (0 != this._item._itemid) return false;

		//	this._item._itemid = itemid;
		//	this._item._amount = 1;
		//	this._item._fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;
		//	return true;
		//}

	}
}