using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.XR;

namespace MyCraft
{
	public class TechBase : JSonDatabase
	{
		//public int id;  //
		//public string Title;
		//public string Description;

		public TechnologyCost cost;    //소모비용

		public List<int> prev_techs = new List<int>();
		public List<int> next_techs = new List<int>();
		public List<int> rewards = new List<int>();

		public string icon;

        public bool Learned { get { return (1.0f <= this._fillAmount); } }    //연구를 완료했나?
        public float _fillAmount { get; set; }	//Tech 연구률(0~1.0)

        //public float _fillAmount;	//현재진행도

        ////2d 이미지용
        //public Sprite Sprite;
        ////3d 모델용
        //public GameObject prefab;

        //public TechBase() { }
        public TechBase(JsonData json) : base(json)
		{
			//this.id = (ushort)json["id"];
			//this.Title = json["title"].ToString();
			//this.Description = json["desc"].ToString();

			LoadCost(json);
			LoadPrevTechnology(json);
			LoadNextTechnology(json);
			LoadReward(json);
			//Debug.Log($"load tech({this.id})");
		}

		public override string section() { return "technologies"; }
		void LoadCost(JsonData json)
		{
			if (false == json.Keys.Contains("cost"))
				return;

			this.cost = new TechnologyCost(json["cost"]);
		}

		void LoadPrevTechnology(JsonData json)
		{
			if (false == json.Keys.Contains("pre-tech"))
				return;

			for (int i = 0; i < json["pre-tech"].Count; ++i)
			{
				int techid = (int)json["pre-tech"][i]["techid"];
				this.prev_techs.Add(techid);

				//////next(자신의 id를 부여한다.)
				//Debug.Log($"tech({techid}) next add({this.id})");
				//TechBase itemPrev = Managers.Game.TechBases.FetchItemByID(techid);
				////itemPrev.Title = id.ToString();
				//itemPrev.next_techs.Add(this.id);
			}
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