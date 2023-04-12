using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace MyCraft
{
    public class KeyController : MonoBehaviour
    {
        //private TerrainManager terrain_manager;
        //private BeltManager belt_manager;
        //private InserterManager inserter_manager;
        //private ChestManager chest_manager;
        //private DrillManager drill_manager;

        //private MouseController mouse_controller;

        // Use this for initialization
        void Start()
        {
            //this.terrain_manager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
            //this.belt_manager = terrain_manager.GetComponentInChildren<BeltManager>();
            //this.inserter_manager = terrain_manager.GetComponentInChildren<InserterManager>();
            //this.chest_manager = terrain_manager.GetComponentInChildren<ChestManager>();
            //this.drill_manager = terrain_manager.GetComponentInChildren<DrillManager>();

            //this.mouse_controller = GameObject.Find("Input/Mouse").GetComponent<MouseController>();

            StartCoroutine(CheckKey());

        }

        // Update is called once per frame
        //void Update () {

        //    CheckKey_Func();

        //}

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
                //HG[2017.07.19]인벤창에 막혀 들고있던 아이템을 terrain에 설치하기 어려워 주석처리함.
                //(컨셉:직접 인벤에 넣어라)
                //if(null != InvenBase.choiced_item)
                //{
                //    GameManager.GetTerrainManager().SetChoicePrefab(null);
                //    CancelChoicedItem();
                //}
                //else
                if(true == GameManager.GetInventory().GetActive())
                {
                    GameManager.GetInventory().SetActive(false);
                    GameManager.GetChestInven().SetActive(false);
                    GameManager.GetMachineInven().SetActive(false);
                    GameManager.GetStoneFurnaceInven().SetActive(false);
                    GameManager.GetSkillInven().SetActive(false);
                    GameManager.GetTechInven().SetActive(false);
                    GameManager.GetTechDesc().SetActive(false);
                }
                else if(true == GameManager.GetTechInven().GetActive())
                {
                    GameManager.GetTechInven().SetActive(false);
                    GameManager.GetTechDesc().SetActive(false);
                }
                else
                {

                    GameManager.GetSystemMenu().SetActive(!GameManager.GetSystemMenu().GetActive());
                }
            }

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
                //인벤이 활성화 되어있으면 열수 없다.
                if (false == GameManager.GetInventory().GetActive())
                {
                    //GameObject obj = GameManager.GetInventory().gameObject;
                    //Debug.Log("active : " + obj.active);
                    //Debug.Log("active : " + obj.activeSelf);
                    //Debug.Log("active : " + obj.activeInHierarchy);
                    //GameManager.GetInventory().gameObject.SetActive(!GameManager.GetInventory().gameObject.activeSelf);

                    GameManager.GetInventory().SetActive(true);
                    GameManager.GetSkillInven().SetActive(true);
                }
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
                }


                //if (null != this.startblock)
                //    Debug.Log("startblock forward: " + this.startblock.transform.forward);


                //OnMouseMove()갱신을 위해서.
                GameManager.GetMouseController().mouse_refresh = true;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                //GameObject obj = GameManager.GetInventory().gameObject;
                //Debug.Log("active : " + obj.active);
                //Debug.Log("active : " + obj.activeSelf);
                //Debug.Log("active : " + obj.activeInHierarchy);
                //GameManager.GetInventory().gameObject.SetActive(!GameManager.GetInventory().gameObject.activeSelf);

                //GameManager.GetTechInven().SetActive(!GameManager.GetTechInven().GetActive());
                //GameManager.GetTechDesc().SetActive(!GameManager.GetTechDesc().GetActive());
                if(false == GameManager.GetTechInven().GetActive())
                {
                    GameManager.GetTechInven().SetActive(true);
                    GameManager.GetTechDesc().SetActive(true);
                }
                ////inven 이외에는 모두 닫는다.
                //GameManager.GetChestInven().SetActive(false);
                //GameManager.GetStoneFurnaceInven().SetActive(false);
                //GameManager.GetMachineInven().SetActive(false);
            }



            if (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                if(Input.GetKeyDown(KeyCode.S))
                {
                    Debug.Log("save mode");
                }
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