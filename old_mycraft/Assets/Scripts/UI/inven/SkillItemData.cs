using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
    public class SkillItemData : ItemData
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

        void Awake()
        {
            //if(0 < this.transform.childCount)
            //    textAmount = this.transform.GetChild(0).GetComponent<Text>();

            //inven = GameManager.GetInventory();
            //tooltip = GameManager.GetTooltip();
        }
        //private void Start()
        //{

        //}
        public override void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("point enter");
            if (false == InvenBase.choiced_item)
                GameManager.GetTooltip().Activate((base.database));
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("point exit");
            GameManager.GetTooltip().Deactivate();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (false == InvenBase.choiced_item)
                GameManager.GetTooltip().Activate((base.database));

            base.OnPointerDown(eventData);
        }

        protected override void OnMouseLButtonDown()
        {
            base.OnMouseLButtonDown();

            //Debug.Log("Skill item clicked");
            ItemBase itembase = (ItemBase)base.database;
            if(false == this.owner.SetOutput(itembase))
            {
                //HG_TODO : block에서 생산함 outpu설정하지 않을 경우에는
                //          자체스킬로 인벤에 넣어주는 기능
                //..

                CreateItem();
                //for (int i = 0; i < skillbase.cost.items.Count; ++i)
                //{
                //    SkillCostItem costitem = skillbase.cost.items[i];


                //    //인벤 & quick에서 필요한 아이템 존재여부 체크

                //    //필요한 아이템 삭제

                //}

                ////아이템 생성(quick 또는 인벤에 넣어준다.)
                //for (int i=0; i<skillbase.outputs.Count; ++i)
                //    GameManager.AddItem(skillbase.outputs[i].itemid, skillbase.outputs[i].amount);


            }
            //if(null != this.owner._block)
            //{

            //    return;
            //}
            //base.owner.Reset();
        }

        protected override void OnMouseRButtonDown()
        {
            base.OnMouseRButtonDown();

            for(int i=0; i<5; ++i)
            {
                if (false == CreateItem())
                    break;
                //Debug.Log("skill create item: " + (i + 1));
            }
        }

        //cost아이템이 부족하면 false를 리턴합니다.
        private bool CreateItem()
        {
            ItemBase itembase = (ItemBase)base.database;
            //생산시설이 필요한 경우에는 직접 생산할 수 없습니다.
            if (false == itembase.DIY) return false;

            for (int i = 0; i < itembase.cost.items.Count; ++i)
            {
                BuildCostItem costitem = itembase.cost.items[i];
                //인벤 & quick에서 필요한 아이템 존재여부 체크
                int amount = GameManager.GetInventory().GetAmount(costitem.itemid);
                amount += GameManager.GetQuickInven().GetAmount(costitem.itemid);
                if (amount < costitem.amount)
                {
                    //Debug.Log("need more item amount: " + amount + "/" + costitem.amount);
                    return false;
                }
                //..
            }

            for (int i = 0; i < itembase.cost.items.Count; ++i)
            {
                BuildCostItem costitem = itembase.cost.items[i];
                //필요한 아이템 삭제
                int amount = costitem.amount;
                amount = GameManager.GetInventory().SubItem(costitem.itemid, amount);
                if (0 < amount)
                    amount = GameManager.GetQuickInven().SubItem(costitem.itemid, amount);
                //..
            }

            //아이템 생성(quick 또는 인벤에 넣어준다.)
            //for (int i = 0; i < itembase.outputs.Count; ++i)
            //    GameManager.AddItem(itembase.outputs[i].itemid, itembase.outputs[i].amount);
            GameManager.AddItem(itembase.id, itembase.cost.outputs);

            return true;
        }
    }

}