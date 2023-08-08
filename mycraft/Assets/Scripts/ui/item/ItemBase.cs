using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;

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
		public int Vitality;    //(광물의 경우 자원의 수량)block의 HP량.
		public int Stackable;   //겹치기 최대개수

		public Color DebugColor;

		//public ItemBase() { }
		public ItemBase(JsonData json) : base(json)
		//public void ConstructDatabase(JsonData json)
		{
			//this.id             = (ushort)json["id"];
			//this.type           = (BLOCKTYPE)(int)json["type"];
			//this.Title          = json["title"].ToString();
			this.Description      = Managers.Locale.GetLocale("items", string.Format($"{this.id.ToString()}-desc"));

			//this.DIY            = (int)json["DIY"];

			LoadLearn(json);
			LoadDIY(json);
			LoadItemCost(json);
			LoadState(json);

			this.Stackable      = (int)json["stackable"];

			this.DebugColor     = Color.green;
		}
		public override string _iconPath() { return "Textures/ui/"; }

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

		public override void EnterTooltip(Tooltip tooltip)
		{
			base.EnterTooltip(tooltip);

			//cost
			if (null != this.cost)
			{
				//time
				GameObject slot_time = tooltip.CreateCost();
				GameObject time = tooltip.CreateSkill(slot_time.transform.GetChild(0).transform);
				//image
				ItemBase itembase0 = Managers.Game.ItemBases.FetchItemByID(1);    //time    
				time.GetComponent<Image>().sprite = itembase0.icon;
				time.name = itembase0.Title;
				//amount
				slot_time.transform.GetChild(1).GetComponent<Text>().text = " " + this.cost.time.ToString();

				//items
				for (int i = 0; i < this.cost.items.Count; ++i)
				{
					GameObject slot_cost = tooltip.CreateCost();
					GameObject skill = tooltip.CreateSkill(slot_cost.transform.GetChild(0).transform);
					//image
					ItemBase itembase1 = Managers.Game.ItemBases.FetchItemByID(this.cost.items[i].itemid);
					skill.GetComponent<Image>().sprite = itembase1.icon;
					skill.name = itembase1.Title;
					//amount(inven + quick) : 인벤의 아이템개수보다 적으면 GRAY로 표기됩니다.
					int amount = Managers.Game.Inventories.GetAmount(this.cost.items[i].itemid);
					amount += Managers.Game.QuickInvens.GetAmount(this.cost.items[i].itemid);
					if (amount < this.cost.items[i].amount)  //부족할때
					{
						slot_cost.transform.GetChild(1).GetComponent<Text>().text = " " + amount.ToString() + "/" + this.cost.items[i].amount.ToString() + " x " + itembase1.Title.ToString();
						slot_cost.transform.GetChild(1).GetComponent<Text>().color = Color.gray;
					}
					else    //충분할때.
					{
						slot_cost.transform.GetChild(1).GetComponent<Text>().text = " " + this.cost.items[i].amount.ToString() + " x " + itembase1.Title.ToString();
						slot_cost.transform.GetChild(1).GetComponent<Text>().color = Color.white;
					}
				}
			}

			//comment
			GameObject comment = tooltip.CreateComment();
			comment.GetComponent<Text>().text = this.Description;

			//totalcost(미구현)
			//GameObject totalcost = tooltip.CreateTotalCost();
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

	public class PipeItemBase : ItemBase
	{
		//goods의 이동속도
		public float speed;

		public PipeItemBase(JsonData json) : base(json)
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
		public float perSecond; //(단위:s)초당채굴량

		public DrillItemBase(JsonData json) : base(json)
		{
			this.perSecond = float.Parse(json["perSecond"].ToString());
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