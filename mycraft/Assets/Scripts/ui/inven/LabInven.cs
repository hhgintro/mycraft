using FactoryFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyCraft
{
	public class LabInven : InvenBase
	{
		//public InvenItemData choiced_item = null;    //인벤에서 선택된 개체

		void Awake()
		{
			//HG_TODO: 추후 아래 object들을 생성하도록 합니다.
			base._progress.Add(this.transform.Find("Progress/bar").GetComponent<Image>());

			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
				, this.transform.Find("Input-Panel")));

			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
				, this.transform.Find("Output-Panel")));

			base._panels.Add(new InvenPanel(base._panels.Count, 0, this
				, this.transform.Find("Chip-Panel")));
		}

        void Start()
        {
			//locale
			//title text
			Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
		}

		public override bool CheckPickupGoods() { return true; }
		public override void OnReset()
		{
			base._building.OnReset();
			this.Clear();
			//active
			Managers.Game.SkillInvens.gameObject.SetActive(true);
			//de-active
			this.gameObject.SetActive(false);
			Managers.Game.Inventories.gameObject.SetActive(false);
		}

		public override void LinkInven(Building building, Dictionary<int, int> inputs, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
			base.LinkInven(building, inputs, panels, progresses, destroy);
			this.AddReset(base._panels[0]);
		}
		// destroy : amount가 0이면 파괴
		public override void LinkInven(Building building, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
			base.LinkInven(building, panels, progresses, destroy);

			//this.AddReset(base._panels[0]);
			this.AddReset(base._panels[0]);
		}
			//protected override void Clear()
			//{
			//    base.Clear();
			//}

			//public override void LinkInven(BlockScript block, int slotAmount, List<List<BlockSlot>> slots)
			//{
			//    base.LinkInven(block, slotAmount, slots);
			//}


			//public override void Save(BinaryWriter writer)
			//{
			//    base.Save(writer);
			//}

			//public override void Load(BinaryReader reader)
			//{
			//    base.Load(reader);
			//}


		}//..class Inventory
	}//..namespace MyCraft