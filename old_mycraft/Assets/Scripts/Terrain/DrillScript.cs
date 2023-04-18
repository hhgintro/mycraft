using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MyCraft
{
    public class DrillScript : BlockScript
    {
        //private TerrainManager terrain_manager;

        //List<BeltGoods> prefabs_goods;

        //HG[2017.05.16] 방금 mining한 자원(석탄/철광석/...)
        //  중복 생성하지 않도록 obj를 맴버로 가지고 있어야 합니다.
        public BeltGoods _mineral = null;

        //belt와 달리 terrain에서 검색하지 않기때문에.
        //terrain에 생성되지 않은 상태(마우스 홀딩)에서도 작동하고 있어서
        //예외처리가 필요하여 변수를 추가합니다.
        //public bool running = false;


        void Start()
        {
            //_blocktype = BLOCKTYPE.DRILL;
            //if (null == base._itembase)
            //    base._itembase = GameManager.GetItemBase().FetchItemByID(this._itembase.id);

            //this.terrain_manager = GameObject.Find("Terrain").GetComponent<TerrainManager>();

            //this.prefabs_goods = new List<BeltGoods>();
            //this.prefabs_goods.Add(this.transform.Find("prefab_belt_goods").GetComponent<BeltGoods>());

            //BeltGoods obj = this.transform.parent.Find("prefab_belt_goods").GetComponent<BeltGoods>();
            //this.prefabs_goods.Add(obj);


            //terrain에 위치하게 되면 true로 활성화해 주어야 합니다.
            if (true == this._bOnTerrain)
            {
                base.SetMeshRender(1.0f);
                base._bStart = true;
                StartCoroutine(CheckMining());
            }
            else
            {
                //반투명하게...
                base.SetMeshRender(0.5f);
            }

        }

        //// Update is called once per frame
        //void Update()
        //{

        //}

        IEnumerator CheckMining()
        {
            while (true)
            {
                yield return new WaitForSeconds(((DrillItemBase)this._itembase).delay);
                StartCoroutine(CheckDelivery());
                break;
            }
        }

        IEnumerator CheckDelivery()
        {
            while (true)
            {
                bool end = CheckDelivery_Func();
                if (true == end)
                {
                    StartCoroutine(CheckMining());
                    break;//생산된 goods의 운반이 완료되었습니다.
                }

                yield return 0;
            }
        }
        bool CheckDelivery_Func()
        {
            //terrain에 위치하게 되면 true로 활성화해 주어야 합니다.
            //if (false == this.running)
            //    return false;

            BlockScript mineral = GameManager.GetTerrainManager().mineral_layer.GetBlock(this.transform.position);
            if (null == mineral)
            {
                //주변에 채취가 가능한 자원이 없습니다.
                //Debug.Log("not found mineral");
                return true;// false;
            }


            //HG_TEST : CHEST의 경우에는
            //      BeltGoods개체를 만들지 않아도 되므로 먼저 처리합니다.
            BlockScript script_front = GameManager.GetTerrainManager().block_layer.GetBlock(this.transform.position + this.transform.forward);
            if (null != script_front)
            {
                switch (script_front._itembase.type)
                {
                    case BLOCKTYPE.CHEST:
                    case BLOCKTYPE.STONE_FURNACE:
                        {
                            //놓을자리가 이미 찼다면...false를 리턴
                            if (true == script_front.PutdownGoods(mineral._itembase.id, 1))
                                return true;//지급완료
                        } break;
                }
            }


            //생성
            if (null == this._mineral)
            {
                //HG_TODO : item.json에서 설정된 값을 넣어줘야 합니다.
                //ID = MINERAL;
                this._mineral = GameManager.CreateMineral(mineral._itembase.id, this.transform);//, this.transform.position);
                //Hierarchy 위치설정
                //this.mineral.transform.SetParent(this.transform);
                //생성되는 mineral은 위치.
                this._mineral.transform.position = base.CheckDestPosFrontBlock(this._mineral);
            }

            //놓을자리가 이미 찼다면...
            //chest의 경우에는 BeltGoods개체를 만들지 않아도 되므로 위에서 먼저 처리합니다.
            //(함수내에서는 구문이 있어도 Drill에서는 처리하지 않음)
            if (false == base.CheckPutdownGoods(this._mineral))
                return false;

            //성공
            this._mineral = null;
            return true;
        }

        public override void DeleteBlock()
        {
            if (null == this._mineral)
                return;

            //가지고 있는 obj(BeltGoods)를 인벤토리에 넣어줍니다.
            int amount = GameManager.AddItem(this._mineral.itemid, 1);
            if (amount <= 0)
                GameObject.Destroy(this._mineral.gameObject);
        }

        //goods를 내려놓을 위치를 가져옵니다.
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
                //return target.PutdownGoods(BELT_ROW.LEFT, BELT_COL.SECOND, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW1, BELT_COL.SECOND);
            }
            //back
            else if (dot < -0.9f)
            {
                //Debug.Log("forward: back");
                //return target.PutdownGoods(BELT_ROW.RIGHT, BELT_COL.SECOND, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW2, BELT_COL.SECOND);
            }

            dot = Vector3.Dot(this.transform.right, script_front.transform.forward);
            //right
            if (0.9f < dot)
            {
                //Debug.Log("forward: right");
                //가까운쪽에 놓는다.
                //return target.PutdownGoods(BELT_ROW.RIGHT, BELT_COL.SECOND, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW2, BELT_COL.SECOND);
            }
            //left
            else if (dot < -0.9f)
            {
                //Debug.Log("forward: left");
                //가까운쪽에 놓는다.
                //return target.PutdownGoods(BELT_ROW.LEFT, BELT_COL.SECOND, goods);
                return script_front.GetBeltSector(BELT_ROW.ROW1, BELT_COL.SECOND);
            }
            return null;
        }


        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            ////mineral
            //int itemid = 0;
            //if (null != this.mineral) itemid = (int)this.mineral.itemid;
            //writer.Write(itemid);
            ////...

        }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            ////mineral
            //int itemid = reader.ReadInt32();
            //if (0 != itemid)
            //    this.mineral = GameManager.CreateMineral(itemid, this.transform);//, this.transform.position);
            ////....

        }

    }
}