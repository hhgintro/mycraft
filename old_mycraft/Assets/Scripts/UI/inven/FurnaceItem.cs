using System.Collections.Generic;
using LitJson;

namespace MyCraft
{
    public class FurnaceItem
    {
        public int inputs;      //input의 slot 최대개수
        public int fuels;       //fuels의 slot 최대개수
        public int outputs;     //outputs의 slot 최대개수

        //public FurnaceInput input;// = new FurnaceInput();
        //public FurnaceFuel fuel;// = new List<FurnaceFuel>();
        //재련할 아이템 리스트
        public List<FurnaceInputItem> input = new List<FurnaceInputItem>();
        //연료로 사용될 아이템 리스트
        public List<FurnaceFuelItem> fuel = new List<FurnaceFuelItem>();

        public FurnaceItem(JsonData json)
        {
            this.inputs = (int)json["inputs"];
            this.fuels = (int)json["fuels"];
            this.outputs = (int)json["outputs"];

            //this.input = new FurnaceInput(json["input"]);
            //this.fuel = new FurnaceFuel(json["fuel"]);
            for (int i = 0; i < json["input"].Count; ++i)
                this.input.Add(new FurnaceInputItem(json["input"][i]));
            for (int i = 0; i < json["fuel"].Count; ++i)
                this.fuel.Add(new FurnaceFuelItem(json["fuel"][i]));
        }
    }

    //public class FurnaceInput
    //{
    //    //public int amount;
    //    public List<FurnaceInputItem> items = new List<FurnaceInputItem>();
    //    public FurnaceInput(JsonData json)
    //    {
    //        //this.amount = (int)json["amount"];
    //        for(int i=0; i<json["item"].Count; ++i)
    //            items.Add(new FurnaceInputItem(json["item"][i]));
    //        //Debug.Log("amount:" + this.amount);
    //    }
    //}
    public class FurnaceInputBase
    {
        public int itemid;  //재료아이템
        public int limit;   //자동겹침 제한

        public FurnaceInputBase(JsonData json)
        {
            if (json.Count <= 0)
                return;

            this.itemid = (int)json["id"];
            this.limit = (int)json["limit"];
        }
    }
    
    public class FurnaceInputItem : FurnaceInputBase
    {
        //public int itemid;  //재료아이템
        //public int limit;   //자동겹침 제한
        public float build_time;    //생산시간
        public int output;  //만들어지는 아이템

        public FurnaceInputItem(JsonData json) : base(json)
        {
            if (json.Count <= 0)
                return;

            //this.itemid = (int)json["id"];
            //this.limit = (int)json["limit"];
            this.build_time = float.Parse(json["build-time"].ToString());
            this.output = (int)json["output"];
            //Debug.Log("input -> id:" + this.id + " build-time:" + this.build_time + " output:" + this.output);
        }
    }

    //public class FurnaceFuel
    //{
    //    //public int amount;
    //    public List<FurnaceFuelItem> items = new List<FurnaceFuelItem>();
    //    public FurnaceFuel(JsonData json)
    //    {
    //        //this.amount = (int)json["amount"];
    //        for (int i = 0; i < json["item"].Count; ++i)
    //            items.Add(new FurnaceFuelItem(json["item"][i]));
    //        //Debug.Log("amount:" + this.amount);
    //    }
    //}
    public class FurnaceFuelItem : FurnaceInputBase
    {
        //public int itemid;  //연료 아이템
        //public int limit;   //자동겹침 제한
        public float burning_time;  //연소시간

        public FurnaceFuelItem(JsonData json) : base(json)
        {
            //this.itemid = (int)json["id"];
            //this.limit = (int)json["limit"];
            this.burning_time = float.Parse(json["burning-time"].ToString());
            //Debug.Log("fuel -> id:" + this.id + " burning-time:" + this.burning_time);
        }
    }

}//..namespace MyCraft