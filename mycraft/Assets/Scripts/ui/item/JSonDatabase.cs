using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace MyCraft
{
	public abstract class JSonDatabase
	{

		public int id;  //
		public string type;
		public string Title;
		public string Description;
		public Sprite icon;
		//public string building; //building prefab의 경로
		public GameObject prefab; //building prefab
		public float scaleOnBelt = 1.0f;

		//public JSonDatabase() { }
		public JSonDatabase(JsonData json)
		{
			this.id = (ushort)json["id"];
			this.type = json["type"].ToString();
			this.Title = Managers.Locale.GetLocale(section(), string.Format($"{this.id.ToString()}-title"));
			this.Description = Managers.Locale.GetLocale(section(), string.Format($"{this.id.ToString()}-desc"));
			//Debug.Log($"section({section()}), Title({this.Title})");
			this.icon = Managers.Resource.Load<Sprite>(json["icon"].ToString());

			LoadPrefab(json);
			LoadScaleOnBelt(json);
		}

		public virtual string section() { return "(empty)"; }		
		void LoadPrefab(JsonData json)
		{
			if (false == json.Keys.Contains("prefab"))
				return;

			string path = Path.Combine("prefabs", json["prefab"].ToString());
			this.prefab = Managers.Resource.Load<GameObject>(path);
			if (null == this.prefab)
			{
				Debug.LogError($"Error: not found prefab({path})");
				return;
			}

			if (prefab.TryGetComponent(out FactoryFramework.LogisticComponent componet))
				componet._itembase = (ItemBase)this;

			//if (prefab.TryGetComponent(out FactoryFramework.Conveyor _))
			//	Debug.Log($"{prefab.name}: componet<Conveyor>");
			//else if (prefab.TryGetComponent(out FactoryFramework.Driller _))
			//	Debug.Log($"{prefab.name}: componet<Producer>");
			//else
			//{
			//	if (prefab.TryGetComponent(out FactoryFramework.PowerGridComponent _) && prefab.TryGetComponent(out FactoryFramework.LogisticComponent _))
			//		Debug.Log($"{prefab.name}: componet<BOTH Logistic/PowerGrid>");
			//	else if (prefab.TryGetComponent(out FactoryFramework.PowerGridComponent _))
			//		Debug.Log($"{prefab.name}: componet<PowerGridComponent>");
			//	else if (prefab.TryGetComponent(out FactoryFramework.LogisticComponent _))
			//		Debug.Log($"{prefab.name}: componet<LogisticComponent>");
			//}
		}
		void LoadScaleOnBelt(JsonData json)
		{
			if (false == json.Keys.Contains("scaleOnBelt"))
				return;
			this.scaleOnBelt = float.Parse(json["scaleOnBelt"].ToString());
		}

		public virtual void EnterTooltip(Tooltip tooltip)
		{
			tooltip.CreateTitle(this.Title);
			//tooltip.SetTitle(this.Title);
		}
		//public virtual void LeaveTooltip(Tooltip tooltip)
		//{ }
	}//..JSonDatabase

}//..namespace MyCraft