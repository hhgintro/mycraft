using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyCraft
{
    public class BlockLeaf : BlockScript
    {
        ////육면체의 각 면체를 담당하는 class
        ////Block은 6개의 leaf으로 구성되었습니다.
        ////belt의 경우는 leaf를 구성하지 않았습니다.

        ////void Awake()
        ////{
        ////    //위쪽에 다른 block이 있는 경우 up을 그리지 않는다.(모든 방향의 block도 동일하게 처리)
        ////    //this.up.SetActive(false);
        ////    //Reflesh(DIRECTION.UP, false);
        ////}

        // Use this for initialization

        ////// Update is called once per frame
        ////void Update () {

        ////}

        ////private override void DeleteBlock(GameObject obj)

        //public override GameObject GetBodyObject()
        //{
        //    return this.transform.parent.gameObject;
        //}

    }//..class BlockLeaf
}//..namespace MyCraft