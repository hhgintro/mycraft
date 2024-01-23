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

		protected override void fnAwake()
		{
			base.fnAwake();

            base._panels.Add(new InvenPanel(base._panels.Count, 0, this, this.transform.Find("Slot Panel")));
            base._panels.Add(new InvenPanel(base._panels.Count, 0, this, this.transform.Find("Viewport/Slot Panel")));

            //base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
        }

		protected override void fnStart()
		{
			foreach (var tech in Managers.Game.TechBases.database)
                this.AddTech(VIEW, tech.Key);

            //locale
            //title text
            Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());

        }

		//TechInven에 연구에 필요한 소진률을 확인 받는다.
		public float GetFillAmount(float fillAmount)
        {
            TechItemData itemdata = GetResearch();
            if (null == itemdata) return 0.0f;

			//if(1.0f < itemdata._fillAmount + fillAmount) fillAmount = 1.0f - itemdata._fillAmount;
			fillAmount = itemdata.AddFillAmount(fillAmount);
            if(itemdata.Learned)
            {
                //연구완료
                OnResearchCompleted((TechBase)itemdata.database);
				Debug.Log($"Tech({itemdata.database.Title}) 연구완료");
				RESEARCH.Remove(0);
			}
			return fillAmount;//Tech에 기여한 소진률.
		}
        //연구목록중 처음꺼(연구중 or 연구대기중)
        public TechItemData GetResearch()
        {
            if (0 == RESEARCH._slots.Count) return null;
            return (TechItemData)RESEARCH._slots[0].GetItemData();
		}
		//연구시작
		public bool OnResearch(TechBase techbase)
        {
            //n개까지만 등록가능
            if (MyCraft.Common.MAX_TECH_RESEARCH < RESEARCH._slots.Count) return false;
            
            //중복체크
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
        void OnResearchCompleted(TechBase techbase)
        {
            for (int s = 0; s < VIEW._slots.Count; ++s)
            {
                //            ItemData itemdata = VIEW._slots[s].GetItemData();
                //            //one-self
                //            if (itemdata.database == techbase)
                //            {
                //                VIEW._slots[s].GetComponent<Image>().color = Color.green;
                //                continue;
                //            }

                //if(base.IsNextTech(itemdata, techbase))
                //            {
                //                if(base.IsAllLearnedPreTech((TechBase)itemdata.database))
                //		VIEW._slots[s].GetComponent<Image>().color = Color.yellow;
                //}
                OnResearchCompleted(VIEW._slots[s], techbase);
            }

            //TechDesc에 완료통보
            Managers.Game.TechDescs.OnResearchCompleted(techbase);
        }

        //삭제예정인 코드
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