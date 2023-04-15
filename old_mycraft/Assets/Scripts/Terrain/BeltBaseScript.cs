﻿using System.Collections;
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
                base.SetMeshRender(1.0f);
                StartCoroutine(TranslateObject());
            }
            else
            {
                //반투명하게...
                base.SetMeshRender(0.5f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            //TranslateObject_Func();
        }

        IEnumerator TranslateObject()
        {
            while (true)
            {
                TranslateObject_Func();
                yield return 0;
            }
        }

        protected virtual BeltSector GetNextSector(int idx)
        {
            return this.sectors[idx].next;
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
                    if (IsSpliter())
                    {
                        int a = 0;
                        a = 0;
                    }
                    float speed = ((BeltItemBase)this.sectors[i].GetObj().sector.owner._itembase).speed;//현 sector의 부모(owner)로 부터 speed의 정보를 가져옵니다.
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
                if (null == this.sectors[i].obj)
                    continue;

                //sector가 가지고 있는 obj(BeltGoods)를 인벤토리에 넣어줍니다.
                int amount = GameManager.AddItem(this.sectors[i].obj.itemid, 1);
                if(amount <= 0)
                    GameObject.Destroy(this.sectors[i].obj.gameObject);
            }
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

        public override void LinkBeltSector(BELT_ROW row1, BELT_ROW row2, BlockScript next, BELT_ROW lrow, BELT_COL lcol, BELT_ROW rrow, BELT_COL rcol)
        {
            if (null == next || false == next.IsTransport())
                return;

            this.GetBeltSector(row1, BELT_COL.FIRST).next = ((BeltBaseScript)next).GetBeltSector(lrow, lcol);
            this.GetBeltSector(row2, BELT_COL.FIRST).next = ((BeltBaseScript)next).GetBeltSector(rrow, rcol);

        }

        public BeltSector GetBeltSector(BELT_ROW r, BELT_COL c)
        {
            int idx = ((int)r * (int)BELT_COL.MAX) + (int)c;
            if (sectors.Length <= idx)
                return null;
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

        public override BeltGoods PickupGoods(BlockScript script_front)
        {
            if (null == script_front)
                return null;

            //HG_TODO : sector를 순회하면서 아이템을 가져오고 있습니다.
            //          아이템을 가져올 우선 순위를 정함도 좋을 듯 싶습니다.
            for (int i = 0; i < this.sectors.Length; ++i)
            {
                if (null == this.sectors[i].obj)
                    continue;

                //front에 넣을 수 없다면...다음꺼를 찾는다.
                if (false == script_front.CheckPutdownGoods(this.sectors[i].obj.itemid))
                    continue;


                BeltGoods obj = this.sectors[i].obj;
                this.sectors[i].obj = null;
                return obj;
            }
            return null;
        }


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
            //생성을 위해 먼저 처리되므로, 여기에서는 처리하지 않습니다.
            //int turn_weight = reader.ReadInt32
            base.Load(reader);

            //sector
            for (int i = 0; i < this.sectors.Length; ++i)
                this.sectors[i].Load(reader);
            //...
        }


    }//..class BeltScript
}//..namespace MyCraft