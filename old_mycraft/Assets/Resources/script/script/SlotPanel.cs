using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class SlotPanel<T>
    {
        public int _panel;  //자신의 번호
        public int _amount { get; private set; } //slot 개수
        //public List<BlockSlot> _slots = new List<BlockSlot>();
        public List<T> _slots = new List<T>();


        public SlotPanel(int panel, int amount)//, Progress progress)
        {
            this._panel = panel;
            //this._amount = amount;
            //this._progress = progress;

            this._slots.Clear();
            //for (int i = 0; i < amount; ++i)
            //    //this._slots.Add(new BlockSlot(panel));
            //    this._slots.Add(default(T));

            //this.SetAmount(amount);
        }

        public virtual void Clear() { }
        //slot 개수를 설정합니다.
        public virtual void SetAmount(int amount)
        {
            this._amount = amount;
            //for (int i = 0; i < amount; ++i)
            //    //this._slots.Add(new BlockSlot(panel));
            //    this._slots.Add(default(T));
        }

    }

    public class BlockSlotPanel : SlotPanel<BlockSlot>
    {
        //public int _panel;  //자신의 번호
        //public int _amount; //slot 개수
        //public List<BlockSlot> _slots = new List<BlockSlot>();
        ////public List<T> _slots = new List<T>();

        //public BlockSlotPanel(int panel, int amount)
        //{
        //    this._panel = panel;
        //    this._amount = amount;

        //    this._slots.Clear();
        //    for (int i = 0; i < amount; ++i)
        //        this._slots.Add(new BlockSlot(panel));
        //}

        public BlockSlotPanel(int panel, int amount)
            : base(panel, amount)//, progress)
        {
            SetAmount(amount);
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void SetAmount(int amount)
        {
            base.SetAmount(amount);
            for (int i = 0; i < amount; ++i)
                this._slots.Add(new BlockSlot(i, base._panel));
            //this._slots.Add(default(T));
        }

        //amount : 필요한 개수이상이면 해당slot정보가 리턴됩니다.
        public BlockSlot GetFillSlot(int amount)
        {
            //뒤에서 체크하는 이유는 중간에 뺄때...crash방지차원.
            for (int i = base._slots.Count - 1; 0 <= i; --i)
            {
                if (amount <= base._slots[i].GetItemAmount())
                    return base._slots[i];
            }
            return null;
        }
        //amount만큼 있는면 true
        public bool GetFillSlot(int slot, int amount)
        {
            return (amount <= base._slots[slot].GetItemAmount());
        }

        public bool GetIsFull(int itemid)
        {
            for (int i = 0; i < base._slots.Count; ++i)
            {
                if (0 == base._slots[i]._itemid)
                    return false;   //output에 여유가 있다.

                //다른 아이템
                if (itemid != _slots[i]._itemid)
                    continue;

                ItemBase itembase = GameManager.GetItemBase().FetchItemByID(_slots[i]._itemid);
                if (null == itembase) continue;

                if (base._slots[i]._amount < itembase.Stackable)
                    return false;   //output에 여유가 있다.
            }
            return true;//가득 참.
        }


    }

    public class InvenSlotPanel : SlotPanel<Slot>
    {
        public Image _progress;

        private InvenBase _inven;
        private GameObject _objPanel;
        private GameObject _inventorySlot;

        //public int _panel;  //자신의 번호
        //public int _amount;

        //public List<Slot> _slots = new List<Slot>();
        ////public List<T> _slots = new List<T>();

        public InvenSlotPanel(int panel, int amount, InvenBase inven, Image progress, GameObject objPanel, GameObject inventorySlot)
            : base(panel, amount)
        {
            this.Clear();

            this._inven = inven;
            this._objPanel = objPanel;
            this._inventorySlot = inventorySlot;
            //this._panel = panel;

            this.SetAmount(amount);
        }

        public override void Clear()
        {
            base.Clear();

            //this._objPanel = null;
            //this._panel = 0;
            this.SetAmount(0);
            for (int i = 0; i < this._slots.Count; ++i)
            {
                ItemData itemData = this._slots[i].GetItemData();
                if (null != itemData)
                    UnityEngine.Object.Destroy(itemData.gameObject);
                UnityEngine.Object.Destroy(this._slots[i].gameObject);
            }
            this._slots.Clear();
        }

        public override void SetAmount(int amount)
        {
            base.SetAmount(amount);
            for (int i = 0; i < amount; ++i)
                this.CreateSlot();
        }

        public virtual Slot CreateSlot()
        {
            Slot s = UnityEngine.Object.Instantiate(this._inventorySlot).GetComponent<Slot>();
            //_slots.Add(inven.CreateSlot());
            s.panel = this._panel;
            s.slot = _slots.Count;
            s.owner = this._inven;
            s.transform.SetParent(this._objPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution

            _slots.Add(s);
            return s;
        }

    }


    //public class TechSlotPanel : SlotPanel<TechSlot>
    //{
    //    public Image _progress;

    //    private TechInvenBase _inven;
    //    private GameObject _objPanel;
    //    private GameObject _inventorySlot;

    //    //public int _panel;  //자신의 번호
    //    //public int _amount;

    //    //public List<Slot> _slots = new List<Slot>();
    //    ////public List<T> _slots = new List<T>();

    //    public TechSlotPanel(int panel, int amount, TechInvenBase inven, Image progress, GameObject objPanel, GameObject inventorySlot)
    //        : base(panel, amount)
    //    {
    //        this.Clear();

    //        this._inven = inven;
    //        this._objPanel = objPanel;
    //        this._inventorySlot = inventorySlot;
    //        //this._panel = panel;

    //        this.SetAmount(amount);
    //    }

    //    public override void Clear()
    //    {
    //        base.Clear();

    //        //this._objPanel = null;
    //        //this._panel = 0;
    //        this._amount = 0;
    //        for (int i = 0; i < _slots.Count; ++i)
    //        {
    //            ItemData itemData = _slots[i].GetItemData();
    //            if (null != itemData)
    //                UnityEngine.Object.Destroy(itemData.gameObject);
    //            UnityEngine.Object.Destroy(_slots[i].gameObject);
    //        }
    //        _slots.Clear();
    //    }

    //    public override void SetAmount(int amount)
    //    {
    //        base.SetAmount(amount);
    //        for (int i = 0; i < amount; ++i)
    //        {
    //            //items.Add(new Item());
    //            _slots.Add(UnityEngine.UnityEngine.Object.Instantiate(this._inventorySlot).GetComponent<TechSlot>());
    //            //_slots.Add(inven.CreateSlot());
    //            _slots[i].panel = this._panel;
    //            _slots[i].slot = i;
    //            _slots[i].owner = this._inven;
    //            _slots[i].transform.SetParent(this._objPanel.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
    //        }
    //    }



    //}



}