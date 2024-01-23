using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class TurretInven : InvenBase
    {
		//public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

		protected override void fnAwake()
		{
			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
                , this.transform.Find("slot-panel")));
        }

		protected override void fnStart()
		{
			//this.gameObject.SetActive(false);

			//locale
			//title text
			Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }

        public override bool CheckPickupGoods() { return true; }


        //public override void Save(BinaryWriter writer)
        //{
        //    base.Save(writer);
        //}

        //public override void Load(BinaryReader reader)
        //{
        //    base.Load(reader);
        //}


    }//..class Inventory
}//..namespace MyCraft