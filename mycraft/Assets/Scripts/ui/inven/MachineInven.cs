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
    public class MachineInven : InvenBase
    {
        //public InvenItemData choiced_item = null;    //인벤에서 선택된 개체
        public GameObject _panelInvenMain { get; private set; }
        public GameObject _panelInvenSub { get; private set; }
        public GameObject _panelInvenLeaf { get; private set; }

        void Awake()
        {
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

            //HG_TEST : 테스트를 위해서 active로 설정합니다.
            //this.SetActive(false);
            //this.SetActive_1(true);



            //AddItem(1, 54);
            //AddItem(2, 54);

            //locale
            //title text
            Managers.Locale.SetLocale("inven", this.transform.GetChild(0).GetComponent<Text>());
        }

		// destroy : amount가 0이면 파괴
        public override void LinkInven(Building building, List<BuildingPanel> panels, List<Progress> progresses, bool destroy)
		{
            base.LinkInven(building, panels, progresses, destroy);

            //this.AddReset(base._panels[0]);
        }

        public override void Reset()
        {
            base._building.Reset();
            this.Clear();
            this.gameObject.SetActive(false);
            Managers.Game.Inventories.gameObject.SetActive(false);
        }

        public virtual GameObject CreateObject(Transform parent, GameObject prefab)
        {
            GameObject clone = UnityEngine.Object.Instantiate(prefab);//.GetComponent<Slot>();
            clone.transform.SetParent(parent, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            //clone.transform.position = parent.position;
            return clone;
        }

        public virtual GameObject CreateSlot(Transform parent, JSonDatabase database, GameObject prefab)
        {
            GameObject clone = this.CreateObject(parent, prefab);
            //clone.transform.SetParent(parent, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            ////clone.transform.position = parent.position;
            if(null != database)
                clone.GetComponent<Image>().sprite = database.icon;
            return clone;
        }

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