using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    public class SpliterScript : BeltBaseScript
    {
        //belt-goods를 어디로 보낼지 결정한다.
        bool _left  = false;    //ROW1,ROW3: false이면 1, true이면 3
        bool _right = false;    //ROW2,ROW4: false이면 2, true이면 4

        public override void LinkedBelt()
        {
            if (false == this.IsSpliter())
                return;

            if (base._lb)
            {
                if (SENSOR.RF == base._lb._sensor)
                    base._lb._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                else //belt
                    base._lb._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            }

            if (base._rb)
            {
                if (SENSOR.RF == base._rb._sensor)
                    base._rb._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW3, BELT_COL.FORTH, BELT_ROW.ROW4, BELT_COL.FORTH);
                else //belt
                    base._rb._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW3, BELT_COL.FORTH, BELT_ROW.ROW4, BELT_COL.FORTH);
            }

            ////Debug.Log("LinkBetlt " + script._index);
            //// [자신]을 기준으로 back / left / right 의 belt 위치에 따라
            //// [자신의] 가중치를 결정합니다.
            //int weight = CheckWeightChainBelt();

            //bool turn_front = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT);
            ////bool turn_left  = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            ////bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //if (true == turn_front)
            //{
            //    //back(back에 belt가 있다면) - [자신]은 front 상태임(back에 belt가 있으므로)
            //    //BeltScript script_back = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward);
            //    //LinkBeltSector(script_back, (BeltScript)script, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //    BlockScript block_0back = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward - this.transform.right);
            //    BlockScript block_lback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward);
            //    BlockScript block_rback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward + this.transform.right);
            //    BlockScript block_1back = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward + this.transform.right * 2);
            //    if (null != block_lback && block_lback == block_rback) //spliter
            //    {
            //        block_lback.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //        block_lback.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW3, BELT_COL.FORTH, BELT_ROW.ROW4, BELT_COL.FORTH);
            //    }
            //    else
            //    {
            //        if (null != block_lback)
            //        {
            //            if (block_0back == block_lback) //spliter
            //                block_lback.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //            else //belt
            //                block_lback.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //        }
            //        if (null != block_rback)
            //        {
            //            if (block_rback == block_1back) //spliter
            //                block_rback.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW3, BELT_COL.FORTH, BELT_ROW.ROW4, BELT_COL.FORTH);
            //            else //belt
            //                block_rback.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW3, BELT_COL.FORTH, BELT_ROW.ROW4, BELT_COL.FORTH);
            //        }
            //    }
            //}
        }

        //// [자신]을 기준으로 back / left / right 의 belt 위치에 따라 [자신의] 가중치를 결정합니다.
        //public override int CheckWeightChainBelt()
        //{
        //    //주변 block에 의한 가중치
        //    int weight = 0;

        //    if (base._lb || base._rb)   weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);

        //    //lback, mback : left-back, middle-back ( 뒤쪽에 belt/spliter가 있을때를 고려함 )
        //    BlockScript script_lback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward - this.transform.right);
        //    BlockScript script_mback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward);
        //    //BlockScript script_lleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right + this.transform.up);
        //    //BlockScript script_mleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right);
        //    //BlockScript script_lright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right - this.transform.forward);
        //    //BlockScript script_mright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right);


        //    //back
        //    if (true == this.WeightTurn(script_lback, this.transform.forward)
        //        || true == this.WeightTurn(script_mback, this.transform.forward))
        //        weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);
        //    ////left
        //    //if (true == this.WeightTurn(script_lleft, this.transform.right)
        //    //    || true == this.WeightTurn(script_mleft, this.transform.right))
        //    //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.LEFT);
        //    ////right
        //    //if (true == this.WeightTurn(script_lright, -this.transform.right)
        //    //    || true == this.WeightTurn(script_mright, -this.transform.right))
        //    //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.RIGHT);

        //    ////check
        //    //bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
        //    //bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);
        //    //if (0 == weight || (true == turn_left && true == turn_right)) //외압이 없거나, left/right모두에서 올때
        //    //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);

        //    return weight;  //무조건 3개중 1개 => TURN_WEIGHT.FRONT / TURN_WEIGHT.LEFT / TURN_WEIGHT.RIGHT
        //}

        protected override BeltSector GetNextSector(int idx)
        {
            BeltSector sector = this.sectors[idx].next;
            //3번째줄일때...분기한다.
            switch(idx)
            {
                case 2:
                case 10:
                    if(false == _left)      sector = this.sectors[1];
                    else                    sector = this.sectors[9];
                    _left = !_left;
                    break;
                case 6:
                case 14:
                    if(false == _right)     sector = this.sectors[5];
                    else                    sector = this.sectors[13];
                    _right = !_right;
                    break;
            }
            return sector;
        }


    }//..class BeltScript
}//..namespace MyCraft