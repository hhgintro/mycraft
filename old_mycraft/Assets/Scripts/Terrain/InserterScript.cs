using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class InserterScript : BlockScript
    {
        //private TerrainManager terrain_manager;

        private BeltGoods obj = null;

        //belt와 달리 terrain에서 검색하지 않기때문에.
        //terrain에 생성되지 않은 상태(마우스 홀딩)에서도 작동하고 있어서
        //예외처리가 필요하여 변수를 추가합니다.
        //public bool running { get; set; }

        //HG_TODO : inserter자체가 가지고 있는 ani로 control(속도)해야할 부분인데
        //          현재는 dest를 적용하기위해 임시로 적용합니다.
        //public float speed = 10f;


        void Start()
        {
            //_blocktype = BLOCKTYPE.INSERTER;
            if (null == base._itembase)
                base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            //this.terrain_manager = GameObject.Find("Terrain").GetComponent<TerrainManager>();


            if (true == base._bOnTerrain)
            {
                //base.SetMeshRender(1.0f);
                //base._bStart = true;
                StartCoroutine(CheckBackBlock());
            }
            else
            {
                //반투명하게...
                //base.SetMeshRender(0.5f);
            }
        }

        IEnumerator CheckBackBlock()
        {
            while(true)
            {
                bool end = CheckBackBlock_Func();
                if(true == end)
                    break;//goods를 운반하는 동안에는 사용하지 않는다.(CheckDelivery())

                yield return 0;
            }
        }

        //script_back의 belt에 goods가 있으면...Delivery Coroutine을 생성한다.
        bool CheckBackBlock_Func()
        {
            //Debug.Log("Check Belt()");
            if (false == base._bOnTerrain)
                return false;

            //front가 없으면 구지 꺼내오지 않는다.
            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.forward);
            if (null == script_front)
                return false;

            BlockScript script_back = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position - this.transform.forward);
            if (null == script_back)
                return false;

            switch(script_back._itembase.type)
            {
                case BLOCKTYPE.BELT:
                case BLOCKTYPE.BELT_UP:
                case BLOCKTYPE.BELT_DOWN:
                case BLOCKTYPE.SPLITER:
                case BLOCKTYPE.CHEST:
                case BLOCKTYPE.STONE_FURNACE:
                case BLOCKTYPE.MACHINE:
                    this.obj = script_back.PickupGoods(this, script_front);
                    break;

                default:
                    return false;
            }

            if (null != this.obj)
            {
                //HG_TODO : Inserter가 goods를 옮기도록 처리하지만,
                //          현재는 dest position을 설정하여 이동하도록 처리합니다.

                //생성위치: 2x2화로에서 아이템생성하면 기준위치에서만 생성되므로 inserter의 back으로 설정해 줍니다.
                this.obj.transform.position = this.transform.position - this.transform.forward + this.transform.up;
                //다음 위치 설정
                //this.obj.transform.SetParent(this.transform);
                this.obj.dest = this.CheckDestPosFrontBlock(this.obj);
                this.obj.forward = this.transform.forward;// this.obj.dest - this.transform.position;
                //this.obj.forward.Normalize();
                StartCoroutine(CheckDelivery());
                return true;
            }
            return false;
        }

        //private BeltGoods PickupGoodsFromBelt(BlockScript script)
        //{
        //    //HG_TODO : sector를 순회하면서 아이템을 가져오고 있습니다.
        //    //          아이템을 가져올 우선 순위를 정함도 좋을 듯 싶습니다.
        //    for (int i = 0; i < ((BeltScript)script).sectors.Length; ++i)
        //    {
        //        if (null == ((BeltScript)script).sectors[i].obj)
        //            continue;

        //        BeltGoods obj = ((BeltScript)script).sectors[i].obj;
        //        ((BeltScript)script).sectors[i].obj = null;
        //        return obj;
        //    }
        //    return null;
        //}

        IEnumerator CheckDelivery()
        {
            while (true)
            {
                bool end = CheckDelivery_Func();
                if (true == end)
                {
                    StartCoroutine(CheckBackBlock());
                    break;//goods를 확인하는 동안에는 사용하지 않는다.(CheckBelt())
                }

                yield return 0;
            }
 

        }

        //goods를 목표점(belt가 아니면, terrain에, 그도 아니면, 계속 들고 있어야 합니다.)
        // 전달은 완료하면 CheckBelt Coroutine을 생성합니다.
        bool CheckDelivery_Func()
        {
            if (null == this.obj)
                return false;

            //Debug.Log("Check Delivery()");
            if (null == this.obj)
                return false;

            //HG_TODO : 
            //도착여부
            Vector3 lookat = this.obj.dest - this.obj.transform.position;
            if (0f < Vector3.Dot(lookat, this.obj.forward))
            {
                //float speed = this.obj.sector.owner.speed;//현 sector의 부모(owner)로 부터 speed의 정보를 가져옵니다.
                this.obj.transform.position += this.obj.forward * ((InserterItemBase)this._itembase).speed * Time.smoothDeltaTime;
                //Debug.Log("cur pos : " + this.sectors[i].GetObj().transform.position);
                return false;
            }


            //HG_TODO : terrain에 goods를 올려놓을지는 고민해 봐야겠네요.
            //..

            //BlockScript script_front = GameManager.GetTerrainManager().GetBlock(this.transform.position + this.transform.forward);
            //if (null == script_front)
            //    return false;

            //switch(script_front.blocktype)
            //{
            //    case BLOCKTYPE.BELT:
            //        {
            //            //놓을자리가 이미 찼다면...
            //            if (false == script_front.PutdownGoods(BELT_ROW.RIGHT, this.obj.gameObject))
            //                return false;
            //        }
            //        break;

            //    case BLOCKTYPE.CHEST:
            //        {
            //            //놓을자리가 이미 찼다면...
            //            if (false == script_front.PutdownGoods(BELT_ROW.RIGHT, this.obj.gameObject))
            //                return false;
            //        }
            //        break;

            //    default:
            //        return false;
            //}
            //놓을자리가 이미 찼다면...
            if (false == base.CheckPutdownGoods(this.obj))
                return false;


            //if (BLOCKTYPE.BELT != script.blocktype)
            //    return false;

            ////놓을자리가 이미 찼다면...
            //if (false == script.PutdownGoods(BELT_ROW.RIGHT, this.obj.gameObject))
            //    return false;

            //성공
            this.obj = null;
            return true;
        }

        public override void DeleteBlock()
        {
            if (null == this.obj)
                return;

            //가지고 있는 obj(BeltGoods)를 인벤토리에 넣어줍니다.
            int amount = GameManager.AddItem(this.obj.itemid, 1);
            if (amount <= 0)
                GameObject.Destroy(this.obj.gameObject);
        }

        public override BeltSector GetBeltSector(BeltScript script_front)
        {
            //if (null == script_front || null == script_front._itembase || BLOCKTYPE.BELT != script_front._itembase.type)
            //    return null;
            if (null == script_front || false == script_front.IsBelt())
                return null;

            float dot = Vector3.Dot(this.transform.forward, script_front.transform.forward);
            //front
            if (0.9f < dot)
            {
                //Debug.Log("forward: front");
                //return target.PutdownGoods(BELT_ROW.LEFT, BELT_COL.FORTH, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW1, BELT_COL.FORTH);
            }
            //back
            else if (dot < -0.9f)
            {
                //Debug.Log("forward: back");
                //return target.PutdownGoods(BELT_ROW.RIGHT, BELT_COL.FIRST, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW2, BELT_COL.FIRST);
            }

            dot = Vector3.Dot(this.transform.right, script_front.transform.forward);
            //right
            if (0.9f < dot)
            {
                //Debug.Log("forward: right");
                //먼쪽에 놓는다.
                //return target.PutdownGoods(BELT_ROW.LEFT, BELT_COL.SECOND, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW1, BELT_COL.SECOND);
            }
            //left
            else if (dot < -0.9f)
            {
                //Debug.Log("forward: left");
                //먼쪽에 놓는다.
                //return target.PutdownGoods(BELT_ROW.RIGHT, BELT_COL.SECOND, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW2, BELT_COL.SECOND);
            }
            return null;
        }

    }//..class InserterScript
}