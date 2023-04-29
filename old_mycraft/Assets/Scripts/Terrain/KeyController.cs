using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MyCraft
{
    public class KeyController : MonoBehaviour
    {
        GameObject _cheat_text;     //단축키 설명창(켜키/끄기)

        void Start()
        {
            if (null == _cheat_text) _cheat_text = GameObject.Find("Canvas/My Craft/cheat_text");
            _cheat_text.SetActive(true);

            Managers.Chat.gameObject.SetActive(false);   //chatting


            StartCoroutine(CheckKey());

        }

        IEnumerator CheckKey()
        {
            while (true)
            {
                CheckKey_Func();
                yield return 0;
            }
        }
        void CheckKey_Func()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //최상위 UI순
                //  GetSystemMenu
                //  GetTechInven / GetTechDesc
                //  GetInventory / GetSkillInven / GetChestInven / GetMachineInven / GetStoneFurnaceInven

                if(Managers.Chat.gameObject.activeSelf)
                {
                    Managers.Chat.gameObject.SetActive(false);
                    return;
                }

                //tech
                if (true == GameManager.GetTechInven().gameObject.activeSelf)
                {
                    GameManager.GetTechInven().gameObject.SetActive(false);
                    GameManager.GetTechDesc().gameObject.SetActive(false);
                    return;
                }

                bool bChecked = false;
                //inven
                if (true == GameManager.GetInventory().gameObject.activeSelf)
                {
                    GameManager.GetInventory().gameObject.SetActive(false);
                    bChecked = true;
                }
                //chest
                if (true == GameManager.GetChestInven().gameObject.activeSelf)
                {
                    GameManager.GetChestInven().gameObject.SetActive(false);
                    bChecked = true;
                }
                //machine
                if (true == GameManager.GetMachineInven().gameObject.activeSelf)
                {
                    GameManager.GetMachineInven().gameObject.SetActive(false);
                    bChecked = true;
                }
                //stone-furnace
                if (true == GameManager.GetStoneFurnaceInven().gameObject.activeSelf)
                {
                    GameManager.GetStoneFurnaceInven().gameObject.SetActive(false);
                    bChecked = true;
                }
                //skill
                if (true == GameManager.GetSkillInven().gameObject.activeSelf)
                {
                    GameManager.GetSkillInven().gameObject.SetActive(false);
                    bChecked = true;
                }

                if (true == bChecked) return;
                //GameManager.GetSystemMenu().gameObject.SetActive(!GameManager.GetSystemMenu().gameObject.activeSelf);
                Managers.SystemMenu.SetActive(!Managers.SystemMenu.activeSelf);

            }
            //"Enter"
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //꺼져있으면 켜고
                if (false == Managers.Chat.gameObject.activeSelf)
                {
                    Managers.Chat.gameObject.SetActive(true);
                    return;
                }
                //("")를 입력하면 꺼진다.
                else if (false == Managers.Chat.ready)
                {
                    Managers.Chat.gameObject.SetActive(false);
                    return;
                }

                //그외 처리
                //..
            }

            if (Managers.Chat.gameObject.activeSelf)
                return; //chatting창 열려있으면 못움직인다.

            //KeyCode.Alpha0 ~ KeyCode.Alpha9
            //quick slot( 1~10 )에서 아이템을 줍는다.
            for (int i=0; i<10; ++i)
            {
                if (Input.GetKeyDown(i + KeyCode.Alpha1))
                {
                    //InvenItemData itemData = GameManager.GetQuickInven().slots[i].GetItemData();
                    Slot s = GameManager.GetQuickInven().GetInvenSlot(0, i);
                    InvenItemData itemData = (InvenItemData)s.GetItemData();
                    if(null != itemData)
                    {
                        //pickAll의 경우에 교체시에는 pickup이 마지막으로 처리되어 0이 되는 결과가 발생
                        bool noti2block = true;

                        //들고 있다면...
                        if (null != InvenBase.choiced_item)
                        {
                            //같은 아이템을 들고 있다면, 겹치기
                            if (itemData.database.id == InvenBase.choiced_item.database.id)
                            {
                                int amount = itemData.SetStackCount(InvenBase.choiced_item.CheckOverlapItem(itemData.amount), true);
                                if (amount <= 0) Destroy(itemData.gameObject);//남은게 없다면...삭제
                                break;
                            }

                            //들고 있던건 내려놓고(다른아이템인 경우)
                            //InvenItemData.owner.slots[InvenItemData.slot].AddItem(InvenBase.choiced_item);
                            Slot s1 = itemData.owner.GetInvenSlot(itemData.panel, itemData.slot);
                            if (null != s1) s1.AddItem(InvenBase.choiced_item);
                            InvenBase.choiced_item = null;
                            noti2block = false;//교체시에는 Pickup을 block으로 전달하지 않는다.
                        }

                        //모두 줍는다.
                        InvenBase.choiced_item = itemData.PickupAll(itemData.transform.parent.parent.parent.parent.parent, noti2block);
                        //UI
                        this.ActiveIcon(InvenBase.choiced_item);                        
                    }
                    break;
                }
            }//..for (int i=0; i<10; ++i)


            if (Input.GetKeyDown(KeyCode.I))
            {
                if (false == GameManager.GetInventory().gameObject.activeSelf)
                {
                    GameManager.GetInventory().gameObject.SetActive(true);
                    GameManager.GetSkillInven().gameObject.SetActive(true);
                    return;
                }

                GameManager.GetInventory().gameObject.SetActive(false);
                GameManager.GetChestInven().gameObject.SetActive(false);
                GameManager.GetMachineInven().gameObject.SetActive(false);
                GameManager.GetStoneFurnaceInven().gameObject.SetActive(false);
                GameManager.GetSkillInven().gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Tab))  //"Tab"
            {
                if (_cheat_text)            _cheat_text.SetActive(!_cheat_text.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.LeftBracket))  //"["
            {
                Managers.Game.bNewGame = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (Input.GetKeyDown(KeyCode.RightBracket)) //"]"
            {
                GameManager.Save();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (null != GameManager.GetTerrainManager().GetChoicePrefab())
                {
                    GameManager.GetTerrainManager().SetChoicePrefab(GameManager.GetTerrainManager().GetChoicePrefab()._itembase);
                    GameManager.GetTerrainManager().GetChoicePrefab().transform.Rotate(Vector3.up * 90f);
                    GameManager.GetTerrainManager().GetChoicePrefab().Clear();
                }

                //OnMouseMove()갱신을 위해서.
                GameManager.GetMouseController().mouse_refresh = true;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                GameManager.GetTechInven().gameObject.SetActive(!GameManager.GetTechInven().gameObject.activeSelf);
                GameManager.GetTechDesc().gameObject.SetActive(GameManager.GetTechInven().gameObject.activeSelf);
            }

        }

        private void ActiveIcon(InvenItemData itemData)
        {
            if (null == itemData)
                return;

            if (true == ItemInvenBase.bPointerEnter)
                itemData.owner.ActiveIcon();
            else
                itemData.owner.DeactiveIcon();
        }

        private void CancelChoicedItem()
        {
            if (null == InvenBase.choiced_item)
                return;


            //이전위치가 비어있는지 확인한다.
            if (null != InvenBase.choiced_item.owner)
            {
                Slot s = InvenBase.choiced_item.owner.GetInvenSlot(InvenBase.choiced_item.panel, InvenBase.choiced_item.slot);
                if (null != s && s.transform.childCount <= 0)
                {
                    s.AddItem(InvenBase.choiced_item);
                    InvenBase.choiced_item = null;
                    return;
                }
            }
            //quick / inven
            int amount = GameManager.AddItem(InvenBase.choiced_item);
            if (amount <= 0)
            {
                //Destroy(InvenBase.choiced_item.gameObject);
                InvenBase.choiced_item = null;
                return;
            }
        }

    }//..class KeyController
}//..namespace MyCraft