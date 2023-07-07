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

        //public override void Save(BinaryWriter writer)
        //{

        //    //panel : only one

        //    ////slot amount
        //    //writer.Write(this._panels[0]._amount);

        //    ////�ӽ� List<> �� ����
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
