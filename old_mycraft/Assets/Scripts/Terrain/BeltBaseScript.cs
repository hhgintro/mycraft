using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    public class BeltBaseScript : BlockScript
    {

        //belt를 좌우 각각 4부분으로 나눈다.(운반체를 최대 8개까지 올릴수 있다.)
        public BeltSector[] sectors;

        void Start()
        {
            if (true == base._bOnTerrain)
            {
                //base.SetMeshRender(1.0f);
                //base._bStart = true;
                StartCoroutine(TranslateObject());
            }
            else
            {
                //반투명하게...
                //base.SetMeshRender(0.5f);
            }
        }


        IEnumerator TranslateObject()
        {
            while (true)
            {
                TranslateObject_Func();
                yield return 0;
            }
        }

        void TranslateObject_Func()
        {
            //Debug.Log("Check TranslateObject()");
            for (int i = 0; i < this.sectors.Length; ++i)
            {
                if (null == this.sectors[i].GetObj())
                    continue;

                //if (null == this.sectors[i].GetObj().dest)
                //    continue;

                //도착여부
                Vector3 lookat = this.sectors[i].GetObj().dest - this.sectors[i].GetObj().transform.position;
                if (0f < Vector3.Dot(lookat, this.sectors[i].GetObj().forward))
                {
                    float speed = ((BeltItemBase)this.sectors[i].GetObj().sector._owner._itembase).speed;//현 sector의 부모(owner)로 부터 speed의 정보를 가져옵니다.
                    this.sectors[i].GetObj().transform.position += this.sectors[i].GetObj().forward * speed * Time.smoothDeltaTime;
                    //Debug.Log("cur pos : " + this.sectors[i].GetObj().transform.position);
                    continue;
                }

                BeltSector dest = this.GetNextSector(i);
                //next setor가 없다.
                if (null == dest)
                    continue;
                //이미 점유중
                if (null != dest.GetObj())
                    continue;

                //다음 위치 설정
                this.sectors[i].GetObj().dest = dest.transform.position;
                this.sectors[i].GetObj().forward = this.sectors[i].GetObj().dest - this.sectors[i].GetObj().transform.position;
                this.sectors[i].GetObj().forward.Normalize();
                //Debug.Log("dest pos : " + this.sectors[i].GetObj().dest);

                //sector간의 goods 전달
                this.sectors[i].GetObj().transform.SetParent(dest.transform);
                dest.SetObj(this.sectors[i].GetObj());
                this.sectors[i].SetObj(null);
            }
        }

        public override void DeleteBlock()
        {
            for (int i = 0; i < this.sectors.Length; ++i)
            {
                if (null == this.sectors[i]._obj)
                    continue;

                //sector가 가지고 있는 obj(BeltGoods)를 인벤토리에 넣어줍니다.
                int amount = GameManager.AddItem(this.sectors[i]._obj.itemid, 1);
                if(amount <= 0)
                    GameObject.Destroy(this.sectors[i]._obj.gameObject);
            }
        }

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
            int weight = this.CheckWeightChainBlock();

            bool turn_front = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.FRONT);
            //bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            //bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);

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
                    if (SENSOR.RF == base._lb._sensor)
                        base._lb._owner.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                    else
                        base._lb._owner.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
                }
                ////left에서 영향을 받고 있나?
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

                ////right에서 영향을 받고 있나?
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

                return;//****중요(더 아래로 진행못하게)
            }

            //if (true == turn_left)
            //{
            //    //left에 belt가 있다면
            //    //[자신]은 left 상태임(left 에 belt가 있으므로)
            //    //BeltScript script_left = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(block.transform.position - block.transform.right);
            //    //LinkBeltSector(script_left, (BeltScript)block, BELT_ROW.ROW1, BELT_COL.SECOND, BELT_ROW.ROW2, BELT_COL.FORTH);
            //    BlockScript block_lleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right + this.transform.forward);
            //    BlockScript block_mleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right);
            //    if (null != block_mleft)    //script_mleft: 무조건 null이 아니어야 한다.
            //    {
            //        if (block_lleft == block_mleft)
            //            block_mleft.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //        else
            //            block_mleft.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //    }
            //}

            //if (true == turn_right)
            //{
            //    //right에 belt가 있다면
            //    //[자신]은 right 상태임(right 에 belt가 있으므로)
            //    //BeltScript script_right = (BeltScript)GameManager.GetTerrainManager().block_layer.GetBlock(block.transform.position + block.transform.right);
            //    //LinkBeltSector(script_right, (BeltScript)block, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.SECOND);
            //    BlockScript block_lright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right - this.transform.forward);
            //    BlockScript block_mright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right);
            //    if (null != block_mright)    //script_mleft: 무조건 null이 아니어야 한다.
            //    {
            //        if (block_lright == block_mright)
            //            block_mright.LinkBeltSector(BELT_ROW.ROW3, BELT_ROW.ROW4, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //        else
            //            block_mright.LinkBeltSector(BELT_ROW.ROW1, BELT_ROW.ROW2, this, BELT_ROW.ROW1, BELT_COL.FORTH, BELT_ROW.ROW2, BELT_COL.FORTH);
            //    }
            //}
        }

        // [자신]을 기준으로 back / left / right 의 belt 위치에 따라 [자신의] 가중치를 결정합니다.
        public override int CheckWeightChainBlock()
        {
            //주변 block에 의한 가중치
            int weight = 0;

            if (base._lb) weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);
            return weight;

            ////lback, mback : left-back, middle-back ( 뒤쪽에 belt/spliter가 있을때를 고려함 )
            ////BlockScript script_lback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward - this.transform.right);
            //BlockScript script_mback = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward);
            //////BlockScript script_lleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right + this.transform.up);
            ////BlockScript script_mleft = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.right);
            //////BlockScript script_lright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right - this.transform.forward);
            ////BlockScript script_mright = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.right);

            ////back
            ////if (true == this.WeightTurn(script_lback, this.transform.forward)
            ////    || true == this.WeightTurn(script_mback, this.transform.forward))
            //if(true == this.WeightTurn(script_mback, this.transform.forward))
            //    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);
            //////left
            //////if (true == this.WeightTurn(script_lleft, this.transform.right)
            //////    || true == this.WeightTurn(script_mleft, this.transform.right))
            ////if(true == this.WeightTurn(script_mleft, this.transform.right))
            ////    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.LEFT);
            //////right
            //////if (true == this.WeightTurn(script_lright, -this.transform.right)
            //////    || true == this.WeightTurn(script_mright, -this.transform.right))
            ////if(true == this.WeightTurn(script_mright, -this.transform.right))
            ////    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.RIGHT);

            //////check
            ////bool turn_left = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.LEFT);
            ////bool turn_right = Common.CHECK_BIT(weight, (int)TURN_WEIGHT.RIGHT);
            ////if (0 == weight || (true == turn_left && true == turn_right)) //외압이 없거나, left/right모두에서 올때
            ////    weight = Common.ADD_BIT(weight, (int)TURN_WEIGHT.FRONT);

            //return weight;  //무조건 3개중 1개 => TURN_WEIGHT.FRONT / TURN_WEIGHT.LEFT / TURN_WEIGHT.RIGHT
        }

        //script의 forward와 일치히면 true를 리턴합니다.
        protected bool WeightTurn(BlockScript script, Vector3 forward)
        {
            //blocktype
            //if (null == script || null == script._itembase || BLOCKTYPE.BELT != script._itembase.type)
            //    return false;
            if (null == script || false == script.IsTransport())
                return false;

            //forward(오차발생...허용범위로 체크)
            float err_bound = 0.01f;
            float angle = Vector3.Angle(script.transform.forward, forward);
            if (angle < -err_bound || err_bound < angle)
                return false;

            return true;//가중치 적용
        }

        protected virtual BeltSector GetNextSector(int idx)
        {
            return this.sectors[idx]._next;
        }

        public override void LinkBeltSector(BELT_ROW row1, BELT_ROW row2, BlockScript next, BELT_ROW lrow, BELT_COL lcol, BELT_ROW rrow, BELT_COL rcol)
        {
            if (null == next || false == next.IsTransport())
                return;

            this.GetBeltSector(row1, BELT_COL.FIRST)._next = ((BeltBaseScript)next).GetBeltSector(lrow, lcol);
            this.GetBeltSector(row2, BELT_COL.FIRST)._next = ((BeltBaseScript)next).GetBeltSector(rrow, rcol);
            //Debug.LogWarning($"LinkBelt: ({this._index}){this.name} ==> ({next._index}){next.name}");
        }

        public BeltSector GetBeltSector(BELT_ROW r, BELT_COL c)
        {
            int idx = ((int)r * (int)BELT_COL.MAX) + (int)c;
            if (sectors.Length <= idx)
            {
                Debug.LogError($"{this.name}:sector수({sectors.Length})를 확인하세요");
                return null;
            }
            return sectors[idx];
        }

        public override bool PutdownGoods(BELT_ROW row, BELT_COL col, BeltGoods goods)
        {
            //이미 점유중
            BeltSector sector = this.GetBeltSector(row, col);
            if (null != sector.GetObj())
                return false; //이미 점유중

            ////생성
            //GameObject obj = UnityEngine.Object.Instantiate(this.prefabs_goods[0].gameObject);
            //obj.SetActive(true);

            //position
            //HG_TODO : 현재위치 보간이 필요합니다. 넣을때 튀는현상발생.
            //goods.transform.position = sector.transform.position;
            
            //dest position
            goods.dest = sector.transform.position;

            //sector에 등록
            sector.SetObj(goods);   //sector에 obj 등록

            //belt에 등록(이동을 목적으로)
            //this.goods.Add(goods);

            return true;
        }

        public override bool PutdownGoods(BeltGoods goods)
        {
            if (null == goods.sector)
            {
                Debug.LogError("Error: goods의 sector를 설정해 주세요");
                return false;
            }

            //이미 점유중
            if (null != goods.sector.GetObj())
                return false; //이미 점유중

            //sector에 등록
            goods.sector.SetObj(goods);   //sector에 obj 등록
            return true;
        }

        //[자신]을 기준으로 inserter가 물건을 가져갈때 우선 순위.
        private List<BeltSector> PickupInserter(BlockScript inserter)
        {
            List<BeltSector> s = new List<BeltSector>();

            //insert type에 따른 거리를 판단해준다.
            int LEN = 1; //long inserter의 경우는 2

            //front: 앞쪽에 insert가 있다.
            if (Vector3.Distance(inserter.transform.position, this.transform.position + this.transform.forward * LEN) < 0.5f)
            {
                s.Add(this.sectors[0]);
                s.Add(this.sectors[1]);
                s.Add(this.sectors[2]);
                s.Add(this.sectors[3]);
                s.Add(this.sectors[4]);
                s.Add(this.sectors[5]);
                s.Add(this.sectors[6]);
                s.Add(this.sectors[7]);
                return s;
            }
            //back: 뒤쪽에 inserter가 있다.
            if (Vector3.Distance(inserter.transform.position, this.transform.position - this.transform.forward * LEN) < 0.5f)
            {
                s.Add(this.sectors[0]);
                s.Add(this.sectors[1]);
                s.Add(this.sectors[2]);
                s.Add(this.sectors[3]);
                s.Add(this.sectors[4]);
                s.Add(this.sectors[5]);
                s.Add(this.sectors[6]);
                s.Add(this.sectors[7]);
                return s;
            }
            //right: 오른쪽에 inserter가 있다.
            if (Vector3.Distance(inserter.transform.position, this.transform.position + this.transform.right * LEN) < 0.5f)
            {
                s.Add(this.sectors[4]);
                s.Add(this.sectors[5]);
                s.Add(this.sectors[6]);
                s.Add(this.sectors[7]);
                s.Add(this.sectors[0]);
                s.Add(this.sectors[1]);
                s.Add(this.sectors[2]);
                s.Add(this.sectors[3]);
                return s;
            }
            //left: 왼쪽에 inserter가 있다.
            if (Vector3.Distance(inserter.transform.position, this.transform.position - this.transform.right * LEN) < 0.5f)
            {
                s.Add(this.sectors[0]);
                s.Add(this.sectors[1]);
                s.Add(this.sectors[2]);
                s.Add(this.sectors[3]);
                s.Add(this.sectors[4]);
                s.Add(this.sectors[5]);
                s.Add(this.sectors[6]);
                s.Add(this.sectors[7]);
                return s;
            }
            return null;
        }

        private BeltGoods PickupSector(List<BeltSector> sectors, int itemid)
        {
            //sector를 순회하면서 아이템을 가져오고 있습니다.
            for (int s = 0; s < sectors.Count; ++s)
            {
                if (null == sectors[s]._obj)
                    continue;
                if (0 != itemid && itemid != sectors[s]._obj.itemid)
                    continue;

                BeltGoods obj = sectors[s]._obj;
                sectors[s]._obj = null;
                return obj;
            }
            return null;
        }
        //inserter: 물건을 가져가는 로봇팔
        //dutdonws: block_front 에 넣을 수 있는 itemid(null인 경우도 있다. 필요한 경우에 사용하세요.)
        public override BeltGoods PickupGoods(BlockScript inserter, List<int> putdowns/*null*/)
        {
            //방향.
            List<BeltSector> sectors = this.PickupInserter(inserter);
            if (sectors == null) return null;

            //chest인 경우
            if(null == putdowns)
                return PickupSector(sectors, 0);

            for (int i = 0; i < putdowns.Count; ++i)
            {
                BeltGoods goods = PickupSector(sectors, putdowns[i]);
                if (null != goods) return goods;
            }

            return null;
        }
        //public override BeltGoods PickupGoods(BlockScript script_front)
        //{
        //    if (null == script_front)
        //        return null;

        //    //HG_TODO : sector를 순회하면서 아이템을 가져오고 있습니다.
        //    //          아이템을 가져올 우선 순위를 정함도 좋을 듯 싶습니다.
        //    for (int i = 0; i < this.sectors.Length; ++i)
        //    {
        //        if (null == this.sectors[i].obj)
        //            continue;

        //        //front에 넣을 수 없다면...다음꺼를 찾는다.
        //        if (false == script_front.CheckPutdownGoods(this.sectors[i].obj.itemid))
        //            continue;


        //        BeltGoods obj = this.sectors[i].obj;
        //        this.sectors[i].obj = null;
        //        return obj;
        //    }
        //    return null;
        //}


        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            //sector
            for(int i=0; i<this.sectors.Length; ++i)
                this.sectors[i].Save(writer);
            //...

        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            //sector
            for (int i = 0; i < this.sectors.Length; ++i)
                this.sectors[i].Load(reader);
            //...
        }


    }//..class BeltScript
}//..namespace MyCraft