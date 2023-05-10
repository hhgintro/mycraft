using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class PipeManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //HG_TEST : �׽�Ʈ������ public���� ������.
        //public List<PipeScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //���������(belt���� �̵��ϰ� �ִ� ��ǰ�� �����մϴ�.

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

            ////���� ������ script�� back/left/right���� link�� �ɾ��ݴϴ�.
            //script.LinkedBelt();

            ////������ script�� front�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
            //BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward);
            //if(script_front) script_front.manager.ChainBelt(script_front);
        }

        public override void DeleteBlock(BlockScript block)
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (null == block || false == block.IsPipe())
                return;

            //������ script�� front�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
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


        //����:block���� ������°� ����Ǹ�, ������ �ٲ�� �ִ�.
        public override void LinkedSensor(BlockScript script)
        {
            //sensor�� �۵�������, �ֺ��� �������� [�ڽ�(script)]�� ������ �ٲ���� Ȯ���Ѵ�.
            BlockScript prefab = this.ChainBeltPrefab((PipeScript)script);
            if (null == prefab) return;

            //���ϰ�ü�̸� ����.
            if (script == prefab) return;

            prefab.SetSensor(script);
            Debug.LogWarning($"new prefab: {prefab.name}");

            //HG_TODO: turn_weight�� üũ�� �ʿ䰡 ������ ����Ұ�.
            //if (((PipeScript)prefab).turn_weight == ((PipeScript)script).turn_weight)
            //  return;


        }

        //�ڽ��� front(script)�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
        public override BlockScript ChainBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsPipe())
                return null;

            //prefab�� �����ǹǷ� ����
            //script_front�� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
            //����� prefab�� �����ɴϴ�.
            Vector3 forward = script.transform.forward; //���������� ����ߴٰ�
            BlockScript prefab = this.ChainBeltPrefab((PipeScript)script);
            if (null == prefab) return null;

            //prefab.SetSensor(script);

            if (prefab != script)
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
                //������ forword�� ����ߴٰ� ��ü�ɶ� �缳���ϵ��� �����߽��ϴ�.
                ////[�ּ���������]��ó�� �Ʒ������� �ּ����ϸ�, ������ ���� ���ѱ�ü(�����̴� ����) ����߻�
                //prefab.transform.forward = script.transform.forward;
                return prefab;
            }

            prefab = (PipeScript)this.prefabs[0]; //TURN_FRONT
            return prefab;
        }


    }//..class BeltManager
}//..namespace MyCraft