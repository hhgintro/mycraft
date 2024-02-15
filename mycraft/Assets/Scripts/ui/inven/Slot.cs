using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
     public class Slot : MonoBehaviour
        , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler//, IDropHandler
    {
        public InvenBase owner;
        public int _panel;
        public int _slot;
        //public InvenItemData itemData;

        public GameObject _techCancel; //연구중인 Tech 취소버튼.

        ItemBase _recipe;

        public void Clear()
        {
            this._recipe = null;
            if (0 < this.transform.childCount)  //multiple은 background가 없으니 예외처리가 필요.
                this.transform.GetChild(0).gameObject.SetActive(false);
        }
        public ItemData GetItemData()
        {
            if (this.transform.childCount <= 1) //1: slot에 background가 추가되었으므로.
				return null;

            return this.transform.GetChild(1).GetComponent<ItemData>(); //1: slot에 background가 추가되었으므로.
		}

        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (null == this._techCancel) return;
            //this._techCancel.SetActive(true);
            if (null != this._techCancel)   this._techCancel.SetActive(true);

            if(null != _recipe) Managers.Game.Tooltips.Activate((this._recipe));
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            //if (null == this._techCancel) return;
            //this._techCancel.SetActive(false);
            if (null != this._techCancel)   this._techCancel?.SetActive(false);
     
            Managers.Game.Tooltips.Deactivate();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (null == InvenBase.choiced_item)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                //내려놓을 수 있는지 확인한다.
                if (null != this.owner
                    && false == this.owner.CheckPutdownGoods(this._panel, this._slot, InvenBase.choiced_item.database.id))
                    return;

				this.PutdownChoicedItem();
			}
        }

        public void PutdownChoicedItem()
        {
			if (null == InvenBase.choiced_item) return;
            float fillAmount = InvenBase.choiced_item.GetFillAmount();
			this.AddItem(InvenBase.choiced_item, ref fillAmount);
			InvenBase.choiced_item = null;
			Managers.Game.DestoryBuilding();    //들고있는 건물을 내려놓는다.
		}
		public void AddItem(ItemData itemData, ref float fillAmount)
        {
            itemData.GetComponent<Image>().enabled = true;
            itemData.panel = this._panel;
            itemData.slot = this._slot;
            //
            itemData.owner = this.owner;
            itemData.transform.SetParent(this.transform);
            itemData.transform.position = this.transform.position;
            itemData.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //InvenItemData = null;

            //UI
            this.SetInven2Block(this._panel, _slot, itemData.database.id, itemData.amount, fillAmount);
            fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT; //리셋
		}

        //ChestInven에서 변경된 아이템정보를 ChestScript에 반영합니다.
        public void SetInven2Block(int panel, int slot, int id, int amount, float fillAmount)
        {
            if (null == this.owner)
                return;
            this.owner.SetInven2Block(panel, slot, id, amount, fillAmount);
        }

        public void SetRecipe(ItemBase recipe)
        {
            if(null == recipe) return;

            this._recipe = recipe;
            //ItemBase input = Managers.Game.ItemBases.FetchItemByID(itemid);
            this.transform.GetChild(0).GetComponent<Image>().sprite = recipe.icon;
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

    }
}