using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace MyCraft
{
    public class BeltSector : MonoBehaviour
    {
        public BeltBaseScript owner;    //this개체가 소속된 곳

        public BeltGoods obj = null; //등록된 object
        public BeltSector next;



        public BeltGoods GetObj() { return this.obj; }
        public void SetObj(BeltGoods obj)
        {
            this.obj = obj;
            if(null != this.obj)
            {
                this.obj.sector = this;
                obj.transform.SetParent(this.transform);
            }
        }

        public void Save(BinaryWriter writer)
        {
            //base.Save(writer);

            //BeltGoods
            int itemid = 0;
            if (null != this.obj) itemid = (int)this.obj.itemid;
            writer.Write(itemid);
            //...

        }

        public void Load(BinaryReader reader)
        {
            //base.Load(reader);

            //BeltGoods
            int itemid = reader.ReadInt32();
            if (0 != itemid)
            {
                this.obj = GameManager.CreateMineral(itemid, this.transform);//, this.transform.position);
                //Hierarchy 위치설정
                //this.obj.transform.SetParent(this.transform);
                ////생성되는 mineral은 위치.
                //this.obj.transform.position = this.owner.CheckDestPosFrontBlock(this.obj);

                //sector 설정
                this.obj.sector = this;
                //생성되는 mineral은 위치.
                this.obj.transform.position = this.transform.position;


            }
            //....

        }

    };//..class BeltSector
}//..namespace MyCraft