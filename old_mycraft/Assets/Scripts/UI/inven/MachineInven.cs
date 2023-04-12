using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class MachineInven : ItemInvenBase
    {
        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체
        public GameObject _panelInvenMain { get; private set; }
        public GameObject _panelInvenSub { get; private set; }
        public GameObject _panelInvenLeaf { get; private set; }

        void Awake()
        {
            base.Init();

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , this.transform.Find("Progress/bar").GetComponent<Image>()
                , this.transform.Find("Input-Panel").gameObject
                , InvenBase._invenSlot));

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , null
                , this.transform.Find("Output-Panel").gameObject
                , InvenBase._invenSlot));

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , null
                , this.transform.Find("Chip-Panel").gameObject
                , InvenBase._invenSlot));



            base._progress.Add(this.transform.Find("Progress/bar").GetComponent<Image>());
            //this.database = GetComponent<ItemDatabase>();
            //this.inventoryPanel = GameObject.Find("Canvas/ChestInven/Inventory Panel").gameObject;
            //this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;
            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();

            //GameObject _slot = Resources.Load<GameObject>("prefab/ui/Slot") as GameObject;

            //GameObject clonePanel = this.CreateObject(this._panelInvenSub.transform, this._panelInvenLeaf);
            //GameObject cloneSlot = this.CreateSlot(clonePanel.transform, null, _slot);

        }

        void Start()
        {

            //HG_TEST : 테스트를 위해서 active로 설정합니다.
            //this.SetActive(false);
            this.SetActive(true);



            //AddItem(1, 54);
            //AddItem(2, 54);

            //locale
            //title text
            LocaleManager.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }

        public override void LinkInven(BlockScript block, List<BlockSlotPanel> panels, List<Progress> progresses)
        {
            base.LinkInven(block, panels, progresses);

            //this.AddReset(base._panels[0]);
            this.AddReset(base._panels[0]);

            //Slot s = this._panels[0].CreateSlot();

            ////database
            //int id = 0;//reset id
            //SkillBase database = GameManager.GetSkillBase().FetchItemByID(id);
            //if (null == database)
            //{
            //    Debug.LogError("Database is empty : Need Checking Script Execute Order[id:" + id + "]");
            //    return;
            //}

            //GameObject itemObj = UnityEngine.Object.Instantiate(inventoryReset);
            //itemObj.transform.SetParent(s.transform, false); //[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            //itemObj.transform.position = s.transform.position;
            //itemObj.GetComponent<Image>().sprite = database.Sprite;
            //itemObj.name = database.Title;
            ////itemObj.GetComponent<CanvasGroup>().blocksRaycasts = false;//인벤에 생성할때는 true입니다.
            //ItemData itemData = itemObj.GetComponent<ItemData>();
            //itemData.owner = this;
            //itemData.database = database;
            //itemData.panel = base._panels[0]._panel;
            //itemData.slot = s.slot;

        }

        public override void Reset()
        {
            base._block.Reset();
            this.Clear();
            base.SetActive(false);
            GameManager.GetInventory().SetActive(false);
        }
        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);
        }

        public virtual GameObject CreateObject(Transform parent, GameObject prefab)
        {
            GameObject clone = UnityEngine.Object.Instantiate(prefab);//.GetComponent<Slot>();
            clone.transform.SetParent(parent, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            //clone.transform.position = parent.position;
            return clone;
        }

        public virtual GameObject CreateSlot(Transform parent, JSonDatabase database, GameObject prefab)
        {
            GameObject clone = this.CreateObject(parent, prefab);
            //clone.transform.SetParent(parent, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            ////clone.transform.position = parent.position;
            if(null != database)
                clone.GetComponent<Image>().sprite = database.Sprite;
            return clone;
        }
    }//..class Inventory
}//..namespace MyCraft