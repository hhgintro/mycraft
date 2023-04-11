using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class QuickInven : ItemInvenBase
    {
        protected GameObject keyPanel;  //단축번호를 표기한다.

        public GameObject inventoryKey { get; private set; }

        public List<GameObject> keys = new List<GameObject>();

        void Awake()
        {

            //this.database = GetComponent<ItemDatabase>();
            //this.inventoryPanel = GameObject.Find("Item_Canvas/QuickInven/Inventory Panel").gameObject;
            //this.slotPanel = this.inventoryPanel.transform.FindChild("Slot Panel").gameObject;
            //this.keyPanel = this.inventoryPanel.transform.FindChild("Key Panel").gameObject;
            base.canvas_ui = this.transform.GetComponent<CanvasGroup>();
            this.keyPanel = this.transform.Find("Key Panel").gameObject;

            inventoryKey = Resources.Load<GameObject>("prefab/ui/SlotKey") as GameObject;

        }

        void Start()
        {

            base._panels.Add(new InvenSlotPanel(base._panels.Count, 0, this
                , null
                , this.transform.Find("Slot Panel").gameObject
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
                    keys.Add(UnityEngine.Object.Instantiate(inventoryKey));
                    //keys[i].GetComponent<Slot>().id = i;
                    //keys[i].GetComponent<Slot>().owner = this;
                    keys[i].transform.SetParent(keyPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
                                                                           //slot과 같은 위치로 설정해 줍니다.
                    //keys[i].transform.position = slots[i].transform.position;

                    string name = 0.ToString();
                    if (i < 9) name = (i + 1).ToString();

                    keys[i].name = name;
                    if (0 < keys[i].transform.childCount)
                        keys[i].transform.GetChild(0).GetComponent<Text>().text = name;
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