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
            base.Init();

            int slotAmount = 64;// GameManager.GetTechBase().database.Count;
            //base.Start();

            base._panels.Add(new InvenPanel(base._panels.Count, slotAmount, this
                , this.transform.Find("Viewport/Slot Panel")));

            //base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
        }

        void Start()
        {
            foreach(var tech in Managers.Game.TechBases.database)
                this.AddItem(tech.Key, 1);

            //locale
            //title text
            Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());

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