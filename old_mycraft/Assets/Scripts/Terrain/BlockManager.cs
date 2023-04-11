using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BlockManager : MonoBehaviour
    {
        //protected TerrainManager terrain_manager;

        //HG_TEST : �׽�Ʈ������ public���� ������.
        protected List<BlockScript> prefabs = new List<BlockScript>();
        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //���������(belt���� �̵��ϰ� �ִ� ��ǰ�� �����մϴ�.


        protected void LoadPrefab(string path, Transform parent)
        {
            BlockScript block = Managers.Resource.Instantiate(path, parent).GetComponent<BlockScript>();
            block.manager = this;
            block.SetActive(false);
            this.prefabs.Add(block);
        }

        public virtual BlockScript GetChoicePrefab(TURN_WEIGHT weight=TURN_WEIGHT.FRONT)
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