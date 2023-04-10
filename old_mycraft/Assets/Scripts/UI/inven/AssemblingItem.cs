using System.Collections.Generic;
using LitJson;

namespace MyCraft
{
    public class AssemblingItem
    {
        public int inputs;  //input의 slot 최대개수
        public int chips;   //chip의 slot 최대개수
        //public List<AssemblingOutput> outputs = new List<AssemblingOutput>();   //생성할 아이템 list
        public Dictionary<int, SkillBase> outputs = new Dictionary<int, SkillBase>();

        public AssemblingItem(JsonData json)
        {
            this.inputs = (int)json["inputs"];
            this.chips = (int)json["chips"];

            //for(int i=0; i<json["output"].Count; ++i)
            //    this.outputs.Add(new AssemblingOutput(json["output"][i]));

            for(int i=0; i<json["outputskill"].Count; ++i)
            {
                int skillid = (int)json["outputskill"][i];
                SkillBase skill = GameManager.GetSkillBase().FetchItemByID(skillid);
                if (null == skill) continue;

                outputs.Add(skillid, skill);
            }
        }
    }

    public class AssemblingOutput
    {
        public int itemid;          //생성할 아이템
        public float build_time;    //생성에 필요한 시간
        public List<AssemblingInput> inputs = new List<AssemblingInput>();  //재료아이템 list
        public AssemblingOutput(JsonData json)
        {
            this.itemid = (int)json["itemid"];
            this.build_time = float.Parse(json["build-time"].ToString());
            //Debug.Log("assembling: itemid[" + this.itemid + "], build-time[" + this.build_time + "]");

            for (int i=0; i<json["input"].Count; ++i)
                inputs.Add(new AssemblingInput(json["input"][i]));
        }
    }
    public class AssemblingInput
    {
        public int itemid;      //재료아이템
        public int amount;      //개수
        public int limit;       //자동겹침 제한

        public AssemblingInput(JsonData json)
        {
            if (json.Count <= 0)
                return;

            this.itemid = (int)json["itemid"];
            this.amount = (int)json["amount"];
            this.limit = (int)json["limit"];
            //Debug.Log("assembling: input[" + this.itemid + "], amount[" + this.amount + "]");
        }
    }
    
}//..namespace MyCraft