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
 
		public override void InitStart()
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
                CreateItem(1);   //n배 만큼생성
        }

        protected override void OnMouseRButtonDown()
        {
            base.OnMouseRButtonDown();
            CreateItem(5);  //n배 만큼생성
        }

		//cost아이템이 부족하면 false를 리턴합니다.
		//LOOP:수치만큼 생성한다.( = cost.output * LOOP )
		private bool CreateItem(int LOOP)
        {
            if(LOOP <= 0) return false;

            ItemBase itembase = (ItemBase)base.database;
            //생산시설이 필요한 경우에는 직접 생산할 수 없습니다.
            if (false == itembase.DIY)
            {
                //Debug.LogError($"{itembase.Title}은 수작업을 할 수 없습니다.");
                //string msg = "{0}은 수작업을 할 수 없습니다.";
                Debug.LogError(string.Format(cantnot_make_by_hand, itembase.Title));
                return false;
            }

            //재료아이템 존재여부 체크
            for (int i = 0; i < itembase.cost.items.Count; ++i)
            {
                BuildCostItem costitem = itembase.cost.items[i];
                //인벤 & quick에서 필요한 아이템 존재여부 체크
                int amount = Managers.Game.GetAmount(costitem.itemid);
                if (amount < costitem.amount * LOOP)
                {
                    //남은 재료아이템으로 만들수 있는 최대값
                    LOOP = Math.Min(LOOP, (amount / costitem.amount));
                    if (0 == LOOP) return false;
                }
                //..
            }

            //재료아이템 삭제
            for (int i = 0; i < itembase.cost.items.Count; ++i)
            {
                BuildCostItem costitem = itembase.cost.items[i];
                //필요한 아이템 삭제
                int amount = costitem.amount * LOOP;
                Managers.Game.SubItem(costitem.itemid, amount);
            }

            //아이템 생성(quick 또는 인벤에 넣어준다.)
            //Managers.Game.AddItem(itembase.id, itembase.cost.outputs * LOOP);
            Managers.Game.AddItem(itembase.cost.outputs[0].itemid, itembase.cost.outputs[0].amount * LOOP, MyCraft.Global.FILLAMOUNT_DEFAULT);
			return true;
        }
    }

}