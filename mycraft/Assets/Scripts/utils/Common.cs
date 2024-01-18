using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        Item,   //아이템 소모 progress
        Fuel,   //연료 소모 progress
    }

    public enum LAYER_TYPE : byte
    {//unity에서 설정하는 값입니다.
        //BELT_COLLIDEABLE = 7,
        TERRAIN     = 6,
        BELT_COLLIDEABLE,
        PLAYER,
        BUILDING,
        MONSTER,

        //MINERAL,
    }

    class Common
    {
        //building의 INPUT에서 받을 수 있는 아이템 개수를 제한한다.
        public static int INPUT_ALLOW_RATE = 3; //INPUT은 n배수 까지만 받을 수 있다.
        public static int INPUT_ALLOW_CNT = 3;  //INPUT은 n 까지만 받을 수 있다.

        public static int MAX_TECH_RESEARCH = 5;    //예약가능한 연구는 초대 n개까지.

        public static float MAX_RAY_DISTANCE = 100f;  //raycast 최대길이
		public static float BUILDING_ROTATION_MOUSE_ANGLE = 15f;    //건물완공할때 마우스 방향설정 회전각도
        public static float BUILDING_ROTATION_KEYDOWN_ANGLE = 45f;  //건물위치설정할때 "r"눌렀을때 회전하는 각도.

        public static bool bMonsterRegen = true;    //"P"가 눌릴 떄, 몹리젠을 on/off조절합니다.

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

		//folderPath내 파일목록을 수정일자순으로 가져오는 코드
		public static string[] GetFilesInFolderWithExtensionOrderByCreationTime(string folderPath, string extension)
		{
			DirectoryInfo directory = new DirectoryInfo(folderPath);
			List<FileInfo> fileInfoList = directory.GetFiles("*" + extension)
													.OrderBy(file => file.LastWriteTime)
                                                    .ToList();
			string[] filePaths = new string[fileInfoList.Count];
			for (int i = 0; i < fileInfoList.Count; i++)
				filePaths[i] = Path.GetFileNameWithoutExtension(fileInfoList[i].Name);
			return filePaths;
		}
	}//..class Common

}//..namespace MyCraft
