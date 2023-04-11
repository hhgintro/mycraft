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
            LoadPrefab<BeltManager>("hierarchy/terrain", "belt");
            LoadPrefab<InserterManager>("hierarchy/terrain", "inserter");
            LoadPrefab<ChestManager>("hierarchy/terrain", "chest");
            LoadPrefab<DrillManager>("hierarchy/terrain", "drill");
            LoadPrefab<MachineManager>("hierarchy/terrain", "machine");
            LoadPrefab<StoneFurnaceManager>("hierarchy/terrain", "stone-furnace");
            LoadPrefab<StoneManager>("hierarchy/terrain", "stone");
            LoadPrefab<TreeManager>("hierarchy/terrain", "tree");
            LoadPrefab<IronManager>("hierarchy/terrain", "iron-ore");


            int x = -2, z = 4;
            //coal

            //copper-ore

            PutdownBlock(1001, 3, x, z -= 2, GameManager.GetTreeManager().GetChoicePrefab());       //tree
            PutdownBlock(1002, 3, x, z -= 2, GameManager.GetIronManager().GetChoicePrefab());       //iron-ore
            PutdownBlock(1003, 2, x, z -= 2, GameManager.GetStoneManager().GetChoicePrefab());      //stone
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
            }
        }


        public TerrainLayer GetBlockLayer() { return block_layer; }
        public TerrainLayer GetMineralLayer() { return mineral_layer; }


        public BlockScript GetChoicePrefab() { return this.choiced_prefab; }
        public void SetChoicePrefab(BlockScript script)
        {
            //null로 세팅하기 위해서 여기에서는 null체크를 하지마세요
            //의도하지 않은 null이 입력되는지 반드시 확인하세요
            //(반드시 의도한 null만 허용되어야 합니다.)
            //if (null == script) return;

            if (script == this.choiced_prefab)
                return;
            //old
            if (null != this.choiced_prefab)
                this.choiced_prefab.SetActive(false);
            //new
            this.choiced_prefab = script;
            if (null != this.choiced_prefab)
            {
                this.choiced_prefab.SetActive(true);
                //if(null != this.choiced_prefab.transform)
                //    this.choiced_prefab.transform.SetParent(this.transform);
            }

            //Debug.Log("layer:  " + this.choiced_prefab.gameObject.layer);
        }
        public void SetChoicePrefab(ItemBase itembase)
        {
            BlockScript script = this.GetBlockPrefab(itembase.type, TURN_WEIGHT.FRONT);
            if (null == script) return;
            script._itembase = itembase;
            this.SetChoicePrefab(script);
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
                case BLOCKTYPE.BELT:            script = GameManager.GetBeltManager().GetChoicePrefab(weight);          break;
                case BLOCKTYPE.INSERTER:        script = GameManager.GetInserterManager().GetChoicePrefab(weight);      break;
                case BLOCKTYPE.CHEST:           script = GameManager.GetChestManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.DRILL:           script = GameManager.GetDrillManager().GetChoicePrefab(weight);         break;
                case BLOCKTYPE.MACHINE:         script = GameManager.GetMachineManager().GetChoicePrefab(weight);       break;
                case BLOCKTYPE.STONE_FURNACE:   script = GameManager.GetStoneFurnaceManager().GetChoicePrefab(weight);  break;
                case BLOCKTYPE.GROUND_BELT:     script = GameManager.GetBeltGroundManager().GetChoicePrefab(weight);    break;
             }
            return script;
        }

        //선택된 prefab을 x,z 위치할때 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
        public void ChainBlock(int posx, int posz, BlockScript prefab)
        {
            if (null == prefab) return;

            //위치갱신:먼저 위치를 잡아줘야 ChainBeltPrefab()를 수행할 수 있습니다.
            prefab.SetPos(posx, 0, posz);

            BeltScript newscript = null;
            switch (prefab._itembase.type)
            {
                case BLOCKTYPE.BELT:
                    newscript = GameManager.GetBeltManager().ChainBeltPrefab((BeltScript)prefab);
                    break;
                case BLOCKTYPE.GROUND_BELT:
                    newscript = GameManager.GetBeltGroundManager().ChainBeltPrefab((BeltScript)prefab);
                    break;
            }
            if (null != newscript)
            {
                this.SetChoicePrefab(newscript);
                //위치갱신 : prefab이 변경된 경우 다시 위치 설정
                newscript.SetPos(posx, 0, posz);
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
            if (null == script) return;

            ////target : picking 된 obj가 target의 일부인 경우에 target을 찾기 위해 GetBodyObject()를 호출합니다.
            //GameObject target = GetBlock(obj).GetBodyObject();
            //BlockScript script = GetBlock(target);
            if (true == removeblock)
                this.block_layer.SubBlock(script);

            switch(script._itembase.type)
            {
                case BLOCKTYPE.BELT:
                case BLOCKTYPE.GROUND_BELT:
                    GameManager.GetBeltManager().DeleteBlock(script);
                    break;

                default:
                    script.DeleteBlock();
                    break;
            }

            //인벤에 다시 넣어줍니다.
            GameManager.GetInventory().AddItem(script._itembase.id, 1);

            //HG_TODO : 삭제하지 않고 pool에서 관리하도록 바꿔야 합니다.(crash발생)
            GameObject.Destroy(script.gameObject);
        }
        //removeblock:terrain에서 뺄지 여부 판단.(block교체시에는 미리 빼기에 예외가 필요합니다)
        public void DeleteBlock(GameObject obj, bool removeblock)
        {
            this.DeleteBlock(this.block_layer.GetBlock(obj), removeblock);
        }

        public BlockScript CreateMineral(int x, int y, int z, BlockScript prefab)
        {
            if (null == prefab)// || BLOCKTYPE.MINERAL != prefab.blocktype)
                return null;

            //이미 점유중
            //if (null != GetBlock(x, y, z)) return null;
            BlockScript front_script = this.mineral_layer.GetBlock(x, y, z, prefab);
            if (null != front_script)
                return null;

            //clone
            BlockScript script = prefab.Clone();
            if (null == script) return null;

            //terrain에 위치시키다.
            script.SetPos(x, y, z);
            this.mineral_layer.AddBlock(script);

            if (null != script.manager)
                script.manager.CreateBlock(script);
            return script;
        }
        public virtual void Save(BinaryWriter writer)
        {
            Dictionary<int, BlockData> blocks = this.block_layer.GetBlockList();

            ////x 검색
            //foreach (var tmp_zy in this.block_xyz)
            //{
            //    //z 검색
            //    foreach (var tmp_y in tmp_zy.Value)
            //    {
            //        //y 검색
            //        foreach (var tmp in tmp_y.Value)
            //        {
            //            //Debug.Log(tmp_zy.Key + "/" + tmp_y.Key + "/" + tmp.Key + ":"
            //            //    + "blocktype/" + tmp.Value.blocktype.ToString());
            //            blocks.Add(new BlockData(tmp_zy.Key, tmp.Key, tmp_y.Key, tmp.Value));
            //        }
            //    }
            //}

            //block count
            writer.Write(blocks.Count);
            //block info
            //for(int i=0; i<blocks.Count; ++i)
            foreach(var tmp_block in blocks)
            {
                //sep
                //writer.Write((int)999);

                // blocks[i].Save(writer);
                //position
                writer.Write(tmp_block.Value.posx);
                writer.Write(tmp_block.Value.posy);
                writer.Write(tmp_block.Value.posz);
                //Debug.Log("***write pos:" + tmp_block.posx + "/" + blocks[i].posy + "/" + blocks[i].posz);
                //itemid
                writer.Write((int)tmp_block.Value.script._itembase.id);
                //Debug.Log("write blocktype:" + blocks[i].script.blocktype);
                //angle
                writer.Write(tmp_block.Value.script.transform.eulerAngles.y);
                //Debug.Log("write angle:" + blocks[i].script.transform.eulerAngles.y);
                //data
                tmp_block.Value.script.Save(writer);
            }


        }

        public virtual void Load(BinaryReader reader)
        {
            //block count
            int blockcount = reader.ReadInt32();
            for(int i=0; i<blockcount; ++i)
            {
                //sep
                //int sep = reader.ReadInt32();
                //Debug.Log("sep:" + sep); //999

            //position
            int posx = reader.ReadInt32();
                int posy = reader.ReadInt32();
                int posz = reader.ReadInt32();
                //Debug.Log("***read pos:" + posx + "/" + posy + "/" + posz);

                //itemid
                int itemid = reader.ReadInt32();
                ItemBase itembase = GameManager.GetItemBase().FetchItemByID(itemid);
                if (null == itembase)
                {
                    Debug.LogError("not found loaded itemid " + itemid.ToString());
                    continue;
                }
                //Debug.Log("load: blocktype[" + blocktype + "], pos:[" + posx + "},{" + posy + "},{" + posz + "}]");
                //angle
                float angley = reader.ReadSingle();
                //Debug.Log("read angle:" + angley);
                int turn_weight = (int)TURN_WEIGHT.FRONT;
                if(BLOCKTYPE.BELT == (BLOCKTYPE)itembase.type
                    || BLOCKTYPE.GROUND_BELT == (BLOCKTYPE)itembase.type)
                {
                    turn_weight = reader.ReadInt32();
                    //Debug.Log("read turn_weight:" + turn_weight);
                }

                BlockScript prefab = this.GetBlockPrefab(itembase.type, (TURN_WEIGHT)turn_weight);
                if (null == prefab) continue;
                prefab._itembase = itembase;

                //angle
                prefab.transform.eulerAngles = new Vector3(0f, angley, 0f);

                BlockScript script = this.CreateBlock(this.GetBlockLayer(), posx, posy, posz, prefab);
                if (null == script)
                {
                    Debug.LogError("fail load: itemid[" + itemid
                        + "], pos:[" + posx + "},{" + posy + "},{" + posz + "}]");
                    continue;
                }

                script._itembase = itembase;
                script.Load(reader);
                //float angle = reader.ReadSingle();

            }

        }

        public virtual void Save(FileStream fs)
        {
            //BinaryFormatter bf = new BinaryFormatter();

        }
        public virtual void Load(FileStream fs)
        {
            //BinaryFormatter bf = new BinaryFormatter();

        }

    }//..class TerrainManager


    public class BlockData
    {
        public int posx, posy, posz;
        public BlockScript script;

        public BlockData(int x, int y, int z, BlockScript script)
        {
            this.posx = x; this.posy = y; this.posz = z;
            this.script = script;
        }
    }
}//..namespace MyCraft