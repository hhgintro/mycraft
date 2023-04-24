using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    public class BeltScript : BeltBaseScript
    {
        public TURN_WEIGHT turn_weight;


        //새로 생성된 script의 back/left/right에서 link를 걸어줍니다.
        public override void LinkedBelt()
        {
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return;
            if (false == this.IsBelt())
                return;

            //Debug.Log("LinkBetlt " + script._index);
            // [자신]을 기준으로 back / left / right 의 belt 위치에 따라
            // [자신의] 가중치를 결정합니다.
            int weight = this.CheckWeightChainBelt();

            bool turn_front = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT);
            bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //back으로 부터( [자신]의 가중치가 front - back이 belt가 있다)
            if (true == turn_front)
            {
                //**************************************************************//
                //영향력이 확인되었으므로 상태체크를 하지 않는다.( null / blocktype )


                ////back(back에 belt가 있다면) - [자신]은 front 상태임(back에 belt가 있으므로)
                ////BeltScript script_back = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.forward);
                ////LinkBeltSector(script_back, (BeltScript)script, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //BlockScript block_lback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward - this.transform.right);
                //BlockScript block_mback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward);
                //if (null != block_mback)    //script_mback: 무조건 null이 아니어야 한다.
                //{
                //    if (block_lback == block_mback)
                //        block_mback.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //    else
                //        block_mback.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //}
                if (base._lb)
                {
                    if(SENSOR.RF == base._lb._sensor)
                        base._lb._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                    else
                        base._lb._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                }

                //left에서 영향을 받고 있나?
                //if (true == turn_left)
                //{
                //    //left에 belt가 있다면 - [자신]은 front 상태임(back에 belt가 있으므로)
                //    //BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position - script.transform.right);
                //    //LinkBeltSector(script_left, (BeltScript)script, BELT_ROW.ROW1, BELT_COL.FIRST, BELT_ROW.ROW1, BELT_COL.THIRD);
                //    BlockScript block_lleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right + this.transform.forward);
                //    BlockScript block_mleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right);
                //    if (null != block_mleft)    //script_mleft: 무조건 null이 아니어야 한다.
                //    {
                //        if (block_lleft == block_mleft)
                //            block_mleft.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FIRST, BELT_ROW.ROW1, BELT_COL.THIRD);
                //        else
                //            block_mleft.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FIRST, BELT_ROW.ROW1, BELT_COL.THIRD);
                //    }
                //}
                if (base._L)
                {
                    if (SENSOR.RF == base._L._sensor)
                        base._L._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FIRST, BELT_ROW.ROW1, BELT_COL.THIRD);
                    else
                        base._L._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FIRST, BELT_ROW.ROW1, BELT_COL.THIRD);
                }

                //right에서 영향을 받고 있나?
                //if (true == turn_right)
                //{
                //    //right에 belt가 있다면 - [자신]은 front 상태임(back에 belt가 있으므로)
                //    //BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(script.transform.position + script.transform.right);
                //    //LinkBeltSector(script_right, (BeltScript)script, BELT_ROW.ROW2, BELT_COL.THIRD, BELT_ROW.ROW2, BELT_COL.FIRST);
                //    BlockScript block_lright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right - this.transform.forward);
                //    BlockScript block_mright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right);
                //    if (null != block_mright)    //script_mleft: 무조건 null이 아니어야 한다.
                //    {
                //        if (block_lright == block_mright)
                //            block_mright.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW2, BELT_COL.THIRD, BELT_ROW.ROW2, BELT_COL.FIRST);
                //        else
                //            block_mright.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW2, BELT_COL.THIRD, BELT_ROW.ROW2, BELT_COL.FIRST);
                //    }
                //}
                if(base._R)
                {
                    if (SENSOR.RF == base._R._sensor)
                        base._R._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW2, BELT_COL.THIRD, BELT_ROW.ROW2, BELT_COL.FIRST);
                    else
                        base._R._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW2, BELT_COL.THIRD, BELT_ROW.ROW2, BELT_COL.FIRST);

                }

                return;//****중요(더 아래로 진행못하게)
            }

            //left에 belt가 있다면
            //[자신]은 left 상태임(left 에 belt가 있으므로)
            if (true == turn_left)  //HG_TODO: 이부분이 필요한가???(아래 base._L 만 체크하면 되지 않을까)
            {
                ////BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(block.transform.position - block.transform.right);
                ////LinkBeltSector(script_left, (BeltScript)block, BELT_ROW.ROW1, BELT_COL.SECOND, BELT_ROW.ROW2, BELT_COL.FORTH);
                //BlockScript block_lleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right + this.transform.forward);
                //BlockScript block_mleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right);
                //if (null != block_mleft)    //script_mleft: 무조건 null이 아니어야 한다.
                //{
                //    if (block_lleft == block_mleft)
                //        block_mleft.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //    else
                //        block_mleft.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //}
                if(base._L)
                {
                    if (SENSOR.RF == base._L._sensor)
                        base._L._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                    else
                        base._L._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                }
            }

            //right에 belt가 있다면
            //[자신]은 right 상태임(right 에 belt가 있으므로)
            if (true == turn_right)//HG_TODO: 이부분이 필요한가???(아래 base._R 만 체크하면 되지 않을까)
            {
                ////BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(block.transform.position + block.transform.right);
                ////LinkBeltSector(script_right, (BeltScript)block, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.SECOND);
                //BlockScript block_lright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right - this.transform.forward);
                //BlockScript block_mright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right);
                //if (null != block_mright)    //script_mleft: 무조건 null이 아니어야 한다.
                //{
                //    if (block_lright == block_mright)
                //        block_mright.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //    else
                //        block_mright.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                //}
                if(base._R)
                {
                    if (SENSOR.RF == base._R._sensor)
                        base._R._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                    else
                        base._R._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);

                }
            }
        }

        // [자신]을 기준으로 back / left / right 의 belt 위치에 따라 [자신의] 가중치를 결정합니다.
        public override int CheckWeightChainBelt()
        {
            //주변 block에 의한 가중치
            int weight = 0;

            if(base._lb)    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);
            if(base._L)     weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.LEFT);
            if(base._R)     weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //check
            bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);
            if (true == turn_left && true == turn_right) //left/right모두에서 올때
                weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);

            return weight;


            ////lback, mback : left-back, middle-back ( 뒤쪽에 belt/spliter가 있을때를 고려함 )
            ////BlockScript script_lback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward - this.transform.right);
            //BlockScript script_mback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward);
            ////BlockScript script_lleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right + this.transform.up);
            //BlockScript script_mleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right);
            ////BlockScript script_lright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right - this.transform.forward);
            //BlockScript script_mright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right);


            ////back
            ////if (true == this.WeightTurn(script_lback, this.transform.forward)
            ////    || true == this.WeightTurn(script_mback, this.transform.forward))
            //if(true == this.WeightTurn(script_mback, this.transform.forward))
            //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);
            ////left
            ////if (true == this.WeightTurn(script_lleft, this.transform.right)
            ////    || true == this.WeightTurn(script_mleft, this.transform.right))
            //if(true == this.WeightTurn(script_mleft, this.transform.right))
            //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.LEFT);
            ////right
            ////if (true == this.WeightTurn(script_lright, -this.transform.right)
            ////    || true == this.WeightTurn(script_mright, -this.transform.right))
            //if(true == this.WeightTurn(script_mright, -this.transform.right))
            //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            ////check
            //bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            //bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);
            //if (true == turn_left && true == turn_right) //left/right모두에서 올때
            //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);

            //return weight;
        }

        public override void Save(BinaryWriter writer)
        {
            writer.Write((byte)this.turn_weight);

            base.Save(writer);
        }

        public override void Load(BinaryReader reader)
        {
            //생성을 위해 먼저 처리되므로, 여기에서는 처리하지 않습니다.
            //byte turn_weight = reader.ReadByte()

            base.Load(reader);
        }


    }//..class BeltScript
}//..namespace MyCraft