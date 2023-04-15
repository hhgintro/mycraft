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
        public string Description;
        public bool learn = false;  //true:연구가 되면 스킬창에 노출됩니다.)
        public bool DIY = true;     //do it yourself(false이면 생산시설에서만 생성할 수 있습니다)
        public BuildCost cost;      //[자신]이 만들어질때 필요한 아이템
        //public List<BuildCostItem> outputs = new List<BuildCostItem>();

        public int Power;
        public int Defence;
        public int Vitality;
        public int Stackable;   //겹치기 최대개수
        public string icon;     //icon image filename


        public ItemBase() { }
        public ItemBase(JsonData json)
        //public void ConstructDatabase(JsonData json)
        {
            this.id             = (ushort)json["id"];
            this.type           = (BLOCKTYPE)(int)json["type"];
            this.Title          = json["title"].ToString();
            this.Description    = json["desc"].ToString();

            //this.DIY            = (int)json["DIY"];

            LoadLearn(json);
            LoadDIY(json);
            LoadItemCost(json);
            LoadState(json);

            this.Stackable      = (int)json["stackable"];
            this.icon           = json["icon"].ToString();
            this.Sprite         = Managers.Resource.Load<Sprite>("Textures/ui/" + this.icon);

            //LoadFurnace(json);
            //LoadAssembling(json);
        }

        void LoadLearn(JsonData json)
        {
            if (false == json.Keys.Contains("learn"))
                return;

            this.learn = (bool)json["learn"];
        }
        void LoadDIY(JsonData json)
        {
            if (false == json.Keys.Contains("DIY"))
                return;

            this.DIY = (bool)json["DIY"];
        }
        void LoadItemCost(JsonData json)
        {
            if (false == json.Keys.Contains("cost"))
                return;

            this.cost = new BuildCost(json["cost"]);
        }
        void LoadState(JsonData json)
        {
            if (false == json.Keys.Contains("state"))
                return;

            this.Power      = (int)json["state"]["power"];
            this.Defence    = (int)json["state"]["defence"];
            this.Vitality   = (int)json["state"]["vitality"];
        }

    }//..class Item

    public class BuildCost
    {
        public float time;  //만들때 소요되는 시간
        public List<BuildCostItem> items = new List<BuildCostItem>();
        public int outputs;        //만들어지는 생산품의 개수

        public BuildCost(JsonData json)
        {
            this.time = float.Parse(json["time"].ToString());

            for (int i = 0; i < json["items"].Count; ++i)
                this.items.Add(new BuildCostItem(json["items"][i]));

            this.outputs = (int)json["outputs"];
        }
    }

    public class BuildCostItem
    {
        public int itemid;
        public int amount;

        public BuildCostItem(JsonData json)
        {
            this.itemid = (int)json["itemid"];
            this.amount = (int)json["amount"];
        }
    }

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