using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BeltManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //HG_TEST : �׽�Ʈ������ public���� ������.
        //public List<BeltScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //���������(belt���� �̵��ϰ� �ִ� ��ǰ�� �����մϴ�.

        void Awake()
        {
            base.LoadPrefab("blocks/transport-belt-front", 1100, this.transform.GetChild(0));
            base.LoadPrefab("blocks/transport-belt-turn-left", 1100, this.transform.GetChild(0));
            base.LoadPrefab("blocks/transport-belt-turn-right", 1100, this.transform.GetChild(0));
        }

        public override void OnAutomaticConnectBelt(Vector3 belt_hold_start, Vector3 point, BlockScript prefab)
        {
            if (null == prefab) return;

            Vector3 pos = belt_hold_start;  //������ġ(������ġ ���� �����Ѵ�.)
            Vector3 belt_hold_end = point;  //������ġ

            //(-)�����̸� start/end�� �ٲ��ش�.
            Vector3 forward = prefab.transform.forward;
            float dir = Vector3.Dot(prefab.transform.forward, point - belt_hold_start);
            //Debug.LogWarning($"Dot:{dir}");
            if (dir < 0)
            {
                pos = point;                        //������ġ(������ġ ���� �����Ѵ�.)
                belt_hold_end = belt_hold_start;    //������ġ

                if (Common.OnBound(forward.x, 0)) pos.x = belt_hold_start.x;  // z������ ���ϹǷ� x���� ���� ����
                if (Common.OnBound(forward.z, 0)) pos.z = belt_hold_start.z;  // x������ ���ϹǷ� z���� ���� ����
            }
            else
            {
                if (Common.OnBound(forward.x, 0)) belt_hold_end.x = belt_hold_start.x;  // z������ ���ϹǷ� x���� ���� ����
                if (Common.OnBound(forward.z, 0)) belt_hold_end.z = belt_hold_start.z;  // x������ ���ϹǷ� z���� ���� ����
            }
            //Debug.Log($"�ڵ�����: {belt_hold_start} ==> {belt_hold_end}");

            //end position
            int endx = Common.PosRound(belt_hold_end.x);
            int endy = Common.PosRound(belt_hold_end.y);
            int endz = Common.PosRound(belt_hold_end.z);

            bool isUp = false;          //(���� ������)���� �ö󰡾��ϳ�?
            bool isDown = false;        //(���� ������)�Ʒ��� ���������ϳ�?

            int count = -1;
            while (++count < 20)   //�ִ� n��(���� ��������)
            {
                // ********************************** //
                //1. ��ǥ�� ���̱��� ���� �ö󰣴�.(��������.
                //2. �ö󰡴�(��������) ������ ������ ����
                //3. �庮�� ������ �Ѵ´�.

                //current position
                int posx = Common.PosRound(pos.x);
                int posy = Common.PosRound(pos.y);
                int posz = Common.PosRound(pos.z);
                //����ġ �������̸�...����
                if (null != GameManager.GetTerrainManager().GetBlockLayer().GetBlock(posx, posy, posz))
                    return;

                //arrive
                if (posx == endx && posy == endy && posz == endz)
                {
                    prefab = Automatic_End(ref isUp, ref isDown);
                    if (null != prefab)
                    {
                        //terrain�� block�� �����մϴ�.
                        prefab.transform.forward = forward; //����.
                        GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, prefab);
                    }
                    return;
                }

                //��ǥ�� ��ġ�� �ٸ���...���̺��� �����Ѵ�.
                if (endy != posy)
                {
                    //��ǥ���� ���ٸ�...
                    if (posy < endy)
                    {
                        // *********************** //
                        //�켱���� : �� -> �� -> �Ʒ�

                        //��ġ�� ��.�ٷ���
                        prefab = Automatic_Up(pos, ref isUp);
                        //��ġ�� ��.�ٷξ�
                        if (null == prefab) prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                        //��ġ�� ��.�ٷξƷ�
                        if (null == prefab) prefab = Automatic_Down(pos, ref isDown);
                    }
                    //��ǥ���� ���ٸ�...
                    else
                    {
                        // *********************** //
                        //�켱���� : �Ʒ� -> �� -> ��

                        //��ġ�� ��.�ٷξƷ�
                        prefab = Automatic_Down(pos, ref isDown);
                        //��ġ�� ��.�ٷξ�
                        if (null == prefab) prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                        //��ġ�� ��.�ٷ���
                        if (null == prefab) prefab = Automatic_Up(pos, ref isUp);
                    }
                }
                //��ǥ���� ���� �����̴�.
                else
                {
                    // *********************** //
                    //�켱���� : �� -> �� -> �Ʒ�

                    //��ġ�� ��.�ٷξ�
                    prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                    //��ġ�� ��.�ٷ���
                    if (null == prefab) prefab = Automatic_Up(pos, ref isUp);
                    //��ġ�� ��.�ٷξƷ�
                    if (null == prefab) prefab = Automatic_Down(pos, ref isDown);
                }

                //terrain�� block�� �����մϴ�.
                prefab.transform.forward = forward; //����.
                GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, prefab);
                ////arrive
                //if (posx == endx && posy == endy && posz == endz)
                //{
                //    //prefab.transform.forward = forward; //����.
                //    //this.CreateBlock(posx, posy, posz, GameManager.GetBeltManager().GetChoicePrefab());
                //    break;
                //}

                //���� ��ġ ����
                if (true == isUp) pos += Vector3.up;
                else if (true == isDown) pos += Vector3.down;
                else pos += forward;
            }
        }

        private BlockScript Automatic_End(ref bool isUp, ref bool isDown)
        {
            BlockScript prefab = null;

            //�ö󰡴� ���̸� up-end
            if (true == isUp) prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
            //�������� ���̸� down-end
            else if (true == isDown) prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
            //������
            else prefab = GameManager.GetBeltManager().GetChoicePrefab();

            isUp = false;   //�ö󰡴°� ��
            isDown = false;   //�������°� ��
            return prefab;
        }

        //pos: ������ġ
        private BlockScript Automatic_Front(Vector3 pos, Vector3 forward, ref bool isUp, ref bool isDown)
        {
            BlockScript prefab = null;

            //next: ��ġ�� ������ ����.
            Vector3 next = pos + forward;
            int nextx = Common.PosRound(next.x);
            int nexty = Common.PosRound(next.y);
            int nextz = Common.PosRound(next.z);

            // *********************** //
            //�켱���� : �� -> �� -> �Ʒ�

            //��ġ�� ������ ������ ����ִٸ�.
            BlockScript block = GameManager.GetTerrainManager().GetBlockLayer().GetBlock(nextx, nexty, nextz);
            if (null == block)
            {
                //�ö󰡴� ���̸� up-end
                if (true == isUp) prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
                //�������� ���̸� down-end
                else if (true == isDown) prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
                //������
                else prefab = GameManager.GetBeltManager().GetChoicePrefab();

                isUp = false;   //�ö󰡴°� ��
                isDown = false;   //�������°� ��
            }
            //else
            //{
            //    //�ӽ��׽�Ʈ
            //    //��Ʈ�̸� ������ �ش�.
            //    if(block.IsTransport())
            //    {
            //        //���������̸�...
            //        if (true == Common.IsSameForward(block.transform.forward, forward))
            //        {
            //            //�ö󰡴� ���̸� up-end
            //            if (true == isUp)           prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
            //            //�������� ���̸� down-end
            //            else if (true == isDown)    prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
            //            //������
            //            else                        prefab = GameManager.GetBeltManager().GetChoicePrefab();

            //            isUp    = false;   //�ö󰡴°� ��
            //            isDown  = false;   //�������°� ��
            //        }
            //    }

            //}
            return prefab;
        }

        private BlockScript Automatic_Up(Vector3 pos, ref bool isUp)
        {
            BlockScript prefab = null;

            //��ġ�� ��.�ٷ���
            Vector3 upper = pos + Vector3.up;
            int upperx = Common.PosRound(upper.x);
            int uppery = Common.PosRound(upper.y);
            int upperz = Common.PosRound(upper.z);

            //��ġ�� ��.�ٷ��� ����ִ�...�ö󰣴�.
            if (null == GameManager.GetTerrainManager().GetBlockLayer().GetBlock(upperx, uppery, upperz))
            {
                //�ö󰡴� ���̸� up-middle
                if (true == isUp) prefab = GameManager.GetBeltVerticalUpMiddleManager().GetChoicePrefab();
                //�� �ö󰡴� ���̸� up-begin
                else prefab = GameManager.GetBeltVerticalUpBeginManager().GetChoicePrefab();

                isUp = true;    //�ö󰡴���.
            }
            return prefab;
        }

        private BlockScript Automatic_Down(Vector3 pos, ref bool isDown)
        {
            BlockScript prefab = null;

            //��ġ�� ��.�ٷξƷ�
            Vector3 under = pos + Vector3.down;
            int underx = Common.PosRound(under.x);
            int undery = Common.PosRound(under.y);
            int underz = Common.PosRound(under.z);

            //��ġ�� ��.�ٷξƷ� ����ִ�.
            if (null == GameManager.GetTerrainManager().GetBlockLayer().GetBlock(underx, undery, underz))
            {
                //�������� ���̸� down-middle
                if (true == isDown) prefab = GameManager.GetBeltVerticalDownMiddleManager().GetChoicePrefab();
                //�� �������� ���̸� down-begin
                else prefab = GameManager.GetBeltVerticalDownBeginManager().GetChoicePrefab();

                isDown = true;    //����������.
            }
            return prefab;
        }

        public override void CreateBlock(BlockScript script)
        {
            if (null == script) return;
            base.CreateBlock(script);

            ////���� ������ script�� back/left/right���� link�� �ɾ��ݴϴ�.
            //script.LinkedBelt();

            ////������ script�� front�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
            //BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward);
            //if(script_front) script_front.manager.ChainBelt(script_front);
        }

        public override void DeleteBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == script || false == script.IsBelt())
                return;

            //������ script�� front�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
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


        //����:block���� ������°� ����Ǹ�, ������ �ٲ�� �ִ�.
        public override void LinkedSensor(BlockScript script)
        {
            //sensor�� �۵�������, �ֺ��� �������� [�ڽ�(script)]�� ������ �ٲ���� Ȯ���Ѵ�.
            BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            if (null == prefab) return;

            //���ϰ�ü�̸� ����.
            if (script == prefab) return;

            prefab.SetSensor(script);
            Debug.LogWarning($"new prefab: {prefab.name}");

            //HG_TODO: turn_weight�� üũ�� �ʿ䰡 ������ ����Ұ�.
            //if (((BeltScript)prefab).turn_weight == ((BeltScript)script).turn_weight)
            //  return;


        }

        //�ڽ��� front(script)�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
        public override BlockScript ChainBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsBelt())
                return null;

            //prefab�� �����ǹǷ� ����
            //script_front�� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
            //����� prefab�� �����ɴϴ�.
            Vector3 forward = script.transform.forward; //���������� ����ߴٰ�
            BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            if (null == prefab) return null;

            //prefab.SetSensor(script);

            if (((BeltScript)prefab).turn_weight != ((BeltScript)script).turn_weight)
            {
                //terrain ��ġ�� ����ش�.
                GameManager.GetTerrainManager().block_layer.SubBlock(script);

                //new
                BlockScript newscript = prefab.Clone();
                if (null == newscript) return null;
                newscript._itembase = script._itembase;
                newscript.manager = null;
                newscript.transform.forward = forward;  //���������� �缳���մϴ�.

                //terrain�� ��ġ��Ű��.
                newscript.SetPos(Common.PosRound(script.transform.position.x)
                    , Common.PosRound(script.transform.position.y)
                    , Common.PosRound(script.transform.position.z));
                GameManager.GetTerrainManager().block_layer.AddBlock(newscript);

                script.manager.CreateBlock(newscript);


                //this.LinkedBelt(newscript);


                //���� : ��ü�ٷ� ������ ���� ó���ϱ� ������ �����ÿ��� block�� �������� �ʽ��ϴ�.
                GameManager.GetTerrainManager().DeleteBlock(script, false);

                //������ ����� ��� script_front�� back/left/right���� link�� �ɾ��ݴϴ�.
                BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(newscript.transform.position + newscript.transform.forward);
                if(script_front) script_front.LinkedBelt();

                return script_front;
            }

            //������ ������� ���� ���
            //script�� back/left/right���� link�� �ɾ��ݴϴ�.
            script.LinkedBelt();
            return script;
        }

        //���õ� prefab�� x,z ��ġ�Ҷ� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
        //����� prefab�� �����ɴϴ�.
        public BeltScript ChainBeltPrefab(BeltScript script)
        {
            int weight = script.CheckWeightChainBlock();

            BeltScript prefab = null;
            /*
             * back �� �����ϴ� ���� ������ TURN_FRONT
             * left�� right ��� �����ϴ� ���� TURN_FRONT
             * left�� �����ϴ� ���� TURN_LEFT
             * right�� �����ϴ� ���� TURN_RIGHT
             * */
            //weight
            if (0 == weight)
            {
                prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
                //������ forword�� ����ߴٰ� ��ü�ɶ� �缳���ϵ��� �����߽��ϴ�.
                ////�Ʒ��ּ������ϸ�, ������⿡ �������� ������ȯ�� prefab�� ������ �ٲ��
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }


            if (Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT))
            {
                prefab = (BeltScript)this.prefabs[0]; //TURN_FRONT
                //������ forword�� ����ߴٰ� ��ü�ɶ� �缳���ϵ��� �����߽��ϴ�.
                ////[�ּ���������]�Ʒ��� �ּ��ߴ���, left�� �ֺ��������� front�� �ɶ� ���������� ����ϴ���.
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
                //������ forword�� ����ߴٰ� ��ü�ɶ� �缳���ϵ��� �����߽��ϴ�.
                ////[�ּ���������]��ó�� �Ʒ������� �ּ����ϸ�, ������ ���� ���ѱ�ü(�����̴� ����) ����߻�
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }
            //right
            if (true == turn_right)
            {
                prefab = (BeltScript)this.prefabs[2];   //TURN_RIGHT
                prefab._itembase = script._itembase;
                //������ forword�� ����ߴٰ� ��ü�ɶ� �缳���ϵ��� �����߽��ϴ�.
                ////[�ּ���������]��ó�� �Ʒ������� �ּ����ϸ�, ������ ���� ���ѱ�ü(�����̴� ����) ����߻�
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }

            //������ ��Ȳ�̸�...
            if (null == prefab)
                Debug.LogError("critical not found prefab");

            return prefab;
        }


    }//..class BeltManager
}//..namespace MyCraft