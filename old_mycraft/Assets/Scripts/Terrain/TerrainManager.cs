using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    ////***** 매우 중요함 *****//
    ////x검색 -> z검색 -> y검색
    ////높이값을 확인하기 위해서 y검색을 마지막에 진행합니다.
    //using BLOCK_Y = Dictionary<int/*y*/, BlockScript>;
    //using BLOCK_ZY = Dictionary<int/*z*/, Dictionary<int, BlockScript>>;
    //using BLOCK_XYZ = Dictionary<int/*x*/, Dictionary<int/*z*/, Dictionary<int, BlockScript>>>;

    public class TerrainManager : MonoBehaviour
    {
        //private BeltManager belt_manager;
        //private InserterManager inserter_manager;
        private BlockScript choiced_prefab = null;  //생성을 위해 선택된 개체


        //block 3차원 배열
        //private BLOCK_XYZ block_xyz = new BLOCK_XYZ();
        public TerrainLayer block_layer = new TerrainLayer();
        //mineral 3차원 배열
        public TerrainLayer mineral_layer = new TerrainLayer();

        void Awake()
        {
            LoadPrefab<ChestManager>("hierarchy/terrain", "chest");
            LoadPrefab<BeltManager>("hierarchy/terrain", "belt");
            LoadPrefab<BeltUpManager>("hierarchy/terrain", "belt-up");
            LoadPrefab<BeltDownManager>("hierarchy/terrain", "belt-down");
            LoadPrefab<SpliterManager>("hierarchy/terrain", "spliter");
            LoadPrefab<InserterManager>("hierarchy/terrain", "inserter");
            LoadPrefab<DrillManager>("hierarchy/terrain", "drill");
            LoadPrefab<MachineManager>("hierarchy/terrain", "machine");
            LoadPrefab<StoneFurnaceManager>("hierarchy/terrain", "stone-furnace");
            LoadPrefab<StoneManager>("hierarchy/terrain", "stone");
            LoadPrefab<TreeManager>("hierarchy/terrain", "tree");
            LoadPrefab<IronManager>("hierarchy/terrain", "iron-ore");
            LoadPrefab<CopperManager>("hierarchy/terrain", "copper-ore");
            LoadPrefab<CoalManager>("hierarchy/terrain", "coal");
            LoadPrefab<CrudeOilManager>("hierarchy/terrain", "crude-oil");

        }

        void Start()
        {
            Managers.Game.OnGame();

            int x = -2, z = 8;
            //coal

            //copper-ore

            PutdownBlock(30, 2, x, z -= 2, GameManager.GetCoalManager().GetChoicePrefab());         //coal
            PutdownBlock(50, 3, x, z -= 2, GameManager.GetCopperManager().GetChoicePrefab());       //copper-ore
            PutdownBlock(10, 3, x, z -= 2, GameManager.GetTreeManager().GetChoicePrefab());         //tree
            PutdownBlock(40, 3, x, z -= 2, GameManager.GetIronManager().GetChoicePrefab());         //iron-ore
            PutdownBlock(20, 2, x, z -= 2, GameManager.GetStoneManager().GetChoicePrefab());        //stone
            PutdownBlock(30, 2, x, z -= 2, GameManager.GetCoalManager().GetChoicePrefab());         //coal
            PutdownBlock(60, 2, x, z -= 2, GameManager.GetCrudeOilManager().GetChoicePrefab());     //crude-old
        }

        void LoadPrefab<T>(string path, string name) where T : BlockManager
        {
            GameObject go = Managers.Resource.Instantiate(path, this.transform);
            go.name = name;
            go.AddComponent<T>();
        }

        void PutdownBlock(int itemid, int count, int x, int z, BlockScript prefab)
        {
            for (int i = 0; i < count; ++i)
            {
                BlockScript block = CreateBlock(this.GetMineralLayer(), x-i, 0, z, prefab);
                block._itembase = GameManager.GetItemBase().FetchItemByID(itemid);//BLOCKTYPE.RAW_WOOD
                if (null == block._itembase)
                    Debug.LogError($"Fail: not found itemid {itemid}");
            }
        }


        public TerrainLayer GetBlockLayer() { return block_layer; }
        public TerrainLayer GetMineralLayer() { return mineral_layer; }


        public BlockScript GetChoicePrefab() { return this.choiced_prefab; }
        public void SetChoicePrefab(BlockScript block)
        {
            //null로 세팅하기 위해서 여기에서는 null체크를 하지마세요
            //의도하지 않은 null이 입력되는지 반드시 확인하세요
            //(반드시 의도한 null만 허용되어야 합니다.)
            //if (null == script) return;

            if (block == this.choiced_prefab)
                return;
            //old
            if (null != this.choiced_prefab)
            {
                ////외형변화가 필요한 block은 위치를 강제로 옮겨서 sensor 체크 이후에 비활성화 되도록 처리한다.
                //switch(this.choiced_prefab._itembase.type)
                //{
                //    case BLOCKTYPE.BELT:
                //    case BLOCKTYPE.BELT_UP:
                //    case BLOCKTYPE.BELT_DOWN:
                //    case BLOCKTYPE.SPLITER:
                //        this.choiced_prefab.ForceMove();        //OnTriggerExit()을 위해 위치를 강제로 이동시킨다.
                //        this.choiced_prefab._deactive = true;   //OnTriggerExit()이후에 SetActive(false)하기 위해.
                //        break;
                //    default:
                //        this.choiced_prefab.SetActive(false);
                //        break;
                //}
                this.choiced_prefab.SetActive(false);
            }
            //new
            this.choiced_prefab = block;
            if (null != this.choiced_prefab)
            {
                //choiced_prefab.Clear(); //belt모형이 바뀔때, 이전기록을 그대로 가지고 있어, 잘못판단하는 경우가 있었다.(sensor체크부분)
                this.choiced_prefab.SetActive(true);
                this.choiced_prefab.Clear();
                //if(null != this.choiced_prefab.transform)
                //    this.choiced_prefab.transform.SetParent(this.transform);
            }

            //Debug.Log("layer:  " + this.choiced_prefab.gameObject.layer);
        }
        public BlockScript SetChoicePrefab(ItemBase itembase)
        {
            BlockScript block = this.GetBlockPrefab(itembase.type, TURN_WEIGHT.FRONT);
            if (null == block) return null;
            block._itembase = itembase;
            this.SetChoicePrefab(block);
            return block;
        }
        //public void SetChoicePrefab(BLOCKTYPE blocktype)
        //{
        //    BlockScript script = this.GetBlockPrefab(blocktype, TURN_WEIGHT.FRONT);
        //    if (null == script) return;
        //    this.SetChoicePrefab(script);
        //}
        public BlockScript GetBlockPrefab(BLOCKTYPE blocktype, TURN_WEIGHT weight)
        {
            BlockScript script = null;
            switch (blocktype)
            {
                case BLOCKTYPE.CHEST:           script = GameManager.GetChestManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.BELT:            script = GameManager.GetBeltManager().GetChoicePrefab(weight);          break;
                case BLOCKTYPE.BELT_UP:         script = GameManager.GetBeltUpManager().GetChoicePrefab(weight);        break;
                case BLOCKTYPE.BELT_DOWN:       script = GameManager.GetBeltDownManager().GetChoicePrefab(weight);      break;
                case BLOCKTYPE.SPLITER:         script = GameManager.GetSpliterManager().GetChoicePrefab(weight);       break;
                case BLOCKTYPE.INSERTER:        script = GameManager.GetInserterManager().GetChoicePrefab(weight);      break;
                case BLOCKTYPE.DRILL:           script = GameManager.GetDrillManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.MACHINE:         script = GameManager.GetMachineManager().GetChoicePrefab(weight);       break;
                case BLOCKTYPE.STONE_FURNACE:   script = GameManager.GetStoneFurnaceManager().GetChoicePrefab(weight);  break;
             }
            return script;
        }

        //선택된 prefab을 x,z 위치할때 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
        public void ChainBlock(int posx, int posy, int posz, BlockScript prefab)
        {
            if (null == prefab) return;

            //위치갱신:먼저 위치를 잡아줘야 ChainBeltPrefab()를 수행할 수 있습니다.
            prefab.SetPos(posx, posy, posz);
            ChainBlock(prefab);
        }
        public void ChainBlock(BlockScript prefab)
        {
            if (null == prefab) return;

            BeltScript newscript = null;
            switch (prefab._itembase.type)
            {
                case BLOCKTYPE.BELT:
                    newscript = GameManager.GetBeltManager().ChainBeltPrefab((BeltScript)prefab);
                    break;
                    //case BLOCKTYPE.GROUND_BELT:
                    //    newscript = GameManager.GetSpliterManager().ChainBeltPrefab((BeltScript)prefab);
                    //    break;
            }
            if (null != newscript)
            {
                newscript.SetPos(prefab.transform.position);    //위치갱신 : prefab이 변경된 경우 다시 위치 설정
                this.SetChoicePrefab(newscript);
            }
        }

        public BlockScript CreateBlock(TerrainLayer layer, int x, int y, int z, BlockScript prefab)
        {
            if (null == prefab) return null;

            //이미 점유중
            //if (null != GetBlock(x, y, z)) return null;
            BlockScript front_script = layer.GetBlock(x, y, z, prefab);
            if(null != front_script)
            {
                //if(BLOCKTYPE.MINERAL != front_script._blocktype)
                //    return null;

                //HG_TODO : mineral이면 choiced_item, 퀵슬롯, 인벤에 주워 넣습니다.
                //..

                return null;
            }

            //clone
            BlockScript script = prefab.Clone();
            if (null == script) return null;

            script._itembase = prefab._itembase;
            //terrain에 위치시키다.
            script.SetPos(x, y, z);
            layer.AddBlock(script);

            if(null != script.manager)
                script.manager.CreateBlock(script);
            return script;
        }


        //removeblock:terrain에서 빼지 여부 판단.(block교체시에는 미리 빼기에 예외가 필요합니다)
        public void DeleteBlock(BlockScript script, bool removeblock)
        {
            if (null == script || null == script._itembase) return;
            if (BLOCKTYPE.NATURAL_RESOURCE == script._itembase.type)
                return;

            ////target : picking 된 obj가 target의 일부인 경우에 target을 찾기 위해 GetBodyObject()를 호출합니다.
            //GameObject target = GetBlock(obj).GetBodyObject();
            //BlockScript script = GetBlock(target);
            if (true == removeblock)
                this.block_layer.SubBlock(script);

            switch(script._itembase.type)
            {
                case BLOCKTYPE.BELT:
                //case BLOCKTYPE.GROUND_BELT:
                    GameManager.GetBeltManager().DeleteBlock(script);
                    break;
                case BLOCKTYPE.BELT_UP:
                    GameManager.GetBeltUpManager().DeleteBlock(script);
                    break;
                case BLOCKTYPE.BELT_DOWN:
                    GameManager.GetBeltDownManager().DeleteBlock(script);
                    break;

                case BLOCKTYPE.SPLITER:
                    GameManager.GetSpliterManager().DeleteBlock(script);
                    break;

                default:
                    script.DeleteBlock();
                    break;
            }

            //인벤에 다시 넣어줍니다.
            GameManager.AddItem(script._itembase.id, 1);


            //OnTriggerExit()이 호출되도록 이동시켜준다.(Exit()이 발생하기 전에 삭제되는듯 하다.)
            //   그리하여, Exit()에서 삭제하도록 한다.
            switch (script._itembase.type)
            {
                case BLOCKTYPE.BELT:      //HG_TODO: 지금은 작동하니깐. 추후에 변경을 고려
                case BLOCKTYPE.BELT_UP:
                case BLOCKTYPE.BELT_DOWN:
                case BLOCKTYPE.SPLITER:  //HG_TODO: 지금은 작동하니깐. 추후에 변경을 고려
                    {
                        script.ForceMove(); //OnTriggerExit()을 위해 위치를 강제로 이동시킨다.
                        script._destory = true; //OnTriggerExit()이후에 삭제할 개체들
                    }
                    break;

                default:
                    //HG_TODO : 삭제하지 않고 pool에서 관리하도록 바꿔야 합니다.(crash발생)
                    GameObject.Destroy(script.gameObject);
                    break;
            }
        }
        //removeblock:terrain에서 뺄지 여부 판단.(block교체시에는 미리 빼기에 예외가 필요합니다)
        public void DeleteBlock(GameObject obj, bool removeblock)
        {
            this.DeleteBlock(this.block_layer.GetBlock(obj), removeblock);
        }

        //public BlockScript CreateMineral(int x, int y, int z, BlockScript prefab)
        //{
        //    if (null == prefab)// || BLOCKTYPE.MINERAL != prefab.blocktype)
        //        return null;

        //    //이미 점유중
        //    //if (null != GetBlock(x, y, z)) return null;
        //    BlockScript front_script = this.mineral_layer.GetBlock(x, y, z, prefab);
        //    if (null != front_script)
        //        return null;

        //    //clone
        //    BlockScript script = prefab.Clone();
        //    if (null == script) return null;

        //    //terrain에 위치시키다.
        //    script.SetPos(x, y, z);
        //    this.mineral_layer.AddBlock(script);

        //    if (null != script.manager)
        //        script.manager.CreateBlock(script);
        //    return script;
        //}
        public virtual void Save(BinaryWriter writer)
        {
            Dictionary<int, BlockData> blocks = this.block_layer.GetBlockList();

            //block count
            writer.Write(blocks.Count);
            //block info
            //for(int i=0; i<blocks.Count; ++i)
            foreach(var tmp_block in blocks)
                tmp_block.Value.Save(writer);
        }

        public virtual void Load(BinaryReader reader)
        {

            //block count
            int blockcount = reader.ReadInt32();
            for(int i=0; i<blockcount; ++i)
            {
                BlockData data = new BlockData();
                data.Load(reader);
            }

        }

    }//..class TerrainManager


    public class BlockData
    {
        public int posx, posy, posz;
        public BlockScript block;

        public BlockData() { }
        public BlockData(int x, int y, int z, BlockScript script)
        {
            this.posx = x; this.posy = y; this.posz = z;
            this.block = script;
        }

        public void Save(BinaryWriter writer)
        {
            //position
            writer.Write(this.posx);
            writer.Write(this.posy);
            writer.Write(this.posz);
            //itemid
            writer.Write((short)this.block._itembase.id);
            //angle
            writer.Write(this.block.transform.eulerAngles.y);

            //data
            block.Save(writer);
        }
        public void Load(BinaryReader reader)
        {
            //position
            this.posx = reader.ReadInt32();
            this.posy = reader.ReadInt32();
            this.posz = reader.ReadInt32();
            //itemid
            short itemid = reader.ReadInt16();
            //angle
            float angley = reader.ReadSingle();

            ItemBase itembase = GameManager.GetItemBase().FetchItemByID(itemid);
            if (null == itembase)
            {
                Debug.LogError($"not found loaded itemid: {itemid}");
                return;
            }

            byte turn_weight = (byte)TURN_WEIGHT.FRONT;
            if (BLOCKTYPE.BELT == (BLOCKTYPE)itembase.type)
            {
                turn_weight = reader.ReadByte();
                //Debug.Log("read turn_weight:" + turn_weight);
            }

            BlockScript prefab = GameManager.GetTerrainManager().GetBlockPrefab(itembase.type, (TURN_WEIGHT)turn_weight);
            if (null == prefab) return;
            prefab._itembase = itembase;

            //angle
            prefab.transform.eulerAngles = new Vector3(0f, angley, 0f);

            this.block = GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, prefab);
            if (null == this.block)
            {
                Debug.LogError($"fail load: itemid[{itemid}], pos:[{posx},{posy},{posz}]");
                return;
            }

            this.block._itembase = itembase;    //GetTerrainManager().CreateBlock()에서 처리되는거 같다.
            this.block.Load(reader);
        }
    }
}//..namespace MyCraft