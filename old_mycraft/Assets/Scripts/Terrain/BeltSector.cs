using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace MyCraft
{
    public class BeltSector : MonoBehaviour
    {
        public BeltBaseScript _owner;    //this개체가 소속된 곳

        public BeltGoods _obj = null; //등록된 object
        public BeltSector _next;

        private void Awake()
        {
            this._owner = this.transform.parent.parent.GetComponent<BeltBaseScript>();
        }



        public BeltGoods GetObj() { return this._obj; }
        public void SetObj(BeltGoods obj)
        {
            this._obj = obj;
            if(null != this._obj)
            {
                this._obj.sector = this;
                obj.transform.SetParent(this.transform);
            }
        }

        public void Save(BinaryWriter writer)
        {
            //base.Save(writer);

            //BeltGoods
            int itemid = 0;
            if (null != this._obj) itemid = (int)this._obj.itemid;
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
                this._obj = GameManager.CreateMineral(itemid, this.transform);//, this.transform.position);
                //Hierarchy 위치설정
                //this.obj.transform.SetParent(this.transform);
                ////생성되는 mineral은 위치.
                //this.obj.transform.position = this.owner.CheckDestPosFrontBlock(this.obj);

                //sector 설정
                this._obj.sector = this;
                //생성되는 mineral은 위치.
                this._obj.transform.position = this.transform.position;


            }
            //....

        }

    };//..class BeltSector
}//..namespace MyCraft