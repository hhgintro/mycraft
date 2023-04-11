using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class TechInven : TechInvenBase
    {

        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

        void Awake()
        {

            //this.database = GetComponent<ItemDatabase>();
            //this.inventoryPanel = GameObject.Find("Item_Canvas/Inventory/Inventory Panel").gameObject;
            //this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;
            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();

        }

        void Start()
        {

            int slotAmount = 64;// GameManager.GetTechBase().database.Count;
            //base.Start();

            base._panels.Add(new InvenSlotPanel(base._panels.Count, slotAmount, this
                , null
                , this.transform.Find("Viewport/Slot Panel").gameObject
                , base._invenSlot));

            ////HG_TEST : 테스트 아이템 지급
            //AddItem((int)BLOCKTYPE.BELT, 54);
            //AddItem((int)BLOCKTYPE.INSERTER, 54);
            //AddItem((int)BLOCKTYPE.INSERTER, 54);
            //AddItem((int)BLOCKTYPE.CHEST, 54);
            //AddItem((int)BLOCKTYPE.CHEST, 54);
            //AddItem((int)BLOCKTYPE.CHEST, 54);
            //AddItem((int)BLOCKTYPE.DRILL, 54);
            //AddItem((int)BLOCKTYPE.DRILL, 54);
            //AddItem((int)BLOCKTYPE.DRILL, 54);
            //AddItem((int)BLOCKTYPE.DRILL, 54);
            //AddItem((int)BLOCKTYPE.MINERAL, 54);

            foreach(var tech in GameManager.GetTechBase().database)
                this.AddItem(tech.Key, 1);

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