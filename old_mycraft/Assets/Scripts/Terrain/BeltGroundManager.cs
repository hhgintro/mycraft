using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BeltGroundManager : BlockManager
    {
        int GROUND_COUNT = 5;   //ground belt는 지하로 (5-1)칸을 가진다.
        //private TerrainManager terrain_manager;

        //HG_TEST : 테스트용으로 public으로 선언함.
        //public List<BeltScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //관리대상목록(belt에서 이동하고 있는 물품을 관리합니다.

        void Awake()
        {
            //this.terrain_manager = this.transform.GetComponentInParent<TerrainManager>();


            //this.prefabs_belt = new List<BeltScript>();
            //this.prefabs.Add(this.transform.Find("prefab/basic-transport-belt").GetComponent<BeltScript>());
            //this.prefabs.Add(this.transform.Find("prefab/prefab_belt_turn_left").GetComponent<BeltScript>());
            //this.prefabs.Add(this.transform.Find("prefab/prefab_belt_turn_right").GetComponent<BeltScript>());
            BlockScript script = this.transform.Find("prefab/basic-transport-ground-belt-input").GetComponent<BeltScript>();
            script.manager = this;
            this.prefabs.Add(script);
            script = this.transform.Find("prefab/basic-transport-ground-belt-output").GetComponent<BeltScript>();
            script.manager = this;
            this.prefabs.Add(script);
            script = this.transform.Find("prefab/basic-transport-ground-belt-middle").GetComponent<BeltScript>();
            script.manager = this;
            this.prefabs.Add(script);


            //this.prefabs_goods = new List<BeltGoods>();
            //this.prefabs_goods.Add(this.transform.Find("prefab/prefab_belt_goods").GetComponent<BeltGoods>());

        }

        //public override BlockScript GetChoicePrefab(TURN_WEIGHT weight)
        //{
        //    if (this.prefabs.Count <= 0)
        //        return null;

        //    BlockScript prefab = null;
        //    switch (weight)
        //    {
        //        case TURN_WEIGHT.FRONT: prefab = this.prefabs[0]; break;
        //        case TURN_WEIGHT.LEFT:  prefab = this.prefabs[1]; break;
        //        case TURN_WEIGHT.RIGHT: prefab = this.prefabs[2]; break;
        //    }
        //    if (null == prefab) return null;
        //    prefab.GetComponent<Collider>().enabled = false;
        //    return prefab;
        //}


        //자신의 front(script)가 (외형)변경되어져야 하는지 체크합니다.
        public BlockScript ChainBelt(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsBelt())
                return null;

            //prefab이 생성되므로 인해
            //script_front의 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
            //변경될 prefab을 가져옵니다.
            BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            if (null == prefab) return null;


            if (((BeltScript)prefab).turn_weight != ((BeltScript)script).turn_weight)
            {
                //terrain 위치를 비워준다.
                GameManager.GetTerrainManager().block_layer.SubBlock(script);

                //new
                BlockScript newscript = prefab.Clone();
                if (null == newscript) return null;
                newscript._itembase = script._itembase;
                newscript.manager = null;

                //terrain에 위치시키다.
                newscript.SetPos(Common.PosRounding(script.transform.position.x)
                    , Common.PosRounding(script.transform.position.y)
                    , Common.PosRounding(script.transform.position.z));
                GameManager.GetTerrainManager().block_layer.AddBlock(newscript);

                script.manager.CreateBlock(newscript);


                //this.LinkedBelt(newscript);


                //삭제 : 교체바로 전에서 뺴고 처리하기 때문에 삭제시에는 block을 제거하지 않습니다.
                GameManager.GetTerrainManager().DeleteBlock(script, false);

                //외형이 변경된 경우 script_front와 next_front와의 link를 잡아준다.
                BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(newscript.transform.position + newscript.transform.forward);
                //this.LinkedBelt(script_front);

                return script_front;
            }

            //외형이 변경되지 않은 경우
            //script_front와 link는 잡아준다.
            //this.LinkedBelt(script);
            return script;
        }

        //선택된 prefab을 x,z 위치할때 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
        //변경될 prefab을 가져옵니다.
        public BeltScript ChainBeltPrefab(BeltScript script)
        {
            int weight = this.CheckWeightChainBelt(script);

            BeltScript prefab = null;


            //weight
            if (0 == weight)
            {
                prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
                prefab.transform.forward = script.transform.forward;
                return prefab;
            }

            bool turn_back = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.BACK);
            //back
            if (true == turn_back)
            {
                prefab = (BeltScript)this.prefabs[1];   //TURN_BACK
                prefab._itembase = script._itembase;
                prefab.transform.forward = script.transform.forward;
                return prefab;
            }


            //예외의 상황이면...
            if (null == prefab)
                Debug.LogError("critical not found prefab");

            return prefab;
        }

        // [자신]을 기준으로 back / left / right 의 belt 위치에 따라 [자신의] 가중치를 결정합니다.
        private int CheckWeightChainBelt(BlockScript script)
        {
            ////주변 block에 의한 가중치
            //int weight = 0;

            //for (int i = 0; i < GROUND_COUNT; ++i)
            //{
            //    BlockScript script_back = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward * (i + 1));
            //    if (null == script_back) continue;
            //    //grand belt
            //    if (BLOCKTYPE.GROUND_BELT != script_back._itembase.type) continue;
            //    //turn weight
            //    if (((BeltScript)script).turn_weight != ((BeltScript)script_back).turn_weight) continue;

            //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.BACK);
            //    break;
            //}

            //BlockScript script_front = terrain_manager.GetBlock(script.transform.position + script.transform.forward);
            BlockScript script_back = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward);
            BlockScript script_left = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
            BlockScript script_right = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);

            //주변 block에 의한 가중치
            int weight = 0;

            //back
            if (true == this.WeightTurn(script_back, script.transform.forward))
                weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);
            //left
            if (true == this.WeightTurn(script_left, script.transform.right))
                weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.LEFT);
            //right
            if (true == this.WeightTurn(script_right, -script.transform.right))
                weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            return weight;
        }

        //script의 forward와 일치히면 true를 리턴합니다.
        private bool WeightTurn(BlockScript script, Vector3 forward)
        {
            //blocktype
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return false;
            if (null == script || false == script.IsBelt())
                return false;


            //forward(오차발생...허용범위로 체크)
            float err_bound = 0.01f;
            float angle = Vector3.Angle(script.transform.forward, forward);
            if (angle < -err_bound || err_bound < angle)
                return false;

            return true;//가중치 적용
        }


        public override void CreateBlock(BlockScript script)
        {
            if (null == script) return;
            base.CreateBlock(script);

            BlockScript script_back = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward);
            if (null != script_back && BLOCKTYPE.BELT == script_back._itembase.type)
            {
                this.LinkedBelt(script);
                return;
            }

            for (int i = 0; i < GROUND_COUNT; ++i)
            {
                script_back = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward * (i + 1));
                if (null == script_back) continue;
                //grand belt
                if (BLOCKTYPE.GROUND_BELT != script_back._itembase.type) continue;
                //turn weight
                if (((BeltScript)script).turn_weight != ((BeltScript)script_back).turn_weight) continue;

                //BeltScript prefab = (BeltScript)this.prefabs[2];
                //prefab._itembase = script._itembase;
                ////angle
                //prefab.transform.eulerAngles = script.transform.eulerAngles;

                //새로 생성된 script의 back/left/right에서 link를 걸어줍니다.
                this.LinkedBelt(script);
                break;
            }
        }


        private void LinkedBelt(BlockScript script)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            //if (null == script || BLOCKTYPE.GROUND_BELT != script._itembase.type)
            if (null == script || false == script.IsBelt())
                return;


            // [자신]을 기준으로 back / left / right 의 belt 위치에 따라
            // [자신의] 가중치를 결정합니다.
            int weight = CheckWeightChainBelt(script);

            bool turn_front = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT);
            bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //back으로 부터( [자신]의 가중치가 front - back이 belt가 있다)
            if (true == turn_front)
            {
                //**************************************************************//
                //영향력이 확인되었으므로 상태체크를 하지 않는다.( null / blocktype )

                //back(back에 belt가 있다면) - [자신]은 front 상태임(back에 belt가 있으므로)
                BeltScript script_back = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward);
                //script_back.GetBeltSector(BELT_ROW.LEFT, BELT_COL.FIRST).next = ((BeltScript)script).GetBeltSector(BELT_ROW.LEFT, BELT_COL.FORTH);
                //script_back.GetBeltSector(BELT_ROW.RIGHT, BELT_COL.FIRST).next = ((BeltScript)script).GetBeltSector(BELT_ROW.LEFT, BELT_COL.FORTH);
                LinkBeltSector(script_back, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FORTH, BELT_ROW.RIGHT, BELT_COL.FORTH);

                if (true == turn_left)
                {
                    //left에 belt가 있다면 - [자신]은 front 상태임(back에 belt가 있으므로)
                    BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                    LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FIRST, BELT_ROW.LEFT, BELT_COL.THIRD);
                }

                if (true == turn_right)
                {
                    //right에 belt가 있다면 - [자신]은 front 상태임(back에 belt가 있으므로)
                    BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                    LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.RIGHT, BELT_COL.THIRD, BELT_ROW.RIGHT, BELT_COL.FIRST);
                }

                return;//****중요(더 아래로 진행못하게)
            }

            //HG_TODO : left,right의 모두인 경우나, 각각의 경우에 next 위치는 현재는 동일하다.
            //          추후  모두인경우와 분리를 검토하기 위해 각각의 경우로 나눠 놓았을 뿐이다.
            if (true == turn_left && true == turn_right)
            {
                //left 와 right 양쪽에 belt가 있다면...
                //[자신]은 front 상태임(left 와 right에 모두 belt가 있으므로)

                //left
                BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FIRST, BELT_ROW.LEFT, BELT_COL.THIRD);
                //right
                BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.RIGHT, BELT_COL.THIRD, BELT_ROW.RIGHT, BELT_COL.FIRST);

                return;//****중요(더 아래로 진행못하게)
            }

            if (true == turn_left)
            {
                //left에 belt가 있다면
                //[자신]은 left 상태임(left 에 belt가 있으므로)
                BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.SECOND, BELT_ROW.RIGHT, BELT_COL.FORTH);
            }

            if (true == turn_right)
            {
                //right에 belt가 있다면
                //[자신]은 right 상태임(right 에 belt가 있으므로)
                BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FORTH, BELT_ROW.RIGHT, BELT_COL.SECOND);
            }
        }

        //prev의 앞쪽이 연결될 next의 ROW/COL을 설정해 줍니다.
        private void LinkBeltSector(BeltScript prev, BeltScript next, BELT_ROW lrow, BELT_COL lcol, BELT_ROW rrow, BELT_COL rcol)
        {
            //if (null == prev || null == prev._itembase || BLOCKTYPE.BELT != prev._itembase.type)
            //    return;
            //if (null == next || null == next._itembase || BLOCKTYPE.BELT != next._itembase.type)
            //    return;
            if (null == prev || false == prev.IsBelt())
                return;
            if (null == next || false == next.IsBelt())
                return;

            prev.GetBeltSector(BELT_ROW.LEFT, BELT_COL.FIRST).next = next.GetBeltSector(lrow, lcol);
            prev.GetBeltSector(BELT_ROW.RIGHT, BELT_COL.FIRST).next = next.GetBeltSector(rrow, rcol);
        }

        public void PutdownGoods(BlockScript script, BELT_ROW row)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == script || false == script.IsBelt())
                return;

            ////생성및 sector에 등록
            //GameObject obj = UnityEngine.Object.Instantiate(this.prefabs_goods[0].gameObject);
            //obj.SetActive(true);

            //script.PutdownGoods(row, obj);
        }

        public override void DeleteBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == script || false == script.IsBelt())
                return;

            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward);
            this.LinkedBelt(this.ChainBelt(script_front));

            //script.DeleteBlock();
            base.DeleteBlock(script);
        }


    }//..class BeltManager
}//..namespace MyCraft