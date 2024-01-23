using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class QuickInven : InvenBase
    {
        protected GameObject keyPanel;  //단축번호를 표기한다.
        public List<GameObject> keys = new List<GameObject>();

		protected override void fnAwake()
		{
			base._panels.Add(new InvenPanel((byte)base._panels.Count, 0, this
                , this.transform.Find("slot-panel")));

            this.keyPanel = this.transform.Find("key-panel").gameObject;
            //this.keyPanel.GetComponent<Image>().raycastTarget = false;  //퀵슬롯 아이템을 줍거나 넣을수 없었다.
        }

		protected override void fnStart()
		{
			//frame이 끝나야 slot의 위치를 알수 있어서 coroutine을 사용합니다.
			StartCoroutine(CheckSlotPosition());
        }

        IEnumerator CheckSlotPosition()
        {
            yield return new WaitForEndOfFrame();

            for (int p = 0; p < base._panels.Count; ++p)
            {
                List<Slot> slots = base._panels[p]._slots;
                for (int i = 0; i < slots.Count; ++i)
                {
                    RectTransform rt = (RectTransform)slots[i].transform;
                    //items.Add(new Item());
                    //keys.Add(UnityEngine.Object.Instantiate(InvenBase.Hotkey));
                    //keys.Add(Managers.Resource.Instantiate("ui/QuickHotkey", keyPanel.transform));
                    ////keys[i].GetComponent<Slot>().id = i;
                    ////keys[i].GetComponent<Slot>().owner = this;
                    //keys[i].transform.SetParent(keyPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
                    //slot과 같은 위치로 설정해 줍니다.
                    //keys[i].transform.position = slots[i].transform.position;

                    string name = 0.ToString();
                    if (i < 9) name = (i + 1).ToString();

                    GameObject go = Managers.Resource.Instantiate("Prefabs/ui/QuickHotkey", null);
                    go.transform.SetParent(keyPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
                    go.name = name;
                    if (0 < go.transform.childCount) go.transform.GetChild(0).GetComponent<Text>().text = name;
                    keys.Add(go);
                }
            }
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