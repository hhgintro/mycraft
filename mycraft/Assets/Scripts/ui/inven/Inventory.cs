using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class Inventory : InvenBase
    {
        void Awake()
        {
            base._panels.Add(new InvenPanel((byte)base._panels.Count, 0, this
                , this.transform.Find("slot-panel")));
        }

        void Start()
        {

            //title
            Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }

        public override bool CheckPickupGoods() { return true; }

		private void Update()
		{
			float height = 100;
			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				//Debug.Log("확대");
				Resize(0);

				RectTransform inven = (RectTransform)this.transform;
				//inven.sizeDelta = new Vector2(inven.sizeDelta.x, inven.sizeDelta.y + height * Time.deltaTime);
				//this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
			}
			//if (Input.GetKey(KeyCode.UpArrow))
			//{
			//	RectTransform inven = (RectTransform)this.transform;
			//	inven.sizeDelta = new Vector2(inven.sizeDelta.x, inven.sizeDelta.y + height * Time.deltaTime);
			//	this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
			//}
			//if (Input.GetKey(KeyCode.DownArrow))
			//{
			//	RectTransform inven = (RectTransform)this.transform;
			//	inven.sizeDelta = new Vector2(inven.sizeDelta.x, inven.sizeDelta.y - height * Time.deltaTime);
			//	this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
			//}
			//if (Input.GetKeyDown(KeyCode.DownArrow))
			//{
			//	RectTransform inven = (RectTransform)this.transform;
			//	Debug.Log($"inven:{inven.rect}");

			//	GridLayoutGroup panelGrid = this.transform.Find("slot-panel").GetComponent<GridLayoutGroup>();
			//	RectTransform panel = panelGrid.GetComponent<RectTransform>();
			//	Debug.Log($"panel:{panel.rect}");
			//}
		}

		public bool Resize(int p)
		{
			if (this._panels.Count <= p) return false;
			if(this._panels[p]._slots.Count <=0) return false;

			RectTransform inven = (RectTransform)this.transform;

			GridLayoutGroup panelGrid = this.transform.Find("slot-panel").GetComponent<GridLayoutGroup>();
			//row개수
			int rows = panelGrid.transform.childCount / panelGrid.constraintCount + 1;
			//
			float height = panelGrid.padding.top// + panelGrid.padding.bottom
				+ panelGrid.spacing.y + panelGrid.cellSize.y * rows;
				//"slot-panel"위치가 title 조금아래에 위치함.
				//+ panelGrid.GetComponent<RectTransform>().rect.top - inven.rect.top;
			Debug.Log($"height:{inven.sizeDelta.y}=>{height}");

			//변경
			inven.sizeDelta = new Vector2(inven.sizeDelta.x, height);
			this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
			return true;
		}

		//public override void Save(BinaryWriter writer)
		//{

			//    //panel : only one

			//    ////slot amount
			//    //writer.Write(this._panels[0]._item._amount);

			//    ////임시 List<> 에 저장
			//    //List<ItemData> items = new List<ItemData>();
			//    //for (int i = 0; i < this._panels[0]._slots.Count; ++i)
			//    //{
			//    //    ItemData itemData = this._panels[0]._slots[i].GetItemData();
			//    //    if (null == itemData) continue;
			//    //    items.Add(itemData);
			//    //}

			//    ////item count
			//    //writer.Write(items.Count);
			//    ////item info
			//    //for (int i = 0; i < items.Count; ++i)
			//    //{
			//    //    writer.Write(items[i].slot);    //slot
			//    //    writer.Write(items[i].database.id); //item id
			//    //    writer.Write(items[i].amount);  //amount
			//    //}
			//    base.Save(writer);
			//}

			//public override void Load(BinaryReader reader)
			//{
			//    //panel : only one

			//    ////slot amount
			//    //int slotAmount = reader.ReadInt32();
			//    //this._panels[0].SetAmount(slotAmount);
			//    ////Debug.Log("slot = " + slotAmount);

			//    ////item count
			//    //int itemcount = reader.ReadInt32();
			//    ////Debug.Log("itemcount = " + itemcount);
			//    ////item info
			//    //for (int i = 0; i < itemcount; ++i)
			//    //{
			//    //    int panel = 0;
			//    //    int slot = reader.ReadInt32();
			//    //    int id = reader.ReadInt32();
			//    //    int amount = reader.ReadInt32();
			//    //    //Debug.Log("slot[" + slot + "], id[" + id + "], amount[" + amount + "]");

			//    //    SetItem(panel, slot, id, amount);
			//    //}
			//    base.Load(reader);
			//}
	}
}//..namespace MyCraft
