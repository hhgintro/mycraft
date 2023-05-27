using FactoryFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyCraft
{
    public class GameScene : BaseScene
    {
        Vector3 offset = new Vector3(50f, -16f, 0); //인벤아이템을 잡았을때 마우스와 이격거리

        void Awake()
        {
            Init();
        }

        void Start()
        {
            //Managers.Input.KeyAction -= OnKeyDown;
            //Managers.Input.KeyAction += OnKeyDown;

            ShowInitUIs();      //초기 UI

            //HG_TEST: 임시로 불러올수 있는지 체크용(성공)
            //GameManager에서 불러올수 없어서 여기에 추가함.
            //Managers.Game._buildingPlacement = FindObjectOfType<BuildingPlacement>();
            //Managers.Game._conveyorPlacement = FindObjectOfType<ConveyorPlacement>();
            Managers.Game.InitPlacement(FindObjectOfType<BuildingPlacement>(), FindObjectOfType<ConveyorPlacement>());

            if(true == Managers.Game.bNewGame)
            {
                AddInvenItems();    //인벤 아이템
                AddQuickItems();    //퀵인벤 아이템
            }
            else
            {
                //FindObjectOfType<SerializeManager>().Load();
                Managers.Game.Load();
			}
			Application.runInBackground = true;
        }


        private void Update()
        {
            if (InvenBase.choiced_item)
                InvenBase.choiced_item.transform.position = Input.mousePosition + offset;

            OnKeyDown();
        }

        protected override void Init()
        {
            base.Init();
            SceneType = Define.Scene.Game;
        }

        public override void Clear()
        {
            Debug.Log("Logo Scene Clear!");
        }

        private void ShowInitUIs()
        {
            Managers.Game.Inventories.gameObject.SetActive(true);
            Managers.Game.QuickInvens.gameObject.SetActive(true);
            //    GameManager.GetTechInven().gameObject.SetActive(true);
            //    GameManager.GetTechDesc().gameObject.SetActive(true);

            Managers.Game.Tooltips.gameObject.SetActive(false);
            Managers.Game.ChestInvens.gameObject.SetActive(false);
			Managers.Game.FactoryInvens.gameObject.SetActive(false);
            Managers.Game.ForgeInvens.gameObject.SetActive(false);
			Managers.Game.SkillInvens.gameObject.SetActive(false);

			//    GameManager.GetDeleteProgress().gameObject.SetActive(false);

			//    //GameManager.GetSystemMenu().gameObject.SetActive(false);
			//    Managers.SystemMenu.SetActive(false);
		}

		private void AddInvenItems()
        {
            //inven slot
            if (0 < Managers.Game.Inventories._panels.Count)
                Managers.Game.Inventories._panels[0].SetSlots(21);

            //Managers.Game.Inventories.Resize();


            Managers.Game.Inventories.AddItem(2010, 54);    //iron-plate
            Managers.Game.Inventories.AddItem(2010, 54);    //iron-plate
            Managers.Game.Inventories.AddItem(2011, 54);    //iron-gear
            Managers.Game.Inventories.AddItem(2020, 54);    //copper-plate
            Managers.Game.Inventories.AddItem(2021, 54);    //copper-cable
            Managers.Game.Inventories.AddItem(2000, 54);    //wood

            Managers.Game.Inventories.AddItem(10, 54);     //raw-wood
            Managers.Game.Inventories.AddItem(20, 54);     //stone
            Managers.Game.Inventories.AddItem(40, 54);     //iron-ore
            Managers.Game.Inventories.AddItem(50, 54);     //copper-ore

			Managers.Game.Inventories.AddItem(1100, 100);   //belt
			Managers.Game.Inventories.AddItem(1100, 100);   //belt
			Managers.Game.Inventories.AddItem(1100, 100);   //belt
			Managers.Game.Inventories.AddItem(1100, 100);   //belt

		}

		private void AddQuickItems()
        {
            //quick slot
            if (0 < Managers.Game.QuickInvens._panels.Count)
                Managers.Game.QuickInvens._panels[0].SetSlots(10);

            Managers.Game.QuickInvens.AddItem(1100, 100);   //belt
            Managers.Game.QuickInvens.AddItem(1000, 54);    //chest
            Managers.Game.QuickInvens.AddItem(1040, 54);    //drill
            Managers.Game.QuickInvens.AddItem(1050, 54);    //forge
            Managers.Game.QuickInvens.AddItem(1060, 54);    //machine
            Managers.Game.QuickInvens.AddItem(1200, 54);    //splitter
            Managers.Game.QuickInvens.AddItem(1210, 54);    //merger


        }

        void OnKeyDown()
        {
            //ESC
            if (Input.GetKeyDown(KeyCode.Escape)) { OnKeyDownEsc(); return; }
            //0~9
            if (true == OnKeyDownNum()) return;

            OnKeyDownElse();
        }
        void OnKeyDownEsc()
        {
            //최상위 UI순
            //  GetSystemMenu
            //  GetTechInven / GetTechDesc
            //  GetInventory / GetSkillInven / GetChestInven / GetMachineInven / GetStoneFurnaceInven

            //if (Managers.SystemMenu.gameObject.activeSelf)
            //{
            //    Managers.SystemMenu.gameObject.SetActive(false);
            //    return;
            //}

            //if (Managers.Chat.gameObject.activeSelf)
            //{
            //    Managers.Chat.gameObject.SetActive(false);
            //    return;
            //}

            ////tech
            //if (true == Managers.Game.TechInven.gameObject.activeSelf)
            //{
            //    Managers.Game.TechInven.gameObject.SetActive(false);
            //    Managers.Game.TechDesc.gameObject.SetActive(false);
            //    return;
            //}

            //inven
            Managers.Game.Inventories.gameObject.SetActive(false);
            Managers.Game.ChestInvens.gameObject.SetActive(false);
			Managers.Game.FactoryInvens.gameObject.SetActive(false);
            Managers.Game.ForgeInvens.gameObject.SetActive(false);
			Managers.Game.SkillInvens.gameObject.SetActive(false);
		}

		bool OnKeyDownNum()
        {
            for (int i = 0; i < 10; ++i)
            {
                if (Input.GetKeyDown(i + KeyCode.Alpha1))
                {
                    //InvenItemData itemData = GameManager.GetQuickInven().slots[i].GetItemData();
                    Slot s = Managers.Game.QuickInvens.GetInvenSlot(0, i);
                    InvenItemData itemData = (InvenItemData)s.GetItemData();
                    if (null != itemData)
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
                        //this.ActiveIcon(InvenBase.choiced_item); //HG_TEST: 이 코드가 필요할지 체크할 것
                        ////private void ActiveIcon(InvenItemData itemData)
                        ////{
                        ////    if (null == itemData)
                        ////        return;

                        ////    if (true == ItemInvenBase.bPointerEnter)
                        ////        itemData.owner.ActiveIcon();
                        ////    else
                        ////        itemData.owner.DeactiveIcon();
                        ////}

                    }
                    return true;
                }
            }
            return false;
        }

        void OnKeyDownElse()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (false == Managers.Game.Inventories.gameObject.activeSelf)
                {
                    Managers.Game.Inventories.gameObject.SetActive(true);
			        Managers.Game.SkillInvens.gameObject.SetActive(true);
					return;
                }

                Managers.Game.Inventories.gameObject.SetActive(false);
                Managers.Game.ChestInvens.gameObject.SetActive(false);
			    Managers.Game.FactoryInvens.gameObject.SetActive(false);
                Managers.Game.ForgeInvens.gameObject.SetActive(false);
				Managers.Game.SkillInvens.gameObject.SetActive(false);
				return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))  //"Tab"
            {
                //if (_cheat_text) _cheat_text.SetActive(!_cheat_text.activeSelf);
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftBracket))  //"["
            {
                Managers.Game.bNewGame = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }
            if (Input.GetKeyDown(KeyCode.RightBracket)) //"]"
            {
                //GameManager.Save();
                //FindObjectOfType<SerializeManager>().Save();
                Managers.Game.Save();
				return;
            }
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    if (null != GameManager.GetTerrainManager().GetChoicePrefab())
            //    {
            //        //GameManager.GetTerrainManager().SetChoicePrefab(GameManager.GetTerrainManager().GetChoicePrefab()._itembase);
            //        GameManager.GetTerrainManager().GetChoicePrefab().transform.Rotate(Vector3.up * 90f);
            //        GameManager.GetTerrainManager().GetChoicePrefab().Clear();
            //    }

            //    //OnMouseMove()갱신을 위해서.
            //    GameManager.GetMouseController().mouse_refresh = true;
            //    return;
            //}
            if (Input.GetKeyDown(KeyCode.T))
            {
                //Managers.Game.TechInvens.gameObject.SetActive(!Managers.Game.TechInvens.gameObject.activeSelf);
                //Managers.Game.TechDescs.gameObject.SetActive(Managers.Game.TechInvens.gameObject.activeSelf);
                return;
            }
        }
	}
}