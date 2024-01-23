using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class Inventory : InvenBase
    {
		protected override void fnAwake()
		{
			base._panels.Add(new InvenPanel((byte)base._panels.Count, 0, this
                , this.transform.Find("slot-panel")));
        }

		protected override void fnStart()
		{

			//title
			Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }

        public override bool CheckPickupGoods() { return true; }

        //클릭한 아이템을 이동시킨다.(half가 true이면 반만 보낸다.)
        public override int MoveItemData(InvenBase targetInven, bool half, int itemid, int panel, int slot)
        {
            if (true == Managers.Game.ChestInvens.gameObject.activeSelf)
                return base.MoveItemData(Managers.Game.ChestInvens, half, itemid, panel, slot);

            return 0;
        }
        //클릭한 아이템과 동일한 아이템 모두를 이동시킨다.(half가 true이면 반만 보낸다.)
        public override void MoveSameItemData(InvenBase targetBase, bool half, int itemid, int panel)
        {
            if (true == Managers.Game.ChestInvens.gameObject.activeSelf)
            {
                base.MoveSameItemData(Managers.Game.ChestInvens, half, itemid, panel);
                return;
            }
        }

        //update() : 인벤창 크기 변경 테스트중...
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

		//인벤창 크기 변경 테스트중...(정상작동 안함)
		//HG_TODO: 구지 크기 조정을 할 필요가 있을까. 창 크기를 크게 만들어 고정하면 되지 않겠나, 고려할 것
		public bool Resize(int panel)
		{
			if (this._panels.Count <= panel) return false;
			if(this._panels[panel]._slots.Count <=0) return false;

			RectTransform inven = (RectTransform)this.transform;

			GridLayoutGroup gridLayout = this.transform.Find("slot-panel").GetComponent<GridLayoutGroup>();
			////row개수
			//int rows = gridLayout.transform.childCount / gridLayout.constraintCount + 1;
			////
			//float height = gridLayout.padding.top// + gridLayout.padding.bottom
			//	+ gridLayout.spacing.y + gridLayout.cellSize.y * rows;
			//	//"slot-panel"위치가 title 조금아래에 위치함.
			//	//+ gridLayout.GetComponent<RectTransform>().rect.top - inven.rect.top;
			//Debug.Log($"height:{inven.sizeDelta.y}=>{height}");
			//Debug.Log($"inven:{inven.rect.top}, gridLayout{gridLayout.GetComponent<RectTransform>().rect.top}");

			////변경
			//inven.sizeDelta = new Vector2(inven.sizeDelta.x, height);
			//this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;


			//float availableWidth = inven.rect.width;
			//float availableHeight = inven.rect.height;

			//int columnCount = gridLayout.constraintCount;
			//int rowCount = Mathf.CeilToInt((float)transform.childCount / columnCount);

			//float cellWidth = (availableWidth - gridLayout.padding.horizontal - gridLayout.spacing.x * (columnCount - 1)) / columnCount;
			//float cellHeight = (availableHeight - gridLayout.padding.vertical - gridLayout.spacing.y * (rowCount - 1)) / rowCount;

			////gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
			//inven.sizeDelta = new Vector2(cellWidth, cellHeight);
			//this.GetComponent<RectTransform>().sizeDelta = inven.sizeDelta;
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
