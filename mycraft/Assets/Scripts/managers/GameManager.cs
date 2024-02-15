using FactoryFramework;
using StarterAssets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

namespace MyCraft
{
	public class GameManager
	{
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileInt(string section, string key, int def, string filePath);


		//system창
		private GameObject _systemmenu;
		private GameObject _placement;
		//ChatManager _chat_manager;
		
		//json파일 로드정보
		private JSonParser<ItemBase> _itembase;
		private JSonParser<TechBase> _techbase;
		private JSonParser<Category> _categories;
		//각종 인벤
		private Inventory _inventory;
		private QuickInven _quickinven;
		private ChestInven _chestinven;
		private TurretInven _turretinven;
		private ForgeInven _forgeinven;
		private FactoryInven _factoryinven;
		private LabInven _labinven;
		private SkillInven _skillinven;
		private TechInven _techinven;
		private TechDescription _techdesc;

		private Tooltip _tooltip;			//툴팁
		private Coordinates _coodinates;	//좌표
		private DestoryProcess _destoryProcess;		//(마우스우클릭)건물삭제 progress바

		private ThirdPersonController _player;

		private GameObject _powersymbol;
		private GameObject _powerGrid;
		private GameObject _powerCable;



		//public SystemMenuManager SystemMenu	{ get { if (null == _systemmenu)	_systemmenu = GameObject.FindObjectOfType(typeof(SystemMenuManager)) as SystemMenuManager; return _systemmenu; } }
		public GameObject SystemMenu			{ get { if (null == _systemmenu)	_systemmenu = GameObject.Find("Canvas").transform.GetChild(4).gameObject; return _systemmenu; } }
		public GameObject Placement				{ get { if (null == _placement)		_placement = GameObject.Find("Placement"); return _placement; } }

		public JSonParser<ItemBase> ItemBases	{ get { if (null == _itembase)		_itembase = new JSonParser<ItemBase>(Path.Combine(Application.dataPath, "../config/json/items.json")); return _itembase; } }
		public JSonParser<TechBase> TechBases	{ get { if (null == _techbase)		_techbase = new JSonParser<TechBase>(Path.Combine(Application.dataPath, "../config/json/technology.json")); return _techbase; } }
		public JSonParser<Category> Categories	{ get { if (null == _categories)	_categories = new JSonParser<Category>(Path.Combine(Application.dataPath, "../config/json/categories.json")); return _categories; } }

		public Inventory Inventories			{ get { if (null == _inventory)		_inventory = GameObject.FindObjectOfType(typeof(Inventory)) as Inventory; return _inventory; } }
		public QuickInven QuickInvens			{ get { if (null == _quickinven)	_quickinven = GameObject.FindObjectOfType(typeof(QuickInven)) as QuickInven; return _quickinven; } }
		public TurretInven TurretInvens			{ get { if (null == _turretinven)	_turretinven = GameObject.FindObjectOfType(typeof(TurretInven)) as TurretInven; return _turretinven; } }
		public ChestInven ChestInvens			{ get { if (null == _chestinven)	_chestinven = GameObject.FindObjectOfType(typeof(ChestInven)) as ChestInven; return _chestinven; } }
		public ForgeInven ForgeInvens			{ get { if (null == _forgeinven)	_forgeinven = GameObject.FindObjectOfType(typeof(ForgeInven)) as ForgeInven; return _forgeinven; } }
		public FactoryInven FactoryInvens		{ get { if (null == _factoryinven)	_factoryinven = GameObject.FindObjectOfType(typeof(FactoryInven)) as FactoryInven; return _factoryinven; } }
		public LabInven LabInvens				{ get { if (null == _labinven)		_labinven = GameObject.FindObjectOfType(typeof(LabInven)) as LabInven; return _labinven; } }
		public SkillInven SkillInvens			{ get { if (null == _skillinven)	_skillinven = GameObject.FindObjectOfType(typeof(SkillInven)) as SkillInven; return _skillinven; } }
		public TechInven TechInvens				{ get { if (null == _techinven)		_techinven = GameObject.FindObjectOfType(typeof(TechInven)) as TechInven; return _techinven; } }
		public TechDescription TechDescs		{ get { if (null == _techdesc)		_techdesc = GameObject.FindObjectOfType(typeof(TechDescription)) as TechDescription; return _techdesc; } }

