using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace MyCraft
{
    public class Categories : JSonDatabase
    {
        //public int id;  //
        //public int category;  //
        //public string title;
        public string desc;

        public int panel;   //등록할 panel
        public int DIY;//do it yourself(0이면 생산시설에서만 생성할 수 있습니다)
        //public int itemid;

        public List<SkillPanel> panels;
        public string icon;


        public Categories() { }
        public Categories(JsonData json)
        {
            this.id = (int)json["id"];
            this.Title = json["title"].ToString();
            this.desc = json["desc"].ToString();

            LoadSkillPanel(json);

            this.icon = json["icon"].ToString();
            //Debug.Log("skill icon: " + this.icon);

            this.Sprite = Managers.Resource.Load<Sprite>("Textures/ui/tech/" + this.icon);
        }

        void LoadSkillPanel(JsonData json)
        {
            if (false == json.Keys.Contains("panels"))
                return;

            //this.panels = new SkillPanel(json["panels"]);
            this.panels = new List<SkillPanel>();
            for (int i = 0; i < json["panels"].Count; ++i)
                this.panels.Add(new SkillPanel(json["panels"][i]));
        }

        public override void EnterTooltip(Tooltip tooltip)
        {
            //title
            tooltip.CreateTitle(this.Title);
            //tooltip.SetTitle(this.Title);

            ////cost
            //if (null != this.cost)
            //{
            //    //time
            //    GameObject slot_time = tooltip.CreateCost();
            //    GameObject time = tooltip.CreateSkill(slot_time.transform.GetChild(0).transform);
            //    //image
            //    ItemBase itembase0 = GameManager.GetItemBase().FetchItemByID(0);
            //    time.GetComponent<Image>().sprite = itembase0.Sprite;
            //    time.name = itembase0.Title;
            //    //amount
            //    slot_time.transform.GetChild(1).GetComponent<Text>().text = " " + this.cost.time.ToString();

            //    //items
            //    for (int i = 0; i < this.cost.items.Count; ++i)
            //    {
            //        GameObject slot_cost = tooltip.CreateCost();
            //        GameObject skill = tooltip.CreateSkill(slot_cost.transform.GetChild(0).transform);
            //        //image
            //        ItemBase itembase1 = GameManager.GetItemBase().FetchItemByID(this.cost.items[i].itemid);
            //        skill.GetComponent<Image>().sprite = itembase1.Sprite;
            //        skill.name = itembase1.Title;
            //        //amount(inven + quick) : 인벤의 아이템개수보다 적으면 GRAY로 표기됩니다.
            //        int amount = GameManager.GetInventory().GetAmount(this.cost.items[i].itemid);
            //        //amount += GameManager.GetInventory().GetAmount(this.cost.items[i].itemid);
            //        if (amount < this.cost.items[i].amount)  //부족할때
            //        {
            //            slot_cost.transform.GetChild(1).GetComponent<Text>().text = " " + amount.ToString() + "/" + this.cost.items[i].amount.ToString() + " x " + itembase1.Title.ToString();
            //            slot_cost.transform.GetChild(1).GetComponent<Text>().color = Color.gray;
            //        }
            //        else    //충분할때.
            //        {
            //            slot_cost.transform.GetChild(1).GetComponent<Text>().text = " " + this.cost.items[i].amount.ToString() + " x " + itembase1.Title.ToString();
            //        }

            //    }
            //}

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
        public List<SkillPanelItem> items = new List<SkillPanelItem>();

        public SkillPanel(JsonData json)
        {
            if (false == json.Keys.Contains("items"))
                return;

            for (int i = 0; i < json["items"].Count; ++i)
                this.items.Add(new SkillPanelItem(json["items"][i]));
        }
    }

    public class SkillPanelItem
    {
        public int itemid;

        public SkillPanelItem(JsonData json)
        {
            this.itemid = (int)json[0];
        }
    }

}//..namespace MyCraft