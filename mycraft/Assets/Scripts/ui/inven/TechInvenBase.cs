using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace MyCraft
{
	public class TechInvenBase : InvenBase, IPointerEnterHandler, IPointerExitHandler
	{
		//protected GameObject inventoryPanel;
		//public List<TechSlotPanel> _panels = new List<TechSlotPanel>();
		//protected GameObject slotPanel;
		
		////protected ItemDatabase database;
		//public GameObject inventorySlot;
		//public GameObject inventoryItem;
		public GameObject inventoryTech;
		//protected CanvasGroup canvas_ui;

		//ChestInven은 ChestScript와 연결됩니다.
		//protected BlockScript _block { get; set; }

		//protected List<Image> _progress = new List<Image>();

		//protected int slotAmount;
		//public List<Item> items = new List<Item>();
		//public List<Slot> slots = new List<Slot>();

		//private Vector2 offset;

		////public static InvenItemData choiced_item = null;     //인벤에서 선택된 개체
		//public static bool bPointerEnter { get; set; }      //인벤위에 마우스가 위치했는지???

		//public static Color Slot_Green = new Color((float)0x00 / 0xff, (float)0xfd / 0xff, (float)0x6a / 0xff, (float)0xff / 0xff);
		//public static Color Slot_Yellow = new Color((float)0xfd / 0xff, (float)0xe1 / 0xff, (float)0x00 / 0xff, (float)0xff / 0xff);
		//public static Color Slot_Red = new Color((float)0xff / 0xff, (float)0x42 / 0xff, (float)0x42 / 0xff, (float)0xff / 0xff);

		protected void Init()
		{
			//this._panels.Add(new InvenSlotPanel(0, this.slotAmount, this
			//    , this.transform.FindChild("Slot Panel").gameObject
			//    , inventorySlot));
			//this.slotPanel = this.transform.FindChild("Slot Panel").gameObject;

			//GridLayoutGroup grid = this.slotPanel.GetComponent<GridLayoutGroup>();
			//grid.cellSize = new Vector2(16, 16);

			//InitSlot();
			this.inventoryTech = Managers.Resource.Load<GameObject>("prefabs/ui/Tech");

			//인벤 가이드 동영상
			//https://www.youtube.com/watch?v=dIq_7BeEjKE
		}

		//public override void Clear()
		//{
		//    for (int p = 0; p < this._panels.Count; ++p)
		//        this._panels[p].Clear();
		//}

		//protected virtual void InitSlot()
		//{
		//    for (int i = 0; i < this.slotAmount; ++i)
		//    {
		//        //items.Add(new Item());
		//        slots.Add(Instantiate(inventorySlot).GetComponent<Slot>());
		//        slots[i].id = i;
		//        slots[i].owner = this;
		//        slots[i].transform.SetParent(slotPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
		//    }

		//    //32 : slot의 크기 (+1 slot간의 간격)
		//    //*10 : 한줄에 10개
		//    // +2 : 오른쪽 끝 여백
		//    float width = (32 + 1) * 10f + 2;
		//    //32 : slot의 크기 (+1 slot간의 간격)
		//    //(x * 0.099) + 1 : slot은 한줄에 10개씩
		//    //4 : 맨 아래쪽 추가 여백.
		//    //slotPanel : slotPanel을 내린 만큼 보정해준다.(내리면 (-)이므로 빼줘야.. 값이 보정됩니다.)
		//    //Debug.Log(((RectTransform)this.transform).sizeDelta);
		//    //Debug.Log(((RectTransform)slotPanel.transform).sizeDelta);
		//    float height = (32 + 1) * ((int)(this.slotAmount * 0.099f) + 1) + 4
		//        - ((RectTransform)slotPanel.transform).sizeDelta.y;
		//    RectTransform rt = (RectTransform)this.transform;
		//    rt.sizeDelta = new Vector2(width, height);
		//}

		//public TechSlot GetInvenSlot(int panel, int slot)
		//{
		//    if (this._panels.Count <= panel)
		//        return null;

		//    TechSlotPanel p = this._panels[panel];
		//    if (null == p) return null;

		//    if (p._slots.Count <= slot)
		//        return null;

		//    return p._slots[slot];
		//}

		//Block에서 변경된 내용을 Inven에 반영합니다.
		public void SetItem(int panel, int slot, int itemid, int amount)
		{
			//Slot s = this.GetInvenSlot(panel, slot);
			//if (null == s) return;

			//InvenItemData itemData = s.GetItemData();
			////if (null != InvenItemData && InvenItemData.item.itemid != id)
			////{
			////    Debug.LogError("error: inven different item id");
			////    return;
			////}

			////덮어쓰기
			//if (null != InvenItemData)
			//{
			//    if (amount <= 0)
			//    {
			//        Destroy(InvenItemData.gameObject);
			//        //Debug.Log("inven item slot[" + slot + "], amount[" + amount + "]");
			//        return;
			//    }

			//    InvenItemData.SetStackCount(amount, false);
			//    return;
			//}

			////생성
			////database
			//TechBase itemToAdd = GameManager.GetTechBase().FetchItemByID(itemid);
			//if (null == itemToAdd)
			//{
			//    Debug.LogError("Database is empty : Need Checking Script Execute Order");
			//    return;
			//}

			//this.CreateItemData(this, s.transform, panel, slot, itemToAdd, ref amount, false);

		}

		public virtual void AddTech(InvenPanel panel, int id)
		{
			TechBase techbase = Managers.Game.TechBases.FetchItemByID(id);
			if (null == techbase)
			{
				Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
				return;
			}

			this.AddTech(panel, techbase);
        }

        //bCancel: true이면 취소버튼을 추가한다.
        public virtual void AddTech(InvenPanel panel, TechBase techbase, bool bCancel=false)
		{
            Slot slot = panel.CreateSlot();
            base.CreateTechData(this, slot.transform, panel._panel, slot._slot, techbase);

            //color
            //연구완료했으면...green
            if (true == techbase.learned) slot.GetComponent<Image>().color = Color.green;
            //prev 모두 연구 완료했으면 yellow, 그외는 red
            else
            {
                bool all_learned = true;   //prev모두 연구완료했는가?
                foreach (var prev in techbase.prev_techs)
                {
                    TechBase itemPrev = Managers.Game.TechBases.FetchItemByID(prev);
                    if (false == itemPrev.learned)
                    {
                        all_learned = false;
                        break;
                    }
                }
                if (all_learned) slot.GetComponent<Image>().color = Color.yellow;
                else slot.GetComponent<Image>().color = Color.red;
            }

			//resear창에 노출되었을때문 cancel버튼이 활성화 된다.
			if (bCancel)
			{
				GameObject cancel = base.CreateTechCancel(slot.transform);
				slot._techCancel = cancel;
                cancel.SetActive(false);	//마우스가 올라갔을때에 보인다.
			}
        }


        public virtual void Save(BinaryWriter writer)
		{
			////slot amount
			//writer.Write(this.slotAmount);

			////임시 List<> 에 저장
			//List<InvenItemData> items = new List<InvenItemData>();
			//for (int i = 0; i < this.slots.Count; ++i)
			//{
			//    InvenItemData itemData = this.slots[i].GetItemData();
			//    if (null == InvenItemData) continue;
			//    items.Add(InvenItemData);
			//}

			////item count
			//writer.Write(items.Count);
			////item info
			//for (int i = 0; i < items.Count; ++i)
			//{
			//    writer.Write(items[i].slot);    //slot
			//    writer.Write(items[i].item.itemid); //item id
			//    writer.Write(items[i].amount);  //amount
			//}

		}

		public virtual void Load(BinaryReader reader)
		{
			////slot amount
			//int slotAmount = reader.ReadInt32();
			//slotAmount = slotAmount + 1 - 1;
			////Debug.Log("slot = " + slotAmount);

			////item count
			//int itemcount = reader.ReadInt32();
			////Debug.Log("itemcount = " + itemcount);
			////item info
			//for (int i=0; i<itemcount; ++i)
			//{
			//    int panel = 0;
			//    int slot = reader.ReadInt32();
			//    int id = reader.ReadInt32();
			//    int amount = reader.ReadInt32();
			//    //Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

			//    SetItem(panel, slot, id, amount);
			//}

		}

	}//..class ItemInvenBase

 

}//..namespace MyCraft