		public Tooltip Tooltips					{ get { if (null == _tooltip)		_tooltip = GameObject.FindObjectOfType(typeof(Tooltip)) as Tooltip; return _tooltip; } }
		public Coordinates Coordinates			{ get { if (null == _coodinates)	_coodinates = GameObject.FindObjectOfType(typeof(Coordinates)) as Coordinates; return _coodinates; } }
		public DestoryProcess DestoryProcess	{ get { if (null == _destoryProcess) _destoryProcess = GameObject.FindObjectOfType(typeof(DestoryProcess)) as DestoryProcess; return _destoryProcess; } }

		public ThirdPersonController Player		{ get { if (null == _player)		_player = GameObject.FindObjectOfType(typeof(ThirdPersonController)) as ThirdPersonController; return _player; } }

		public GameObject PowerSymbol			{ get { if (null == _powersymbol)	_powersymbol = Managers.Resource.Load<GameObject>("prefabs/buildings/PowerSymbol/PowerSymbolQuad"); return _powersymbol; } }
		public GameObject PowerGrid				{ get { if (null == _powerGrid)		_powerGrid = Managers.Resource.Load<GameObject>("prefabs/buildings/Power Grid"); return _powerGrid; } }
		public GameObject PowerCable			{ get { if (null == _powerCable)	_powerCable = Managers.Resource.Load<GameObject>("prefabs/buildings/Power Cable"); return _powerCable; } }

		//저장경로
		public string _save_dir = Application.dataPath + "/../save";


		//public bool bNewGame;
		public string _load_filename;	//읽을 파일명
		//public StringBuilder _locale;

		//FactoryFramework
		private BuildingPlacement _buildingPlacement;
		private ConveyorPlacement _conveyorPlacement;
		private RailPlacement _railPlacement;
		private GameObject _holding = null;	//들고있는 개체

		public void Init()
		{
			////HG_TEST: 임시로 불러올수 있는지 체크용(성공)
			//_buildingPlacement = FindObjectOfType<BuildingPlacement>();
			//_conveyorPlacement = FindObjectOfType<ConveyorPlacement>();
			//GameObject inven = GameObject.Find("Canvas\Inventory").transform.GetChild(4).gameObject
		}
		public void Clear()
		{
			_systemmenu = null;
			_itembase = null;
			_techbase = null;
			_categories = null;

			_inventory = null;
			_quickinven = null;
			_chestinven = null;
			_forgeinven = null;
			_factoryinven = null;
			_skillinven = null;
			_techinven = null;
			_techdesc = null;

			_tooltip = null;
			_coodinates = null;
			_destoryProcess = null;

			_player = null;
		}

		public void InitPlacement(BuildingPlacement building, ConveyorPlacement conveyor, RailPlacement rail)
		{
			_buildingPlacement	= building;
			_conveyorPlacement	= conveyor;
			_railPlacement		= rail;

			//building
			_buildingPlacement.finishPlacementEvent.OnEvent -= FinishPlaceBuilding;
			_buildingPlacement.finishPlacementEvent.OnEvent += FinishPlaceBuilding;
			//conveyor
			_conveyorPlacement.finishPlacementEvent.OnEvent -= FinishPlaceBuilding;
			_conveyorPlacement.finishPlacementEvent.OnEvent += FinishPlaceBuilding;
			////rail
			//_railPlacement.finishPlacementEvent.OnEvent -= FinishPlaceBuilding;
			//_railPlacement.finishPlacementEvent.OnEvent += FinishPlaceBuilding;
		}

