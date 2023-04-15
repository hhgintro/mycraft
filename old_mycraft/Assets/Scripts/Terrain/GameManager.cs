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


        private static SystemMenu systemmenu;

        //private static ItemDatabase item_database;
        private static JSonParser<ItemBase> _itembase;
        private static JSonParser<TechBase> _techbase;
        private static JSonParser<Categories> _categories;

        private static TerrainManager _terrain_manager;
        private static BeltManager _belt_manager;
        private static SpliterManager _spliter_manager;
        private static InserterManager _inserter_manager;
        private static ChestManager _chest_manager;
        private static DrillManager _drill_manager;
        private static MachineManager _machine_manager;
        private static StoneFurnaceManager _stone_furnace_manager;

        private static IronManager _iron_manager;
        private static StoneManager _stone_manager;
        private static TreeManager _tree_manager;

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

        private static GameObject _invenPanel;
        private static GameObject _invenSlot;
        private static GameObject _invenItem;
        private static GameObject _invenSkill;
        private static GameObject _invenReset;


        public bool bNewGame { get; set; }
        public StringBuilder _locale;


        public void OnGame()
        {
            if(true == this.bNewGame)
            {
                //inven slot
                if(0 < GameManager.GetInventory()._panels.Count)
                    GameManager.GetInventory()._panels[0].SetAmount(20);

                //HG_TEST : 테스트 아이템 지급
                GameManager.GetInventory().AddItem(2010, 100);  //iron-plate
                GameManager.GetInventory().AddItem(2011, 100);  //iron-gear
                GameManager.GetInventory().AddItem(2000, 100);  //wood

                GameManager.GetInventory().AddItem(10, 54);     //raw-wood
                GameManager.GetInventory().AddItem(20, 54);     //stone
                GameManager.GetInventory().AddItem(40, 54);     //iron-ore
                GameManager.GetInventory().AddItem(50, 54);     //copper-ore

                GameManager.GetInventory().AddItem(1010, 54);   //belt


                //quick slot
                if (0 < GameManager.GetQuickInven()._panels.Count)
                    GameManager.GetQuickInven()._panels[0].SetAmount(10);

                GameManager.GetQuickInven().AddItem(1000, 54);  //chest
                GameManager.GetQuickInven().AddItem(1010, 54);  //belt
                GameManager.GetQuickInven().AddItem(1020, 54);  //spliter
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



        public static SystemMenu GetSystemMenu() {
            if (null == systemmenu)
                systemmenu = GameObject.Find("Canvas/SystemMenu").GetComponent<SystemMenu>();
            return systemmenu;
        }
        public static JSonParser<ItemBase> GetItemBase() {
            //if (null == item_database)
            //    item_database = GameObject.Find("Canvas/Inventory").GetComponent<ItemDatabase>();
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

        //public static GameObject GetInvenPanel()
        //{
        //    if(null == _invenPanel)
        //        _invenPanel = Managers.Resource.Load<GameObject>("prefab/ui/Slot Panel");
        //    return _invenPanel;
        //}
        //public static GameObject GetInvenSlot()
        //{
        //    if(null == _invenSlot)
        //       _invenSlot = Managers.Resource.Load<GameObject>("prefab/ui/Slot");
        //    return _invenSlot;
        //}
        //public static GameObject GetInvenItem()
        //{
        //    if(null == _invenItem)
        //        _invenItem = Managers.Resource.Load<GameObject>("prefab/ui/Item");
        //    return _invenItem;
        //}
        //public static GameObject GetInvenSkill()
        //{
        //    if(null == _invenSkill)
        //        _invenSkill = Managers.Resource.Load<GameObject>("prefab/ui/Skill");
        //    return _invenSkill;
        //}
        //public static GameObject GetInvenReset()
        //{
        //    if(null == _invenReset)
        //        _invenReset = Managers.Resource.Load<GameObject>("prefab/ui/Reset");
        //    return _invenReset;
        //}

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
            using (FileStream fs = File.Create(Application.persistentDataPath + "/savefile.sav"))
            {
                BinaryWriter writer = new BinaryWriter(fs);

                GameManager.GetInventory().Save(writer);
                GameManager.GetQuickInven().Save(writer);

                GameManager.GetTerrainManager().Save(writer);


                fs.Close();
            }

            //BinarySerialize(GetInventory(), Application.persistentDataPath + "/savefile.sav");
        }

        void Load()
        {
            if (File.Exists(Application.persistentDataPath + "/savefile.sav"))
            {
                using (FileStream fs = File.Open(Application.persistentDataPath + "/savefile.sav", FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    GameManager.GetInventory().Load(reader);
                    GameManager.GetQuickInven().Load(reader);

                    GameManager.GetTerrainManager().Load(reader);
                    //for (int i = 0; i < 4; ++i)
                    //    Debug.Log(i + " reader:" + reader.ReadInt32());

                    fs.Close();

                }

                //BinaryDeserialize<Inventory>(Application.persistentDataPath + "/savefile.sav");
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