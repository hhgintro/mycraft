using FactoryFramework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class SkillInven : TechInvenBase
    {
        private int category;
        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체
        //public GameObject inventoryPanel;

        private InvenPanel _group;

        protected override void fnAwake()
        {
			base.fnAwake();
        }

		protected override void fnStart()
		{
            Transform skill_group = this.transform.Find("Categories");
            Transform parent = this.transform.Find("Skills");
            
            //categories
            _group = new InvenPanel(0, 0, this, skill_group);
            for(int i=0; i<Managers.Game.Categories.database.Count; ++i)
                base.AddCategory(_group, i);
            //skills
            for (int i = 0; i < 10; ++i)
            {
                GameObject objPanel = Managers.Resource.Instantiate("Prefabs/ui/Slot Panel", null);
                objPanel.transform.SetParent(parent, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
                objPanel.name = "Slot-Panel-" + (i + 1);

                base._panels.Add(new InvenPanel(base._panels.Count, 0, this
                    , objPanel.transform));
            }

            //database
            this.LinkSkillGroup(0);

            //this.SetActive_1(false);

            //locale
            //title text
            Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());

        }

        //protected override void Clear()
        //{
        //    base.Clear();
        //}

        public override bool AssignRecipe(ItemBase itembase)
        {
            //machin에서 skill창을 열었을 경우에는 _block이 null이 아닙니다.
            //이때는 machine에서 생성할 아이템정보를 세팅합니다.
            //_block이 null인 경우(인벤에 딸려서 오픈된 경우)에는
            //인벤의 아이템을 재료로하여 아이템을 생성하도록 합니다.
            if (null == base._building) return false;
            if (false == base._building.AssignRecipe(itembase)) return false;
            this.gameObject.SetActive(false);
            return true;
        }

        public virtual void LinkSkillGroup(int category)
        {
            if (Managers.Game.Categories.database.Count <= category)
                return;

            //동일한 경우는...무시
            //if (this.category == category) return;
            this.category = category;

            this.Clear();
            Category categories = Managers.Game.Categories.database[category];
            if (null == categories)
                return;

            for (int p = 0; p < categories.panels.Count; ++p)
            {
                for(int i=0; i<categories.panels[p].items.Count; ++i)
                    this.AddSkill(base._panels[p], categories.panels[p].items[i].itemid, 0);
            }

        }
		public override void LinkInven(Building building, JSonDatabase recipe, Dictionary<int/*itemid*/, BuildingItem> inputs, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
        {
			this._building = building;
		}

		public override void LinkInven(Building building, JSonDatabase recipe, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
			//base.LinkInven(block, slotAmount, slots);
			this._building = building;
        }

        ////Block에서 변경된 내용을 Inven에 반영합니다.
        //public void SetBlock2Inven(int slot, int amount)
        //{
        //    if (this.slots.Count <= slot)
        //        return;

        //    InvenItemData itemData = this.slots[slot].GetItemData();
        //    if (null == InvenItemData)
        //        return;

        //    //Debug.Log("inven item slot[" + slot + "], amount[" + amount + "]");
        //    if (amount <= 0)
        //    {
        //        Destroy(InvenItemData.gameObject);
        //        return;
        //    }
        //    InvenItemData.SetStackCount(amount);
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