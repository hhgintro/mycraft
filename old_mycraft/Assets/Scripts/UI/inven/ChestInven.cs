using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class ChestInven : ItemInvenBase
    {
        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

        void Awake()
        {
            //this.database = GetComponent<ItemDatabase>();
            //this.inventoryPanel = GameObject.Find("Canvas/ChestInven/Inventory Panel").gameObject;
            //this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;
            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();

        }

        void Start()
        {

            this.SetActive(false);
            int slotAmount = 16;
            //base.Start();

            //AddItem(1, 54);
            //AddItem(2, 54);
            base._panels.Add(new InvenSlotPanel(base._panels.Count, slotAmount, this
                , null
                , this.transform.Find("Slot Panel").gameObject
                , base._invenSlot));

            //locale
            //title text
            LocaleManager.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }
        
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