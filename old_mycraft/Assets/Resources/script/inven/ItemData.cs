using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class ItemData : MonoBehaviour
        , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public InvenBase owner;
        //public TechBase itembase;
        public JSonDatabase database;
        public int amount;
        public int panel;
        public int slot;

        //private Inventory inven;
        //private Tooltip tooltip;
        //private Vector2 offset;

        public Text textAmount;

        protected virtual void Awake()
        {
            if(0 < this.transform.childCount)
                textAmount = this.transform.GetChild(0).GetComponent<Text>();

            //inven = GameManager.GetInventory();
            //tooltip = GameManager.GetTooltip();
        }
        //private void Start()
        //{

        //}
        public virtual void OnPointerEnter(PointerEventData eventData)
        { }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("point exit");
            GameManager.GetTooltip().Deactivate();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //L button
            if (Input.GetMouseButtonDown(0)) OnMouseLButtonDown();
            //R button
            else if (Input.GetMouseButtonDown(1)) OnMouseRButtonDown();
        }

        protected virtual void OnMouseLButtonDown()
        { }

        protected virtual void OnMouseRButtonDown()
        { }

        ////RETURN : 겹치고 남은 아이템 개수를 리턴합니다.
        //public virtual int CheckOverlapItem(int add)
        //{
        //    //cnt가 (+)이면 최대개수 초과를 의미합니다.
        //    int cnt = this.amount + add - this.itembase.Stackable;
        //    if(cnt <= 0)
        //    {
        //        this.AddStackCount(add, true);
        //        return 0;
        //    }

        //    this.amount = this.itembase.Stackable;
        //    this.AddStackCount(0, true);
        //    return cnt;//겹치고 남은 개수
        //}


        //bool CheckOverlapPickup(InvenItemData target, InvenItemData additem)
        //{
        //    //들고 있는것이 없으니 pickup가능
        //    if (null == target)
        //        return true;

        //    //잡을 아이템이 없으면 pickup 불가능
        //    if (0 == additem.itembase.id)
        //        return false;

        //    //다른 아이템(ID)을 들고 있으면 pickup 불가능
        //    if (target.itembase.id != additem.itembase.id)
        //        return false;

        //    //HG_TODO : stack count 를 초과해서는 더이상 pickup 불가능
        //    if (target.itembase.Stackable <= target.amount)
        //        return false;

        //    return true;//pickup 가능
        //}

        //public InvenItemData PickupAll(Transform parent, bool noti)
        //{
        //    this.transform.SetParent(this.transform.parent.parent.parent.parent.parent);
        //    GetComponent<CanvasGroup>().blocksRaycasts = false;
        //    //UI
        //    if (true == noti)//block으로 전달합니다.
        //        this.SetInven2Block(this.panel, this.slot, this.itembase.id, 0);//모두 집어들었기때문에 인벤은 amount=0 입니다.
        //    return this;
        //}

        //겹치고 남은 아이템의 개수를 리턴합니다.
        //this.amount <= 0 일때의 처리는 별도로 진행해 주셔야 됩니다.
        public virtual int AddStackCount(int add, bool noti)
        {
            return 0;
        }

        //public int SetStackCount(int val, bool noti)
        //{
        //    this.amount = val;
        //    //Debug.Log("inven slot amount: " + this.amount);
        //    if (0 <= this.amount)
        //        this.textAmount.text = this.amount.ToString();
        //    //UI
        //    if(true == noti)//block으로 전달합니다.
        //        this.SetInven2Block(this.panel, this.slot, this.itembase.id, this.amount);
        //    return this.amount;
        //}

        ////ChestInven에서 변경된 아이템정보를 ChestScript에 반영합니다.
        //public void SetInven2Block(int panel, int slot, int id, int amount)
        //{
        //    if (null == this.owner)
        //        return;
        //    this.owner.SetInven2Block(panel, slot, id, amount);
        //}
    }

}