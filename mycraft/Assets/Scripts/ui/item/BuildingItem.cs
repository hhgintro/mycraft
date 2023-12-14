using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class BuildingItem
	{
		public int _itemid;			//아이템 id
		public int _amount;			//아이템 개수
		public float _fillAmount;   //아이템(물약) 소진률(0~1.0)

		public BuildingItem()
		{
			this.Clear();
		}

		public void Clear()
		{
			_itemid = 0;
			_amount = 0;
			_fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;    //리셋
		}
	}

}