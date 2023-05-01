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
            LoadPrefab<BeltSlopeUpManager>("hierarchy/terrain", "belt-slide-up");
            LoadPrefab<BeltSlopeDownManager>("hierarchy/terrain", "belt-slide-down");
            LoadPrefab<BeltVerticalUpBeginManager>("hierarchy/terrain", "belt-vertical-up-begin");
            LoadPrefab<BeltVerticalUpMiddleManager>("hierarchy/terrain", "belt-vertical-up-middle");
            LoadPrefab<BeltVerticalUpEndManager>("hierarchy/terrain", "belt-vertical-up-end");
            LoadPrefab<BeltVerticalDownEndManager>("hierarchy/terrain", "belt-vertical-down-begin");
            LoadPrefab<BeltVerticalDownBeginManager>("hierarchy/terrain", "belt-vertical-down-middle");
            LoadPrefab<BeltVerticalDownMiddleManager>("hierarchy/terrain", "belt-vertical-down-end");
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

            int x = -4, y = 0, z = 9;

            PutdownBlock(30, 2, x, y, z -= 3);  //coal
            PutdownBlock(40, 3, x, y, z -= 3);  //iron-ore
            PutdownBlock(50, 3, x, y, z -= 3);  //copper-ore
            PutdownBlock(10, 3, x, y, z -= 3);  //tree
            PutdownBlock(20, 2, x, y, z -= 3);  //stone
            PutdownBlock(30, 2, x, y, z -= 3);  //coal
            PutdownBlock(60, 2, x, y, z -= 3);  //crude-old
        }

        void LoadPrefab<T>(string path, string name) where T : BlockManager
        {
            GameObject go = Managers.Resource.Instantiate(path, this.transform);
            go.name = name;
            go.AddComponent<T>();
        }

        public void PutdownBlock(short itemid, int count, int x, int y, int z)
        {
            BlockScript prefab = null;
            switch (itemid)
            {
                case 10:    prefab = GameManager.GetTreeManager().GetChoicePrefab();        break;
                case 20:    prefab = GameManager.GetStoneManager().GetChoicePrefab();       break;
                case 30:    prefab = GameManager.GetCoalManager().GetChoicePrefab();        break;
                case 40:    prefab = GameManager.GetIronManager().GetChoicePrefab();        break;
                case 50:    prefab = GameManager.GetCopperManager().GetChoicePrefab();      break;
                case 60:    prefab = GameManager.GetCrudeOilManager().GetChoicePrefab();    break;
            }
            if(null == prefab)
            {
                Debug.LogError($"Fail: not found natural-mineral({itemid})");
                return;
            }

            for (int i = 0; i < count; ++i)
            {
                CreateBlock(this.GetMineralLayer(), x-i, y, z, prefab);
                //block._itembase = GameManager.GetItemBase().FetchItemByID(itemid);//BLOCKTYPE.RAW_WOOD
                //if (null == block._itembase)
                //    Debug.LogError($"Fail: not found itemid {itemid}");
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
                this.choiced_prefab.SetActive(false);

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
        }
        public BlockScript SetChoicePrefab(ItemBase itembase)
        {
            BlockScript block = this.GetBlockPrefab(itembase.type, TURN_WEIGHT.FRONT);
            if (null == block) return null;
            block._itembase = itembase;
            this.SetChoicePrefab(block);
            return block;
        }
        public BlockScript GetBlockPrefab(BLOCKTYPE blocktype, TURN_WEIGHT weight)
        {
            BlockScript script = null;
            switch (blocktype)
            {
                case BLOCKTYPE.CHEST:           script = GameManager.GetChestManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.INSERTER:        script = GameManager.GetInserterManager().GetChoicePrefab(weight);      break;
                case BLOCKTYPE.DRILL:           script = GameManager.GetDrillManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.MACHINE:         script = GameManager.GetMachineManager().GetChoicePrefab(weight);       break;

                case BLOCKTYPE.BELT:            script = GameManager.GetBeltManager().GetChoicePrefab(weight);              break;
                case BLOCKTYPE.BELT_UP:         script = GameManager.GetBeltSlopeUpManager().GetChoicePrefab(weight);       break;
                case BLOCKTYPE.BELT_DOWN:       script = GameManager.GetBeltSlopeDownManager().GetChoicePrefab(weight);     break;
                case BLOCKTYPE.BELT_VERTICAL_UP_BEGIN:      script = GameManager.GetBeltVerticalUpBeginManager().GetChoicePrefab(weight);       break;
                case BLOCKTYPE.BELT_VERTICAL_UP_MIDDLE:     script = GameManager.GetBeltVerticalUpMiddleManager().GetChoicePrefab(weight);      break;
                case BLOCKTYPE.BELT_VERTICAL_UP_END:        script = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.BELT_VERTICAL_DOWN_BEGIN:    script = GameManager.GetBeltVerticalDownBeginManager().GetChoicePrefab(weight);     break;
                case BLOCKTYPE.BELT_VERTICAL_DOWN_MIDDLE:   script = GameManager.GetBeltVerticalDownMiddleManager().GetChoicePrefab(weight);    break;
                case BLOCKTYPE.BELT_VERTICAL_DOWN_END:      script = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab(weight);       break;

                case BLOCKTYPE.SPLITER:         script = GameManager.GetSpliterManager().GetChoicePrefab(weight);       break;

                case BLOCKTYPE.STONE_FURNACE:   script = GameManager.GetStoneFurnaceManager().GetChoicePrefab(weight);  break;
             }
            return script;
        }

        public BlockScript CreateBlock(TerrainLayer layer, int x, int y, int z, BlockScript prefab)
        {
            if (null == prefab) return null;

            //이미 점유중
            //if (null != GetBlock(x, y, z)) return null;
            BlockScript front_block = layer.GetBlock(x, y, z, prefab);
            if(null != front_block)
            {
                //if(BLOCKTYPE.MINERAL != front_script._blocktype)
                //    return null;

                //HG_TODO : mineral이면 choiced_item, 퀵슬롯, 인벤에 주워 넣습니다.
                //..

                return null;
            }

            //clone
            BlockScript block = prefab.Clone();
            if (null == block) return null;

            block._itembase = prefab._itembase;
            //terrain에 위치시키다.
            block.SetPos(x, y, z);
            layer.AddBlock(block);

            if(null != block.manager)
                block.manager.CreateBlock(block);
            return block;
        }


        //removeblock:terrain에서 빼지 여부 판단.(block교체시에는 미리 빼기에 예외가 필요합니다)
        public void DeleteBlock(BlockScript block, bool removeblock)
        {
            if (null == block || null == block._itembase) return;
            if (BLOCKTYPE.NATURAL_RESOURCE == block._itembase.type)
                return;

            ////target : picking 된 obj가 target의 일부인 경우에 target을 찾기 위해 GetBodyObject()를 호출합니다.
            //GameObject target = GetBlock(obj).GetBodyObject();
            //BlockScript script = GetBlock(target);
            if (true == removeblock)
                this.block_layer.SubBlock(block);

            block.manager.DeleteBlock(block);

            //인벤에 다시 넣어줍니다.
            GameManager.AddItem(block._itembase.id, 1);


            //OnTriggerExit()이 호출되도록 이동시켜준다.(Exit()이 발생하기 전에 삭제되는듯 하다.)
            //   그리하여, Exit()에서 삭제하도록 한다.
            switch (block._itembase.type)
            {
                case BLOCKTYPE.BELT:      //HG_TODO: 지금은 작동하니깐. 추후에 변경을 고려
                case BLOCKTYPE.BELT_UP:
                case BLOCKTYPE.BELT_DOWN:
                case BLOCKTYPE.BELT_VERTICAL_UP_BEGIN:
                case BLOCKTYPE.BELT_VERTICAL_UP_MIDDLE:
                case BLOCKTYPE.BELT_VERTICAL_UP_END:
                case BLOCKTYPE.BELT_VERTICAL_DOWN_BEGIN:
                case BLOCKTYPE.BELT_VERTICAL_DOWN_MIDDLE:
                case BLOCKTYPE.BELT_VERTICAL_DOWN_END:
                case BLOCKTYPE.SPLITER:  //HG_TODO: 지금은 작동하니깐. 추후에 변경을 고려
                    {
                        ////[자신]이 삭제될때 front의 외형이 바뀔 수 있다면 ForceMove()처리로 삭제되도록 합니다.
                        //if (script._lf || script._rf || script._L || script._R || script._lb || script._rb)
                        //{
                        //    script.ForceMove(true); //OnTriggerExit()을 위해 위치를 강제로 이동시킨다.
                        //    script._destory = true;
                        //}
                        //else
                        //    GameObject.Destroy(script.gameObject);
                        if(false == block.ForceMove())
                            GameObject.Destroy(block.gameObject);
                    }
                    break;

                default:
                    //HG_TODO : 삭제하지 않고 pool에서 관리하도록 바꿔야 합니다.(crash발생)
                    GameObject.Destroy(block.gameObject);
                    break;
            }
        }
        //removeblock:terrain에서 뺄지 여부 판단.(block교체시에는 미리 빼기에 예외가 필요합니다)
        public void DeleteBlock(GameObject obj, bool removeblock)
        {
            this.DeleteBlock(this.block_layer.GetBlock(obj), removeblock);
        }

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