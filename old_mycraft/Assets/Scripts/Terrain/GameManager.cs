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


        //private static SystemMenu _systemmenu;
        //private static ChatManager _chat_manager;

        //private static ItemDatabase item_database;
        private static JSonParser<ItemBase> _itembase;
        private static JSonParser<TechBase> _techbase;
        private static JSonParser<Categories> _categories;

        private static TerrainManager _terrain_manager;
        private static BeltManager _belt_manager;
        private static BeltSlopeUpManager _belt_slope_up_manager;
        private static BeltSlopeDownManager _belt_slope_down_manager;
        private static BeltVerticalUpBeginManager _belt_vertical_up_begin_manager;
        private static BeltVerticalUpMiddleManager _belt_vertical_up_middle_manager;
        private static BeltVerticalUpEndManager _belt_vertical_up_end_manager;
        private static BeltVerticalDownBeginManager _belt_vertical_down_begin_manager;
        private static BeltVerticalDownMiddleManager _belt_vertical_down_middle_manager;
        private static BeltVerticalDownEndManager _belt_vertical_down_end_manager;
        private static SpliterManager _spliter_manager;
        private static InserterManager _inserter_manager;
        private static ChestManager _chest_manager;
        private static DrillManager _drill_manager;
        private static MachineManager _machine_manager;
        private static StoneFurnaceManager _stone_furnace_manager;

        private static IronManager _iron_manager;
        private static CopperManager _copper_manager;
        private static CoalManager _coal_manager;
        private static StoneManager _stone_manager;
        private static TreeManager _tree_manager;
        private static CrudeOilManager _crudeoil_manager;

        private static MouseController _mouse_controller;

        private static BeltGoods _belt_package;

        private static Inventory _inventory;
        private static ChestInven _chestinven;
        private static QuickInven _quickinven;
        private static MachineInven _machineinven;
        private static StoneFurnaceInven _stone_furnace_inven;
        private static SkillInven _skillinven;
        private static TechInven _techinven;
        private static TechDescription _techdesc;
        private static Tooltip _tooltip;

        private static Coordinates _coodinates;  //좌표

        //private static GameObject _invenPanel;
        //private static GameObject _invenSlot;
        //private static GameObject _invenItem;
        //private static GameObject _invenSkill;
        //private static GameObject _invenReset;

        //저장경로
        public static string _save_dir = Application.dataPath + "/../save";


        public bool bNewGame { get; set; }
        public StringBuilder _locale;


        public void OnGame()
        {
            ShowInitUIs();

            if (true == this.bNewGame)
            {
                //inven slot
                if(0 < GameManager.GetInventory()._panels.Count)
                    GameManager.GetInventory()._panels[0].SetAmount(51);

                GameManager.GetInventory().Resize();


                //HG_TEST : 테스트 아이템 지급
                GameManager.GetInventory().AddItem(2010, 100);  //iron-plate
                GameManager.GetInventory().AddItem(2011, 100);  //iron-gear
                GameManager.GetInventory().AddItem(2020, 100);  //copper-plate
                GameManager.GetInventory().AddItem(2021, 100);  //copper-cable
                GameManager.GetInventory().AddItem(2000, 100);  //wood

                GameManager.GetInventory().AddItem(10, 54);     //raw-wood
                GameManager.GetInventory().AddItem(20, 54);     //stone
                GameManager.GetInventory().AddItem(40, 54);     //iron-ore
                GameManager.GetInventory().AddItem(50, 54);     //copper-ore

                GameManager.GetInventory().AddItem(1100, 54);   //belt
                GameManager.GetInventory().AddItem(1130, 100);   //Vertical-Up-Begin
                GameManager.GetInventory().AddItem(1140, 100);   //Vertical-Up-Middle
                GameManager.GetInventory().AddItem(1150, 100);   //Vertical-Up-End
                GameManager.GetInventory().AddItem(1160, 100);   //Vertical-Down-Begin
                GameManager.GetInventory().AddItem(1170, 100);   //Vertical-Down-Middle
                GameManager.GetInventory().AddItem(1180, 100);   //Vertical-Down-End


                //quick slot
                if (0 < GameManager.GetQuickInven()._panels.Count)
                    GameManager.GetQuickInven()._panels[0].SetAmount(10);

                GameManager.GetQuickInven().AddItem(1000, 54);  //chest
                GameManager.GetQuickInven().AddItem(1100, 54);  //belt
                GameManager.GetQuickInven().AddItem(1110, 100); //belt-up
                GameManager.GetQuickInven().AddItem(1120, 100); //belt-down
                GameManager.GetQuickInven().AddItem(1200, 54);  //spliter
                GameManager.GetQuickInven().AddItem(1030, 54);  //inserter
                GameManager.GetQuickInven().AddItem(1040, 54);  //drill
                GameManager.GetQuickInven().AddItem(1050, 54);  //stone-furnace
                GameManager.GetQuickInven().AddItem(1060, 54);  //machine

            }
            else
            {
                this.Load();
            }
            Application.runInBackground = true;
        }

        public void Init()
        {
            //load ini
            Load(Application.dataPath + "/../config/config.ini");
            LocaleManager.Open(Application.streamingAssetsPath + "/locale/" + _locale.ToString() + "/ui.cfg");
        }

        public void Clear()
        {
        
        }

        private void Load(string filepath)
        {
            if (null != _locale) return;

            //locale
            _locale = new StringBuilder(255);
            GetPrivateProfileString("common", "locale", "(empty)", _locale, 255, filepath);
            if (0 == string.Compare(_locale.ToString(), "(empty)"))
                Debug.LogError("Fail : Not Read Locale");
        }

        private void ShowInitUIs()
        {
            GameManager.GetQuickInven().gameObject.SetActive(true);
            GameManager.GetTechInven().gameObject.SetActive(true);
            GameManager.GetTechDesc().gameObject.SetActive(true);
            GameManager.GetTooltip().gameObject.SetActive(true);

            GameManager.GetInventory().gameObject.SetActive(false);
            GameManager.GetChestInven().gameObject.SetActive(false);
            GameManager.GetStoneFurnaceInven().gameObject.SetActive(false);
            GameManager.GetMachineInven().gameObject.SetActive(false);
            GameManager.GetSkillInven().gameObject.SetActive(false);

            //GameManager.GetSystemMenu().gameObject.SetActive(false);
            Managers.SystemMenu.SetActive(false);
        }



        //public static SystemMenu GetSystemMenu() {
        //    if (null == _systemmenu)
        //        _systemmenu = GameObject.Find("Canvas/SystemMenu").GetComponent<SystemMenu>();
        //    return _systemmenu;
        //}
        //public static ChatManager GetChatManager()
        //{
        //    if (null == _chat_manager)
        //        _chat_manager  = GameObject.Find("Canvas/Chatting").GetComponent<ChatManager>();
        //    return _chat_manager;
        //}
        public static JSonParser<ItemBase> GetItemBase() {
            if(null == _itembase)
                _itembase = new JSonParser<ItemBase>(Application.streamingAssetsPath + "/locale/" + Managers.Game._locale.ToString() + "/items.json");
            return _itembase;
        }

        public static JSonParser<TechBase> GetTechBase() {
            if (null == _techbase)
                _techbase = new JSonParser<TechBase>(Application.streamingAssetsPath + "/locale/" + Managers.Game._locale.ToString() + "/technology.json");
            return _techbase;
        }

        public static JSonParser<Categories> GetCategories()
        {
            if (null == _categories)
                _categories = new JSonParser<Categories>(Application.streamingAssetsPath + "/locale/" + Managers.Game._locale.ToString() + "/categories.json");
            return _categories;
        }

        public static TerrainManager GetTerrainManager() {
            if(null == _terrain_manager)
                _terrain_manager = GameObject.Find("Terrain").GetComponent<TerrainManager>();
            return _terrain_manager;
        }
        public static BeltManager GetBeltManager() {
            if(null == _belt_manager)
                _belt_manager = GetTerrainManager().GetComponentInChildren<BeltManager>();
            return _belt_manager;
        }
        public static BeltSlopeUpManager GetBeltSlopeUpManager()
        {
            if (null == _belt_slope_up_manager)
                _belt_slope_up_manager = GetTerrainManager().GetComponentInChildren<BeltSlopeUpManager>();
            return _belt_slope_up_manager;
        }
        public static BeltSlopeDownManager GetBeltSlopeDownManager()
        {
            if (null == _belt_slope_down_manager)
                _belt_slope_down_manager = GetTerrainManager().GetComponentInChildren<BeltSlopeDownManager>();
            return _belt_slope_down_manager;
        }
        public static BeltVerticalUpBeginManager GetBeltVerticalUpBeginManager()
        {
            if (null == _belt_vertical_up_begin_manager)
                _belt_vertical_up_begin_manager = GetTerrainManager().GetComponentInChildren<BeltVerticalUpBeginManager>();
            return _belt_vertical_up_begin_manager;
        }
        public static BeltVerticalUpMiddleManager GetBeltVerticalUpMiddleManager()
        {
            if (null == _belt_vertical_up_middle_manager)
                _belt_vertical_up_middle_manager = GetTerrainManager().GetComponentInChildren<BeltVerticalUpMiddleManager>();
            return _belt_vertical_up_middle_manager;
        }
        public static BeltVerticalUpEndManager GetBeltVerticalUpEndManager()
        {
            if (null == _belt_vertical_up_end_manager)
                _belt_vertical_up_end_manager = GetTerrainManager().GetComponentInChildren<BeltVerticalUpEndManager>();
            return _belt_vertical_up_end_manager;
        }
        public static BeltVerticalDownBeginManager GetBeltVerticalDownBeginManager()
        {
            if (null == _belt_vertical_down_begin_manager)
                _belt_vertical_down_begin_manager = GetTerrainManager().GetComponentInChildren<BeltVerticalDownBeginManager>();
            return _belt_vertical_down_begin_manager;
        }
        public static BeltVerticalDownMiddleManager GetBeltVerticalDownMiddleManager()
        {
            if (null == _belt_vertical_down_middle_manager)
                _belt_vertical_down_middle_manager = GetTerrainManager().GetComponentInChildren<BeltVerticalDownMiddleManager>();
            return _belt_vertical_down_middle_manager;
        }
        public static BeltVerticalDownEndManager GetBeltVerticalDownEndManager()
        {
            if (null == _belt_vertical_down_end_manager)
                _belt_vertical_down_end_manager = GetTerrainManager().GetComponentInChildren<BeltVerticalDownEndManager>();
            return _belt_vertical_down_end_manager;
        }
        public static SpliterManager GetSpliterManager() {
            if (null == _spliter_manager)
                _spliter_manager = GetTerrainManager().GetComponentInChildren<SpliterManager>();
            return _spliter_manager;
        }
        public static InserterManager GetInserterManager() {
            if(null == _inserter_manager)
                _inserter_manager = GetTerrainManager().GetComponentInChildren<InserterManager>();
            return _inserter_manager;
        }
        public static ChestManager GetChestManager() {
            if(null == _chest_manager)
                _chest_manager = GetTerrainManager().GetComponentInChildren<ChestManager>();
            return _chest_manager;
        }
        public static DrillManager GetDrillManager() {
            if(null == _drill_manager)
                _drill_manager = GetTerrainManager().GetComponentInChildren<DrillManager>();
            return _drill_manager;
        }
        public static MachineManager GetMachineManager() {
            if (null == _machine_manager)
                _machine_manager = GetTerrainManager().GetComponentInChildren<MachineManager>();
            return _machine_manager;
        }
        public static StoneFurnaceManager GetStoneFurnaceManager() {
            if (null == _stone_furnace_manager)
                _stone_furnace_manager = GetTerrainManager().GetComponentInChildren<StoneFurnaceManager>();
            return _stone_furnace_manager;
        }
        public static IronManager GetIronManager() {
            if (null == _iron_manager)
                _iron_manager = GetTerrainManager().GetComponentInChildren<IronManager>();
            return _iron_manager;
        }
        public static CopperManager GetCopperManager()
        {
            if (null == _copper_manager)
                _copper_manager = GetTerrainManager().GetComponentInChildren<CopperManager>();
            return _copper_manager;
        }
        public static CoalManager GetCoalManager()
        {
            if (null == _coal_manager)
                _coal_manager = GetTerrainManager().GetComponentInChildren<CoalManager>();
            return _coal_manager;
        }
        public static StoneManager GetStoneManager() {
            if (null == _stone_manager)
                _stone_manager = GetTerrainManager().GetComponentInChildren<StoneManager>();
            return _stone_manager;
        }
        public static TreeManager GetTreeManager() {
            if (null == _tree_manager)
                _tree_manager = GetTerrainManager().GetComponentInChildren<TreeManager>();
            return _tree_manager;
        }
        public static CrudeOilManager GetCrudeOilManager()
        {
            if (null == _crudeoil_manager)
                _crudeoil_manager = GetTerrainManager().GetComponentInChildren<CrudeOilManager>();
            return _crudeoil_manager;
        }

        public static MouseController GetMouseController() {
            if(null == _mouse_controller)
                _mouse_controller = GameObject.Find("Input/Mouse").GetComponent<MouseController>();
            return _mouse_controller;
        }

        public static BeltGoods GetBeltGoods() {
            if(null == _belt_package)
                _belt_package = Managers.Resource.Load<BeltGoods>("prefabs/blocks/belt-goods");
            return _belt_package;
        }

        public static Inventory GetInventory() {
            if(null == _inventory)
                _inventory = GameObject.Find("Canvas/Inventory/Inventory Panel").GetComponent<Inventory>();
            return _inventory;
        }

        public static ChestInven GetChestInven() {
            if (null == _chestinven)
                _chestinven = GameObject.Find("Canvas/Inventory/Chest Panel").GetComponent<ChestInven>();
            return _chestinven;
        }

        public static QuickInven GetQuickInven() {
            if (null == _quickinven)
                _quickinven = GameObject.Find("Canvas/Inventory/Quick Panel").GetComponent<QuickInven>();
            return _quickinven;
        }

        public static MachineInven GetMachineInven() {
            if (null == _machineinven)
                _machineinven = GameObject.Find("Canvas/Inventory/Machine Panel").GetComponent<MachineInven>();
            return _machineinven;
        }
        public static StoneFurnaceInven GetStoneFurnaceInven() {
            if (null == _stone_furnace_inven)
                _stone_furnace_inven = GameObject.Find("Canvas/Inventory/Stone-Furnace Panel").GetComponent<StoneFurnaceInven>();
            return _stone_furnace_inven;
        }
        public static SkillInven GetSkillInven() {
            if (null == _skillinven)
                _skillinven = GameObject.Find("Canvas/Inventory/Skill Panel").GetComponent<SkillInven>();
            return _skillinven;
        }
        public static TechInven GetTechInven() {
            if (null == _techinven)
                _techinven = GameObject.Find("Canvas/Inventory/Technology Panel").GetComponent<TechInven>();
            return _techinven;
        }

        public static TechDescription GetTechDesc() {
            if (null == _techdesc)
                _techdesc = GameObject.Find("Canvas/Inventory/TechDescription Panel").GetComponent<TechDescription>();
            return _techdesc;
        }

        public static Tooltip GetTooltip() {
            if (null == _tooltip)
                _tooltip = GameObject.Find("Canvas/Inventory/Tooltip").GetComponent<Tooltip>();
            return _tooltip;
        }

        public static Coordinates GetCoordinates()
        {
            if (null == _coodinates)
                _coodinates = GameObject.Find("Canvas/Coordinates").GetComponent<Coordinates>();
            return _coodinates;
        }

        public static BeltGoods CreateMineral(int itemid, Transform parent)//, Vector3 pos)
        {
            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(itemid);
            if (null == itembase) return null;

            GameObject obj = UnityEngine.Object.Instantiate(GameManager.GetBeltGoods().gameObject);
            obj.SetActive(true);
            //Hierarchy 위치설정
            obj.transform.SetParent(parent);
            //생성위치
            //obj.transform.position = pos;

            BeltGoods goods             = obj.GetComponent<BeltGoods>();
            goods.itemid                = itemid;    //HG_TODO : item.json에서 설정된 값을 넣어줘야 합니다.
            MeshRenderer render         = goods.transform.GetChild(0).GetComponent<MeshRenderer>();
            render.material.mainTexture = itembase.Sprite.texture;
            return goods;
        }

        public static int AddItem(int itemid, int amount)
        {
            //quick inven
            amount = GetQuickInven().AddItem(itemid, amount, false);
            if (amount <= 0)
                return 0;

            //inventory
            amount = GetInventory().AddItem(itemid, amount);
            if (amount <= 0)
                return 0;

            ////그래도 남아있으면...생성해준다.
            //amount = GetQuickInven().AddItem(itemid, amount);
            //if (amount <= 0)
            //    return 0;

            return amount;
        }

        public static int AddItem(InvenItemData itemData)
        {
            //quick inven
            int amount = GetQuickInven().AddItem(itemData);
            if (amount <= 0)
                return 0;
            itemData.amount = amount;

            //inventory
            amount = GetInventory().AddItem(itemData);
            if (amount <= 0)
                return 0;
            itemData.amount = amount;

            return amount;
        }



        public static void Save()
        {
            if (false == Directory.Exists(_save_dir)) Directory.CreateDirectory(_save_dir);

            //BinarySerialize(GetInventory(), Application.persistentDataPath + "/savefile.sav");
            using (FileStream fs = File.Create(_save_dir + "/savefile.sav"))
            {
                BinaryWriter writer = new BinaryWriter(fs);

                GameManager.GetInventory().Save(writer);
                GameManager.GetQuickInven().Save(writer);

                GameManager.GetTerrainManager().Save(writer);


                fs.Close();
            }
        }

        void Load()
        {
            //BinaryDeserialize<Inventory>(Application.persistentDataPath + "/savefile.sav");
            if (File.Exists(_save_dir + "/savefile.sav"))
            {
                using (FileStream fs = File.Open(_save_dir + "/savefile.sav", FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    GameManager.GetInventory().Load(reader);
                    GameManager.GetQuickInven().Load(reader);

                    GameManager.GetTerrainManager().Load(reader);

                    fs.Close();
                }
            }
        }

        public static void BinarySerialize<T>(T t, string filepath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Create);
            formatter.Serialize(stream, t);
            stream.Close();
        }
        public T BinaryDeserialize<T>(string filepath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);
            T t = (T)formatter.Deserialize(stream);
            stream.Close();
            return t;
        }

    }
}