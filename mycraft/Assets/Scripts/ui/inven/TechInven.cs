using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace MyCraft
{
    public class TechInven : TechInvenBase
    {
        InvenPanel RESEARCH     => base._panels[0];   //연구예약 panel
        InvenPanel VIEW         => base._panels[1];   //연구목록 panel

        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

        void Awake()
        {
            base.Init();

            base._panels.Add(new InvenPanel(base._panels.Count, 0, this, this.transform.Find("Slot Panel")));
            base._panels.Add(new InvenPanel(base._panels.Count, 0, this, this.transform.Find("Viewport/Slot Panel")));

            //base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
        }

        void Start()
        {
            foreach (var tech in Managers.Game.TechBases.database)
                this.AddTech(VIEW, tech.Key);

            //locale
            //title text
            Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());

        }

        //연구시작
        public bool OnResearch(TechBase techbase)
        {
            //중복방지
            for (int s = 0; s < RESEARCH._slots.Count; ++s)
            {
                if (techbase == RESEARCH._slots[s].GetItemData().database)
                    return false;//중복방지
            }
            this.AddTech(RESEARCH, techbase, true); //true:취소버튼을 추가한다.
            return true;
        }
        //연구취소
        public void OnResearchCancel(int slot)
        {
            Debug.Log($"Tech({slot}) 삭제");
            RESEARCH.Remove(slot);
        }
        //연구완료
        public void OnResearchCompleted(int id, List<int> nexts)
        {
            for (int s = 0; s < VIEW._slots.Count; ++s)
            {
                ItemData itemdata = VIEW._slots[s].GetItemData();
                //one-self
                if (itemdata.database.id == id)
                {
                    VIEW._slots[s].GetComponent<Image>().color = Color.green;
                    continue;
                }
                //next-tech
                foreach (var next in nexts)
                {
                    if (itemdata.database.id != next)
                        continue;
                    VIEW._slots[s].GetComponent<Image>().color = Color.yellow;
                    break;
                }
            }
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