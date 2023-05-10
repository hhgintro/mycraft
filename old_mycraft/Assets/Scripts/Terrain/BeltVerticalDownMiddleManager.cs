using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BeltVerticalDownMiddleManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //HG_TEST : 테스트용으로 public으로 선언함.
        //public List<BeltScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //관리대상목록(belt에서 이동하고 있는 물품을 관리합니다.

        void Awake()
        {
            base.LoadPrefab("blocks/transport-belt-vertical-down-middle", 1170, this.transform.GetChild(0));
        }


        public override void CreateBlock(BlockScript script)
        {
            if (null == script) return;
            base.CreateBlock(script);

            ////새로 생성된 script의 back/left/right에서 link를 걸어줍니다.
            //script.LinkedBelt();

            ////생성된 script의 front가 (외형)변경되어져야 하는지 체크합니다.
            //BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward + script.transform.up);
            //if(script_front) script_front.manager.ChainBelt(script_front);
        }

        public override void DeleteBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == script || false == script.IsBelt())
                return;

            //삭제된 script의 front가 (외형)변경되어져야 하는지 체크합니다.
            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward);
            if (script_front) script_front.manager.ChainBlock(script_front);

            //script.DeleteBlock();
            base.DeleteBlock(script);
        }

        //public override BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        //{
        //    if (this.prefabs.Count <= 0)
        //        return null;

        //    BlockScript prefab = null;
        //    switch (weight)
        //    {
        //        case TURN_WEIGHT.FRONT: prefab = this.prefabs[0];  break;
        //        case TURN_WEIGHT.LEFT:  prefab = this.prefabs[1];  break;
        //        case TURN_WEIGHT.RIGHT: prefab = this.prefabs[2];  break;
        //    }
        //    if (null == prefab) return null;
        //    prefab.GetComponent<Collider>().enabled = false;
        //    //prefab.SetMeshRender(0.3f);
        //    return prefab;
        //}


        //자신의 front(script)가 (외형)변경되어져야 하는지 체크합니다.
        public override BlockScript ChainBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsBelt())
                return null;

            ////prefab이 생성되므로 인해
            ////script_front의 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
            ////변경될 prefab을 가져옵니다.
            //BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            //if (null == prefab) return null;


            //if (((BeltScript)prefab).turn_weight != ((BeltScript)script).turn_weight)
            //{
            //    //terrain 위치를 비워준다.
            //    GameManager.GetTerrainManager().block_layer.SubBlock(script);

            //    //new
            //    BlockScript newscript = prefab.Clone();
            //    if (null == newscript) return null;
            //    newscript._itembase = script._itembase;
            //    newscript.manager = null;

            //    //terrain에 위치시키다.
            //    newscript.SetPos(Common.PosRounding(script.transform.position.x)
            //        , Common.PosRounding(script.transform.position.y)
            //        , Common.PosRounding(script.transform.position.z));
            //    GameManager.GetTerrainManager().block_layer.AddBlock(newscript);

            //    script.manager.CreateBlock(newscript);


            //    //this.LinkedBelt(newscript);


            //    //삭제 : 교체바로 전에서 뺴고 처리하기 때문에 삭제시에는 block을 제거하지 않습니다.
            //    GameManager.GetTerrainManager().DeleteBlock(script, false);

            //    //외형이 변경된 경우 script_front의 back/left/right에서 link를 걸어줍니다.
            //    BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(newscript.transform.position + newscript.transform.forward);
            //    if(script_front) script_front.LinkedBelt();

            //    return script_front;
            //}

            //외형이 변경되지 않은 경우
            //script_front와 link는 잡아준다.
            script.LinkedBelt();
            return script;
        }

        ////선택된 prefab을 x,z 위치할때 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
        ////변경될 prefab을 가져옵니다.
        //public BeltScript ChainBeltPrefab(BeltScript script)
        //{
        //    int weight = script.CheckWeightChainBelt();

        //    BeltScript prefab = null;
        //    /*
        //     * back 이 존재하는 경우는 무조건 TURN_FRONT
        //     * left와 right 모두 존재하는 경우는 TURN_FRONT
        //     * left만 존재하는 경우는 TURN_LEFT
        //     * right만 존재하는 경우는 TURN_RIGHT
        //     * */
        //    //weight
        //    //if (0 == weight)
        //    //{
        //    //    prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
        //    //    prefab.transform.forward = script.transform.forward;
        //    //    return prefab;
        //    //}


        //    if (Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT))
        //    {
        //        prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
        //        prefab.transform.forward = script.transform.forward;
        //        return prefab;
        //    }

        //    bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
        //    bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

        //    //left
        //    if (true == turn_left)
        //    {
        //        prefab = (BeltScript)this.prefabs[1];   //TURN_LEFT
        //        prefab._itembase = script._itembase;
        //        prefab.transform.forward = script.transform.forward;
        //        return prefab;
        //    }
        //    //right
        //    if (true == turn_right)
        //    {
        //        prefab = (BeltScript)this.prefabs[2];   //TURN_RIGHT
        //        prefab._itembase = script._itembase;
        //        prefab.transform.forward = script.transform.forward;
        //        return prefab;
        //    }

        //    //예외의 상황이면...
        //    if (null == prefab)
        //        Debug.LogError("critical not found prefab");

        //    return prefab;
        //}


    }//..class BeltManager
}//..namespace MyCraft