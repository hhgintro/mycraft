using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MyCraft
{
    //6면체 방향
    public enum DIRECTION
    {
        FRONT, BACK, LEFT, RIGHT, UP, DOWN,
        /*front_left
         * <--
         *   |
         * */
        FRONT_LEFT, FRONT_RIGHT,
        BACK_LEFT, BACK_RIGHT,
        LEFT_FRONT, LEFT_BACK,
        RIGHT_FRONT, RIGHT_BACK,
        MAX
    };

    public enum BLOCKTYPE
    {
        NONE, BELT, INSERTER, CHEST, DRILL, MACHINE, STONE_FURNACE,
        GROUND_BELT,

        //이 이후의 type은 인벤을 벗어날 때
        // modeling으로 설정하지 않는 type들로 분류되어집니다.
        RAW_WOOD = 1001, //자원(광물,등등...)
        IRON_ORE,
        STONE,

        IRON_PLATE = 2001,
        //TREE,
        //RAW_WOOD,

        SKILL = 5000,
        TECH = 6000,
    }


    public enum BELT_ROW { LEFT, RIGHT, MAX }//left, right
    public enum BELT_COL { FIRST, SECOND, THIRD, FORTH, MAX }//head, middle, tail

    public enum COLLIDER
    {//unity에서 설정하는 값입니다.
        TERRAIN = 8,
        BLOCK,
    }

    //public enum BELT_TURN_WEIGHT
    //{//belt의 회전방향
    //    TURN_LEFT = 1 << 1,  //left turn
    //    TURN_RIGHT = 1 << 2, //right turn
    //    TURN_FRONT = 1 << 3, //직진
    //}

    public enum TURN_WEIGHT
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
                case BLOCKTYPE.GROUND_BELT:     return typeof(BeltItemBase);

                case BLOCKTYPE.INSERTER:        return typeof(InserterItemBase);
                case BLOCKTYPE.CHEST:           return typeof(ChestItemBase);
                case BLOCKTYPE.DRILL:           return typeof(DrillItemBase);
                case BLOCKTYPE.MACHINE:         return typeof(MachineItemBase);
                case BLOCKTYPE.STONE_FURNACE:   return typeof(FurnaceItemBase);

                case BLOCKTYPE.SKILL:           return typeof(SkillBase);
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

        public static int PosRounding(float val)
        {//반올림
            if (0 <= val)
                return (int)(val + 0.5f);
            return (int)(val - 0.5f);
        }

    }//..class Common

}//..namespace MyCraft
