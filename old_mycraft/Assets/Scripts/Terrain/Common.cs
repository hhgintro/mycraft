using System;
using System.Collections.Generic;
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
        BELT            = 1010,
        BELT_UP         = 1011,
        BELT_DOWN       = 1012,
        SPLITER         = 1020,
        INSERTER        = 1030,
        DRILL           = 1040,
        STONE_FURNACE   = 1050,
        MACHINE         = 1060,


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

        //ground-belt
        BACK = 1 << 3,  //back
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
                case BLOCKTYPE.SPLITER:         return typeof(BeltItemBase);

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
