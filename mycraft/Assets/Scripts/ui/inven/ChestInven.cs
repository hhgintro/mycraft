using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class ChestInven : InvenBase
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

        //클릭한 아이템을 이동시킨다.(half가 true이면 반만 보낸다.)
        public override int MoveItemData(InvenBase targetInven, bool half, int itemid, int panel, int slot)
        {
            if(true == Managers.Game.Inventories.gameObject.activeSelf)
                return base.MoveItemData(Managers.Game.Inventories, half, itemid, panel, slot);

            return 0;
        }
        //클릭한 아이템과 동일한 아이템 모두를 이동시킨다.(half가 true이면 반만 보낸다.)
        public override void MoveSameItemData(InvenBase targetBase, bool half, int itemid, int panel)
        {
            if (true == Managers.Game.Inventories.gameObject.activeSelf)
            {
                base.MoveSameItemData(Managers.Game.Inventories, half, itemid, panel);
                return;
            }
        }


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