using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;


namespace MyCraft
{
    public enum BLOCKTYPE : ushort
    {
        NONE,

        NATURAL_RESOURCE,       //천연자원
        INTERMEDIATE_PRODUCTS,  //중간생산품
        
        CHEST           = 1000,

        INSERTER        = 1030,
        DRILL           = 1040,
        STONE_FURNACE   = 1050,
        MACHINE         = 1060,

        BELT            = 1100,
        BELT_UP         = 1110,
        BELT_DOWN       = 1120,

        BELT_VERTICAL_UP_BEGIN      = 1130,
        BELT_VERTICAL_UP_MIDDLE     = 1140,
        BELT_VERTICAL_UP_END        = 1150,
        BELT_VERTICAL_DOWN_BEGIN    = 1160,
        BELT_VERTICAL_DOWN_MIDDLE   = 1170,
        BELT_VERTICAL_DOWN_END      = 1180,

        SPLITER         = 1200,

        PIPE            = 1210,

        PIPE_VERTICAL_UP_BEGIN      = 1220,
        PIPE_VERTICAL_UP_MIDDLE     = 1230,
        PIPE_VERTICAL_UP_END        = 1240,
        PIPE_VERTICAL_DOWN_BEGIN    = 1250,
        PIPE_VERTICAL_DOWN_MIDDLE   = 1260,
        PIPE_VERTICAL_DOWN_END      = 1270,

        SKILL           = 5000,
        TECH            = 6000,
    }

    //Front, Left, Right, Back(어느방향판별용)
    public enum SENSOR : byte { LF, RF, L, R, LB, RB }

    public enum BELT_ROW : byte { ROW1, ROW2, ROW3, ROW4, MAX }        //left -> right
    public enum BELT_COL : byte { FIRST, SECOND, THIRD, FORTH, MAX }   //front -> tail

    public enum LAYER_TYPE : byte
    {//unity에서 설정하는 값입니다.
        TERRAIN     = 8,
        MINERAL,
        BLOCK,      //10
        PLAYER,
        MONSTER,
        SENSOR,
    }

    public enum TRIGGER_DESTORY { DESTROY = -251 }

    //public enum BELT_TURN_WEIGHT
    //{//belt의 회전방향
    //    TURN_LEFT = 1 << 1,  //left turn
    //    TURN_RIGHT = 1 << 2, //right turn
    //    TURN_FRONT = 1 << 3, //직진
    //}

    public enum TURN_WEIGHT : byte
    {//방향에 영향을 주는 방향
        FRONT = 1 << 0,  //back 으로 부터의 가중치
        LEFT = 1 << 1, //left
        RIGHT = 1 << 2, //right

        BOTH = 1 << 3,  // left & right
    }


    class Common
    {
        public static Type GetTypeOf(BLOCKTYPE type)
        {
            switch (type)
            {
                case BLOCKTYPE.BELT:            return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_UP:         return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_DOWN:       return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_VERTICAL_UP_BEGIN:      return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_VERTICAL_UP_MIDDLE:     return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_VERTICAL_UP_END:        return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_VERTICAL_DOWN_BEGIN:    return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_VERTICAL_DOWN_MIDDLE:   return typeof(BeltItemBase);
                case BLOCKTYPE.BELT_VERTICAL_DOWN_END:      return typeof(BeltItemBase);
                case BLOCKTYPE.SPLITER:         return typeof(BeltItemBase);

                case BLOCKTYPE.PIPE:                        return typeof(PipeItemBase);
                case BLOCKTYPE.PIPE_VERTICAL_UP_BEGIN:      return typeof(PipeItemBase);
                case BLOCKTYPE.PIPE_VERTICAL_UP_MIDDLE:     return typeof(PipeItemBase);
                case BLOCKTYPE.PIPE_VERTICAL_UP_END:        return typeof(PipeItemBase);
                case BLOCKTYPE.PIPE_VERTICAL_DOWN_BEGIN:    return typeof(PipeItemBase);
                case BLOCKTYPE.PIPE_VERTICAL_DOWN_MIDDLE:   return typeof(PipeItemBase);
                case BLOCKTYPE.PIPE_VERTICAL_DOWN_END:      return typeof(PipeItemBase);

                case BLOCKTYPE.INSERTER:        return typeof(InserterItemBase);
                case BLOCKTYPE.CHEST:           return typeof(ChestItemBase);
                case BLOCKTYPE.DRILL:           return typeof(DrillItemBase);
                case BLOCKTYPE.STONE_FURNACE:   return typeof(FurnaceItemBase);
                case BLOCKTYPE.MACHINE:         return typeof(MachineItemBase);

                case BLOCKTYPE.SKILL:           return typeof(Categories);
                case BLOCKTYPE.TECH:            return typeof(TechBase);
            }

            return typeof(ItemBase);
        }

        /*
         * weight에 val값을 설정합니다.
         * */
        public static int ADD_BIT(int weight, int val)
        {
            return (weight | val);
        }
        /*
         * weight에서 val값을 제거합니다.
         * */
        public static int SUB_BIT(int weight, int val)
        {
            return weight & ~val;
        }
        /*
         * weight에 val값의 설정유무를 확인합니다.(true이면 설정됨)
         * */
        public static bool CHECK_BIT(int weight, int val)
        {
            return (weight & val) == val;
        }

        //origin: 상공에서 수직아래로 ray를 쏜다.
        public static Vector3 GetMapHeight(Vector3 point, int layerMask)
        {
            float maxDistance = 300f;
            Vector3 origin = new Vector3(point.x, maxDistance-10, point.z);

            Ray ray = new Ray(origin, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
                return hit.point;
            return Vector3.zero;
        }

        public static bool OnBound(float val, float bound)
        {
            float err_bound = 0.01f;
            if (bound - err_bound < val && val < bound + err_bound)
                return true;
            return false;
        }
        public static bool IsSameForward(Vector3 lhs, Vector3 rhs)
        {
            float err_bound = 0.01f;

            float bound = Vector3.Dot(lhs, rhs);
            if (bound < 1-err_bound || 1+err_bound < bound)
                return false;
            return true;
        }

        //block이 원점을 중심으로 두고 있기때문에
        //  block은 (-0.5 ~ 0.5)에 위치한다.
        public static int PosRound(float val)
        {//반올림
            if (0 <= val)
                return (int)(val + 0.5f);
            return (int)(val - 0.5f);
        }
        public static int PosFloor(float val)
        {//버림
            return (int)(val);
        }
        //public static int PosCeil(float val)
        //{//올림
        //    return (int)(val+1);
        //}

    }//..class Common

}//..namespace MyCraft
