using FactoryFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyCraft
{
	public class GameScene : BaseScene
	{
		Vector3 offset = new Vector3(50f, -16f, 0); //인벤아이템을 잡았을때 마우스와 이격거리
		public GameObject _cheat_text;

		void Awake()
		{
			Init();
		}

		void Start()
		{
			Managers.Input.KeyAction -= OnKeyDown_GameScene;
			Managers.Input.KeyAction += OnKeyDown_GameScene;
			//Managers.Input.KeyActionList();

			ShowInitUIs();      //초기 UI

			//HG_TEST: 임시로 불러올수 있는지 체크용(성공)
			//GameManager에서 불러올수 없어서 여기에 추가함.
			//Managers.Game._buildingPlacement = FindObjectOfType<BuildingPlacement>();
			//Managers.Game._conveyorPlacement = FindObjectOfType<ConveyorPlacement>();
			Managers.Game.InitPlacement(FindObjectOfType<BuildingPlacement>(), FindObjectOfType<ConveyorPlacement>());

			if(string.IsNullOrEmpty(Managers.Game._load_filename))
			{
				//초기 지급아이템
				AddInvenItems();    //인벤 아이템(초기 지급아이템)
				AddQuickItems();    //퀵인벤 아이템(초기 지급아이템)
			}
			else
			{
				//FindObjectOfType<SerializeManager>().Load();
				Managers.Game.Load();
				//Invoke("LoadGame", 2f);

				//Load이후에 추가로 지급되는 아이템들
				//float fillAmount = 0.4f;// MyCraft.Global.FILLAMOUNT_DEFAULT;
				//Managers.Game.Inventories.AddItem(801, 10, ref fillAmount);		//potion 1
			}
			Application.runInBackground = true;
		}

		void LoadGame()
		{
			Managers.Game.Load();
		}

		private void Update()
		{
			if (InvenBase.choiced_item)
				InvenBase.choiced_item.transform.position = Input.mousePosition + offset;

			//OnKeyDown();
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
			Managers.Game.Inventories.gameObject.SetActive(false);
			Managers.Game.QuickInvens.gameObject.SetActive(true);
			//GameManager.GetTechInven().gameObject.SetActive(true);
			//GameManager.GetTechDesc().gameObject.SetActive(true);

			Managers.Game.Tooltips.gameObject.SetActive(false);
			Managers.Game.ChestInvens.gameObject.SetActive(false);
			Managers.Game.FactoryInvens.gameObject.SetActive(false);
			Managers.Game.ForgeInvens.gameObject.SetActive(false);
			Managers.Game.SkillInvens.gameObject.SetActive(false);
			Managers.Game.LabInvens.gameObject.SetActive(false);
			Managers.Game.TechInvens.gameObject.SetActive(false);
			Managers.Game.TechDescs.gameObject.SetActive(false);

			//GameManager.GetDeleteProgress().gameObject.SetActive(false);

			SystemMenuManager go = GameObject.FindObjectOfType(typeof(SystemMenuManager)) as SystemMenuManager;

			Managers.Game.SystemMenu.gameObject.SetActive(false);
			_cheat_text.SetActive(false);
		}

		//초기 지급아이템(인벤)
		private void AddInvenItems()
		{
			//inven slot
			if (0 < Managers.Game.Inventories._panels.Count)
				Managers.Game.Inventories._panels[0].SetSlots(41);

			//Managers.Game.Inventories.Resize();

			float fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;
			Managers.Game.Inventories.AddItem(2010, 54, ref fillAmount);    //iron-plate
			Managers.Game.Inventories.AddItem(2010, 54, ref fillAmount);    //iron-plate
			Managers.Game.Inventories.AddItem(2011, 54, ref fillAmount);    //iron-gear
			Managers.Game.Inventories.AddItem(2020, 54, ref fillAmount);    //copper-plate
			Managers.Game.Inventories.AddItem(2021, 54, ref fillAmount);    //copper-cable

			Managers.Game.Inventories.AddItem(10, 54, ref fillAmount);     //raw-wood
			Managers.Game.Inventories.AddItem(20, 54, ref fillAmount);     //stone
			Managers.Game.Inventories.AddItem(40, 54, ref fillAmount);     //iron-ore
			Managers.Game.Inventories.AddItem(50, 54, ref fillAmount);     //copper-ore

			Managers.Game.Inventories.AddItem(1100, 100, ref fillAmount);   //belt
			Managers.Game.Inventories.AddItem(1100, 100, ref fillAmount);   //belt
			Managers.Game.Inventories.AddItem(1100, 100, ref fillAmount);   //belt
			Managers.Game.Inventories.AddItem(1100, 100, ref fillAmount);   //belt
			Managers.Game.Inventories.AddItem(1070, 54, ref fillAmount);    //WaterPurificationPlant
			Managers.Game.Inventories.AddItem(1080, 54, ref fillAmount);    //PumpJack
			Managers.Game.Inventories.AddItem(1065, 10, ref fillAmount);    //Oil-Refinery
			Managers.Game.Inventories.AddItem(1066, 10, ref fillAmount);    //Lab

			fillAmount = 0.4f;
			Managers.Game.Inventories.AddItem(801, 100, ref fillAmount);    //potion 1
			fillAmount = 0.4f;
			Managers.Game.Inventories.AddItem(801, 100, ref fillAmount);    //potion 1

			Debug.Log($"fillAmount:{fillAmount}");
		}

		//초기 지급아이템(퀵슬롯)
		private void AddQuickItems()
		{
			//quick slot
			if (0 < Managers.Game.QuickInvens._panels.Count)
				Managers.Game.QuickInvens._panels[0].SetSlots(10);

			float fillAmount = MyCraft.Global.FILLAMOUNT_DEFAULT;
			Managers.Game.QuickInvens.AddItem(1100, 100, ref fillAmount);   //belt
			//Managers.Game.QuickInvens.AddItem(1070, 100, ref fillAmount);   //생수공장
			Managers.Game.QuickInvens.AddItem(1400, 100, ref fillAmount);   //safe-footing(안전발판)
			Managers.Game.QuickInvens.AddItem(1410, 100, ref fillAmount);   //ramp-way(경사로)
			Managers.Game.QuickInvens.AddItem(1420, 100, ref fillAmount);   //ramp-way(경사로)
			Managers.Game.QuickInvens.AddItem(1000, 54, ref fillAmount);    //chest
			Managers.Game.QuickInvens.AddItem(1040, 54, ref fillAmount);    //drill
			Managers.Game.QuickInvens.AddItem(1050, 54, ref fillAmount);    //forge
			Managers.Game.QuickInvens.AddItem(1060, 54, ref fillAmount);    //machine
			Managers.Game.QuickInvens.AddItem(1200, 54, ref fillAmount);    //splitter
			Managers.Game.QuickInvens.AddItem(1210, 54, ref fillAmount);    //merger

		}

		void OnKeyDown_GameScene()
		{
			//ESC
			if (Input.GetKeyDown(KeyCode.Escape)) { OnKeyDownEsc(); return; }
			//system mune창이 열려있으면...아래는 무시
			if (Managers.Game.SystemMenu.gameObject.activeSelf) return;

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

			//CLOSE system menu
			if (Managers.Game.SystemMenu.gameObject.activeSelf)
			{
				Managers.Game.SystemMenu.gameObject.SetActive(false);
				return;
			}

			//들고있는 아이템은 Quick / Inventory에 넣어준다.
			if( null != InvenBase.choiced_item)
			{
				Managers.Game.AddItem(InvenBase.choiced_item);
				InvenBase.choiced_item = null;
				Managers.Game.DestoryBuilding();    //들고있는 건물을 내려놓는다.
				return;
			}

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
			bool deactive = false;
			if (Managers.Game.Inventories.gameObject.activeSelf)	{ Managers.Game.Inventories.gameObject.SetActive(false); deactive=true; }
			if (Managers.Game.ChestInvens.gameObject.activeSelf)    { Managers.Game.ChestInvens.gameObject.SetActive(false); deactive=true; }
			if (Managers.Game.FactoryInvens.gameObject.activeSelf)  { Managers.Game.FactoryInvens.gameObject.SetActive(false); deactive=true; }
			if (Managers.Game.ForgeInvens.gameObject.activeSelf)    { Managers.Game.ForgeInvens.gameObject.SetActive(false); deactive=true; }
			if (Managers.Game.LabInvens.gameObject.activeSelf)		{ Managers.Game.LabInvens.gameObject.SetActive(false); deactive = true; }
			if (Managers.Game.SkillInvens.gameObject.activeSelf)    { Managers.Game.SkillInvens.gameObject.SetActive(false); deactive = true; }
			if (Managers.Game.TechInvens.gameObject.activeSelf)		{ Managers.Game.TechInvens.gameObject.SetActive(false); deactive = true; }
			if (Managers.Game.TechDescs.gameObject.activeSelf)		{ Managers.Game.TechDescs.gameObject.SetActive(false); deactive = true; }

			//OPEN system menu
			if (false == deactive) Managers.Game.SystemMenu.gameObject.SetActive(true);
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
					if (null == itemData)
					{
						//빈공간에 들고 있던 아이템을 내려 놓는다.
						s.PutdownChoicedItem();
					}
					else
					{
						//pickAll의 경우에 교체시에는 pickup이 마지막으로 처리되어 0이 되는 결과가 발생
						bool noti2block = true;

						//들고 있다면...
						if (null != InvenBase.choiced_item)
						{
							//같은 아이템을 들고 있다면, 겹치기
							if (itemData.database.id == InvenBase.choiced_item.database.id)
							{
								float fillAmount = itemData.GetFillAmount();
								int amount = itemData._SetStackCount(InvenBase.choiced_item._AddStackCount(itemData.amount, ref fillAmount, true), MyCraft.Global.FILLAMOUNT_DEFAULT, true);
								if (amount <= 0) Managers.Resource.Destroy(itemData.gameObject);//남은게 없다면...삭제
								break;
							}

							//들고 있던건 내려놓고(다른아이템인 경우)
							////InvenItemData.owner.slots[InvenItemData.slot].AddItem(InvenBase.choiced_item);
							//Slot s1 = itemData.owner.GetInvenSlot(itemData.panel, itemData.slot);
							//if (null != s1) s1.AddItem(InvenBase.choiced_item);
							s.PutdownChoicedItem();
							noti2block = false;//교체시에는 Pickup을 block으로 전달하지 않는다.
						}

						//모두 줍는다.
						InvenBase.choiced_item = itemData.PickupAll(itemData.transform.parent.parent.parent.parent.parent, noti2block);
						Managers.Game.PlaceBuilding(InvenBase.choiced_item);
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

				Managers.Game.Tooltips.gameObject.SetActive(false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.T))
			{
				Managers.Game.TechInvens.gameObject.SetActive(!Managers.Game.TechInvens.gameObject.activeSelf);
				Managers.Game.TechDescs.gameObject.SetActive(Managers.Game.TechInvens.gameObject.activeSelf);
				return;
			}

			if (Input.GetKeyDown(KeyCode.Tab))  //"Tab"
			{
				if (this._cheat_text) this._cheat_text.SetActive(!this._cheat_text.activeSelf);
				return;
			}
	//        if (Input.GetKeyDown(KeyCode.LeftBracket))  //"["
	//        {
	//            //Managers.Game.bNewGame = false;
	//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	//            return;
	//        }
	//        if (Input.GetKeyDown(KeyCode.RightBracket)) //"]"
	//        {
	//            //GameManager.Save();
	//            //FindObjectOfType<SerializeManager>().Save();
	//            Managers.Game.Save("savefile");
				//return;
	//        }
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
		}
	}
}