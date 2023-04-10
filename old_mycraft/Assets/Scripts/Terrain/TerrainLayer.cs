using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    //***** 매우 중요함 *****//
    //x검색 -> z검색 -> y검색
    //높이값을 확인하기 위해서 y검색을 마지막에 진행합니다.
    using BLOCK_Y = Dictionary<int/*y*/, BlockScript>;
    using BLOCK_ZY = Dictionary<int/*z*/, Dictionary<int, BlockScript>>;
    using BLOCK_XYZ = Dictionary<int/*x*/, Dictionary<int/*z*/, Dictionary<int, BlockScript>>>;

    public class TerrainLayer// : MonoBehaviour
    {
        //block 3차원 배열
        private BLOCK_XYZ block_xyz;// = new BLOCK_XYZ();

        public TerrainLayer()
        {
            this.block_xyz = new BLOCK_XYZ();
        }
        //// Use this for initialization
        //void Start()
        //{
        //    //block_xyz = new BLOCK_XYZ();
        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}

        public BlockScript GetBlock(int x, int y, int z, BlockScript prefab)
        {
            if (null == prefab) return null;

            switch (prefab._blocksize)
            {
                case 1://1칸
                    //현위치
                    return this.GetBlock(x, y, z);

                case 2://4칸
                    {
                        //현위치
                        BlockScript script = this.GetBlock(x, y, z);
                        if (null != script) return script;
                        //front
                        script = this.GetBlock(x, y, z + 1);// prefab.transform.position + prefab.transform.forward);
                        if (null != script) return script;
                        //right
                        script = this.GetBlock(x + 1, y, z);// prefab.transform.position + prefab.transform.right);
                        if (null != script) return script;
                        //front-right
                        script = this.GetBlock(x + 1, y, z + 1);// prefab.transform.position + prefab.transform.forward + prefab.transform.right);
                        if (null != script) return script;
                    } return null;

                case 3://9칸
                    {
                        for (int col = 0; col < prefab._blocksize; ++col)
                        {
                            for (int row = 0; row < prefab._blocksize; ++row)
                            {
                                BlockScript script = this.GetBlock(x - 1 + col, y, z - 1 + row);
                                if (null != script) return script;
                            }
                        }

                        ////front-left
                        //BlockScript script = this.GetBlock(prefab.transform.position + prefab.transform.forward - prefab.transform.right);
                        //if (null != script) return script;
                        ////front
                        //script = this.GetBlock(prefab.transform.position + prefab.transform.forward);
                        //if (null != script) return script;
                        ////front-right
                        //script = this.GetBlock(prefab.transform.position + prefab.transform.forward + prefab.transform.right);
                        //if (null != script) return script;
                        ////left
                        //script = this.GetBlock(prefab.transform.position - prefab.transform.right);
                        //if (null != script) return script;
                        ////현위치
                        //script = this.GetBlock(x, y, z);
                        //if (null != script) return script;
                        ////right
                        //script = this.GetBlock(prefab.transform.position + prefab.transform.right);
                        //if (null != script) return script;
                        ////back-left
                        //script = this.GetBlock(prefab.transform.position - prefab.transform.forward - prefab.transform.right);
                        //if (null != script) return script;
                        ////back
                        //script = this.GetBlock(prefab.transform.position - prefab.transform.forward);
                        //if (null != script) return script;
                        ////back-right
                        //script = this.GetBlock(prefab.transform.position - prefab.transform.forward + prefab.transform.right);
                        //if (null != script) return script;

                    } return null;

                //4칸은 구지 만들지 마세요...(차라리 5칸으로....)
                case 4://16칸
                    return null;

                case 5://25칸
                    {
                        for (int col = 0; col < prefab._blocksize; ++col)
                        {
                            for (int row = 0; row < prefab._blocksize; ++row)
                            {
                                BlockScript script = this.GetBlock(x - 2 + col, y, z - 2 + row);
                                if (null != script) return script;
                            }
                        }
                    } return null;

                //그 이상은 지원하지 않습니다.
                default: return prefab;
            }
        }
        public BlockScript GetBlock(Vector3 pos)
        {
            return GetBlock(Common.PosRounding(pos.x), Common.PosRounding(pos.y), Common.PosRounding(pos.z));
        }
        public BlockScript GetBlock(GameObject obj)
        {
            if (null == obj) return null;
            return obj.GetComponent<BlockScript>();
        }
        public BlockScript GetBlock(int x, int y, int z)
        {
            if (null == this.block_xyz)
                return null;
            //x 검색
            if (false == this.block_xyz.ContainsKey(x))
                return null;
            BLOCK_ZY tmp_zy = this.block_xyz[x];

            //z 검색
            if (false == tmp_zy.ContainsKey(z))
                return null;
            BLOCK_Y tmp_y = tmp_zy[z];

            //y 검색
            if (false == tmp_y.ContainsKey(y))
                return null;

            //Debug.Log("GetBlock: " + x + "," + y + "," + z);
            return tmp_y[y];
        }

        //terrain에 obj를 위치시키다.
        public void AddBlock(BlockScript script)
        {
            SetBlock(script, script);
        }
        public void SubBlock(BlockScript script)
        {
            SetBlock(script, null);
        }

        //script : 체크용
        //obj : 등록할 개체(null일 수도 있다)
        public void SetBlock(BlockScript script, BlockScript obj)
        {
            if (null == script) return;

            int x = Common.PosRounding(script.transform.position.x);
            int y = Common.PosRounding(script.transform.position.y);
            int z = Common.PosRounding(script.transform.position.z);

            switch (script._blocksize)
            {
                case 1://1칸
                    //현위치
                    SetSizeBlock(x, y, z, obj);
                    break;

                case 2://4칸
                    {
                        //현위치
                        SetSizeBlock(x, y, z, obj);
                        //front
                        SetSizeBlock(x, y, z + 1, obj);// script.transform.position + script.transform.forward, obj);
                        //right
                        SetSizeBlock(x + 1, y, z, obj);// script.transform.position + script.transform.right, obj);
                        //front-right
                        SetSizeBlock(x + 1, y, z + 1, obj);// script.transform.position + script.transform.forward + script.transform.right, obj);
                    } break;

                case 3://9칸
                    {
                        for(int col=0; col<script._blocksize; ++col)
                            for(int row=0; row<script._blocksize; ++row)
                                SetSizeBlock(x-1+col, y, z-1+row, obj);


                        ////front-left
                        //SetSizeBlock(x - 1, y, z + 1, obj);
                        ////front
                        //SetSizeBlock(x, y, z + 1, obj);
                        ////front-right
                        //SetSizeBlock(x + 1, y, z + 1, obj);
                        ////left
                        //SetSizeBlock(x - 1, y, z, obj);
                        ////현위치
                        //SetSizeBlock(x, y, z, obj);
                        ////right
                        //SetSizeBlock(x + 1, y, z, obj);
                        ////back-left
                        //SetSizeBlock(x - 1, y, z - 1, obj);
                        ////back
                        //SetSizeBlock(x, y, z, obj);
                        ////back-right
                        //SetSizeBlock(x + 1, y, z - 1, obj);

                    } break;

                //4칸은 구지 만들지 마세요...(차라리 5칸으로....)
                case 4://16칸
                    break;

                case 5://25칸
                    for (int col = 0; col < script._blocksize; ++col)
                        for (int row = 0; row < script._blocksize; ++row)
                            SetSizeBlock(x - 2 + col, y, z - 2 + row, obj);
                    break;

                //그 이상은 지원하지 않습니다.
                default: break;
            }

        }
        public void SetSizeBlock(Vector3 pos, BlockScript obj)
        {
            SetSizeBlock(Common.PosRounding(pos.x), Common.PosRounding(pos.y), Common.PosRounding(pos.z), obj);
        }

        public void SetSizeBlock(int x, int y, int z, BlockScript obj)
        {
            //public Dictionary<int/*z*/, CTile> block_z;
            //public Dictionary<int/*y*/, Dictionary<int, CTile>> block_yz;
            //public Dictionary<int/*x*/, Dictionary<int/*y*/, Dictionary<int, CTile>>> block_xyz;

            //x 검색
            BLOCK_ZY tmp_zy;
            if (false == this.block_xyz.ContainsKey(x))
            {
                tmp_zy = new BLOCK_ZY();
                this.block_xyz.Add(x, tmp_zy);
            }
            else
                tmp_zy = this.block_xyz[x];

            //z 검색
            BLOCK_Y tmp_y;
            if (false == tmp_zy.ContainsKey(z))
            {
                tmp_y = new BLOCK_Y();
                tmp_zy.Add(z, tmp_y);
            }
            else
                tmp_y = tmp_zy[z];

            //y 검색
            if (null == obj)
            {//삭제
                if (false == tmp_y.ContainsKey(y))
                {
                    Debug.Log("error not found tile object");
                    return;
                }
                tmp_y.Remove(y);
                //Debug.Log("--- sub block : [" + x + "," + y + "," + z + "]");
            }
            else
            {//등록
                if (true == tmp_y.ContainsKey(y))
                {
                    Debug.Log("error exists tile object");
                    return;
                }
                tmp_y.Add(y, obj);

                //if(BLOCKTYPE.NONE != obj._blocktype
                //    && obj._blocktype < BLOCKTYPE.RAW_WOOD)
                //Debug.Log("+++ add block : [" + x + "," + y + "," + z + "]");
            }
        }


        public Dictionary<int, BlockData> GetBlockList()
        {
            //List<BlockData> blocks = new List<BlockData>();
            Dictionary<int, BlockData> blocks = new Dictionary<int, BlockData>();

            //x 검색
            foreach (var tmp_zy in this.block_xyz)
            {
                //z 검색
                foreach (var tmp_y in tmp_zy.Value)
                {
                    //y 검색
                    foreach (var tmp in tmp_y.Value)
                    {
                        BlockScript script = tmp.Value;
                        if (null == script) continue;

                        //이미포함된 블럭은 제외
                        if (true == blocks.ContainsKey(script._index))
                            continue;

                        int x = Common.PosRounding(script.transform.position.x);
                        int y = Common.PosRounding(script.transform.position.y);
                        int z = Common.PosRounding(script.transform.position.z);

                        //Debug.Log("id:" + script._id + " " + x + "/" + y + "/" + z + ":"
                        //    + "blocktype/" + script._blocktype.ToString());

                        //이미포함된 블럭은 제외
                        //if (false == blocks.ContainsKey(script._id))
                        //blocks.Add(script._id, new BlockData(tmp_zy.Key, tmp.Key, tmp_y.Key, script));
                        blocks.Add(script._index, new BlockData(x, y, z, script));
                    }
                }
            }
            return blocks;
        }


    }
}//..namespace MyCraft