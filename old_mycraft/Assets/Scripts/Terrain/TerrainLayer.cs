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
            if (null == prefab)
            {
                Debug.LogError("Fail: prefab is null");
                return null;
            }


            Vector3 scale = Quaternion.Euler(prefab.transform.eulerAngles) * prefab.transform.localScale;
            for (int X = Common.PosRounding(Mathf.Min(0, scale.x+1)); X < Common.PosRounding(Mathf.Max(1, scale.x)); ++X)
            {
                for (int Y = Common.PosRounding(Mathf.Min(0, scale.y+1)); Y < Common.PosRounding(Mathf.Max(1, scale.y)); ++Y)
                {
                    for (int Z = Common.PosRounding(Mathf.Min(0, scale.z+1)); Z < Common.PosRounding(Mathf.Max(1, scale.z)); ++Z)
                    {
                        BlockScript block = this.GetBlock(x + X, y + Y, z + Z);
                        if (null != block) return block;

                    }
                }
            }
            return null;    //빈공간이다.
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
        public void SetBlock(BlockScript block, BlockScript obj)
        {
            if (null == block) return;

            int x = Common.PosRounding(block.transform.position.x);
            int y = Common.PosRounding(block.transform.position.y);
            int z = Common.PosRounding(block.transform.position.z);

            Vector3 scale = Quaternion.Euler(block.transform.eulerAngles) * block.transform.localScale;
            for (int X = Common.PosRounding(Mathf.Min(0, scale.x+1)); X < Common.PosRounding(Mathf.Max(1, scale.x)); ++X)
            {
                for (int Y = Common.PosRounding(Mathf.Min(0, scale.y+1)); Y < Common.PosRounding(Mathf.Max(1, scale.y)); ++Y)
                {
                    for (int Z = Common.PosRounding(Mathf.Min(0, scale.z+1)); Z < Common.PosRounding(Mathf.Max(1, scale.z)); ++Z)
                        SetSizeBlock(x+X, y+Y, z+Z, obj);
                }
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
                //Debug.Log($"--- sub block : [{x},{y},{z}]");
            }
            else
            {//등록
                if (true == tmp_y.ContainsKey(y))
                {
                    Debug.Log("error exists tile object");
                    return;
                }
                tmp_y.Add(y, obj);
                //Debug.Log($"+++ add block : [{x},{y},{z}]");
            }
        }


        public Dictionary<int, BlockData> GetBlockList()
        {
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

                        blocks.Add(script._index, new BlockData(x, y, z, script));
                    }
                }
            }
            return blocks;
        }


    }
}//..namespace MyCraft