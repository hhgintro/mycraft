using System.Collections.Generic;
using LitJson;

namespace MyCraft
{
    public class AssemblingItem
    {
        public int inputs;  //input의 slot 최대개수
        public int chips;   //chip의 slot 최대개수
        ////public List<AssemblingOutput> outputs = new List<AssemblingOutput>();   //생성할 아이템 list
        //public Dictionary<int, ItemBase> outputs = new Dictionary<int, ItemBase>();
        public List<int> outputs = new List<int>(); //아직 미구현

        public AssemblingItem(JsonData json)
        {
            this.inputs = (int)json["inputs"];
            this.chips = (int)json["chips"];

            //for(int i=0; i<json["output"].Count; ++i)
            //    this.outputs.Add(new AssemblingOutput(json["output"][i]));

            for (int i = 0; i < json["outputskill"].Count; ++i)
            {
                int itemid = (int)json["outputskill"][i];
                //ItemBase itembase = GameManager.GetItemBase().FetchItemByID(itemid);
                //if (null == itembase) continue;
                //outputs.Add(itemid, itembase);
                outputs.Add(itemid);
            }
        }
    }

    //public class AssemblingOutput
    //{
    //    public int itemid;          //생성할 아이템
    //    public float build_time;    //생성에 필요한 시간
    //    public List<AssemblingInput> inputs = new List<AssemblingInput>();  //재료아이템 list
    //    public AssemblingOutput(JsonData json)
    //    {
    //        this.itemid = (int)json["itemid"];
    //        this.build_time = float.Parse(json["build-time"].ToString());
    //        //Debug.Log("assembling: itemid[" + this.itemid + "], build-time[" + this.build_time + "]");

    //        for (int i=0; i<json["input"].Count; ++i)
    //            inputs.Add(new AssemblingInput(json["input"][i]));
    //    }
    //}
    //public class AssemblingInput
    //{
    //    public int itemid;      //재료아이템
    //    public int amount;      //개수
    //    public int limit;       //자동겹침 제한

    //    public AssemblingInput(JsonData json)
    //    {
    //        if (json.Count <= 0)
    //            return;

    //        this.itemid = (int)json["itemid"];
    //        this.amount = (int)json["amount"];
    //        this.limit = (int)json["limit"];
    //        //Debug.Log("assembling: input[" + this.itemid + "], amount[" + this.amount + "]");
    //    }
    //}
    
}//..namespace MyCraft