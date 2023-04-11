using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class TechDescription : TechInvenBase
    {
        private TechBase _techbase = null;

        public InvenSlotPanel _panelTitle = null;   //title image
        public InvenSlotPanel _panelCost = null;
        public InvenSlotPanel _panelPreTech = null;
        public InvenSlotPanel _panelNextTech = null;
        public InvenSlotPanel _panelReward = null;

        //public GameObject inventorySlot;
        //public GameObject inventoryItem;
        public GameObject _inventoryMultiple;

        void Awake()
        {
            base.Init();

            //this.database = GetComponent<ItemDatabase>();
            //this.inventoryPanel = GameObject.Find("Item_Canvas/Inventory/Inventory Panel").gameObject;
            //this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;
            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();

            LocaleManager.SetLocale("technology", this.transform.Find("Cost/Text").GetComponent<Text>());
            LocaleManager.SetLocale("technology", this.transform.Find("Pre-Tech/Text").GetComponent<Text>());
            LocaleManager.SetLocale("technology", this.transform.Find("Next-Tech/Text").GetComponent<Text>());
            LocaleManager.SetLocale("technology", this.transform.Find("Reward/Text").GetComponent<Text>());
            LocaleManager.SetLocale("technology", this.transform.Find("Research/Text").GetComponent<Text>());

        }

        void Start()
        {

            foreach (var tech in GameManager.GetTechBase().database)
            {
                this.LinkTech(tech.Value);
                break;//맨처음꺼를 설정해주고 멈춘다.
            }
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
            //동일하면...무시
            if (this._techbase == techbase) return;

            this._techbase = techbase;
            Clear();

            //title
            //title text
            this.transform.GetChild(0).GetComponent<Text>().text = techbase.Title;
            //this.transform.FindChild("Slot/Image").GetComponent<Image>().sprite = techbase.Sprite;

            //title-slot
            this._panelTitle = new InvenSlotPanel(0, 0, this
                , null
                , this.transform.Find("Slot Panel").gameObject
                , this._invenSlot);
            //title-item
            this.AddTech(this._panelTitle, techbase.id);
            ////color
            //if (techbase.prev_techs.Count <= 0)
            //    this.transform.FindChild("Slot").GetComponent<Image>().color = InvenBase.Slot_Yellow;
            //else
            //    this.transform.FindChild("Slot").GetComponent<Image>().color = InvenBase.Slot_Red;

            //cost-slot
            this._panelCost = new InvenSlotPanel(0, 0, this
                , null
                , this.transform.Find("Cost/Slot Panel").gameObject
                , this._invenSlot);
            //cost-item
            this.AddTime(this._panelCost, 0, techbase._cost.time);
            for (int i = 0; i < techbase._cost.items.Count; ++i)
                this.AddSkill(this._panelCost, techbase._cost.items[i].itemid, techbase._cost.items[i].amount);
            //multiple
            Slot multiple = UnityEngine.Object.Instantiate(this._inventoryMultiple).GetComponent<Slot>();
            this._panelCost._slots.Add(multiple);
            multiple.panel = this._panelCost._panel;
            //multiple.panel = this._panelCost._slots.Count - 1;
            multiple.owner = this;
            multiple.transform.SetParent(this.transform.Find("Cost/Slot Panel").gameObject.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            multiple.GetComponent<Text>().text = " x " + techbase._cost.mulitple.ToString();
            //pre-tech
            if(0 < techbase.prev_techs.Count)
            {
                this._panelPreTech = new InvenSlotPanel(1, 0, this
                    , null
                    , this.transform.Find("Pre-Tech/Slot Panel").gameObject
                    , this._invenSlot);
                for (int i = 0; i < techbase.prev_techs.Count; ++i)
                    this.AddTech(this._panelPreTech, techbase.prev_techs[i]);
            }
            //next-tech
            if (0 < techbase.next_techs.Count)
            {
                _panelNextTech = new InvenSlotPanel(2, 0, this
                    , null
                    , this.transform.Find("Next-Tech/Slot Panel").gameObject
                    , this._invenSlot);
                for (int i = 0; i < techbase.next_techs.Count; ++i)
                    this.AddTech(this._panelNextTech, techbase.next_techs[i]);
            }
            //reward
            if(0 < techbase.rewards.Count)
            {
                this._panelReward = new InvenSlotPanel(3, 0, this
                    , null
                    , this.transform.Find("Reward/Slot Panel").gameObject
                    , this._invenSlot);
                for (int i = 0; i < techbase.rewards.Count; ++i)
                    this.AddSkill(this._panelReward, techbase.rewards[i], 0);
            }
        }

        public  void AddTime(InvenSlotPanel panel, int id, float time)
        {
            //database
            ItemBase itemToAdd = GameManager.GetItemBase().FetchItemByID(id);
            if(null == itemToAdd)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
                return;
            }

            Slot slot = panel.CreateSlot();
            this.CreateTimeData(this, slot.transform, panel._panel, slot.slot, itemToAdd, this._invenItem, time);
        }

        //InvenItemData가 없이,
        //id로 아이템을 추가하고가 할때 사용합니다.
        //InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        public virtual int AddItem(InvenSlotPanel panel, int id, int itemcount)
        {
            ////들고 있는 아이템이면...
            //if (null != InvenBase.choiced_item)
            //{
            //    if (InvenBase.choiced_item.itembase.id == id)
            //    {
            //        itemcount = InvenBase.choiced_item.AddStackCount(itemcount, true);
            //        if (itemcount <= 0)
            //            return 0;
            //    }
            //}

            ////겹치기
            //itemcount = this.OnOverlapItem(id, itemcount);
            ////더이상 추가할 것이 없다.
            //if (itemcount <= 0)
            //    return 0;

            //database
            ItemBase itemToAdd = GameManager.GetItemBase().FetchItemByID(id);
            if (null == itemToAdd)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
                return itemcount;
            }

            //겹치지 못하고 남은 것이 있다면...생성해서 넣어줍니다.
            itemcount = this.OnCreateItemData(panel, itemToAdd, itemcount);
            return itemcount;
        }

        //InvenItemData가 없이,
        //id로 아이템을 추가하고가 할때 사용합니다.
        //InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        public void AddTech(InvenSlotPanel panel, int id)
        {
            //database
            TechBase itemToAdd = GameManager.GetTechBase().FetchItemByID(id);
            if (null == itemToAdd)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
                return;
            }

            Slot slot = panel.CreateSlot();
            this.CreateTechData(this, slot.transform, panel._panel, slot.slot, itemToAdd, this.inventoryTech);

            //color
            if (itemToAdd.prev_techs.Count <= 0)    slot.GetComponent<Image>().color = Color.yellow;
            else                                    slot.GetComponent<Image>().color = Color.red;

            //List<Slot> slots = panel._slots;
            //for (int i = 0; i < slots.Count; ++i)
            //{
            //    //빈자리를 찾아서...
            //    if (0 < slots[i].transform.childCount)
            //        continue;

            //    //this.items[i] = itemToAdd;
            //    this.CreateTechData(this, slots[i].transform, panel._panel, i, itemToAdd, this.inventoryTech);

            //    //color
            //    if (itemToAdd.prev_techs.Count <= 0)
            //        slots[i].GetComponent<Image>().color = InvenBase.Slot_Yellow;
            //    else
            //        slots[i].GetComponent<Image>().color = InvenBase.Slot_Red;

            //    break;
            //    ////더이상 추가할 것이 없다.
            //    //if (itemcount <= 0)
            //    //    break;
            //}
        }

        ////InvenItemData가 없이,
        ////id로 아이템을 추가하고가 할때 사용합니다.
        ////InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        //public void AddSkill(InvenSlotPanel panel, int id)
        //{
        //    //database
        //    SkillBase itemToAdd = GameManager.GetSkillBase().FetchItemByID(id);
        //    if (null == itemToAdd)
        //    {
        //        Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
        //        return;
        //    }

        //    List<Slot> slots = panel._slots;
        //    for (int i = 0; i < slots.Count; ++i)
        //    {
        //        //빈자리를 찾아서...
        //        if (0 < slots[i].transform.childCount)
        //            continue;

        //        //this.items[i] = itemToAdd;
        //        this.CreateSkillData(this, slots[i].transform, panel._panel, i, itemToAdd);
        //        break;
        //        ////더이상 추가할 것이 없다.
        //        //if (itemcount <= 0)
        //        //    break;
        //    }
        //}

        protected virtual int OnCreateItemData(InvenSlotPanel panel, JSonDatabase itemToAdd, int itemcount)
        {
            List<Slot> slots = panel._slots;
            for (int i = 0; i < slots.Count; ++i)
            {
                //빈자리를 찾아서...
                if (0 < slots[i].transform.childCount)
                    continue;

                //this.items[i] = itemToAdd;
                this.CreateItemData(this, slots[i].transform, panel._panel, i, itemToAdd, this._invenItem, ref itemcount, true);

                //더이상 추가할 것이 없다.
                if (itemcount <= 0)
                    break;
            }
            return itemcount;
        }
 
        public void OnResearchClicked()
        {
            Debug.Log(_techbase.id + " 연구 완료");
        }

        public override bool SetOutput(SkillBase skillbase)
        { return true; }
    }//..class TechDescription
}//..namespace MyCraft