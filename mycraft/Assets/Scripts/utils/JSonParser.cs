using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace MyCraft
{
    public class JSonParser<T> where T : JSonDatabase
    {
        private JsonData json;
        //private T t = new T();
        //public List<T> database = new List<T>();
        public Dictionary<int,T> database = new Dictionary<int,T>();

        public JSonParser(string filename)
        {
            try
            {
				//this.json = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/skills.json"));
                this.json = JsonMapper.ToObject(File.ReadAllText(filename));
                ConstructDatabase();
            }catch(Exception e)
            {
                Debug.LogException(e);
            }
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
				//this.database.Add((int)this.json[i]["id"], (T)Activator.CreateInstance(Common.GetTypeOf((BLOCKTYPE)(int)this.json[i]["type"]), this.json[i]));
				//case 4.
				//string jsontype = this.json[i]["type"].ToString();
				//Type type = Type.GetType(jsontype);
				//System.Object obj = Activator.CreateInstance(type, this.json[i]);
				//this.database.Add((int)this.json[i]["id"], (T)obj);
                string jsontype = "MyCraft." + this.json[i]["type"].ToString();
				this.database.Add((int)this.json[i]["id"], (T)Activator.CreateInstance(Type.GetType(jsontype), this.json[i]));

			}
		}


        public T FetchItemByID(int id)
        {
            if (false == this.database.ContainsKey(id))
                return null;
            return this.database[id];
        }

    }//..class JSonParser

}//..namespace MyCraft