using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using Unity.VisualScripting;

namespace MyCraft
{
	//SlotPanel --> PanelBase
	public class PanelBase<T>
	{
		public int _panel;  //자신의 번호
		public int _slot { get; private set; } //slot 개수

		public List<T> _slots = new List<T>();


		public PanelBase(int panel)
		{
			this._panel = panel;
			//this._amount = amount;
			//this._progress = progress;

			//for (int i = 0; i < amount; ++i)
			//    //this._slots.Add(new BlockSlot(panel));
			//    this._slots.Add(default(T));

			//this.SetAmount(amount);
		}

		public virtual void Clear()
		{
			this._slots.Clear();
		}

		//slot 개수를 설정합니다.
		public virtual void SetSlots(int slot)
		{
			//this.Clear();
			this._slot = slot;
			//for (int i = 0; i < amount; ++i)
			//    //this._slots.Add(new BlockSlot(panel));
			//    this._slots.Add(default(T));
		}

	}

	//BlockSlot --> BuildingSlot
	//BlockSlotPanel --> BuildingPanel
	public class BuildingPanel : PanelBase<BuildingSlot>
	{
		public BuildingPanel(int panel, int slot)
			: base(panel)
		{
			SetSlots(slot);
		}

		public override void Clear()
		{
			base.Clear();
		}

		public override void SetSlots(int slot)
		{
			base.SetSlots(slot);
			for (int i = 0; i < slot; ++i)
				base._slots.Add(new BuildingSlot(base._panel, i));
		}

		//amount : 필요한 개수이상이면 해당slot정보가 리턴됩니다.
		public BuildingSlot GetFillSlot(int amount)
		{
			//뒤에서 체크하는 이유는 중간에 뺄때...crash방지차원.
			for (int i = base._slots.Count - 1; 0 <= i; --i)
			{
				if (amount <= base._slots[i].GetItemAmount())
					return base._slots[i];
			}
			return null;
		}
		//amount만큼 있는면 true
		public bool GetFillSlot(int slot, int amount)
		{
			return (amount <= base._slots[slot].GetItemAmount());
		}

		public bool IsFull(int itemid)
		{
			for (int i = 0; i < base._slots.Count; ++i)
			{
				if (0 == base._slots[i]._itemid)
					return false;   //output에 여유가 있다.

				//다른 아이템
				if (itemid != base._slots[i]._itemid)
					continue;

				ItemBase itembase = Managers.Game.ItemBases.FetchItemByID(base._slots[i]._itemid);
				if (null == itembase) continue;

				if (base._slots[i]._amount < itembase.Stackable)
					return false;   //output에 여유가 있다.
			}
			return true;//가득 참.
		}
		public bool IsFull()
		{
			for (int i = 0; i < base._slots.Count; ++i)
			{
				if (0 == base._slots[i]._itemid) return false;

				MyCraft.ItemBase itembase = MyCraft.Managers.Game.ItemBases.FetchItemByID(base._slots[i]._itemid);
				if (null == itembase) return true;
				if (itembase.Stackable <= base._slots[0]._amount) return true;
			}
			return false;
		}

	}

	//InvenSlotPanel --> InvenPanel
	public class InvenPanel : PanelBase<Slot>
	{
		public Image _progress;

		private InvenBase _inven;
		private Transform _parent;
		//private GameObject _invenSlot;

		//public int _panel;  //자신의 번호
		//public int _amount;

		//public List<Slot> _slots = new List<Slot>();
		////public List<T> _slots = new List<T>();

		public InvenPanel(int panel, int slot, InvenBase inven, Transform parent)
			: base(panel)
		{
			this.Clear();

			this._inven     = inven;
			this._parent    = parent;
			//this._invenSlot = invenSlot;
			//this._panel   = panel;

			this.SetSlots(slot);
		}

		public override void Clear()
		{
			//this._objPanel = null;
			//this._panel = 0;
			this.SetSlots(0);
			for (int i = 0; i < base._slots.Count; ++i)
			{
				ItemData itemData = base._slots[i].GetItemData();
				if (null != itemData)
					Managers.Resource.Destroy(itemData.gameObject);
				Managers.Resource.Destroy(base._slots[i].gameObject);
			}
			base.Clear();
		}

		public override void SetSlots(int slot)
		{
			base.SetSlots(slot);
			for (int i = 0; i < slot; ++i)
				this.CreateSlot();
		}

		public virtual Slot CreateSlot()
		{
			//HG_TODO: [2023.06.15]Pool을 통해 slot을 가져오면, canvas비율때문에 크게 표시되는 현상이 있어서.
			//  우선은 Resource에서 직접로드로 처리함. 추후 pool을 고려할 것
			//Slot s = Managers.Resource.Instantiate("ui/Slot", this._parent).GetComponent<Slot>();
			////s.transform.SetParent(this._parent, true);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
			GameObject go = Resources.Load<GameObject>($"Prefabs/ui/Slot");
			Slot s = UnityEngine.Object.Instantiate(go, this._parent).GetComponent<Slot>();

			s._panel    = this._panel;
			s._slot     = base._slots.Count;
			s.owner     = this._inven;
			base._slots.Add(s);
			return s;
		}

	}
}