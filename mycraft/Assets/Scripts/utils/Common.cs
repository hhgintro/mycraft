using System;
using System.IO;
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

        PIPE            = 1310,

        SKILL           = 5000,
        TECH            = 6000,
    }

    public enum PROGRESSID : byte
    {
        Progress,
        Fuel,
    }

    public enum LAYER_TYPE : byte
    {//unity에서 설정하는 값입니다.
        //BELT_COLLIDEABLE = 7,
        TERRAIN     = 8,
        MINERAL,
        BLOCK,      //10
        PLAYER,
        MONSTER,
        SENSOR,
    }

    class Common
    {
        public static Type GetTypeOf(BLOCKTYPE type)
        {
            switch (type)
            {
                case BLOCKTYPE.CHEST:           return typeof(ChestItemBase);

                case BLOCKTYPE.INSERTER:        return typeof(InserterItemBase);
                case BLOCKTYPE.DRILL:           return typeof(DrillItemBase);
                case BLOCKTYPE.STONE_FURNACE:   return typeof(FurnaceItemBase);
                case BLOCKTYPE.MACHINE:         return typeof(MachineItemBase);

                case BLOCKTYPE.BELT:            return typeof(BeltItemBase);

                case BLOCKTYPE.PIPE:            return typeof(PipeItemBase);

                case BLOCKTYPE.SKILL:           return typeof(Category);
                case BLOCKTYPE.TECH:            return typeof(TechBase);
            }

            return typeof(ItemBase);
        }

        public static void WriteVector3(BinaryWriter writer, Vector3 vector3)
        {
            writer.Write(vector3.x);
            writer.Write(vector3.y);
            writer.Write(vector3.z);
		}
        public static Vector3 ReadVector3(BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
        public static void WriteQuaternion(BinaryWriter writer, Quaternion rotation)
        {
			writer.Write(rotation.x);
			writer.Write(rotation.y);
			writer.Write(rotation.z);
			writer.Write(rotation.w);
		}
        public static Quaternion ReadQuaternion(BinaryReader reader)
        {
			return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
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

    }//..class Common

}//..namespace MyCraft
