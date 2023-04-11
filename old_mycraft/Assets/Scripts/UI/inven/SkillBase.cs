using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace MyCraft
{
    //public class SkillBase : MonoBehaviour
    //{
    //    private List<Skill> database = new List<Skill>();
    //    private JsonData json;

    //    private void Awake()
    //    {
    //        this.json = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/skills.json"));
    //        //Debug.Log("items: " + InvenItemData);
    //    }
    //    void Start()
    //    {
    //        ConstructDatabase();
    //        //Debug.Log("item: " + FetchItemByID(this.InvenItemData.Count).Description);
    //    }

    //    public Skill FetchItemByID(int id)
    //    {
    //        for (int i = 0; i < database.Count; ++i)
    //        {
    //            if (database[i].category == id)
    //                return database[i];
    //        }
    //        return null;
    //    }

    //    void ConstructDatabase()
    //    {
    //        for (int i = 0; i < this.json.Count; ++i)
    //             this.database.Add(new Skill(this.json[i]));
    //    }
    //}//..class ItemDatabase

    public class SkillBase : JSonDatabase
    {
        //public int id;  //
        public int category;  //
                              //public string desc;

        public int panel;   //등록할 panel
        //public string title;
        public string desc;
        public int DIY;//do it yourself(0이면 생산시설에서만 생성할 수 있습니다)
        //public int itemid;

        public SkillCost cost;
        public List<SkillCostItem> outputs = new List<SkillCostItem>();

        public string Slug;

        //2d 이미지용
        //public Sprite Sprite;
        ////3d 모델용
        //public GameObject prefab;

        public SkillPanel _techpanel;



        public SkillBase() { }
        public SkillBase(JsonData json)
        {
            this.id = (int)json["id"];
            this.category = (int)json["category"];
            this.panel = (int)json["panel"];
            this.Title = json["title"].ToString();
            this.desc = json["desc"].ToString();
            this.DIY = (int)json["DIY"];
            //this.itemid = (int)json["itemid"];

            LoadSkillCost(json);
            LoadOutput(json);

            this.Slug = json["slug"].ToString();
            //Debug.Log("skill slug: " + this.Slug);

            this.Sprite = Resources.Load<Sprite>("graphic/ui/" + this.Slug);
        }

        void LoadSkillCost(JsonData json)
        {
            if (false == json.Keys.Contains("cost"))
                return;

            this.cost = new SkillCost(json["cost"]);
        }

        void LoadOutput(JsonData json)
        {
            if (false == json.Keys.Contains("outputs"))
                return;

            for (int i = 0; i < json["outputs"].Count; ++i)
                this.outputs.Add(new SkillCostItem(json["outputs"][i]));
        }

        public override void EnterTooltip(Tooltip tooltip)
        {
            //title
            //tooltip.CreateTitle(this.Title);
            tooltip.SetTitle(this.Title);

            //cost
            if(null != this.cost)
            {
                //time
                GameObject slot_time = tooltip.CreateCost();
                GameObject time = tooltip.CreateSkill(slot_time.transform.GetChild(0).transform);
                //image
                ItemBase itembase0 = GameManager.GetItemBase().FetchItemByID(0);
                time.GetComponent<Image>().sprite = itembase0.Sprite;
                time.name = itembase0.Title;
                //amount
                slot_time.transform.GetChild(1).GetComponent<Text>().text = " " + this.cost.time.ToString();

                //items
                for (int i = 0; i < this.cost.items.Count; ++i)
                {
                    GameObject slot_cost = tooltip.CreateCost();
                    GameObject skill = tooltip.CreateSkill(slot_cost.transform.GetChild(0).transform);
                    //image
                    ItemBase itembase1 = GameManager.GetItemBase().FetchItemByID(this.cost.items[i].itemid);
                    skill.GetComponent<Image>().sprite = itembase1.Sprite;
                    skill.name = itembase1.Title;
                    //amount(inven + quick) : 인벤의 아이템개수보다 적으면 GRAY로 표기됩니다.
                    int amount = GameManager.GetInventory().GetAmount(this.cost.items[i].itemid);
                    //amount += GameManager.GetInventory().GetAmount(this.cost.items[i].itemid);
                    if(amount < this.cost.items[i].amount)  //부족할때
                    {
                        slot_cost.transform.GetChild(1).GetComponent<Text>().text = " " + amount.ToString() + "/" + this.cost.items[i].amount.ToString() + " x " + itembase1.Title.ToString();
                        slot_cost.transform.GetChild(1).GetComponent<Text>().color = Color.gray;
                    }
                    else    //충분할때.
                    {
                        slot_cost.transform.GetChild(1).GetComponent<Text>().text = " " + this.cost.items[i].amount.ToString() + " x " + itembase1.Title.ToString();
                    }

                }
            }

            //comment
            GameObject comment = tooltip.CreateComment();
            comment.GetComponent<Text>().text = this.desc;

            //totalcost(미구현)
            //GameObject totalcost = tooltip.CreateTotalCost();


        }
            //public override void LeaveTooltip(Tooltip tooltip)
            //{
            //}


        }//..class SkillBase

    public class SkillPanel
    {

        public SkillPanel(JsonData json)
        {
            if (false == json.Keys.Contains("tech"))
                return;

            Debug.Log("tech: " + json["tech"].Count);
            //for (int i = 0; i < json["tech"].Count; ++i)
            //    items.Add(new FurnaceInputItem(json["tech"][i]));

        }
    }

    public class SkillCost
    {
        public float time;
        public List<SkillCostItem> items = new List<SkillCostItem>();

        public SkillCost(JsonData json)
        {
            this.time = float.Parse(json["time"].ToString());

            for (int i = 0; i < json["items"].Count; ++i)
                this.items.Add(new SkillCostItem(json["items"][i]));
        }
    }

    public class SkillCostItem
    {
        public int itemid;
        public int amount;

        public SkillCostItem(JsonData json)
        {
            this.itemid = (int)json["itemid"];
            this.amount = (int)json["amount"];
        }
    }

}//..namespace MyCraft