using FactoryFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class ForgeInven : InvenBase
	{
		//public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

		protected override void fnAwake()
		{
			//HG_TODO: 추후 아래 object들을 생성하도록 합니다.
			base._progress.Add(this.transform.Find("Progress/bar").GetComponent<Image>());
			base._progress.Add(this.transform.Find("Fuel-Progress/bar").GetComponent<Image>());

			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
				, this.transform.Find("Input-Panel")));

			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
				, this.transform.Find("Fuel-Panel")));

			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
				, this.transform.Find("Output-Panel")));
		}

		protected override void fnStart()
		{
			//locale
			//title text
			Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
		}

		public override bool CheckPickupGoods() { return true; }


        //protected override void Clear()
        //{
        //    base.Clear();
        //}

        public override void LinkInven(Building building, JSonDatabase recipe, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
			base.LinkInven(building, recipe, panels, progresses, destroy);
            SetRecipe(building, recipe);  //recipe
        }
        //{
        //    base.LinkInven(block, slotAmount, slots);
        //}

        void SetRecipe(Building building, JSonDatabase recipe)
        {
            if (null == building || null == building._itembase) return;
            if (null == recipe) return;
            FurnaceItemBase itembase = (FurnaceItemBase)building._itembase;

            //output
            for(int i=0; i < itembase._furnace.input.Count; ++i)
            {
                if (recipe.id != itembase._furnace.input[i].itemid)
                    continue;

                ItemBase output = Managers.Game.ItemBases.FetchItemByID(itembase._furnace.input[i].output);
                this._panels[2]._slots[0].SetRecipe(output);
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