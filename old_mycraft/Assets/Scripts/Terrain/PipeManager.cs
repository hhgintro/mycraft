using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class PipeManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //HG_TEST : 테스트용으로 public으로 선언함.
        //public List<PipeScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //관리대상목록(belt에서 이동하고 있는 물품을 관리합니다.

        void Awake()
        {
            base.LoadPrefab("blocks/Pipe-Line", 1210, this.transform.GetChild(0));
            base.LoadPrefab("blocks/Pipe-Left", 1210, this.transform.GetChild(0));
            base.LoadPrefab("blocks/Pipe-Right", 1210, this.transform.GetChild(0));
            base.LoadPrefab("blocks/Pipe-Quadruple", 1210, this.transform.GetChild(0));
        }


        public override void CreateBlock(BlockScript script)
        {
            if (null == script) return;
            base.CreateBlock(script);

            ////새로 생성된 script의 back/left/right에서 link를 걸어줍니다.
            //script.LinkedBelt();

            ////생성된 script의 front가 (외형)변경되어져야 하는지 체크합니다.
            //BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward);
            //if(script_front) script_front.manager.ChainBelt(script_front);
        }

        public override void DeleteBlock(BlockScript block)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == block || false == block.IsPipe())
                return;

            //삭제된 script의 front가 (외형)변경되어져야 하는지 체크합니다.
            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(block.transform.position + block.transform.forward);
            if (script_front) script_front.manager.ChainBlock(script_front);

            //script.DeleteBlock();
            base.DeleteBlock(block);
        }

        public override BlockScript GetChoicePrefab(TURN_WEIGHT weight = TURN_WEIGHT.FRONT)
        {
            if (this.prefabs.Count <= 0)
                return null;

            BlockScript prefab = null;
            switch (weight)
            {
                case TURN_WEIGHT.FRONT: prefab = this.prefabs[0];  break;
                case TURN_WEIGHT.LEFT:  prefab = this.prefabs[1];  break;
                case TURN_WEIGHT.RIGHT: prefab = this.prefabs[2];  break;
                case TURN_WEIGHT.BOTH:  prefab = this.prefabs[3];  break;

            }
            if (null == prefab) return null;
            prefab.GetComponent<Collider>().enabled = false;
            //prefab.SetMeshRender(0.3f);
            return prefab;
        }


        //센서:block간의 연결상태가 변경되면, 외형이 바뀔수 있다.
        public override void LinkedSensor(BlockScript script)
        {
            //sensor가 작동했으니, 주변의 영향으로 [자신(script)]의 외형이 바뀌는지 확인한다.
            BlockScript prefab = this.ChainBeltPrefab((PipeScript)script);
            if (null == prefab) return;

            //동일개체이면 무시.
            if (script == prefab) return;

            prefab.SetSensor(script);
            Debug.LogWarning($"new prefab: {prefab.name}");

            //HG_TODO: turn_weight을 체크할 필요가 있을지 고민할것.
            //if (((PipeScript)prefab).turn_weight == ((PipeScript)script).turn_weight)
            //  return;


        }

        //자신의 front(script)가 (외형)변경되어져야 하는지 체크합니다.
        public override BlockScript ChainBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsPipe())
                return null;

            //prefab이 생성되므로 인해
            //script_front의 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
            //변경될 prefab을 가져옵니다.
            Vector3 forward = script.transform.forward; //기존방향을 기억했다가
            BlockScript prefab = this.ChainBeltPrefab((PipeScript)script);
            if (null == prefab) return null;

            //prefab.SetSensor(script);

            if (prefab != script)
            {
                //terrain 위치를 비워준다.
                GameManager.GetTerrainManager().block_layer.SubBlock(script);

                //new
                BlockScript newscript = prefab.Clone();
                if (null == newscript) return null;
                newscript._itembase = script._itembase;
                newscript.manager = null;
                newscript.transform.forward = forward;  //기존방향을 재설정합니다.

                //terrain에 위치시키다.
                newscript.SetPos(Common.PosRound(script.transform.position.x)
                    , Common.PosRound(script.transform.position.y)
                    , Common.PosRound(script.transform.position.z));
                GameManager.GetTerrainManager().block_layer.AddBlock(newscript);

                script.manager.CreateBlock(newscript);


                //this.LinkedBelt(newscript);


                //삭제 : 교체바로 전에서 뺴고 처리하기 때문에 삭제시에는 block을 제거하지 않습니다.
                GameManager.GetTerrainManager().DeleteBlock(script, false);

                //외형이 변경된 경우 script_front의 back/left/right에서 link를 걸어줍니다.
                BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(newscript.transform.position + newscript.transform.forward);
                if(script_front) script_front.LinkedBelt();

                return script_front;
            }

            //외형이 변경되지 않은 경우
            //script의 back/left/right에서 link를 걸어줍니다.
            script.LinkedBelt();
            return script;
        }

        //선택된 prefab을 x,z 위치할때 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
        //변경될 prefab을 가져옵니다.
        public PipeScript ChainBeltPrefab(PipeScript script)
        {
            int weight = script.CheckWeightChainBlock();

            PipeScript prefab = null;

            //weight
            if (0 == weight)
            {
                prefab = (PipeScript)this.prefabs[0]; //TURN_FRONT
                return prefab;
            }

            //both
            if (Common.CHECK_BIT(weight, (int)TURN_WEIGHT.BOTH))
            {
                prefab = (PipeScript)this.prefabs[3]; //TURN_BOTH
                return prefab;
            }
            //left
            if (Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT))
            {
                prefab = (PipeScript)this.prefabs[1];   //TURN_LEFT
                prefab._itembase = script._itembase;
                return prefab;
            }
            //right
            if (Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT))
            {
                prefab = (PipeScript)this.prefabs[2];   //TURN_RIGHT
                prefab._itembase = script._itembase;
                //변경전 forword를 기억했다가 교체될때 재설정하도록 수정했습니다.
                ////[주석하지말것]위처럼 아래방향을 주석을하면, 생성된 블럭이 무한교체(깜빡이는 현상) 현상발생
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }

            prefab = (PipeScript)this.prefabs[0]; //TURN_FRONT
            return prefab;
        }


    }//..class BeltManager
}//..namespace MyCraft