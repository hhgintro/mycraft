using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyCraft
{
    public class BuildingSlot
    {
        public int _panel;  //owner의 번호
        public int _slot;   //자신의 구분자

        public int _itemid;//item id
        public int _amount;

        public BuildingSlot(int panel, int slot)
        {
            this._panel = panel;
            this._slot  = slot;
        }

        public void Clear()
        {
            _itemid = 0;
            _amount = 0;
        }
        //amount만큼 있는면 true
        public int GetItemAmount()
        {
            //itemid
            if (0 == this._itemid) return 0;
            //amount
            return this._amount;
        }

        public bool OnOverlapItem(int itemid, int amount, int stackable)
        {
            //itemID가 다르면...무시
            if (this._itemid != itemid) return false;
            //가득차면...무시
            if (stackable <= this._amount) return false;

            this._amount += amount;
            return true;
        }

        public bool OnCreateItemData(int itemid)
        {
            if (0 != this._itemid) return false;

            this._itemid = itemid;
            this._amount = 1;
            return true;
        }



    }
}