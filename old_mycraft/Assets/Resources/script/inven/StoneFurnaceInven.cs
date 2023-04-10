using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class StoneFurnaceInven : ItemInvenBase
    {
        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

        protected override void Awake()
        {
            base.Awake();

            base._progress.Add(this.transform.Find("Progress/bar").GetComponent<Image>());
            base._progress.Add(this.transform.Find("Fuel-Progress/bar").GetComponent<Image>());
            //this.database = GetComponent<ItemDatabase>();
            //this.inventoryPanel = GameObject.Find("Canvas/ChestInven/Inventory Panel").gameObject;
            //this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;
            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
        }
        protected override void Start()
        {
            base.Start();

            //HG_TEST : 테스트를 위해서 active로 설정합니다.
            //this.SetActive(false);
            this.SetActive(true);

            //int slotAmount = 1;
            //base.Start();

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , this.transform.Find("Progress/bar").GetComponent<Image>()
                , this.transform.Find("Input-Panel").gameObject
                , base._inventorySlot));

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , this.transform.Find("Fuel-Progress/bar").GetComponent<Image>()
                , this.transform.Find("Fuel-Panel").gameObject
                , base._inventorySlot));

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , null
                , this.transform.Find("Output-Panel").gameObject
                , base._inventorySlot));
            //AddItem(1, 54);
            //AddItem(2, 54);

            //locale
            //title text
            LocaleManager.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }

        //protected override void Clear()
        //{
        //    base.Clear();
        //}

        //public override void LinkInven(BlockScript block, int slotAmount, List<List<BlockSlot>> slots)
        //{
        //    base.LinkInven(block, slotAmount, slots);
        //}


        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);
        }


    }//..class Inventory
}//..namespace MyCraft