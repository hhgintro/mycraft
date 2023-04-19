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
            base.Init();

            int slotAmount = 16;
            base._panels.Add(new InvenSlotPanel(base._panels.Count, slotAmount, this
                , null
                , this.transform.Find("Slot Panel").gameObject
                , InvenBase._invenSlot));
        }

        void Start()
        {
            //this.gameObject.SetActive(false);

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