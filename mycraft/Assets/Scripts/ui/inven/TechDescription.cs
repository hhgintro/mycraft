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
            this._panelTitle = new InvenPanel(0, 0, this
                , this.transform.Find("Slot Panel"));
            //title-item
            this.AddTech(this._panelTitle, techbase.id);
            ////color
            //if (techbase.prev_techs.Count <= 0)
            //    this.transform.FindChild("Slot").GetComponent<Image>().color = InvenBase.Slot_Yellow;
            //else
            //    this.transform.FindChild("Slot").GetComponent<Image>().color = InvenBase.Slot_Red;

            //cost-slot
            this._panelCost = new InvenPanel(0, 0, this
                , this.transform.Find("Cost/Slot Panel"));
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
                this._panelPreTech = new InvenPanel(1, 0, this
                    , this.transform.Find("Pre-Tech/Slot Panel"));
                for (int i = 0; i < techbase.prev_techs.Count; ++i)
                    this.AddTech(this._panelPreTech, techbase.prev_techs[i]);
            }
            //next-tech
            if (0 < techbase.next_techs.Count)
            {
                _panelNextTech = new InvenPanel(2, 0, this
                    , this.transform.Find("Next-Tech/Slot Panel"));
                for (int i = 0; i < techbase.next_techs.Count; ++i)
                    this.AddTech(this._panelNextTech, techbase.next_techs[i]);
            }
            //reward
            if(0 < techbase.rewards.Count)
            {
                this._panelReward = new InvenPanel(3, 0, this
                    , this.transform.Find("Reward/Slot Panel"));
                for (int i = 0; i < techbase.rewards.Count; ++i)
                    this.AddSkill(this._panelReward, techbase.rewards[i], 0);
            }
        }

        public  void AddTime(InvenPanel panel, int id, float time)
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
        public virtual int AddItem(InvenPanel panel, int id, int itemcount)
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

        //InvenItemData가 없이,
        //id로 아이템을 추가하고가 할때 사용합니다.
        //InvenItemData를 이동하거나, 인벤에 넣어줄때는 Additem(InvenItemData itemData)를 사용하세요.
        public void AddTech(InvenPanel panel, int id)
        {
            //database
            TechBase itemToAdd = Managers.Game.TechBases.FetchItemByID(id);
            if (null == itemToAdd)
            {
                Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
                return;
            }

            Slot slot = panel.CreateSlot();
            this.CreateTechData(this, slot.transform, panel._panel, slot._slot, itemToAdd);

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
        //public void AddSkill(InvenPanel panel, int id)
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
            Debug.Log(_techbase.id + " 연구 완료");
        }

        public override bool AssignRecipe(ItemBase itembase) { return true; }
    }//..class TechDescription
}//..namespace MyCraft