using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyCraft
{
    public class Grass : BlockScript
    {
        //public GameObject front;
        //public GameObject back;
        //public GameObject left;
        //public GameObject right;
        //public GameObject up;
        //public GameObject down;

        //void Awake()
        //{
        //    //위쪽에 다른 block이 있는 경우 up을 그리지 않는다.(모든 방향의 block도 동일하게 처리)
        //    //this.up.SetActive(false);
        //    //Reflesh(DIRECTION.UP, false);
        //}

        // Use this for initialization
        //void Start()
        //{
        //    //_blocktype = BLOCKTYPE.RAW_WOOD;

        //}

        ////// Update is called once per frame
        ////void Update () {

        ////}

        ///*
        // * direct 방향의 면체를 활성/비활성합니다.
        // * */
        //public override void Refresh(DIRECTION direct, bool active)
        //{
        //    switch (direct)
        //    {
        //        case DIRECTION.FRONT:   this.front.SetActive(active);   break;
        //        case DIRECTION.BACK:    this.back.SetActive(active);    break;
        //        case DIRECTION.LEFT:    this.left.SetActive(active);    break;
        //        case DIRECTION.RIGHT:   this.right.SetActive(active);   break;
        //        case DIRECTION.UP:      this.up.SetActive(active);      break;
        //        case DIRECTION.DOWN:    this.down.SetActive(active);    break;
        //    }
        //}

        ///*
        // * direct 방향에 other 가 있는지 체크합니다.
        // * */
        //public override void CreateBlock(DIRECTION direct, GameObject other)
        //{
        //    switch (direct)
        //    {
        //        case DIRECTION.FRONT:
        //            {//front에 other가 있는지 체크
        //                if (null == other) this.front.SetActive(true);//self
        //                else {
        //                    //this.front.SetActive(false);//self
        //                    other.GetComponent<BlockScript>().Refresh(DIRECTION.BACK, false);//other
        //                }
        //            } break;

        //        case DIRECTION.BACK:
        //            {//back에 other가 있는지 체크
        //                if (null == other) this.back.SetActive(true);//self
        //                else {
        //                    //this.back.SetActive(false);//self
        //                    other.GetComponent<BlockScript>().Refresh(DIRECTION.FRONT, false);//other
        //                }
        //            }
        //            break;

        //        case DIRECTION.LEFT:
        //            {//left에 other가 있는지 체크
        //                if (null == other) this.left.SetActive(true);//self
        //                else
        //                {
        //                    //this.left.SetActive(false);//self
        //                    other.GetComponent<BlockScript>().Refresh(DIRECTION.RIGHT, false);//other
        //                }
        //            }
        //            break;

        //        case DIRECTION.RIGHT:
        //            {//right에 other가 있는지 체크
        //                if (null == other) this.right.SetActive(true);//self
        //                else
        //                {
        //                    //this.right.SetActive(false);//self
        //                    other.GetComponent<BlockScript>().Refresh(DIRECTION.LEFT, false);//other
        //                }
        //            }
        //            break;

        //        case DIRECTION.UP:
        //            {//up에 other가 있는지 체크
        //                if (null == other) this.up.SetActive(true);//self
        //                else
        //                {
        //                    //this.up.SetActive(false);//self
        //                    other.GetComponent<BlockScript>().Refresh(DIRECTION.DOWN, false);//other
        //                }
        //            }
        //            break;

        //        case DIRECTION.DOWN:
        //            {//down에 other가 있는지 체크
        //                if (null == other) this.down.SetActive(true);//self
        //                else
        //                {
        //                    //this.down.SetActive(false);//self
        //                    other.GetComponent<BlockScript>().Refresh(DIRECTION.UP, false);//other
        //                }
        //            }
        //            break;
        //    }
        //}

    }//..class Grass
}//..namespace MyCraft