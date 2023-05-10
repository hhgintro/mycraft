using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class BeltVerticalDownMiddleManager : BlockManager
    {
        //private TerrainManager terrain_manager;

        //HG_TEST : �׽�Ʈ������ public���� ������.
        //public List<BeltScript> prefabs_belt;

        //List<BeltGoods> prefabs_goods;

        //List<BeltGoods> goods;  //���������(belt���� �̵��ϰ� �ִ� ��ǰ�� �����մϴ�.

        void Awake()
        {
            base.LoadPrefab("blocks/transport-belt-vertical-down-middle", 1170, this.transform.GetChild(0));
        }


        public override void CreateBlock(BlockScript script)
        {
            if (null == script) return;
            base.CreateBlock(script);

            ////���� ������ script�� back/left/right���� link�� �ɾ��ݴϴ�.
            //script.LinkedBelt();

            ////������ script�� front�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
            //BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.forward + script.transform.up);
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


        //�ڽ��� front(script)�� (����)����Ǿ����� �ϴ��� üũ�մϴ�.
        public override BlockScript ChainBlock(BlockScript script)
        {
            //if (null == script || null == script._itembase) return null;
            //if (BLOCKTYPE.BELT != script._itembase.type) return null;
            if (null == script || false == script.IsBelt())
                return null;

            ////prefab�� �����ǹǷ� ����
            ////script_front�� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
            ////����� prefab�� �����ɴϴ�.
            //BlockScript prefab = this.ChainBeltPrefab((BeltScript)script);
            //if (null == prefab) return null;


            //if (((BeltScript)prefab).turn_weight != ((BeltScript)script).turn_weight)
            //{
            //    //terrain ��ġ�� ����ش�.
            //    GameManager.GetTerrainManager().block_layer.SubBlock(script);

            //    //new
            //    BlockScript newscript = prefab.Clone();
            //    if (null == newscript) return null;
            //    newscript._itembase = script._itembase;
            //    newscript.manager = null;

            //    //terrain�� ��ġ��Ű��.
            //    newscript.SetPos(Common.PosRounding(script.transform.position.x)
            //        , Common.PosRounding(script.transform.position.y)
            //        , Common.PosRounding(script.transform.position.z));
            //    GameManager.GetTerrainManager().block_layer.AddBlock(newscript);

            //    script.manager.CreateBlock(newscript);


            //    //this.LinkedBelt(newscript);


            //    //���� : ��ü�ٷ� ������ ���� ó���ϱ� ������ �����ÿ��� block�� �������� �ʽ��ϴ�.
            //    GameManager.GetTerrainManager().DeleteBlock(script, false);

            //    //������ ����� ��� script_front�� back/left/right���� link�� �ɾ��ݴϴ�.
            //    BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(newscript.transform.position + newscript.transform.forward);
            //    if(script_front) script_front.LinkedBelt();

            //    return script_front;
            //}

            //������ ������� ���� ���
            //script_front�� link�� ����ش�.
            script.LinkedBelt();
            return script;
        }

        ////���õ� prefab�� x,z ��ġ�Ҷ� �ֺ��� �������� [�ڽ���] ������ ����Ǿ��� �� �ִ�.
        ////����� prefab�� �����ɴϴ�.
        //public BeltScript ChainBeltPrefab(BeltScript script)
        //{
        //    int weight = script.CheckWeightChainBelt();

        //    BeltScript prefab = null;
        //    /*
        //     * back �� �����ϴ� ���� ������ TURN_FRONT
        //     * left�� right ��� �����ϴ� ���� TURN_FRONT
        //     * left�� �����ϴ� ���� TURN_LEFT
        //     * right�� �����ϴ� ���� TURN_RIGHT
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

        //    //������ ��Ȳ�̸�...
        //    if (null == prefab)
        //        Debug.LogError("critical not found prefab");

        //    return prefab;
        //}


    }//..class BeltManager
}//..namespace MyCraft