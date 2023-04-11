using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyCraft
{
    public class BeltGoods : BlockScript
    {
        
        public int itemid;  //item id;
        public Vector3 dest;//도착위치
        public Vector3 forward;//진행방향
        public BeltSector sector;//자신이 올라가 있는 belt의 sector

        // Use this for initialization
        //protected override void Start()
        //{
        //    //_blocktype = BLOCKTYPE.RAW_WOOD;

        //    //base._bStart = true;
        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}


        //obj에 sector를 설정할지
        //sector에 object를 설정할지
        //결정하세요
        //public void SetSector(BeltSector sector)
        //{
        //    if (null == sector)
        //    {
        //        Debug.LogWarning("sector is null");
        //        return;
        //    }

        //    //prev sector
        //    if (null != this.sector)
        //    {
        //        this.sector.obj = null;
        //    }

        //    //next sector
        //    this.sector = sector;
        //    this.sector.obj = this;

        //    //위치
        //    this.transform.position = sector.transform.position;
        //    Debug.Log("init pos: " + this.transform.position);
        //}



    };//..class BeltGoods
}//..namespace MyCraf
