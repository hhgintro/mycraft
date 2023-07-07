using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class ResetItemData : ItemData
    {
        //public ItemInvenBase owner;
        //public ItemBase itembase;
        //public int amount;
        //public int panel;
        //public int slot;

        //private Inventory inven;
        //private Tooltip tooltip;
        //private Vector2 offset;

        //public Text textAmount;

        //protected override void Awake()
        //{
        //    //if(0 < this.transform.childCount)
        //    //    textAmount = this.transform.GetChild(0).GetComponent<Text>();

        //    //inven = GameManager.GetInventory();
        //    //tooltip = GameManager.GetTooltip();
        //}
        //private void Start()
        //{

        //}
        //public override void OnPointerEnter(PointerEventData eventData)
        //{
        //    //Debug.Log("point enter");
        //    if (false == InvenBase.choiced_item)
        //        Managers.Game.Tooltips.Activate((base.database));
        //}

        //public override void OnPointerExit(PointerEventData eventData)
        //{
        //    //Debug.Log("point exit");
        //    Managers.Game.Tooltips.Deactivate();
        //}

        //public override void OnPointerDown(PointerEventData eventData)
        //{
        //    base.OnPointerDown(eventData);
        //    base.owner.Reset();
        //}

        protected override void OnMouseLButtonDown()
        {
            base.owner.OnReset();
        }
        //protected override void OnMouseRButtonDown() { }

    }

}