using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BlockManager : MonoBehaviour
    {
        //protected TerrainManager terrain_manager;

        //HG_TEST : 테스트용으로 public으로 선언함.
        protected List<BlockScript> prefabs;
        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //관리대상목록(belt에서 이동하고 있는 물품을 관리합니다.

        protected virtual void Start()
        {
            this.prefabs = new List<BlockScript>();
        }

        //public BlockScript GetChoicePrefab()
        //{
        //    if (this.prefabs_belt.Count <= 0)
        //        return null;
        //    return this.prefabs_belt[0];
        //}

        public virtual BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        {
            if (this.prefabs.Count <= 0)
                return null;

            BlockScript prefab = this.prefabs[0];
            if (null == prefab) return null;
            prefab.GetComponent<Collider>().enabled = false;
            return prefab;
        }

        public virtual void CreateBlock(BlockScript script)
        {
            if (null == script) return;
            script.manager = this;
            script._bOnTerrain = true;
        }

        public virtual void DeleteBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == script || false == script.IsBelt())
                return;


            script.DeleteBlock();
        }


    }//..class BeltManager
}//..namespace MyCraft