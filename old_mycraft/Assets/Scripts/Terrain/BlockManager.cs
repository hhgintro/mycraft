using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BlockManager : MonoBehaviour
    {
        //protected TerrainManager terrain_manager;

        //HG_TEST : 테스트용으로 public으로 선언함.
        protected List<BlockScript> prefabs = new List<BlockScript>();


        protected void LoadPrefab(string path, short itemid, Transform parent)
        {
            BlockScript block = Managers.Resource.Instantiate(path, parent).GetComponent<BlockScript>();
            block._itembase = GameManager.GetItemBase().FetchItemByID(itemid);
            if(null == block._itembase) Debug.LogError($"Fail: not found itemid {itemid}");
            block.manager = this;
            block.GetComponent<Collider>().enabled = false;
            block.SetActive(false);
            block.SetMeshRender(0.3f);
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

        //센서:block간의 연결상태가 변경되면, 외형이 바뀔수 있다.
        public virtual void LinkedSensor(BlockScript block) { }
        //자신의 front(script)가 (외형)변경되어져야 하는지 체크합니다.
        public virtual BlockScript ChainBlock(BlockScript block) {
            return null;
        }
        public virtual void ChainBelt(Sensor self)
        {

        }

        public virtual void OnAutomaticConnectBelt(Vector3 belt_hold_start, Vector3 point, BlockScript prefab) { }
        public virtual void CreateBlock(BlockScript block)
        {
            if (null == block) return;
            block.manager = this;
            block._bOnTerrain = true;
            block.SetMeshRender(1.0f);
        }

        public virtual void DeleteBlock(BlockScript block)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            //if (null == script || false == script.IsBelt())
            //    return;


            block.DeleteBlock();
        }


    }//..class BeltManager
}//..namespace MyCraft