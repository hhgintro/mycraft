using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class InvenBase : MonoBehaviour
    {
        ////protected GameObject inventoryPanel;
        public List<InvenSlotPanel> _panels = new List<InvenSlotPanel>();
        ////protected GameObject slotPanel;

        //protected ItemDatabase database;
        public static GameObject _invenPanel { get; private set; }
        public static GameObject _invenSlot { get; private set; }
        public static GameObject _invenItem { get; private set; }
        public static GameObject _invenSkill { get; private set; }
        public static GameObject _invenReset { get; private set; }
        public static GameObject _category { get; private set; }


        ////ChestInven은 ChestScript와 연결됩니다.
        protected BlockScript _block { get; set; }

        ////protected List<Image> _progress = new List<Image>();

        ////protected int slotAmount;
        ////public List<Item> items = new List<Item>();
        ////public List<Slot> slots = new List<Slot>();

        ////private Vector2 offset;

        public static InvenItemData choiced_item = null;     //인벤에서 선택된 개체
        public static bool bPointerEnter { get; set; }      //인벤위에 마우스가 위치했는지???


        protected virtual void Init()
        {
            if(null == _invenPanel)     _invenPanel = Managers.Resource.Load<GameObject>("prefabs/ui/Slot Panel");
            if(null == _invenSlot)      _invenSlot = Managers.Resource.Load<GameObject>("prefabs/ui/Slot");
            if(null == _invenItem)      _invenItem = Managers.Resource.Load<GameObject>("prefabs/ui/Item");
            if(null == _invenSkill)     _invenSkill = Managers.Resource.Load<GameObject>("prefabs/ui/Skill");
            if(null == _invenReset)     _invenReset = Managers.Resource.Load<GameObject>("prefabs/ui/Reset");
            if (null == _category)      _category = Managers.Resource.Load<GameObject>("prefabs/ui/Category");
        }

        public virtual void Clear()
        {
            for (int p = 0; p < this._panels.Count; ++p)
                this._panels[p].Clear();
        }

        public virtual void Reset()
        { }

        public virtual bool SetOutput(ItemBase itembase) { return false; }

        ////protected virtual void InitSlot()
        ////{
        ////    for (int i = 0; i < this.slotAmount; ++i)
        ////    {
        ////        //items.Add(new Item());
        ////        slots.Add(Instantiate(inventorySlot).GetComponent<Slot>());
        ////        slots[i].id = i;
        ////        slots[i].owner = this;
        ////        slots[i].transform.SetParent(slotPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
        ////    }

        ////    //32 : slot의 크기 (+1 slot간의 간격)
        ////    //*10 : 한줄에 10개
        ////    // +2 : 오른쪽 끝 여백
        ////    float width = (32 + 1) * 10f + 2;
        ////    //32 : slot의 크기 (+1 slot간의 간격)
        ////    //(x * 0.099) + 1 : slot은 한줄에 10개씩
        ////    //4 : 맨 아래쪽 추가 여백.
        ////    //slotPanel : slotPanel을 내린 만큼 보정해준다.(내리면 (-)이므로 빼줘야.. 값이 보정됩니다.)
        ////    //Debug.Log(((RectTransform)this.transform).sizeDelta);
        ////    //Debug.Log(((RectTransform)slotPanel.transform).sizeDelta);
        ////    float height = (32 + 1) * ((int)(this.slotAmount * 0.099f) + 1) + 4
        ////        - ((RectTransform)slotPanel.transform).sizeDelta.y;
        ////    RectTransform rt = (RectTransform)this.transform;
        ////    rt.sizeDelta = new Vector2(width, height);
        ////}

        public virtual Slot GetInvenSlot(int panel, int slot)
        {
            if (this._panels.Count <= panel)
                return null;

            InvenSlotPanel p = this._panels[panel];
            if (null == p) return null;

            if (p._slots.Count <= slot)
                return null;

            return p._slots[slot];
        }

        public virtual bool Resize()
        {
            Debug.Log($"Resizing panels:{this._panels.Count}");
            for(int p=0; p < this._panels.Count; ++p)
            {
                //last slot
                int s = this._panels[p]._slots.Count - 1;
                RectTransform slot = this._panels[p]._slots[s].GetComponent<RectTransform>();
                Debug.Log($"Resizing last slot rect:({slot.rect}");

            }
            //GridLayoutGroup grid = this.GetComponent<GridLayoutGroup>();
            //grid.padding.left;

            RectTransform inven = this.GetComponent<RectTransform>();
            Debug.Log($"Resizing inven rect:({inven.rect} / {inven.sizeDelta}");
            //this.GetComponent<RectTransform>().sizeDelta = new Vector2(332,350);
            this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
            return true;
        }
        ////Block에서 변경된 내용을 Inven에 반영합니다.
        //public void SetItem(int panel, int slot, int itemid, int amount)
        //{
        //    //Slot s = this.GetInvenSlot(panel, slot);
        //    //if (null == s) return;

        //    //InvenItemData itemData = s.GetItemData();
        //    ////if (null != InvenItemData && InvenItemData.item.itemid != id)
        //    ////{
        //    ////    Debug.LogError("error: inven different item id");
        //    ////    return;
        //    ////}

        //    ////덮어쓰기
        //    //if (null != InvenItemData)
        //    //{
        //    //    if (amount <= 0)
        //    //    {
        //    //        Destroy(InvenItemData.gameObject);
        //    //        //Debug.Log("inven item slot[" + slot + "], amount[" + amount + "]");
        //    //        return;
        //    //    }

        //    //    InvenItemData.SetStackCount(amount, false);
        //    //    return;
        //    //}

        //    ////생성
        //    ////database
        //    //TechBase itemToAdd = GameManager.GetTechBase().FetchItemByID(itemid);
        //    //if (null == itemToAdd)
        //    //{
        //    //    Debug.LogError("Database is empty : Need Checking Script Execute Order");
        //    //    return;
        //    //}

        //    //this.CreateItemData(this, s.transform, panel, slot, itemToAdd, ref amount, false);

        //}

        ////InvenItemData가 없이,
        ////id로 아이템을 추가하고가 할때 사용합니다.
        ////InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        //public virtual int AddItem(int id, int itemcount)
        //{
        //    ////들고 있는 아이템이면...
        //    //if (null != InvenBase.choiced_item)
        //    //{
        //    //    if (InvenBase.choiced_item.itembase.id == id)
        //    //    {
        //    //        itemcount = InvenBase.choiced_item.AddStackCount(itemcount, true);
        //    //        if (itemcount <= 0)
        //    //            return 0;
        //    //    }
        //    //}

        //    ////겹치기
        //    //itemcount = this.OnOverlapItem(id, itemcount);
        //    ////더이상 추가할 것이 없다.
        //    //if (itemcount <= 0)
        //    //    return 0;

        //    //database
        //    TechBase itemToAdd = GameManager.GetTechBase().FetchItemByID(id);
        //    if (null == itemToAdd)
        //    {
        //        Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
        //        return itemcount;
        //    }

        //    //겹치지 못하고 남은 것이 있다면...생성해서 넣어줍니다.
        //    itemcount = this.OnCreateItemData(itemToAdd, itemcount);
        //    return itemcount;
        //}

        //////InvenItemData를 인벤에 넣어줄때 사용합니다.
        //////InvenItemData 없이, id로 아이템을 추가하고자 할때에는 AddItem(int id, int itemcount)를 사용하세요.
        ////public virtual int AddItem(InvenItemData itemData)
        ////{
        ////    //겹치기
        ////    InvenItemData.amount = this.OnOverlapItem(InvenItemData.itembase.id, InvenItemData.amount);
        ////    //더이상 추가할 것이 없다.
        ////    if (InvenItemData.amount <= 0)
        ////    {
        ////        Destroy(InvenItemData.gameObject);
        ////        return 0;   //남은 개수
        ////    }

        ////    //겹치지 못하고 남은 것이 있다면...빈자리를 찾아서 넣어줍니다.
        ////    for (int p = 0; p < this._panels.Count; ++p)
        ////    {
        ////        List<TechSlot> slots = this._panels[p]._slots;
        ////        for (int i = 0; i < slots.Count; ++i)
        ////        {
        ////            if (0 < slots[i].transform.childCount)
        ////                continue;

        ////            slots[i].AddItem(InvenItemData);
        ////            return 0;   //남은 개수
        ////        }
        ////    }
        ////    return InvenItemData.amount;
        ////}

        //////겹치기
        //////RETURN : 겹치고 남은 개수를 리턴합니다.
        ////protected virtual int OnOverlapItem(int id, int itemcount)
        ////{
        ////    for (int p = 0; p < this._panels.Count; ++p)
        ////    {
        ////        List<TechSlot> slots = this._panels[p]._slots;
        ////        for (int i = 0; i < slots.Count; ++i)
        ////        {
        ////            InvenItemData itemData = slots[i].GetItemData();
        ////            if (null == InvenItemData) continue;
        ////            if (InvenItemData.itembase.id != id) continue;

        ////            //겹치고 남은 개수를 리턴합니다.
        ////            itemcount = InvenItemData.CheckOverlapItem(itemcount);
        ////            //InvenItemData.AddStackCount(0);

        ////            //남은 개수가 없다.
        ////            if (itemcount <= 0)
        ////                break;
        ////        }
        ////    }

        ////    return itemcount;
        ////}

        //protected virtual int OnCreateItemData(TechBase itemToAdd, int itemcount)
        //{
        //    for (int p = 0; p < this._panels.Count; ++p)
        //    {
        //        List<TechSlot> slots = this._panels[p]._slots;
        //        for (int i = 0; i < slots.Count; ++i)
        //        {
        //            //빈자리를 찾아서...
        //            if (0 < slots[i].transform.childCount)
        //                continue;

        //            //this.items[i] = itemToAdd;
        //            this.CreateItemData(this, slots[i].transform, p, i, itemToAdd, ref itemcount, true);

        //            //color
        //            if (itemToAdd.prev_techs.Count <= 0)
        //                slots[i].GetComponent<Image>().color = TechInvenBase.Slot_Yellow;
        //            else
        //                slots[i].GetComponent<Image>().color = TechInvenBase.Slot_Red;
        //            //TechSlot ts = slots[i].GetComponent<TechSlot>();
        //            //Debug.Log("tech slot" + i + ts.ToString());

        //            //더이상 추가할 것이 없다.
        //            if (itemcount <= 0)
        //                break;
        //        }
        //    }
        //    return itemcount;
        //}
        public virtual void AddReset(InvenSlotPanel panel)
        {
            int id = 0;//reset
            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(id);
            if (null == itembase)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
                return;
            }

            Slot slot = panel.CreateSlot();
            this.CreateSkillData(this, slot.transform, panel._panel, slot.slot, itembase, InvenBase._invenReset, 0);
        }

        //InvenItemData가 없이,
        //id로 아이템을 추가하고가 할때 사용합니다.
        //InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        public virtual void AddSkill(InvenSlotPanel panel, int itemid, int amount)
        {
            if (itemid <= 0)
            {
                Debug.LogError("you need check skill id: " + itemid);
                return;
            }

            //database
            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(itemid);
            //ItemBase itemToAdd = GameManager.GetItemBase().FetchItemByID(id);
            if (null == itembase)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + itemid + "]");
                return;
            }


            Slot slot = panel.CreateSlot();
            this.CreateSkillData(this, slot.transform, panel._panel, slot.slot, itembase, InvenBase._invenSkill, amount);
        }

        public virtual void AddCategory(InvenSlotPanel panel, int category)
        {
            //database
            Categories categories = GameManager.GetCategories().FetchItemByID(category);
            //ItemBase itemToAdd = GameManager.GetItemBase().FetchItemByID(id);
            if (null == categories)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + category + "]");
                return;
            }


            Slot slot = panel.CreateSlot();
            this.CreateCategory(this, slot.transform, panel._panel, slot.slot, categories, InvenBase._category);
        }

        ////InvenItemData가 없이,
        ////id로 아이템을 추가하고가 할때 사용합니다.
        ////InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        //public virtual void AddReset(InvenSlotPanel panel)//, int id)
        //{
        //    //database
        //    int id = 0;//reset id
        //    SkillBase itemToAdd = GameManager.GetSkillBase().FetchItemByID(id);
        //    if (null == itemToAdd)
        //    {
        //        Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
        //        return;
        //    }

        //    Slot s = this._panels[0].CreateSlot();
        //    this.CreateSkillData(this, s.transform, s.panel, s.slot, itemToAdd);
        //}

        protected virtual ItemData CreateObject(InvenBase owner, Transform parent
            , int panel, int slot, JSonDatabase database, GameObject itemObj)
        {
            GameObject clone = UnityEngine.Object.Instantiate(itemObj);
            clone.transform.SetParent(parent, false); //[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            //clone.transform.position = parent.position;   //이거때문에 아이콘이 우측아래에 찍힘.
            clone.GetComponent<Image>().sprite = database.Sprite;
            clone.name = database.Title;
            //clone.GetComponent<CanvasGroup>().blocksRaycasts = false;//인벤에 생성할때는 true입니다.
            
            ItemData itemData = clone.GetComponent<ItemData>();
            itemData.owner      = owner;
            itemData.database   = database;
            itemData.panel      = panel;
            itemData.slot       = slot;
            return itemData;
        }

        public virtual ItemData CreateItemData(InvenBase owner, Transform parent
            , int panel, int slot, JSonDatabase database, GameObject itemObj, ref int amount
            , bool noti)
        {
            ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, itemObj);
            //겹치고 남은 아이템 개수
            amount = itemData.AddStackCount(amount, noti);

            return itemData;
        }


        public virtual void CreateTimeData(InvenBase owner, Transform parent
            , int panel, int slot, JSonDatabase database, GameObject itemObj, float time)
        {
            ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, itemObj);
            //겹치고 남은 아이템 개수
            itemData.textAmount.text = time.ToString();
            //amount = InvenItemData.AddStackCount(amount, noti);
            //return InvenItemData;
        }

        public virtual void CreateTechData(InvenBase owner, Transform parent
            , int panel, int slot, JSonDatabase database, GameObject itemObj)
        {
            ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, itemObj);

            RectTransform rt = (RectTransform)itemData.transform;
            rt.sizeDelta = Vector2.one * 60f;
        }

        public virtual void CreateCategory(InvenBase owner, Transform parent
            , int panel, int slot, JSonDatabase database, GameObject itemObj)
        {
            SkillGroupData itemData = (SkillGroupData)this.CreateObject(owner, parent, panel, slot, database, itemObj);
            itemData.category = slot;

            RectTransform rt = (RectTransform)itemData.transform;
            rt.sizeDelta = Vector2.one * 60f;
        }

        public virtual void CreateSkillData(InvenBase owner, Transform parent
            , int panel, int slot, JSonDatabase database, GameObject itemObj, int amount)
        {
            ItemData itemData = this.CreateObject(owner, parent, panel, slot, database, itemObj);
            RectTransform rt = (RectTransform)itemData.transform;
            rt.sizeDelta = Vector2.one * 30f;

            if(null != itemData.textAmount)
                itemData.textAmount.text = (0 < amount) ? amount.ToString() : "";
        }

        public virtual bool CheckPutdownGoods(int panel, int slot, int itemid)
        { return false; }

        public virtual bool CheckPickupGoods()
        { return false; }
        ////{
        ////    if (null == this._block) return true;
        ////    return this._block.CheckPutdownGoods(panel, slot, itemid);
        ////}

        //public virtual void OnPointerEnter(PointerEventData eventData)
        //{
        //    //Debug.Log("enter pointer");
        //    if (false == this.GetActive())
        //        return;

        //    this.ActiveIcon();
        //    bPointerEnter = true;
        //}

        //public virtual void OnPointerExit(PointerEventData eventData)
        //{
        //    //Debug.Log("exit pointer");
        //    this.DeactiveIcon();
        //    bPointerEnter = false;
        //}

        public virtual void ActiveIcon()
        {
            if (null == InvenBase.choiced_item)
                return;
            GameManager.GetTerrainManager().SetChoicePrefab((BlockScript)null);
            InvenBase.choiced_item.GetComponent<Image>().enabled = true;
        }
        public virtual void DeactiveIcon()
        {
            if (null == InvenBase.choiced_item) return;
            //자원(광물)은 예외...
            if (InvenBase.choiced_item.database.type < BLOCKTYPE.CHEST) return;

            GameManager.GetTerrainManager().SetChoicePrefab((ItemBase)InvenBase.choiced_item.database);
            GameManager.GetMouseController().mouse_refresh = true;//prefab을 찍을 수 있도록 설정
            InvenBase.choiced_item.GetComponent<Image>().enabled = false;
        }
        ////public void OnDrag(PointerEventData eventData)
        ////{
        ////    this.transform.position = eventData.position - offset;
        ////}

        ////public void OnPointerDown(PointerEventData eventData)
        ////{
        ////    offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
        ////}

        //public bool GetActive() {
        //    if (null == canvas_ui || 1f != canvas_ui.alpha)
        //        return false;
        //    return true;
        //}
        //public void SetActive(bool active)
        //{
        //    if (null == canvas_ui) return;

        //    if (true == active)
        //    {
        //        canvas_ui.alpha = 1f;
        //        canvas_ui.blocksRaycasts = true;
        //        return;
        //    }

        //    canvas_ui.alpha = 0f;
        //    canvas_ui.blocksRaycasts = false;
        //}

        ////public virtual void LinkInven(BlockScript block, List<BlockSlotPanel> panels, List<Progress> progresses)
        ////{
        ////    //HG_TEST: 테스트를 위해서 같은 개체일 경우에도 다시 생성시킵니다.(버그 확인후에는 원상복귀합니다)
        ////    //같은 개체일 경우에는 생성을 진행하지 않는다.
        ////    if (null == block)// || this._block == block)
        ////        return;

        ////    //old chest
        ////    if (null != this._block)
        ////        this._block.SetInven(null);
        ////    //new chest
        ////    this._block = block;
        ////    this._block.SetInven(this);

        ////    //기존정보 삭제
        ////    this.Clear();


        ////    ////slot
        ////    //this.slotAmount = slotAmount;
        ////    //InitSlot();

        ////    if(this._panels.Count != panels.Count)
        ////    {
        ////        Debug.LogError("Different Panel Count(block/inven): " + panels.Count + "/" + this._panels.Count);
        ////        return;
        ////    }

        ////    //panel 정보
        ////    if (null == panels || panels.Count <= 0) return;
        ////    for(int p=0; p<panels.Count; ++p)
        ////    {
        ////        //slot
        ////        this._panels[p].SetAmount(panels[p]._amount);

        ////        //item 생성
        ////        List<BlockSlot> slots = panels[p]._slots;
        ////        for (int i = 0; i < slots.Count; ++i)
        ////        {
        ////            if (slots[i]._itemid <= 0) continue;
        ////            if (slots[i]._amount <= 0) continue;

        ////            if(p != slots[i]._panel)
        ////            {
        ////                Debug.LogError("Diff Panel(panel/slot): " + p + "/" + slots[i]._panel);
        ////            }
        ////            this.SetItem(p, i, slots[i]._itemid, slots[i]._amount);
        ////        }
        ////    }

        ////    //progress
        ////    for(int i=0; i<progresses.Count; ++i)
        ////        this.SetProgress(i, progresses[i]._fillAmount);
        ////}

        //ChestInven에서 변경된 아이템정보를 ChestScript에 반영합니다.
        public virtual void SetInven2Block(int panel, int slot, int id, int amount)
        { }
            ////    if (null == this._block)
            ////        return;
            ////    this._block.SetItem(panel, slot, id, amount);
            ////}

            ////public virtual void SetProgress(int id, float fillAmount)
            ////{
            ////    if (id < this._progress.Count)
            ////        this._progress[id].fillAmount = fillAmount;
            ////}

            //public virtual void Save(BinaryWriter writer)
            //{
            //    ////slot amount
            //    //writer.Write(this.slotAmount);

            //    ////임시 List<> 에 저장
            //    //List<InvenItemData> items = new List<InvenItemData>();
            //    //for (int i = 0; i < this.slots.Count; ++i)
            //    //{
            //    //    InvenItemData itemData = this.slots[i].GetItemData();
            //    //    if (null == InvenItemData) continue;
            //    //    items.Add(InvenItemData);
            //    //}

            //    ////item count
            //    //writer.Write(items.Count);
            //    ////item info
            //    //for (int i = 0; i < items.Count; ++i)
            //    //{
            //    //    writer.Write(items[i].slot);    //slot
            //    //    writer.Write(items[i].item.itemid); //item id
            //    //    writer.Write(items[i].amount);  //amount
            //    //}

            //}

            //public virtual void Load(BinaryReader reader)
            //{
            //    ////slot amount
            //    //int slotAmount = reader.ReadInt32();
            //    //slotAmount = slotAmount + 1 - 1;
            //    ////Debug.Log("slot = " + slotAmount);

            //    ////item count
            //    //int itemcount = reader.ReadInt32();
            //    ////Debug.Log("itemcount = " + itemcount);
            //    ////item info
            //    //for (int i=0; i<itemcount; ++i)
            //    //{
            //    //    int panel = 0;
            //    //    int slot = reader.ReadInt32();
            //    //    int id = reader.ReadInt32();
            //    //    int amount = reader.ReadInt32();
            //    //    //Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

            //    //    SetItem(panel, slot, id, amount);
            //    //}

            //}

        }//..class InvenBase



}//..namespace MyCraft