using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

namespace MyCraft
{
    public class ItemBase : JSonDatabase
    {
        //public int itemid;  //
        //public string Title;
        public int Power;
        public int Defence;
        public int Vitality;
        public string Description;
        public int Stackable;   //겹치기 최대개수
        public string Slug;     //icon image filename

        ////2d 이미지용
        //public Sprite Sprite;
        ////3d 모델용
        //public GameObject prefab;

        //public FurnaceItem _furnace;
        //public AssemblingItem _assembling;

        public ItemBase() { }
        public ItemBase(JsonData json)
        //public void ConstructDatabase(JsonData json)
        {
            this.id = (int)json["id"];
            this.type = (BLOCKTYPE)(int)json["type"];
            this.Title = json["title"].ToString();
            this.Power = (int)json["state"]["power"];
            this.Defence = (int)json["state"]["defence"];
            this.Vitality = (int)json["state"]["vitality"];
            this.Description = json["desc"].ToString();
            this.Stackable = (int)json["stackable"];
            this.Slug = json["slug"].ToString();

            this.Sprite = Resources.Load<Sprite>("graphic/ui/" + this.Slug);

            //LoadFurnace(json);
            //LoadAssembling(json);
        }

        //void LoadFurnace(JsonData json)
        //{
        //    if (false == json.Keys.Contains("furnace"))
        //        return;

        //    this._furnace = new FurnaceItem(json["furnace"]);
        //}

        //void LoadAssembling(JsonData json)
        //{
        //    if (false == json.Keys.Contains("assembling"))
        //        return;

        //    this._assembling = new AssemblingItem(json["assembling"]);
        //}

    }//..class Item

    public class BeltItemBase : ItemBase
    {
        //goods의 이동속도
        public float speed;

        public BeltItemBase(JsonData json) : base(json)
        {
            this.speed = float.Parse(json["speed"].ToString());
        }
    }

    public class InserterItemBase : ItemBase
    {
        //goods를 운반하는 속도
        public float speed;

        public InserterItemBase(JsonData json) : base(json)
        {
            this.speed = float.Parse(json["speed"].ToString());
        }
    }

    public class ChestItemBase : ItemBase
    {
        public int Slots;   //slot개수

        public ChestItemBase(JsonData json) : base(json)
        {
            this.Slots = (int)json["slots"];
        }
    }

    public class DrillItemBase : ItemBase
    {
        //goods를 생산하는 delay 시간
        public float delay; //

        public DrillItemBase(JsonData json) : base(json)
        {
            this.delay = float.Parse(json["delay"].ToString());
        }

    }

    public class MachineItemBase : ItemBase
    {
        public AssemblingItem _assembling;

        public MachineItemBase(JsonData json) : base(json)
        {
            this._assembling = new AssemblingItem(json["assembling"]);
        }
    }

    public class FurnaceItemBase : ItemBase
    {
        public FurnaceItem _furnace;

        public FurnaceItemBase(JsonData json) : base(json)
        {
            this._furnace = new FurnaceItem(json["furnace"]);
        }        
    }

}//..namespace MyCraft