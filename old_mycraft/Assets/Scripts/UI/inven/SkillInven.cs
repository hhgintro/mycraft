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

        void Awake()
        {
            //base.Awake();
            base.Init();

            Transform parent = this.transform.Find("Skills");

            for(int i=0; i<5; ++i)
            {
                GameObject objPanel = UnityEngine.Object.Instantiate(InvenBase._invenPanel);
                objPanel.transform.SetParent(parent, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
                objPanel.name = "Slot-Panel-" + (i + 1);// i.ToString("D2");

                base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                    , null
                    , objPanel
                    , InvenBase._invenSlot));
            }

            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
        }
        
        void Start()
        {
            this.SetActive(false);

            //database
            this.LinkSkillGroup(0);

            //locale
            //title text
            LocaleManager.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());

        }

        //protected override void Clear()
        //{
        //    base.Clear();
        //}

        public override bool SetOutput(SkillBase skillbase)
        {
            //machin에서 skill창을 열었을 경우에는 _block이 null이 아닙니다.
            //이때는 machine에서 생성할 아이템정보를 세팅합니다.
            //_block이 null인 경우(인벤에 딸려서 오픈된 경우)에는
            //인벤의 아이템을 재료로하여 아이템을 생성하도록 합니다.
            if (null != base._block)
            {
                base._block.SetOutput(skillbase);
                this.SetActive(false);
                return true;
            }
            return false;
        }

        public virtual void LinkSkillGroup(int category)
        {
            //동일한 경우는...무시
            //if (this.category == category) return;
            this.category = category;

            this.Clear();
            for (int i = 0; i < GameManager.GetSkillBase().database.Count; ++i)
            {
                SkillBase database = GameManager.GetSkillBase().database[i];
                if (database.id <= 0)
                    continue;//reset skill은 제외시킨다.
                if(database.category != category)
                    continue;
                int panel = database.panel;
                this.AddSkill(base._panels[panel], database.id, 0);
            }

        }

        public virtual void LinkInven(BlockScript block, List<BlockSlotPanel> panels, List<Progress> progresses)
        {
            //base.LinkInven(block, slotAmount, slots);
            this._block = block;
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