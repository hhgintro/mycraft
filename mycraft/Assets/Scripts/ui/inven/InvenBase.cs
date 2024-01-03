using FactoryFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class InvenBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		//protected static GameObject _panel { get; private set; }
		//protected static GameObject _slot { get; private set; }
		//protected static GameObject _item { get; private set; }
		//protected static GameObject _reset { get; private set; }
		//protected static GameObject _category { get; private set; }
		//protected static GameObject _hotkey { get; private set; }   //quick hotkey

		//public static GameObject Panel { get { if (null == _panel) _panel = Managers.Resource.Load<GameObject>("prefabs/ui/Slot Panel"); return _panel; } }
		//public static GameObject Slot { get { if (null == _slot) _slot = Managers.Resource.Load<GameObject>("prefabs/ui/Slot"); return _slot; } }
		//public static GameObject Item { get { if (null == _item) _item = Managers.Resource.Load<GameObject>("prefabs/ui/Item"); return _item; } }
		//public static GameObject Reset { get { if (null == _reset) _reset = Managers.Resource.Load<GameObject>("prefabs/ui/Reset"); return _reset; } }
		//public static GameObject Category { get { if (null == _category) _category = Managers.Resource.Load<GameObject>("prefabs/ui/Category"); return _category; } }
		//public static GameObject Hotkey { get {  if(null == _hotkey) _hotkey = Managers.Resource.Load<GameObject>("prefabs/ui/QuickHotkey"); return _hotkey; } }


		public List<InvenPanel> _panels = new List<InvenPanel>();
		protected List<Image> _progress = new List<Image>();


		//ChestInven은 ChestScript와 연결됩니다.
		protected Building _building;
		public static InvenItemData choiced_item = null;     //인벤에서 선택된 개체

		public virtual bool CheckPutdownGoods(int panel, int slot, int itemid)
		{
			if (null == this._building) return true;
			return this._building.CheckPutdownGoods(panel, slot, itemid);
		}
		public virtual bool CheckPickupGoods() { return false; }
		public virtual void OnReset() {}
		public virtual bool AssignRecipe(ItemBase itembase) { return false; }
		public virtual void Clear()
		{
			for (int p = 0; p < this._panels.Count; ++p)
				this._panels[p].Clear();
		}
		public virtual Slot GetInvenSlot(int panel, int slot)
		{
			if (this._panels.Count <= panel)				return null;
			if (this._panels[panel]._slots.Count <= slot)	return null;
			return this._panels[panel]._slots[slot];
		}

		public virtual bool Resize()
		{
			//last panel,slot
			int p = this._panels.Count - 1;
			int s = this._panels[p]._slots.Count - 1;
			RectTransform slot = this._panels[p]._slots[s].GetComponent<RectTransform>();


			//GridLayoutGroup grid = this.GetComponent<GridLayoutGroup>();
			//grid.padding.left;
			RectTransform inven = (RectTransform)this.transform;
			Debug.Log($"Resizing inven rect:({inven.rect} / {inven.sizeDelta}");
			//this.GetComponent<RectTransform>().sizeDelta = new Vector2(332,350);
			this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
			
			//정확한 크기를 어떻게 알수 있을까?(아래코드로 길이는 늘어난다)
			//inven.sizeDelta = new Vector2(inven.sizeDelta.x, 64f);

			return true;
		}

		#region LINK_INVEN
		// destroy : amount가 0이면 파괴
		public virtual void LinkInven(Building building, Dictionary<int/*itemid*/, BuildingItem> inputs, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
			//HG_TEST: 테스트를 위해서 같은 개체일 경우에도 다시 생성시킵니다.(버그 확인후에는 원상복귀합니다)
			//같은 개체일 경우에는 생성을 진행하지 않는다.
			if (null == building)// || this._block == block)
				return;

			//old chest
			if (null != this._building) this._building.SetInven(null);
			//new chest
			this._building = building;
			this._building.SetInven(this);

			//기존정보 삭제
			this.Clear();


			//input
			int p = 0; int s = 0;
			this._panels[p].SetSlots(inputs.Count);
			foreach (int itemid in inputs.Keys)
				this.SetItem(p, s++, itemid, inputs[itemid]._amount, inputs[itemid]._fillAmount, destroy);

			//panel 정보( output )
			for (p = 0; p < panels.Count; ++p)
			{
				//slot
				this._panels[p+1].SetSlots(panels[p]._slots.Count);

				//item 생성
				List<BuildingSlot> slots = panels[p]._slots;
				for (int i = 0; i < slots.Count; ++i)
				{
					if (slots[i]._item._itemid <= 0) continue;
					if (slots[i]._item._amount <= 0) continue;//@@
					this.SetItem(p+1, i, slots[i]._item._itemid, slots[i]._item._amount, slots[i]._item._fillAmount, destroy);
				}
			}

			//progress
			if (null != progresses)
			{
				for (int i = 0; i < progresses.Count; ++i)
					this.SetProgress(i, progresses[i]._fillAmount);
			}
		}

		// destroy : amount가 0이면 파괴
		public virtual void LinkInven(Building building, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
			//HG_TEST: 테스트를 위해서 같은 개체일 경우에도 다시 생성시킵니다.(버그 확인후에는 원상복귀합니다)
			//같은 개체일 경우에는 생성을 진행하지 않는다.
			if (null == building)// || this._block == block)
				return;

			//old chest
			if (null != this._building) this._building.SetInven(null);
			//new chest
			this._building = building;
			this._building.SetInven(this);

			//기존정보 삭제
			this.Clear();


			////slot
			//this.slotAmount = slotAmount;
			//InitSlot();

			if (null == panels || this._panels.Count != panels.Count)
			{
				Debug.LogError("Different Panel Count(block/inven): " + panels.Count + "/" + this._panels.Count);
				return;
			}

			//panel 정보
			for (int p = 0; p < panels.Count; ++p)
			{
				//slot
				this._panels[p].SetSlots(panels[p]._slots.Count);

				//item 생성
				List<BuildingSlot> slots = panels[p]._slots;
				for (int i = 0; i < slots.Count; ++i)
				{
					if (slots[i]._item._itemid <= 0) continue;
					if (slots[i]._item._amount <= 0) continue;//@@
					this.SetItem(p, i, slots[i]._item._itemid, slots[i]._item._amount, slots[i]._item._fillAmount, destroy);
				}
			}

			//progress
			if (null != progresses)
			{
				for (int i = 0; i < progresses.Count; ++i)
					this.SetProgress(i, progresses[i]._fillAmount);
			}
		}

		//ChestInven에서 변경된 아이템정보를 ChestScript에 반영합니다.
		public virtual void SetInven2Block(int panel, int slot, int itemid, int amount, float fillAmount)
		{
			if (null == this._building) return;
			this._building.SetItem(panel, slot, itemid, amount, fillAmount);
		}

		public virtual void SetProgress(int id, float fillAmount)
		{
			if (this._progress.Count <= id) return;
			this._progress[id].fillAmount = fillAmount;
		}
		#endregion //..LINK_INVEN

		#region CreateObject
		protected virtual GameObject CreateObject(Transform parent, GameObject itemObj)
		{
			//GameObject clone = UnityEngine.Object.Instantiate(itemObj);
			GameObject clone = Managers.Resource.Instantiate(itemObj, null);
			clone.transform.SetParent(parent, false); //[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
			//clone.transform.position = parent.position;   //이거때문에 아이콘이 우측아래에 찍힘.
			return clone;
		}

		protected virtual ItemData CreateObject(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database, GameObject itemObj)
		{
			GameObject clone = CreateObject(parent, itemObj);
			if(null == clone) return null;

			clone.GetComponent<Image>().sprite = database.icon;
			clone.name = database.Title;
			//clone.GetComponent<CanvasGroup>().blocksRaycasts = false;//인벤에 생성할때는 true입니다.

			ItemData itemData = clone.GetComponent<ItemData>();
			itemData.owner      = owner;
			itemData.database   = database;
			itemData.panel      = panel;
			itemData.slot       = slot;
			
			itemData.fnStart();
			return itemData;
		}

		public virtual ItemData CreateItemData(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database, ref int amount, ref float fillAmount
			, bool noti)
		{
			ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, Managers.Resource.Load<GameObject>("prefabs/ui/Item"));
			//겹치고 남은 아이템 개수
			amount = itemData._AddStackCount(amount, ref fillAmount, noti);
			return itemData;
		}


		public virtual void CreateTimeData(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database, float time)
		{
			ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, Managers.Resource.Load<GameObject>("prefabs/ui/Time"));
			//겹치고 남은 아이템 개수
			itemData.textAmount.text = time.ToString();
			//amount = InvenItemData.AddStackCount(amount, noti);
			//return InvenItemData;
		}

		public virtual void CreateTechData(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database)
		{
			ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, Managers.Resource.Load<GameObject>("prefabs/ui/Tech"));

			RectTransform rt = (RectTransform)itemData.transform;
			rt.sizeDelta = Vector2.one * 128f;
		}
        public virtual GameObject CreateTechCancel(Transform parent)
        {
            return this.CreateObject(parent, Managers.Resource.Load<GameObject>("prefabs/ui/Tech-Cancel"));

            //ItemData itemData =
            //RectTransform rt = (RectTransform)itemData.transform;
            ////rt.sizeDelta = Vector2.one * 128f;
        }

        public virtual void CreateCategory(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database)
		{
			SkillGroupData itemData = (SkillGroupData)this.CreateObject(owner, parent, panel, slot, database, Managers.Resource.Load<GameObject>("prefabs/ui/Category"));
			itemData.category = slot;

			RectTransform rt = (RectTransform)itemData.transform;
			rt.sizeDelta = Vector2.one * 128f;
		}

		public virtual void CreateSkillData(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database, int amount)
		{
			ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, Managers.Resource.Load<GameObject>("prefabs/ui/Skill"));
			RectTransform rt = (RectTransform)itemData.transform;
			rt.sizeDelta = Vector2.one * 64f;

			if (null != itemData.textAmount)
				itemData.textAmount.text = (0 < amount) ? amount.ToString() : "";
		}

		public virtual void CreateResetData(InvenBase owner, Transform parent
			, int panel, int slot, JSonDatabase database, int amount)
		{
			ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, Managers.Resource.Load<GameObject>("prefabs/ui/Reset"));
			RectTransform rt = (RectTransform)itemData.transform;
			rt.sizeDelta = Vector2.one * 64f;

			if (null != itemData.textAmount)
				itemData.textAmount.text = (0 < amount) ? amount.ToString() : "";
		}
		#endregion //..CreateObject

		#region ADD_ITEM
		//Inven에 있는 아이템 개수의 합
		public int GetAmount(int itemid)
		{
			int amount = 0;
			for (int p = 0; p < this._panels.Count; ++p)
			{
				for (int s = 0; s < this._panels[p]._slots.Count; ++s)
				{
					ItemData itemData = this._panels[p]._slots[s].GetItemData();
					if (null == itemData) continue;
					//같은 아이템인지 체크
					if (itemid != itemData.database.id) continue;
					amount += itemData.amount;
				}
			}
			return amount;
		}

		//Block에서 변경된 내용을 Inven에 반영합니다.
		// destroy : amount가 0이면 파괴
		public void SetItem(int panel, int slot, int itemid, int amount, float fillAmount, bool destroy)
		{
			Slot s = this.GetInvenSlot(panel, slot);
			if (null == s) return;

			InvenItemData itemData = (InvenItemData)s.GetItemData();
			if (null != itemData)
			{
				Color color = itemData.GetComponent<Image>().color;
				color.a = 1.0f;

                if (amount <= 0)
				{
					//이미지를 남길경우에 대한 처리필요
					if (true == destroy)
					{
						Managers.Resource.Destroy(itemData.gameObject);
						return;
					}
					color.a = 0.3f;
				}
				itemData.SetStackCount(amount, fillAmount, false);
                itemData.GetComponent<Image>().color = color;
                return;
            }

			if(amount <= 0) return;

			//생성
			//database
			ItemBase itemToAdd = Managers.Game.ItemBases.FetchItemByID(itemid);
			if (null == itemToAdd)
			{
				Debug.LogError($"Database is empty({itemid}) : Need Checking Script Execute Order");
				return;
			}

			this.CreateItemData(this, s.transform, panel, slot, itemToAdd, ref amount, ref fillAmount, false);
		}

		//InvenItemData가 없이,
		//id로 아이템을 추가하고가 할때 사용합니다.
		//InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
		//bCreate: false이면 자리가 있어도 생성하지 않는다.(true이면 빈자리에 생성해 준다.)
		public virtual int AddItem(int id, int itemcount, ref float fillAmount, bool bCreate = true)
		{
			//들고 있는 아이템이면...
			if (null != InvenBase.choiced_item)
			{
				if (InvenBase.choiced_item.database.id == id)
				{
					itemcount = InvenBase.choiced_item._AddStackCount(itemcount, ref fillAmount, true);
					if (itemcount <= 0) return 0;
				}
			}

			//겹치기
			itemcount = this.OnOverlapItem(id, itemcount, ref fillAmount);
			//더이상 추가할 것이 없다.
			if (itemcount <= 0) return 0;
			//빈자리가 있더라도, 생성하지는 않는다.
			if (false == bCreate) return itemcount;

			//database
			ItemBase itemToAdd = Managers.Game.ItemBases.FetchItemByID(id);
			if (null == itemToAdd)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
				return itemcount;
			}

			//겹치지 못하고 남은 것이 있다면...생성해서 넣어줍니다.
			itemcount = this.OnCreateItemData(itemToAdd, itemcount, ref fillAmount);
			return itemcount;
		}

		//InvenItemData를 인벤에 넣어줄때 사용합니다.
		//InvenItemData 없이, id로 아이템을 추가하고자 할때에는 AddItem(int id, int itemcount)를 사용하세요.
		public virtual int AddItem(InvenItemData itemData, ref float fillAmount)
		{
			//겹치기
			itemData.amount = this.OnOverlapItem(itemData.database.id, itemData.amount, ref fillAmount);
			//더이상 추가할 것이 없다.
			if (itemData.amount <= 0)
			{
				Managers.Resource.Destroy(itemData.gameObject);
				return 0;   //남은 개수
			}

			//겹치지 못하고 남은 것이 있다면...빈자리를 찾아서 넣어줍니다.
			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<Slot> slots = this._panels[p]._slots;
				for (int i = 0; i < slots.Count; ++i)
				{
					//빈자리를 찾아서...
					if (1 < slots[i].transform.childCount)  //1: slot에 background가 추가되었으므로.
						continue;

					slots[i].AddItem(itemData, ref fillAmount);
					return 0;   //남은 개수
				}
			}
			return itemData.amount;
		}

		//RETURN: 더 빼야하는 아이템 개수를 리턴합니다.
		public virtual int SubItem(int itemid, int amount)
		{
			if (amount < 0) return 0;

			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<Slot> slots = this._panels[p]._slots;
				//뒤에서 부터 아이템을 빼준다.
				for (int i = slots.Count - 1; 0 <= i; --i)
				{
					InvenItemData itemData = (InvenItemData)slots[i].GetItemData();
					if (null == itemData) continue;
					//같은 아이템인지 체크
					if (itemid != itemData.database.id) continue;

					//
					if (amount < itemData.amount)
					{
						itemData.amount -= amount;
						itemData.SetStackCount(itemData.amount, MyCraft.Global.FILLAMOUNT_DEFAULT, true);
						return 0;
					}

					amount -= itemData.amount;
					itemData.amount = 0;
					Managers.Resource.Destroy(itemData.gameObject);
					if (amount < 0)
						return 0;
				}
			}
			return 0;
		}

		//겹치기
		//RETURN : 겹치고 남은 개수를 리턴합니다.
		protected virtual int OnOverlapItem(int itemid, int itemcount, ref float fillAmount)
		{
			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<Slot> slots = this._panels[p]._slots;
				for (int i = 0; i < slots.Count; ++i)
				{
					InvenItemData itemData = (InvenItemData)slots[i].GetItemData();
					if (null == itemData) continue;
					if (itemData.database.id != itemid) continue;

					//겹치고 남은 개수를 리턴합니다.
					itemcount = itemData._AddStackCount(itemcount, ref fillAmount, true);
					//InvenItemData.AddStackCount(0);

					//남은 개수가 없다.
					if (itemcount <= 0)
						break;
				}
			}

			return itemcount;
		}

		protected virtual int OnCreateItemData(ItemBase itemToAdd, int itemcount, ref float fillAmount)
		{
			for (int p = 0; p < this._panels.Count; ++p)
			{
				List<Slot> slots = this._panels[p]._slots;
				for (int i = 0; i < slots.Count; ++i)
				{
					//빈자리를 찾아서...
					if (1 < slots[i].transform.childCount)	//1: slot에 background가 추가되었으므로.
						continue;

					//this.items[i] = itemToAdd;
					this.CreateItemData(this, slots[i].transform, p, i, itemToAdd, ref itemcount, ref fillAmount, true);

					//더이상 추가할 것이 없다.
					if (itemcount <= 0)
						break;
				}
			}
			return itemcount;
		}
		#endregion  //..ADD_ITEM

		#region ADD_SKILL
		public virtual void AddReset(InvenPanel panel)
		{
			int id = 0;//reset
			ItemBase itembase = Managers.Game.ItemBases.FetchItemByID(id);
			if (null == itembase)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
				return;
			}

			Slot slot = panel.CreateSlot();
			this.CreateResetData(this, slot.transform, panel._panel, slot._slot, itembase, 0);
		}

		//InvenItemData가 없이,
		//id로 아이템을 추가하고가 할때 사용합니다.
		//InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
		public virtual void AddSkill(InvenPanel panel, int itemid, int amount)
		{
			if (itemid <= 0)
			{
				Debug.LogError("you need check skill id: " + itemid);
				return;
			}

			//database
			ItemBase itembase = Managers.Game.ItemBases.FetchItemByID(itemid);
			//ItemBase itemToAdd = GameManager.GetItemBase().FetchItemByID(id);
			if (null == itembase)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + itemid + "]");
				return;
			}


			Slot slot = panel.CreateSlot();
			this.CreateSkillData(this, slot.transform, panel._panel, slot._slot, itembase, amount);
		}

		public virtual void AddCategory(InvenPanel panel, int category)
		{
			//database
			Category categories = Managers.Game.Categories.FetchItemByID(category);
			//ItemBase itemToAdd = GameManager.GetItemBase().FetchItemByID(id);
			if (null == categories)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + category + "]");
				return;
			}


			Slot slot = panel.CreateSlot();
			this.CreateCategory(this, slot.transform, panel._panel, slot._slot, categories);
		}
		#endregion //..ADD_SKILL

		#region POINT_ENTER
		public void OnPointerEnter(PointerEventData eventData)
		{
			ActiveIcon();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			DeactiveIcon();
		}

		public virtual void ActiveIcon()
		{
			if (null == InvenBase.choiced_item)
				return;

			//Managers.Game.DestoryBuilding();
			////GameManager.GetTerrainManager().SetChoicePrefab((BlockScript)null);
			InvenBase.choiced_item.GetComponent<Image>().enabled = true;
		}
		public virtual void DeactiveIcon()
		{
			if (null == InvenBase.choiced_item) return;
			//자원(광물)은 인벤을 벗어나도 icon상태를 유지합니다
			//if (InvenBase.choiced_item.database.type < BLOCKTYPE.CHEST) return;
			if (0 == InvenBase.choiced_item.database.type.CompareTo("ItemBase")) return;

			Managers.Game.PlaceBuilding(InvenBase.choiced_item);
			//GameManager.GetTerrainManager().SetChoicePrefab((ItemBase)InvenBase.choiced_item.database);
			//GameManager.GetMouseController().mouse_refresh = true;//prefab을 찍을 수 있도록 설정
			InvenBase.choiced_item.GetComponent<Image>().enabled = false;
		}
		#endregion //..POINT_ENTER

		#region SAVE
		public virtual void Save(BinaryWriter bw)
		{
			//1. slot amount
			bw.Write(this._panels[0]._slots.Count);

			//임시 List<> 에 저장
			List<InvenItemData> items = new List<InvenItemData>();
			for (int i = 0; i < this._panels[0]._slots.Count; ++i)
			{
				InvenItemData itemData = (InvenItemData)this._panels[0]._slots[i].GetItemData();
				if (null == itemData) continue;
				items.Add(itemData);
			}

			//2. item count
			bw.Write(items.Count);
			//3. item info
			for (int i = 0; i < items.Count; ++i)
			{
				bw.Write(items[i].slot);		//slot
				bw.Write(items[i].database.id); //item id
				bw.Write(items[i].amount);		//amount
				bw.Write(items[i]._fillAmount); //fillAmount
			}
		}

		public virtual void Load(BinaryReader reader)
		{
			//1. slot amount
			int slotAmount = reader.ReadInt32();
			this._panels[0].SetSlots(slotAmount);
			//Debug.Log("slot = " + slotAmount);

			//2. item count
			int itemcount = reader.ReadInt32();
			//Debug.Log("itemcount = " + itemcount);
			//3. item info
			for (int i = 0; i < itemcount; ++i)
			{
				int panel			= 0;
				int slot			= reader.ReadInt32();
				int id				= reader.ReadInt32();
				int amount			= reader.ReadInt32();
				float fillAmount	= reader.ReadSingle();
				//Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

				SetItem(panel, slot, id, amount, fillAmount, false);
			}
		}
		#endregion //..SAVE
	}
}//..namespace MyCraft
