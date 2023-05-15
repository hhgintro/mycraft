using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BeltManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //HG_TEST : 테스트용으로 public으로 선언함.
        //public List<BeltScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //관리대상목록(belt에서 이동하고 있는 물품을 관리합니다.

        void Awake()
        {
            base.LoadPrefab("blocks/transport-belt-front", 1100, this.transform.GetChild(0));
            base.LoadPrefab("blocks/transport-belt-turn-left", 1100, this.transform.GetChild(0));
            base.LoadPrefab("blocks/transport-belt-turn-right", 1100, this.transform.GetChild(0));
        }

        public override void OnAutomaticConnectBelt(Vector3 belt_hold_start, Vector3 point, BlockScript prefab)
        {
            if (null == prefab) return;

            Vector3 pos = belt_hold_start;  //현재위치(시작위치 부터 시작한다.)
            Vector3 belt_hold_end = point;  //종료위치

            //(-)음수이면 start/end를 바꿔준다.
            Vector3 forward = prefab.transform.forward;
            float dir = Vector3.Dot(prefab.transform.forward, point - belt_hold_start);
            //Debug.LogWarning($"Dot:{dir}");
            if (dir < 0)
            {
                pos = point;                        //현재위치(시작위치 부터 시작한다.)
                belt_hold_end = belt_hold_start;    //종료위치

                if (Common.OnBound(forward.x, 0)) pos.x = belt_hold_start.x;  // z방향을 향하므로 x값은 같게 설정
                if (Common.OnBound(forward.z, 0)) pos.z = belt_hold_start.z;  // x방향을 향하므로 z값은 같게 설정
            }
            else
            {
                if (Common.OnBound(forward.x, 0)) belt_hold_end.x = belt_hold_start.x;  // z방향을 향하므로 x값은 같게 설정
                if (Common.OnBound(forward.z, 0)) belt_hold_end.z = belt_hold_start.z;  // x방향을 향하므로 z값은 같게 설정
            }
            //Debug.Log($"자동연결: {belt_hold_start} ==> {belt_hold_end}");

            //end position
            int endx = Common.PosRound(belt_hold_end.x);
            int endy = Common.PosRound(belt_hold_end.y);
            int endz = Common.PosRound(belt_hold_end.z);

            bool isUp = false;          //(앞이 막혀서)위로 올라가야하나?
            bool isDown = false;        //(앞이 막혀서)아래로 내려가야하나?

            int count = -1;
            while (++count < 20)   //최대 n개(무한 루프방지)
            {
                // ********************************** //
                //1. 목표점 높이까지 먼저 올라간다.(내려간다.
                //2. 올라가다(내려가다) 막히면 앞으로 전진
                //3. 장벽이 있으면 넘는다.

                //current position
                int posx = Common.PosRound(pos.x);
                int posy = Common.PosRound(pos.y);
                int posz = Common.PosRound(pos.z);
                //현위치 점유중이면...종료
                if (null != GameManager.GetTerrainManager().GetBlockLayer().GetBlock(posx, posy, posz))
                    return;

                //arrive
                if (posx == endx && posy == endy && posz == endz)
                {
                    prefab = Automatic_End(ref isUp, ref isDown);
                    if (null != prefab)
                    {
                        //terrain에 block을 생성합니다.
                        prefab.transform.forward = forward; //방향.
                        GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, prefab);
                    }
                    return;
                }

                //목표점 위치와 다르면...높이부터 조정한다.
                if (endy != posy)
                {
                    //목표점이 높다면...
                    if (posy < endy)
                    {
                        // *********************** //
                        //우선순위 : 위 -> 앞 -> 아래

                        //설치할 곳.바로위
                        prefab = Automatic_Up(pos, ref isUp);
                        //설치할 곳.바로앞
                        if (null == prefab) prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                        //설치할 곳.바로아래
                        if (null == prefab) prefab = Automatic_Down(pos, ref isDown);
                    }
                    //목표점이 낮다면...
                    else
                    {
                        // *********************** //
                        //우선순위 : 아래 -> 앞 -> 위

                        //설치할 곳.바로아래
                        prefab = Automatic_Down(pos, ref isDown);
                        //설치할 곳.바로앞
                        if (null == prefab) prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                        //설치할 곳.바로위
                        if (null == prefab) prefab = Automatic_Up(pos, ref isUp);
                    }
                }
                //목표점과 같은 높이이다.
                else
                {
                    // *********************** //
                    //우선순위 : 앞 -> 위 -> 아래

                    //설치할 곳.바로앞
                    prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                    //설치할 곳.바로위
                    if (null == prefab) prefab = Automatic_Up(pos, ref isUp);
                    //설치할 곳.바로아래
                    if (null == prefab) prefab = Automatic_Down(pos, ref isDown);
                }

                //terrain에 block을 생성합니다.
                prefab.transform.forward = forward; //방향.
                GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, prefab);
                ////arrive
                //if (posx == endx && posy == endy && posz == endz)
                //{
                //    //prefab.transform.forward = forward; //방향.
                //    //this.CreateBlock(posx, posy, posz, GameManager.GetBeltManager().GetChoicePrefab());
                //    break;
                //}

                //다음 위치 설정
                if (true == isUp) pos += Vector3.up;
                else if (true == isDown) pos += Vector3.down;
                else pos += forward;
            }
        }

        private BlockScript Automatic_End(ref bool isUp, ref bool isDown)
        {
            BlockScript prefab = null;

            //올라가는 중이면 up-end
            if (true == isUp) prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
            //내려가는 중이면 down-end
            else if (true == isDown) prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
            //전진중
            else prefab = GameManager.GetBeltManager().GetChoicePrefab();

            isUp = false;   //올라가는거 끝
            isDown = false;   //내려가는거 끝
            return prefab;
        }

        //pos: 현재위치
        private BlockScript Automatic_Front(Vector3 pos, Vector3 forward, ref bool isUp, ref bool isDown)
        {
            BlockScript prefab = null;

            //next: 설치할 곳보다 앞쪽.
            Vector3 next = pos + forward;
            int nextx = Common.PosRound(next.x);
            int nexty = Common.PosRound(next.y);
            int nextz = Common.PosRound(next.z);

            // *********************** //
            //우선순위 : 앞 -> 위 -> 아래

            //설치할 곳보다 앞쪽이 비어있다면.
            BlockScript block = GameManager.GetTerrainManager().GetBlockLayer().GetBlock(nextx, nexty, nextz);
            if (null == block)
            {
                //올라가는 중이면 up-end
                if (true == isUp) prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
                //내려가는 중이면 down-end
                else if (true == isDown) prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
                //전진중
                else prefab = GameManager.GetBeltManager().GetChoicePrefab();

                isUp = false;   //올라가는거 끝
                isDown = false;   //내려가는거 끝
            }
            //else
            //{
            //    //임시테스트
            //    //밸트이면 연결해 준다.
            //    if(block.IsTransport())
            //    {
            //        //같은방향이면...
            //        if (true == Common.IsSameForward(block.transform.forward, forward))
            //        {
            //            //올라가는 중이면 up-end
            //            if (true == isUp)           prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
            //            //내려가는 중이면 down-end
            //            else if (true == isDown)    prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
            //            //전진중
            //            else                        prefab = GameManager.GetBeltManager().GetChoicePrefab();

            //            isUp    = false;   //올라가는거 끝
            //            isDown  = false;   //내려가는거 끝
            //        }
            //    }

            //}
            return prefab;
        }

        private BlockScript Automatic_Up(Vector3 pos, ref bool isUp)
        {
            BlockScript prefab = null;

            //설치할 곳.바로위
            Vector3 upper = pos + Vector3.up;
            int upperx = Common.PosRound(upper.x);
            int uppery = Common.PosRound(upper.y);
            int upperz = Common.PosRound(upper.z);

            //설치할 곳.바로위 비어있다...올라간다.
            if (null == GameManager.GetTerrainManager().GetBlockLayer().GetBlock(upperx, uppery, upperz))
            {
                //올라가는 중이면 up-middle
                if (true == isUp) prefab = GameManager.GetBeltVerticalUpMiddleManager().GetChoicePrefab();
                //막 올라가는 중이면 up-begin
                else prefab = GameManager.GetBeltVerticalUpBeginManager().GetChoicePrefab();

                isUp = true;    //올라가는중.
            }
            return prefab;
        }

        private BlockScript Automatic_Down(Vector3 pos, ref bool isDown)
        {
            BlockScript prefab = null;

            //설치할 곳.바로아래
            Vector3 under = pos + Vector3.down;
            int underx = Common.PosRound(under.x);
            int undery = Common.PosRound(under.y);
            int underz = Common.PosRound(under.z);

            //설치할 곳.바로아래 비어있다.
            if (null == GameManager.GetTerrainManager().GetBlockLayer().GetBlock(underx, undery, underz))
            {
                //내려가는 중이면 down-middle
                if (true == isDown) prefab = GameManager.GetBeltVerticalDownMiddleManager().GetChoicePrefab();
                //막 내려가는 중이면 down-begin
                else prefab = GameManager.GetBeltVerticalDownBeginManager().GetChoicePrefab();

                isDown = true;    //낼려가는중.
            }
            return prefab;
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
            BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            if (null == prefab) return;

            //동일개체이면 무시.
            if (script == prefab) return;

            prefab.SetSensor(script);
            Debug.LogWarning($"new prefab: {prefab.name}");

            //HG_TODO: turn_weight을 체크할 필요가 있을지 고민할것.
            //if (((BeltScript)prefab).turn_weight == ((BeltScript)script).turn_weight)
            //  return;


        }

        //자신의 front(script)가 (외형)변경되어져야 하는지 체크합니다.
        public override BlockScript ChainBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsBelt())
                return null;

            //prefab이 생성되므로 인해
            //script_front의 주변의 영향으로 [자신의] 외형이 변경되어질 수 있다.
            //변경될 prefab을 가져옵니다.
            Vector3 forward = script.transform.forward; //기존방향을 기억했다가
            BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            if (null == prefab) return null;

            //prefab.SetSensor(script);

            if (((BeltScript)prefab).turn_weight != ((BeltScript)script).turn_weight)
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
        public BeltScript ChainBeltPrefab(BeltScript script)
        {
            int weight = script.CheckWeightChainBlock();

            BeltScript prefab = null;
            /*
             * back 이 존재하는 경우는 무조건 TURN_FRONT
             * left와 right 모두 존재하는 경우는 TURN_FRONT
             * left만 존재하는 경우는 TURN_LEFT
             * right만 존재하는 경우는 TURN_RIGHT
             * */
            //weight
            if (0 == weight)
            {
                prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
                //변경전 forword를 기억했다가 교체될때 재설정하도록 수정했습니다.
                ////아래주석해제하면, 진행방향에 수직으로 방향전환시 prefab의 방향이 바뀐다
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }


            if (Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT))
            {
                prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
                //변경전 forword를 기억했다가 교체될때 재설정하도록 수정했습니다.
                ////[주석하지말것]아래를 주석했더니, left가 주변영향으로 front가 될때 기존방향을 상실하더라.
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }

            bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //left
            if (true == turn_left)
            {
                prefab = (BeltScript)this.prefabs[1];   //TURN_LEFT
                prefab._itembase = script._itembase;
                //변경전 forword를 기억했다가 교체될때 재설정하도록 수정했습니다.
                ////[주석하지말것]위처럼 아래방향을 주석을하면, 생성된 블럭이 무한교체(깜빡이는 현상) 현상발생
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }
            //right
            if (true == turn_right)
            {
                prefab = (BeltScript)this.prefabs[2];   //TURN_RIGHT
                prefab._itembase = script._itembase;
                //변경전 forword를 기억했다가 교체될때 재설정하도록 수정했습니다.
                ////[주석하지말것]위처럼 아래방향을 주석을하면, 생성된 블럭이 무한교체(깜빡이는 현상) 현상발생
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }

            //예외의 상황이면...
            if (null == prefab)
                Debug.LogError("critical not found prefab");

            return prefab;
        }


    }//..class BeltManager
}//..namespace MyCraft