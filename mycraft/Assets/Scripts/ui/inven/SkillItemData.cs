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
        string cantnot_make_by_hand;
        void Start()
        {
            cantnot_make_by_hand = Managers.Locale.GetLocale("skill-item-data", "cantnot_make_by_hand");
        }

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

        //    if (false == InvenBase.choiced_item)
        //        Managers.Game.Tooltips.Activate((base.database));
        //}

        protected override void OnMouseLButtonDown()
        {
            base.OnMouseLButtonDown();

            //Debug.Log("Skill item clicked");
            ItemBase itembase = (ItemBase)base.database;
            if(false == this.owner.AssignRecipe(itembase))
            {
                //HG_TODO : block에서 생산함 output설정하지 않을 경우에는
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
            if (false == itembase.DIY)
            {
                //Debug.LogError($"{itembase.Title}은 수작업을 할 수 없습니다.");
                //string msg = "{0}은 수작업을 할 수 없습니다.";
                Debug.LogError(string.Format(cantnot_make_by_hand, itembase.Title));
                return false;
            }

            for (int i = 0; i < itembase.cost.items.Count; ++i)
            {
                BuildCostItem costitem = itembase.cost.items[i];
                //인벤 & quick에서 필요한 아이템 존재여부 체크
                int amount = Managers.Game.Inventories.GetAmount(costitem.itemid);
                amount += Managers.Game.QuickInvens.GetAmount(costitem.itemid);
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
                amount = Managers.Game.Inventories.SubItem(costitem.itemid, amount);
                if (0 < amount)
                    amount = Managers.Game.QuickInvens.SubItem(costitem.itemid, amount);
                //..
            }

            //아이템 생성(quick 또는 인벤에 넣어준다.)
            Managers.Game.AddItem(itembase.id, itembase.cost.outputs);
            return true;
        }
    }

}