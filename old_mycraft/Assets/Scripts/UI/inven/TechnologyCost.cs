using System.Collections.Generic;
using LitJson;

namespace MyCraft
{
    public class TechnologyCost
    {
        public int mulitple;   //time/item을 한 묶음으로 n만큼 소모합니다.
        public float time;  //소요시간
        public List<TechnologyCostItem> items = new List<TechnologyCostItem>();

        public TechnologyCost(JsonData json)
        {
            this.mulitple = (int)json["multiple"];
            this.time = float.Parse(json["time"].ToString());

            LoadCostItem(json);
        }

        public void LoadCostItem(JsonData json)
        {
            if (false == json.Keys.Contains("item"))
                return;

            for (int i = 0; i < json["item"].Count; ++i)
                items.Add(new TechnologyCostItem(json["item"][i]));

        }

    }

    public class TechnologyCostItem
    {
        public int itemid;  //item id
        public int amount;  //아이템 개수

        public TechnologyCostItem(JsonData json)
        {
            this.itemid = (int)json["itemid"];
            this.amount = (int)json["amount"];
            //Debug.Log("tech cost item: " + this.itemid + "/" + this.amount);
        }

    }

}//..namespace MyCraft