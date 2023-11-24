using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
	public class TechDescription : TechInvenBase
	{
		private TechBase _techbase;

		public InvenPanel _panelTitle;   //title image
		public InvenPanel _panelCost;
		public InvenPanel _panelPreTech;
		public InvenPanel _panelNextTech;
		public InvenPanel _panelReward;

		//public GameObject inventorySlot;
		//public GameObject inventoryItem;
		public GameObject _inventoryMultiple;

		void Awake()
		{
			base.Init();

			//this.database = GetComponent<ItemDatabase>();
			//this.inventoryPanel = GameObject.Find("Item_Canvas/Inventory/Inventory Panel").gameObject;
			//this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;

			//base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
		}

		void Start()
		{

			foreach (var tech in Managers.Game.TechBases.database)
			{
				this.LinkTech(tech.Value);
				break;//맨처음꺼를 설정해주고 멈춘다.
			}

			Managers.Locale.SetLocale("technology", this.transform.Find("Cost/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("technology", this.transform.Find("Pre-Tech/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("technology", this.transform.Find("Next-Tech/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("technology", this.transform.Find("Reward/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("technology", this.transform.Find("Research/Text").GetComponent<Text>());
		}

		public override void Clear()
		{
			if (null != _panelTitle) _panelTitle.Clear();
			if (null != _panelCost)  _panelCost.Clear();
			if (null != _panelPreTech) _panelPreTech.Clear();
			if (null != _panelNextTech) _panelNextTech.Clear();
			if (null != _panelReward) _panelReward.Clear();
		}

		public void LinkTech(TechBase techbase)
		{
			//HG_TEST : 디버깅을 위해 주석처리함( 상용화시 주석해제할 것 )
			//동일하면...무시
			//if (this._techbase == techbase) return;

			this._techbase = techbase;
			Clear();

			//title
			//title text
			this.transform.GetChild(0).GetComponent<Text>().text = techbase.Title;
			//this.transform.FindChild("Slot/Image").GetComponent<Image>().sprite = techbase.Sprite;

			//title-slot
			this._panelTitle = new InvenPanel(0, 0, this, this.transform.Find("Slot Panel"));
			//title-item
			this.AddTech(this._panelTitle, techbase.id);
			////color
			//if (techbase.prev_techs.Count <= 0)
			//    this.transform.FindChild("Slot").GetComponent<Image>().color = InvenBase.Slot_Yellow;
			//else
			//    this.transform.FindChild("Slot").GetComponent<Image>().color = InvenBase.Slot_Red;

			//cost-slot
			this._panelCost = new InvenPanel(0, 0, this, this.transform.Find("Cost/Slot Panel"));
			//cost-item
			this.AddTime(this._panelCost, 1, techbase._cost.time);
			for (int i = 0; i < techbase._cost.items.Count; ++i)
				this.AddItem(this._panelCost, techbase._cost.items[i].itemid, techbase._cost.items[i].amount);
			//multiple
			Slot multiple = UnityEngine.Object.Instantiate(this._inventoryMultiple).GetComponent<Slot>();
			this._panelCost._slots.Add(multiple);
			multiple._panel = this._panelCost._panel;
			//multiple.panel = this._panelCost._slots.Count - 1;
			multiple.owner = this;
			multiple.transform.SetParent(this.transform.Find("Cost/Slot Panel").gameObject.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
			multiple.GetComponent<Text>().text = " x " + techbase._cost.mulitple.ToString();
			//pre-tech
			if(0 < techbase.prev_techs.Count)
			{
				this._panelPreTech = new InvenPanel(1, 0, this, this.transform.Find("Pre-Tech/Slot Panel"));
				for (int i = 0; i < techbase.prev_techs.Count; ++i)
					this.AddTech(this._panelPreTech, techbase.prev_techs[i]);
			}
			//next-tech
			if (0 < techbase.next_techs.Count)
			{
				_panelNextTech = new InvenPanel(2, 0, this, this.transform.Find("Next-Tech/Slot Panel"));
				for (int i = 0; i < techbase.next_techs.Count; ++i)
					this.AddTech(this._panelNextTech, techbase.next_techs[i]);
			}
			//reward
			if(0 < techbase.rewards.Count)
			{
				this._panelReward = new InvenPanel(3, 0, this, this.transform.Find("Reward/Slot Panel"));
				for (int i = 0; i < techbase.rewards.Count; ++i)
					this.AddSkill(this._panelReward, techbase.rewards[i], 0);
			}
		}

		public void AddTime(InvenPanel panel, int id, float time)
		{
			//database
			ItemBase itemToAdd = Managers.Game.ItemBases.FetchItemByID(id);
			if(null == itemToAdd)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
				return;
			}

			Slot slot = panel.CreateSlot();
			this.CreateTimeData(this, slot.transform, panel._panel, slot._slot, itemToAdd, time);
		}

		//InvenItemData가 없이,
		//id로 아이템을 추가하고가 할때 사용합니다.
		//InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
		public int AddItem(InvenPanel panel, int id, int itemcount)
		{
			//database
			ItemBase itemToAdd = Managers.Game.ItemBases.FetchItemByID(id);
			if (null == itemToAdd)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
				return itemcount;
			}

			//겹치지 못하고 남은 것이 있다면...생성해서 넣어줍니다.
			Slot slot = panel.CreateSlot();
			itemcount = this.OnCreateItemData(panel, itemToAdd, itemcount);
			return itemcount;
		}

		protected virtual int OnCreateItemData(InvenPanel panel, JSonDatabase itemToAdd, int itemcount)
		{
			List<Slot> slots = panel._slots;
			for (int i = 0; i < slots.Count; ++i)
			{
				//빈자리를 찾아서...
				if (1 < slots[i].transform.childCount)  //1: slot에 background가 추가되었으므로.
					continue;

				//this.items[i] = itemToAdd;
				this.CreateItemData(this, slots[i].transform, panel._panel, i, itemToAdd, ref itemcount, true);

				//더이상 추가할 것이 없다.
				if (itemcount <= 0)
					break;
			}
			return itemcount;
		}
 
		public void OnResearchClicked()
		{
			if(true == _techbase.learned)
			{
				Debug.Log(_techbase.id + "이미 연구 완료했습니다.");
				return;
			}
			foreach (var prev in _techbase.prev_techs)
			{
				TechBase itemPrev = Managers.Game.TechBases.FetchItemByID(prev);
				if (false == itemPrev.learned)
				{
					Debug.Log($"이전 연구({itemPrev.Title})가 완료되지 않았습니다.");
					return;
				}
			}
			//연구소에 "연구 등록"을 진행합니다.
			if(true == Managers.Game.TechInvens.OnResearch(_techbase))
				Debug.Log(_techbase.id + " 연구 등록");
            return;

			//_techbase.learned = true;
			//Debug.Log(_techbase.id + " 연구 완료");

			//이 창에서는 등록된 Tech와 허가 Tech만 색을 변경하면되고
			//TechInven 창에서는
			//_techbase는 GREEN, _techbase.next는 색이변경되어야 하는지 체크(prev)
			//..

			List<int> nexts = new List<int>();  //_techbase연구에 의해 색이변하는 next만

			//title-slot
			for (int s = 0; s < this._panelTitle._slots.Count; ++s)
			{
				this._panelTitle._slots[s].GetComponent<Image>().color = Color.green;
			}
			//next-tech
			for (int s = 0; s < this._panelNextTech._slots.Count; ++s)
			{
				Slot slot = this._panelNextTech._slots[s];
				TechBase techbase = (TechBase)slot.GetItemData().database;

				bool all_learned = true;   //prev모두 연구완료했는가?
				foreach (var prev in techbase.prev_techs)
				{
					TechBase itemPrev = Managers.Game.TechBases.FetchItemByID(prev);
					if (false == itemPrev.learned)
					{
						//Debug.Log($"이전 연구({itemPrev.Title})가 완료되지 않았습니다.");
						all_learned = false;
						break;
					}
				}

				if (all_learned)
				{
					slot.GetComponent<Image>().color = Color.yellow;
					nexts.Add(techbase.id);
				}
				else slot.GetComponent<Image>().color = Color.red;
			}

			//tech inven
			Managers.Game.TechInvens.OnResearchCompleted(_techbase.id, nexts);
		}

		public override bool AssignRecipe(ItemBase itembase) { return true; }
	}//..class TechDescription
}//..namespace MyCraft