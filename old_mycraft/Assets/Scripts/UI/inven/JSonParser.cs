using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace MyCraft
{
    public class JSonParser<T> where T : JSonDatabase, new()
    {
        private JsonData json;
        //private T t = new T();
        //public List<T> database = new List<T>();
        public Dictionary<int,T> database = new Dictionary<int,T>();

        public JSonParser(string filename)
        {
            //this.json = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/skills.json"));
            this.json = JsonMapper.ToObject(File.ReadAllText(filename));
            ConstructDatabase();
        }

        public void ConstructDatabase()
        {
            //t.ConstructDatabase();
            //for (int i = 0; i < this.json.Count; ++i)
            //    this.database.Add(new T(this.json[i]));
            for (int i = 0; i < this.json.Count; ++i)
            {
                //case.1
                //this.database.Add((T)Activator.CreateInstance(typeof(T), this.json[i]));
                //case.2
                //string type = this.json[i]["type"].ToString();
                //Debug.Log("type: " + type.ToString());
                //Type TTT = Type.GetType(type);
                //Debug.Log("TTT: " + TTT.ToString());
                //this.database.Add((T)Activator.CreateInstance(Type.GetType(this.json[i]["type"].ToString()), this.json[i]));
                //case 3.
                //int type = (int)this.json[i]["type"];
                //Debug.Log("type: " + type.ToString());
                //Type TTT = Common.GetTypeOf((BLOCKTYPE)type);
                //Debug.Log("TTT: " + TTT.ToString());
                this.database.Add((int)this.json[i]["id"], (T)Activator.CreateInstance(Common.GetTypeOf((BLOCKTYPE)(int)this.json[i]["type"]), this.json[i]));

            }
        }


        public T FetchItemByID(int id)
        {
            if (false == this.database.ContainsKey(id))
                return null;
            return this.database[id];
            ////// return t.FetchItemByID(id);
            //// return (T)t.FetchItemByID(id);
            //for (int i = 0; i < database.Count; ++i)
            //{
            //    if (database[i].id == id)
            //        return database[i];
            //}
            //return null;
        }

        //private List<Skill> database = new List<Skill>();
        //private JsonData json;

        //private void Awake()
        //{
        //    this.json = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/skills.json"));
        //    //Debug.Log("items: " + InvenItemData);
        //}
        //void Start()
        //{
        //    ConstructDatabase();
        //    //Debug.Log("item: " + FetchItemByID(this.InvenItemData.Count).Description);
        //}

        //public Skill FetchItemByID(int id)
        //{
        //    for (int i = 0; i < database.Count; ++i)
        //    {
        //        if (database[i].category == id)
        //            return database[i];
        //    }
        //    return null;
        //}

        //void ConstructDatabase()
        //{
        //    for (int i = 0; i < this.json.Count; ++i)
        //         this.database.Add(new Skill(this.json[i]));
        //}
    }//..class ItemDatabase

    public abstract class JSonDatabase
    {
        public int id;  //
        public BLOCKTYPE type;
        public string Title;
        //2d 이미지용
        public Sprite Sprite;

        public JSonDatabase() { }
        public JSonDatabase(JsonData json) { }
        //public virtual void ConstructDatabase(JsonData json)

        public virtual void EnterTooltip(Tooltip tooltip)
        {
            tooltip.CreateTitle(this.Title);
            //tooltip.SetTitle(this.Title);
        }
        //public virtual void LeaveTooltip(Tooltip tooltip)
        //{ }
    }

}//..namespace MyCraft