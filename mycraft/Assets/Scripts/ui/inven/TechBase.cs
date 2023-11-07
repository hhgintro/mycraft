using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

namespace MyCraft
{
    public class TechBase : JSonDatabase
    {
        //public int id;  //
        //public string Title;
        public string Description;

        public TechnologyCost _cost;    //소모비용

        public List<int> prev_techs = new List<int>();
        public List<int> next_techs = new List<int>();
        public List<int> rewards = new List<int>();

        public string icon;

        ////2d 이미지용
        //public Sprite Sprite;
        ////3d 모델용
        //public GameObject prefab;

        //public TechBase() { }
        public TechBase(JsonData json) : base(json)
		{
            this.id = (ushort)json["id"];
            this.Title = json["title"].ToString();
            this.Description = json["desc"].ToString();

            LoadCost(json);
            LoadPrevTechnology(json);
            LoadNextTechnology(json);
            LoadReward(json);
        }

        void LoadCost(JsonData json)
        {
            if (false == json.Keys.Contains("cost"))
                return;

            this._cost = new TechnologyCost(json["cost"]);
        }

        void LoadPrevTechnology(JsonData json)
        {
            if (false == json.Keys.Contains("pre-tech"))
                return;

            for (int i = 0; i < json["pre-tech"].Count; ++i)
                this.prev_techs.Add((int)json["pre-tech"][i]["techid"]);
        }

        void LoadNextTechnology(JsonData json)
        {
            if (false == json.Keys.Contains("next-tech"))
                return;

            for (int i = 0; i < json["next-tech"].Count; ++i)
                this.next_techs.Add((int)json["next-tech"][i]["techid"]);
        }

        void LoadReward(JsonData json)
        {
            if (false == json.Keys.Contains("reward"))
                return;

            for (int i = 0; i < json["reward"].Count; ++i)
            {
                //this.rewards.Add((int)json["reward"][i]["skill"]);
                int reward = (int)json["reward"][i]["skill"];
                this.rewards.Add(reward);
                //Debug.Log("tech reward: " + reward);
            }
        }
    }//..class Item



}//..namespace MyCraft