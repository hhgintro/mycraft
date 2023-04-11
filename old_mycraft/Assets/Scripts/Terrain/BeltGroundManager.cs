using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BeltGroundManager : BlockManager
    {
        int GROUND_COUNT = 5;   //ground belt�� ���Ϸ� (5-1)ĭ�� ������.
        //private TerrainManager terrain_manager;

        //HG_TEST : �׽�Ʈ������ public���� ������.
        //public List<BeltScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //���������(belt���� �̵��ϰ� �ִ� ��ǰ�� �����մϴ�.

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


        //�ڽ��� front(script)�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
        public BlockScript ChainBelt(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsBelt())
                return null;

            //prefab�� �����ǹǷ� ����
            //script_front�� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
            //����� prefab�� �����ɴϴ�.
            BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            if (null == prefab) return null;


            if (((BeltScript)prefab).turn_weight != ((BeltScript)script).turn_weight)
            {
                //terrain ��ġ�� ����ش�.
                GameManager.GetTerrainManager().block_layer.SubBlock(script);

                //new
                BlockScript newscript = prefab.Clone();
                if (null == newscript) return null;
                newscript._itembase = script._itembase;
                newscript.manager = null;

                //terrain�� ��ġ��Ű��.
                newscript.SetPos(Common.PosRounding(script.transform.position.x)
                    , Common.PosRounding(script.transform.position.y)
                    , Common.PosRounding(script.transform.position.z));
                GameManager.GetTerrainManager().block_layer.AddBlock(newscript);

                script.manager.CreateBlock(newscript);


                //this.LinkedBelt(newscript);


                //���� : ��ü�ٷ� ������ ���� ó���ϱ� ������ �����ÿ��� block�� �������� �ʽ��ϴ�.
                GameManager.GetTerrainManager().DeleteBlock(script, false);

                //������ ����� ��� script_front�� next_front���� link�� ����ش�.
                BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(newscript.transform.position + newscript.transform.forward);
                //this.LinkedBelt(script_front);

                return script_front;
            }

            //������ ������� ���� ���
            //script_front�� link�� ����ش�.
            //this.LinkedBelt(script);
            return script;
        }

        //���õ� prefab�� x,z ��ġ�Ҷ� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
        //����� prefab�� �����ɴϴ�.
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


            //������ ��Ȳ�̸�...
            if (null == prefab)
                Debug.LogError("critical not found prefab");

            return prefab;
        }

        // [�ڽ�]�� �������� back / left / right �� belt ��ġ�� ���� [�ڽ���] ����ġ�� �����մϴ�.
        private int CheckWeightChainBelt(BlockScript script)
        {
            ////�ֺ� block�� ���� ����ġ
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

            //�ֺ� block�� ���� ����ġ
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

        //script�� forward�� ��ġ���� true�� �����մϴ�.
        private bool WeightTurn(BlockScript script, Vector3 forward)
        {
            //blocktype
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return false;
            if (null == script || false == script.IsBelt())
                return false;


            //forward(�����߻�...�������� üũ)
            float err_bound = 0.01f;
            float angle = Vector3.Angle(script.transform.forward, forward);
            if (angle < -err_bound || err_bound < angle)
                return false;

            return true;//����ġ ����
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

                //���� ������ script�� back/left/right���� link�� �ɾ��ݴϴ�.
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


            // [�ڽ�]�� �������� back / left / right �� belt ��ġ�� ����
            // [�ڽ���] ����ġ�� �����մϴ�.
            int weight = CheckWeightChainBelt(script);

            bool turn_front = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT);
            bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //back���� ����( [�ڽ�]�� ����ġ�� front - back�� belt�� �ִ�)
            if (true == turn_front)
            {
                //**************************************************************//
                //������� Ȯ�εǾ����Ƿ� ����üũ�� ���� �ʴ´�.( null / blocktype )

                //back(back�� belt�� �ִٸ�) - [�ڽ�]�� front ������(back�� belt�� �����Ƿ�)
                BeltScript script_back = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward);
                //script_back.GetBeltSector(BELT_ROW.LEFT, BELT_COL.FIRST).next = ((BeltScript)script).GetBeltSector(BELT_ROW.LEFT, BELT_COL.FORTH);
                //script_back.GetBeltSector(BELT_ROW.RIGHT, BELT_COL.FIRST).next = ((BeltScript)script).GetBeltSector(BELT_ROW.LEFT, BELT_COL.FORTH);
                LinkBeltSector(script_back, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FORTH, BELT_ROW.RIGHT, BELT_COL.FORTH);

                if (true == turn_left)
                {
                    //left�� belt�� �ִٸ� - [�ڽ�]�� front ������(back�� belt�� �����Ƿ�)
                    BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                    LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FIRST, BELT_ROW.LEFT, BELT_COL.THIRD);
                }

                if (true == turn_right)
                {
                    //right�� belt�� �ִٸ� - [�ڽ�]�� front ������(back�� belt�� �����Ƿ�)
                    BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                    LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.RIGHT, BELT_COL.THIRD, BELT_ROW.RIGHT, BELT_COL.FIRST);
                }

                return;//****�߿�(�� �Ʒ��� ������ϰ�)
            }

            //HG_TODO : left,right�� ����� ��쳪, ������ ��쿡 next ��ġ�� ����� �����ϴ�.
            //          ����  ����ΰ��� �и��� �����ϱ� ���� ������ ���� ���� ������ ���̴�.
            if (true == turn_left && true == turn_right)
            {
                //left �� right ���ʿ� belt�� �ִٸ�...
                //[�ڽ�]�� front ������(left �� right�� ��� belt�� �����Ƿ�)

                //left
                BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FIRST, BELT_ROW.LEFT, BELT_COL.THIRD);
                //right
                BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.RIGHT, BELT_COL.THIRD, BELT_ROW.RIGHT, BELT_COL.FIRST);

                return;//****�߿�(�� �Ʒ��� ������ϰ�)
            }

            if (true == turn_left)
            {
                //left�� belt�� �ִٸ�
                //[�ڽ�]�� left ������(left �� belt�� �����Ƿ�)
                BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.SECOND, BELT_ROW.RIGHT, BELT_COL.FORTH);
            }

            if (true == turn_right)
            {
                //right�� belt�� �ִٸ�
                //[�ڽ�]�� right ������(right �� belt�� �����Ƿ�)
                BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.LEFT, BELT_COL.FORTH, BELT_ROW.RIGHT, BELT_COL.SECOND);
            }
        }

        //prev�� ������ ����� next�� ROW/COL�� ������ �ݴϴ�.
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

            ////������ sector�� ���
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