		//설치할 건물을 생성합니다.
		public void PlaceBuilding(InvenItemData item)
		{
			if (null == item || null == item.database || item.amount <= 0) return;
			GameObject prefab = item.database.prefab;
			if (null == prefab) return;
			if (null != _holding) return;

			if (prefab.TryGetComponent(out FactoryFramework.Conveyor _))
			{
				_holding = _conveyorPlacement.StartPlacingConveyor(prefab);
				_holding.GetComponent<FactoryFramework.Conveyor>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.Rail _))
			{
				_holding = _railPlacement.StartPlacingRail(prefab);   //자원체크
				_holding.GetComponent<FactoryFramework.Rail>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.Driller _))
			{
				_holding = _buildingPlacement.StartPlacingBuilding(prefab, true);	//자원체크
				_holding.GetComponent<FactoryFramework.Driller>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.LogisticComponent _))
			{
				_holding = _buildingPlacement.StartPlacingBuilding(prefab, false);
				_holding.GetComponent<FactoryFramework.LogisticComponent>()._itembase = (ItemBase)item.database;
			}
			else if (prefab.TryGetComponent(out FactoryFramework.PowerGridComponent _))
				_holding = _buildingPlacement.StartPlacingBuilding(prefab, false);
		}
		public void DestoryBuilding()
		{
			if (null == _holding) return;

			if (_holding.TryGetComponent(out FactoryFramework.Conveyor _))
				_conveyorPlacement.ForceCancel();
			else if (_holding.TryGetComponent(out FactoryFramework.Driller _))
				_buildingPlacement.ForceCancel();
			else if (_holding.TryGetComponent(out FactoryFramework.PowerGridComponent _) || _holding.TryGetComponent(out FactoryFramework.LogisticComponent _))
				_buildingPlacement.ForceCancel();
			_holding = null;
		}

		private void FinishPlaceBuilding()
		{
			if (_holding.TryGetComponent<LogisticComponent>(out LogisticComponent component))
			{
				component.SetEnable_2(true);
				Debug.Log($"{component.name} enable:true");
			}
			Debug.Log($"{_holding.name} 완공");
			GameObject finishBuilding = _holding;	//막 완공된 건물
			_holding = null;   //재할당을 받기위해 null로 설정한다.

			//각 이벤트 위치어서 개수를 미리 정산해 줍니다.
			//InvenBase.choiced_item.AddStackCount(-1, false);
			if (InvenBase.choiced_item.amount <= 0)
			{
				Managers.Resource.Destroy(InvenBase.choiced_item.gameObject);
				InvenBase.choiced_item = null;
				return;
			}
			//아이템의 수량이 남아 있다면, prefab을 생성해 줍니다.
			if (null != InvenBase.choiced_item)
			{
				this.PlaceBuilding(InvenBase.choiced_item);
				//방금 완공된 건물이 전봇대이면...
				if(finishBuilding.TryGetComponent<PowerPole>(out PowerPole pole))
				{
					if (finishBuilding.TryGetComponent<PowerGridComponent>(out PowerGridComponent source))
					{
						//위치를 맞춰준다.(생성할때-전기줄이 생겼다없어져서(좌표:0,0,0))
						_holding.transform.position = source.transform.position;
						source.Connect(_holding?.GetComponent<PowerGridComponent>());
					}
				}
			}
		}

		//소유한 아이템 개수
		public int GetAmount(int itemid)
		{
			int amount = Managers.Game.Inventories.GetAmount(itemid);
			amount += Managers.Game.QuickInvens.GetAmount(itemid);
			if (null == InvenBase.choiced_item) return amount;

			if(itemid == InvenBase.choiced_item.database.id)
				amount += InvenBase.choiced_item.amount;
			return amount;
		}

		public int AddItem(int itemid, int amount, float fillAmount)
		{
			if (0 == amount) return 0;

			////choice item
			//if (itemid == InvenBase.choiced_item.database.id)
			//    InvenBase.choiced_item.AddStackCount()

			//quick inven
			//HG_TEST: 테스트로 bCreate를 true로 설정합니다.(default:false)
			//  인벤아이템을 모두 가져왔을때 자리를 완전히 비우지말고 amount만 0 으로 설정 이미지는 "흑백"으로 처리함으로
			//  이후 회수될때 원자리를 찾아 가면 좋을듯 싶다.
			if(null == Managers.Game || null == Managers.Game.QuickInvens || null == Managers.Game.Inventories)
			{
				int a = 0;
				a = 0;
			}
			amount = Managers.Game.QuickInvens.AddItem(itemid, amount, ref fillAmount, true);
			if (amount <= 0) return 0;

			//inventory
			amount = Managers.Game.Inventories.AddItem(itemid, amount, ref fillAmount);
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
			float fillAmount = itemData.GetFillAmount();
			int amount = Managers.Game.QuickInvens.AddItem(itemData, ref fillAmount);
			if (amount <= 0)
				return 0;
			itemData.amount = amount;

			//inventory
			amount = Managers.Game.Inventories.AddItem(itemData, ref fillAmount);
			if (amount <= 0)
				return 0;
			itemData.amount = amount;

			return amount;
		}

		public int SubItem(int itemid, int amount)
		{
			//inventory
			amount = Managers.Game.Inventories.SubItem(itemid, amount);
			if (amount <= 0)
				return 0;

			//quick inven
			amount = Managers.Game.QuickInvens.SubItem(itemid, amount);
			if (amount <= 0)
				return 0;

			return amount;
		}

		//가지고 있는 아이템 개수
		public int GetItem(int itemid)
		{
			int amount = Managers.Game.Inventories.GetAmount(itemid);
			amount += Managers.Game.QuickInvens.GetAmount(itemid);
			return amount;
		}

		#region SAVE
		public void Save(string filename)
		{
			if (false == Directory.Exists(_save_dir)) Directory.CreateDirectory(_save_dir);
			if(false == filename.Contains(".sav"))	filename += ".sav";	//확장자 추가

			//BinarySerialize(GetInventory(), Application.persistentDataPath + "/savefile.sav");
			using (FileStream fs = File.Create(Path.Combine(_save_dir, filename)))
			{
				BinaryWriter bw = new BinaryWriter(fs);

				//screen-shot
				SaveScreenShot(bw);

				_inventory.Save(bw);
				_quickinven.Save(bw);


				//// build a lookup of Guid -> SerializationReference
				//// this is used to re-link the conveyor belts to buildings
				//Dictionary<Guid, SerializationReference> lookup = new Dictionary<Guid, SerializationReference>();
				//// keep conveyors for a seoncd pass once all buildings are setup
				//List<SerializationReference> conveyors = new List<SerializationReference>();

				List<Conveyor> conveyors = new List<Conveyor>();

				var serializables = _buildingPlacement.gameObject.GetComponentsInChildren<SerializationReference>();
				bw.Write(serializables.Length);     //building count
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
						logistic.Save(bw);
						continue;
					}
					Debug.LogError($"{obj.name}을 Save할 로직이 준비되어있지 않습니다.");
				}

				//conveyor는 별도로 빼서 아래에서 처리한다. connect처리때문에.
				foreach (var obj in conveyors)
				{
					Conveyor conveyor = obj.GetComponent<Conveyor>();
					conveyor.Save(bw);
				}


				Managers.Game.Player.Save(bw);

				//전기줄
				bw.Write(CableRendererManager.instance.Cables.Count);
				foreach(var key in CableRendererManager.instance.Cables.Keys)
				{
					//Debug.Log($"key:{key.Item1}/{key.Item2}");
					bw.Write(key.Item1.GUID.ToString());
					bw.Write(key.Item2.GUID.ToString());
				}

				fs.Close();
			}
		}

		public void Load()
		{
			//BinaryDeserialize<Inventory>(Application.persistentDataPath + "/savefile.sav");
			string filepath = Path.Combine(_save_dir, _load_filename);
			if(false == filepath.Contains(".sav")) filepath += ".sav";	//확장자 추가
			if (false == File.Exists(filepath))
			{
				Debug.LogError($"load failed({filepath})");
				return;
			}

			Debug.Log($"[{_load_filename}] 파일을 로드합니다.");
			using (FileStream fs = File.Open(filepath, FileMode.Open))
			{
				BinaryReader reader = new BinaryReader(fs);

				//screen-shot : 이미지 만큼 건너띄고 읽어옵니다.
				int offset = 4 + reader.ReadInt32(); //load: 총길이( 4(int) + bytes.Length ) 
				fs.Seek(offset, SeekOrigin.Begin);

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
					//instantiated.transform.parent = _buildingPlacement.transform.Find($"Pool_{prefab.name}") ?? (new GameObject($"Pool_{prefab.name}") { transform = { parent = _buildingPlacement.transform } }).transform;
					instantiated.transform.parent = MyCraft.Common.ParentPool(_buildingPlacement.transform, prefab.name);

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

						//재사용시(불러오기) material이 녹색으로 노출되거나 collider가 disable된 경우가 있어서
						_conveyorPlacement.SetEnable_1(conveyor, true);

                        conveyors.Add(conveyor);
						lookup.Add(conveyor.GUID, conveyor);	//conveyor랑 연결될 conveyor GUID
						continue;
					}


					if (instantiated.TryGetComponent(out LogisticComponent building))
					{
						building._itembase = prefab.GetComponent<LogisticComponent>()._itembase;
						building.Load(reader);

                        //재사용시(불러오기) material이 녹색으로 노출되거나 collider가 disable된 경우가 있어서
                        _buildingPlacement.SetEnable_1(building, true);

                        //Debug.Log($"lookup:({lookup.Count})[{building.name}:{building.GUID}]");
                        lookup.Add(building.GUID, building);	//conveyor랑 연결될 buildings GUID
                        //if(false==lookup.ContainsKey(building.GUID))
                        //	lookup.Add(building.GUID, building);
                        //else
                        //	Debug.Log($"lookup:({lookup.Count})[{building.name}:{building.GUID}]");

                        continue;
					}
					Debug.LogError($"{instantiated.name}을 Load할 로직이 준비되어있지 않습니다.");
				}

				Managers.Game.Player.Load(reader);

				//전기줄
				int cnt = reader.ReadInt32();
				for (int i = 0; i < cnt; ++i)
				{
					var connectA = lookup[new Guid(reader.ReadString())].GetComponent<PowerGridComponent>();
					var connectB = lookup[new Guid(reader.ReadString())].GetComponent<PowerGridComponent>();
					connectA.Connect(connectB);
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
							OutputSocket osocket = building.GetOutputSocketByIndex(conveyor.data.outputSocketIndex);
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
							InputSocket isocket = building.GetInputSocketByIndex(conveyor.data.inputSocketIndex);
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

		private void SaveScreenShot(BinaryWriter bw)
		{
			// 캡쳐할 이미지의 크기를 결정합니다.
			int width = Screen.width;
			int height = Screen.height;

			// UI를 제외한 스크린샷을 찍기 위해 메인 카메라의 Culling Mask 임시 변경
			Camera mainCamera = Camera.main;
			int originalCullingMask = mainCamera.cullingMask;
			mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

			// RenderTexture 생성하여 메인 카메라 출력 설정
			RenderTexture rt = new RenderTexture(width, height, 24);
			mainCamera.targetTexture = rt;

			// RenderTexture에 렌더링
			mainCamera.Render();

			// Texture2D 객체를 생성합니다. 이 때 크기를 이전에 정한 width와 height로 지정합니다.
			RenderTexture.active = rt;
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

			// 캡쳐한 내용을 Texture2D에 저장합니다.
			tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			tex.Apply();

			// 메인 카메라 출력 설정과 Culling Mask 복구
			mainCamera.targetTexture = null;
			mainCamera.cullingMask = originalCullingMask;
			RenderTexture.active = null;

			// Texture2D를 byte 배열로 변환합니다.
			byte[] bytes = tex.EncodeToPNG();

			//save : 총길이( 4(int) + bytes.Length ) 
			bw.Write(bytes.Length);
			bw.Write(bytes);

			// Texture2D 객체를 메모리에서 해제합니다.
			UnityEngine.Object.Destroy(tex);
		}

		//private void LoadScreenShot(BinaryReader br)
		//{
		//	// byte 배열을 Texture2D로 변환합니다.
		//	Texture2D tex = new Texture2D(1, 1);
		//	int length = br.Read();
		//	tex.LoadImage(br.ReadBytes(length));

		//	// SpriteRenderer를 사용하여 Texture2D를 Sprite로 변환하여 화면에 나타냅니다.
		//	Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
		//	spriteRenderer.sprite = sprite;
		//}

		#endregion //..SAVE

	}
}