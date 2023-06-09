﻿using FactoryFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace MyCraft
{
	public class GameManager
	{
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileInt(string section, string key, int def, string filePath);


		//private GameObject _systemmenu;
		//ChatManager _chat_manager;

		private JSonParser<ItemBase> _itembase;
		private JSonParser<TechBase> _techbase;
		private JSonParser<Category> _categories;

		private Inventory _inventory;
		private QuickInven _quickinven;
		private ChestInven _chestinven;
		private ForgeInven _forgeinven;
		private FactoryInven _factoryinven;
		private SkillInven _skillinven;
		private TechInven _techinven;
		private TechDescription _techdesc;

		private Tooltip _tooltip;
		private Coordinates _coodinates;  //좌표
		private DestoryProcess _destoryProcess;


		//public GameObject SystemMenu { get { if (null == _systemmenu) _systemmenu = GameObject.FindObjectOfType(typeof(SystemMenu)); return _systemmenu; } }
		//public ChatManager Chat { get { if (null == Instance._chat_manager) Instance._chat_manager = GameObject.Find("Gameplay UI/Chatting").GetComponent<ChatManager>(); return Instance._chat_manager; } }
		public JSonParser<ItemBase> ItemBases { get { if (null == _itembase) _itembase = new JSonParser<ItemBase>(Path.Combine(Application.streamingAssetsPath, "locale", Managers.Locale._locale.ToString(), "items.json")); return _itembase; } }
		public JSonParser<TechBase> TechBases { get { if (null == _techbase) _techbase = new JSonParser<TechBase>(Path.Combine(Application.streamingAssetsPath, "locale", Managers.Locale._locale.ToString(), "technology.json")); return _techbase; } }
		public JSonParser<Category> Categories { get { if (null == _categories) _categories = new JSonParser<Category>(Path.Combine(Application.streamingAssetsPath, "locale", Managers.Locale._locale.ToString(), "categories.json")); return _categories; } }

		public Inventory Inventories { get { if (null == _inventory) _inventory = GameObject.FindObjectOfType(typeof(Inventory)) as Inventory; return _inventory; } }
		public QuickInven QuickInvens { get { if (null == _quickinven) _quickinven = GameObject.FindObjectOfType(typeof(QuickInven)) as QuickInven; return _quickinven; } }
		public ChestInven ChestInvens { get { if (null == _chestinven) _chestinven = GameObject.FindObjectOfType(typeof(ChestInven)) as ChestInven; return _chestinven; } }
		public ForgeInven ForgeInvens { get { if (null == _forgeinven) _forgeinven = GameObject.FindObjectOfType(typeof(ForgeInven)) as ForgeInven; return _forgeinven; } }
		public FactoryInven FactoryInvens { get { if (null == _factoryinven) _factoryinven = GameObject.FindObjectOfType(typeof(FactoryInven)) as FactoryInven; return _factoryinven; } }
		public SkillInven SkillInvens { get { if (null == _skillinven) _skillinven = GameObject.FindObjectOfType(typeof(SkillInven)) as SkillInven; return _skillinven; } }
		public TechInven TechInvens { get { if (null == _techinven) _techinven = GameObject.FindObjectOfType(typeof(TechInven)) as TechInven; return _techinven; } }
		public TechDescription TechDescs { get { if (null == _techdesc) _techdesc = GameObject.FindObjectOfType(typeof(TechDescription)) as TechDescription; return _techdesc; } }

		public Tooltip Tooltips { get { if (null == _tooltip) _tooltip = GameObject.FindObjectOfType(typeof(Tooltip)) as Tooltip; return _tooltip; } }
		public Coordinates Coordinates { get { if (null == _coodinates) _coodinates = GameObject.FindObjectOfType(typeof(Coordinates)) as Coordinates; return _coodinates; } }
		public DestoryProcess DestoryProcess { get { if (null == _destoryProcess) _destoryProcess = GameObject.FindObjectOfType(typeof(DestoryProcess)) as DestoryProcess; return _destoryProcess; } }



		//저장경로
		public string _save_dir = Application.dataPath + "/../save";


		public bool bNewGame;
		//public StringBuilder _locale;

		//FactoryFramework
		private BuildingPlacement _buildingPlacement;
		private ConveyorPlacement _conveyorPlacement;
		private GameObject _choiced_building = null;

		public void Init()
		{
			////HG_TEST: 임시로 불러올수 있는지 체크용(성공)
			//_buildingPlacement = FindObjectOfType<BuildingPlacement>();
			//_conveyorPlacement = FindObjectOfType<ConveyorPlacement>();
		}
		public void Clear()
		{

		}

		public void InitPlacement(BuildingPlacement building, ConveyorPlacement conveyor)
		{
			_buildingPlacement = building;
			_conveyorPlacement = conveyor;

			//building
			_buildingPlacement.finishPlacementEvent.OnEvent -= FinishPlaceBuilding;
			_buildingPlacement.finishPlacementEvent.OnEvent += FinishPlaceBuilding;
			//conveyor
			_conveyorPlacement.finishPlacementEvent.OnEvent -= FinishPlaceBuilding;
			_conveyorPlacement.finishPlacementEvent.OnEvent += FinishPlaceBuilding;
		}

		public void PlaceBuilding(InvenItemData item)
		{
			if (null == item || null == item.database) return;
			GameObject prefab = item.database.prefab;
			if (null == prefab) return;

			if (prefab.TryGetComponent(out FactoryFramework.Conveyor _))
			{
				_choiced_building = _conveyorPlacement.StartPlacingConveyor(prefab);
				_choiced_building.GetComponent<FactoryFramework.Conveyor>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.Driller _))
			{
				_choiced_building = _buildingPlacement.StartPlacingBuilding(prefab, true);
				_choiced_building.GetComponent<FactoryFramework.Driller>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.LogisticComponent _))
			{
				_choiced_building = _buildingPlacement.StartPlacingBuilding(prefab, false);
				_choiced_building.GetComponent<FactoryFramework.LogisticComponent>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.PowerGridComponent _))
				_choiced_building = _buildingPlacement.StartPlacingBuilding(prefab, false);
		}
		public void DestoryBuilding()
		{
			if (null == _choiced_building) return;

			if (_choiced_building.TryGetComponent(out FactoryFramework.Conveyor _))
				_conveyorPlacement.ForceCancel();
			else if (_choiced_building.TryGetComponent(out FactoryFramework.Driller _))
				_buildingPlacement.ForceCancel();
			else if (_choiced_building.TryGetComponent(out FactoryFramework.PowerGridComponent _) || _choiced_building.TryGetComponent(out FactoryFramework.LogisticComponent _))
				_buildingPlacement.ForceCancel();
			_choiced_building = null;
		}

		private void FinishPlaceBuilding()
		{
			Debug.Log($"{_choiced_building.name} 완공");
			_choiced_building = null;   //재할당을 받기위해 null로 설정한다.

			//각 이벤트 위치어서 개수를 미리 정산해 줍니다.
			//InvenBase.choiced_item.AddStackCount(-1, false);
			if (InvenBase.choiced_item.amount <= 0)
			{
				Managers.Resource.Destroy(InvenBase.choiced_item.gameObject);
				InvenBase.choiced_item = null;
				return;
			}
			//개수가 남아있다면...건물을 생성해서 들고있는다.
			if (null != InvenBase.choiced_item) this.PlaceBuilding(InvenBase.choiced_item);
		}

		//private void FinishPlaceConveyor()
		//{
		//    //들고있는것에서 val 만큼 뺀다.
		//    //GameManager.SubItem(InvenBase.choiced_item.database.id, val);

		//    FinishPlaceBuilding();
		//}

		//소유한 아이템 개수
		public int GetAmount(int itemid)
		{
			int amount = 0;
			if (itemid == InvenBase.choiced_item.database.id)
				amount += InvenBase.choiced_item.amount;
			amount += Managers.Game.Inventories.GetAmount(itemid);
			amount += Managers.Game.QuickInvens.GetAmount(itemid);
			return amount;
		}

		public int AddItem(int itemid, int amount)
		{
			if (0 == amount) return 0;

			////choice item
			//if (itemid == InvenBase.choiced_item.database.id)
			//    InvenBase.choiced_item.AddStackCount()

			//quick inven
			//HG_TEST: 테스트로 bCreate를 true로 설정합니다.(default:false)
			//  인벤아이템을 모두 가져왔을때 자리를 완전히 비우지말고 amount만 0 으로 설정 이미지는 "흑백"으로 처리함으로
			//  이후 회수될때 원자리를 찾아 가면 좋을듯 싶다.
			amount = Managers.Game.QuickInvens.AddItem(itemid, amount, true);
			if (amount <= 0) return 0;

			//inventory
			amount = Managers.Game.Inventories.AddItem(itemid, amount);
			if (amount <= 0) return 0;

			////그래도 남아있으면...생성해준다.
			//amount = GetQuickInven().AddItem(itemid, amount);
			//if (amount <= 0)
			//    return 0;

			return amount;
		}

		public int AddItem(InvenItemData itemData)
		{
			//quick inven
			int amount = Managers.Game.QuickInvens.AddItem(itemData);
			if (amount <= 0)
				return 0;
			itemData.amount = amount;

			//inventory
			amount = Managers.Game.Inventories.AddItem(itemData);
			if (amount <= 0)
				return 0;
			itemData.amount = amount;

			return amount;
		}

		public int SubItem(int itemid, int amount)
		{
			//quick inven
			amount = Managers.Game.QuickInvens.SubItem(itemid, amount);
			if (amount <= 0)
				return 0;

			//inventory
			amount = Managers.Game.Inventories.SubItem(itemid, amount);
			if (amount <= 0)
				return 0;

			////그래도 남아있으면...생성해준다.
			//amount = GetQuickInven().AddItem(itemid, amount);
			//if (amount <= 0)
			//    return 0;

			return amount;
		}

		#region SAVE
		public void Save()
		{
			if (false == Directory.Exists(_save_dir)) Directory.CreateDirectory(_save_dir);

			//BinarySerialize(GetInventory(), Application.persistentDataPath + "/savefile.sav");
			using (FileStream fs = File.Create(_save_dir + "/savefile.sav"))
			{
				BinaryWriter writer = new BinaryWriter(fs);
				_inventory.Save(writer);
				_quickinven.Save(writer);


				//// build a lookup of Guid -> SerializationReference
				//// this is used to re-link the conveyor belts to buildings
				//Dictionary<Guid, SerializationReference> lookup = new Dictionary<Guid, SerializationReference>();
				//// keep conveyors for a seoncd pass once all buildings are setup
				//List<SerializationReference> conveyors = new List<SerializationReference>();

				List<Conveyor> conveyors = new List<Conveyor>();

				var serializables = _buildingPlacement.gameObject.GetComponentsInChildren<SerializationReference>();
				writer.Write(serializables.Length);     //building count
				foreach (var obj in serializables)
				{
					////HG_CHECK:InstantiateBuildingData()를 추가해야 하나???(테스트 할것)
					//lookup.Add(obj.GUID, obj);

					//conveyor는 별도로 빼서 아래에서 처리한다. connect처리때문에.
					if (obj.TryGetComponent<Conveyor>(out Conveyor conveyor))
					{
						conveyors.Add(conveyor);
						continue;
					}

					if (obj.TryGetComponent(out LogisticComponent logistic))
					{
						logistic.Save(writer);
						continue;
					}
					Debug.LogError($"{obj.name}을 Save할 로직이 준비되어있지 않습니다.");
				}

				//conveyor는 별도로 빼서 아래에서 처리한다. connect처리때문에.
				foreach (var obj in conveyors)
				{
					Conveyor conveyor = obj.GetComponent<Conveyor>();
					conveyor.Save(writer);
				}


				fs.Close();
			}
		}

		public void Load()
		{
			//BinaryDeserialize<Inventory>(Application.persistentDataPath + "/savefile.sav");
			string filepath = _save_dir + "/savefile.sav";
			if (false == File.Exists(filepath))
			{
				Debug.LogError($"load failed({filepath})");
				return;
			}

			using (FileStream fs = File.Open(filepath, FileMode.Open))
			{
				BinaryReader reader = new BinaryReader(fs);
				_inventory.Load(reader);
				_quickinven.Load(reader);


				// build a lookup of Guid -> SerializationReference
				// this is used to re-link the conveyor belts to buildings
				Dictionary<Guid, LogisticComponent> lookup = new Dictionary<Guid, LogisticComponent>();
				// keep conveyors for a seoncd pass once all buildings are setup
				List<Conveyor> conveyors = new List<Conveyor>();

				//GameManager.GetTerrainManager().Load(reader);
				int cntBuilding = reader.ReadInt32();   //building count
				for (int i = 0; i < cntBuilding; ++i)
				{
					string path = reader.ReadString();
					GameObject prefab = Managers.Resource.Load<GameObject>(path);
					if (null == prefab)
					{
						Debug.LogError($"Fail: prefab({path})");
						continue;
					}
					GameObject instantiated = Managers.Resource.Instantiate(prefab);    //HG[2023.06.01]테스트필요
					instantiated.transform.parent = _buildingPlacement.transform.Find($"Pool_{prefab.name}") ?? (new GameObject($"Pool_{prefab.name}") { transform = { parent = _buildingPlacement.transform } }).transform;

					////conveyor는 별도로 빼서 아래에서 처리한다. connect처리때문에.
					//if (instantiated.TryGetComponent<Conveyor>(out Conveyor conveyor))
					//{
					//	conveyors.Add(conveyor);
					//	continue;
					//}

					//conveyor는 save할때 맨 뒤쪽에 추가됨니다.
					if (instantiated.TryGetComponent<Conveyor>(out Conveyor conveyor))
					{
						conveyor.Load(reader);
						conveyors.Add(conveyor);
						//conveyor랑 연결될 buildings GUID
						lookup.Add(conveyor.GUID, conveyor);
						continue;
					}


					if (instantiated.TryGetComponent(out LogisticComponent building))
					{
						building._itembase = prefab.GetComponent<LogisticComponent>()._itembase;
						building.Load(reader);

						//conveyor랑 연결될 buildings GUID
						lookup.Add(building.GUID, building);
						continue;
					}
					Debug.LogError($"{instantiated.name}을 Load할 로직이 준비되어있지 않습니다.");
				}
				fs.Close();

				foreach( var conveyor in conveyors)
				{
					string inputSocketGUID = conveyor.tmpInputGuid;// conveyor.InputSocketGuid;
					string outputSocketGUID = conveyor.tmpOutputGuid;// conveyor.OutputSocketGuid;

					///
					// This part may look confusing. Input sockets only connect to output sockets and vice versa.
					// if inputSocketGUID exists, it is pointing to a building or other conveyor's output socket
					///
					if (inputSocketGUID != null && !inputSocketGUID.Equals(""))
					{
						var inputConnection = lookup[new Guid(inputSocketGUID)];
						if (inputConnection.TryGetComponent(out Building building))
						{
							OutputSocket osocket = building.GetOutputSocketByIndex(conveyor.data.inputSocketIndex);
							osocket?.Connect(conveyor.inputSocket);
						}
						else if (inputConnection.TryGetComponent(out Conveyor conv))
						{
							conv.outputSocket.Connect(conveyor.inputSocket);
						}
					}
					if (outputSocketGUID != null && !outputSocketGUID.Equals(""))
					{
						var outputConnection = lookup[new Guid(outputSocketGUID)];
						if (outputConnection.TryGetComponent(out Building building))
						{
							InputSocket isocket = building.GetInputSocketByIndex(conveyor.data.outputSocketIndex);
							isocket?.Connect(conveyor.outputSocket);
						}
						else if (outputConnection.TryGetComponent(out Conveyor conv))
						{
							conveyor.outputSocket.Connect(conv.inputSocket);
						}
					}
				}
			}
		}

		#endregion //..SAVE

	}
}