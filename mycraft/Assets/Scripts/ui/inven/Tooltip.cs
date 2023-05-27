using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class Tooltip : MonoBehaviour
    {
        private JSonDatabase _itembase;
        //private string data;
        //private GameObject tooltip;

        //prefab
        //private GameObject prefabTitle;      //title
        //private GameObject prefabCost;       //소모 아이템 정보
        //private GameObject prefabComment;    //기타 설명
        //private GameObject prefabTotalcost;  //총재료,제작가능시설

        //private GameObject _title;      //title
        //private GameObject _cost;       //소모 아이템 정보
        //private GameObject _comment;    //기타 설명
        //private GameObject _totalcost;  //총재료,제작가능시설
        //private GameObject _slot;       //
        //private GameObject _skill;      //

        private List<GameObject> _objs = new List<GameObject>();

        //private Text textTooltip;

        void Awake()
        {
            //this.tooltip = GameObject.Find("Gameplay UI/Tooltip");

            //_title = Managers.Resource.Load<GameObject>("prefabs/ui/Tooltip-Title");
            //_cost = Managers.Resource.Load<GameObject>("prefabs/ui/Tooltip-Cost");
            //_comment = Managers.Resource.Load<GameObject>("prefabs/ui/Tooltip-Comment");
            //_totalcost = Managers.Resource.Load<GameObject>("prefabs/ui/Tooltip-TotalCost");
            //_slot = Managers.Resource.Load<GameObject>("prefabs/ui/Slot");
            //_skill = Managers.Resource.Load<GameObject>("prefabs/ui/Skill");

            //this.GetComponent<CanvasGroup>().alpha = 0f;
            //this.gameObject.SetActive(false);

        }

        void OnEnable()
        {
            StartCoroutine(CheckPosition());
        }

        //protected GameObject CreateObject(GameObject obj, Transform parent)
        //{
        //    GameObject clone = UnityEngine.Object.Instantiate(obj);
        //    clone.transform.SetParent(parent, false); //[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
        //    //clone.transform.position = parent.position;
        //    this._objs.Add(clone);
        //    return clone;
        //}

        //public GameObject SetTitle(string text)
        //{
        //    GameObject clone = this.transform.Find("Tooltip-Title").gameObject;
        //    clone.GetComponent<Text>().text = text;
        //    return clone;
        //}
        public GameObject CreateTitle(string text)
        {
            //GameObject clone = CreateObject(_title, this.transform);
            GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Tooltip-Title", this.transform);
            clone.GetComponentInChildren<Text>().text = text;
            this._objs.Add(clone);
            return clone;
        }
        public GameObject CreateCost()
        {
            //GameObject clone = CreateObject(_cost, this.transform);
            GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Tooltip-Cost", this.transform);
            this._objs.Add(clone);
            return clone;
        }
        public GameObject CreateComment()
        {
            //GameObject clone = CreateObject(_comment, this.transform);
            GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Tooltip-Comment", this.transform);
            this._objs.Add(clone);
            return clone;
        }
        public GameObject CreateTotalCost()
        {
            //GameObject clone = CreateObject(_totalcost, this.transform);
            GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Tooltip-TotalCost", this.transform);
            this._objs.Add(clone);
            return clone;
        }
        public GameObject CreateSlot(Transform parent)
        {
            //GameObject clone = CreateObject(_slot, parent);
            GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Slot", this.transform);
            this._objs.Add(clone);
            return clone;
        }
        public GameObject CreateSkill(Transform parent)
        {
            //GameObject clone = CreateObject(_skill, parent);
            GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Skill", this.transform);
            this._objs.Add(clone);
            return clone;
        }

        IEnumerator CheckPosition()
        {
            while (true)
            {
                CheckPosition_Func();
                yield return 0;
            }
        }

        void CheckPosition_Func()
        {
            if (false == this.gameObject.activeSelf)
                return;


            float ratio = 60f;
            Vector3 pos = Input.mousePosition;

            RectTransform rt = (RectTransform)this.gameObject.transform;

            //width
            if (Screen.width < Input.mousePosition.x + rt.sizeDelta.x + ratio)
                pos -= Vector3.right * (rt.sizeDelta.x + 60f);
            else
                pos += Vector3.right * 60f;

            //height
            if (Input.mousePosition.y < rt.sizeDelta.y + ratio)
                pos += Vector3.up * rt.sizeDelta.y;


            //this.gameObject.transform.position = new Vector3(1000, 500);
            this.gameObject.transform.position = pos;

        }

        public void Activate(JSonDatabase itembase)
        {
            if(itembase == this._itembase) return;//동일하면...무시

            //old
            if (null != this._itembase)
                DestructDataString();
            //new
            this._itembase = itembase;
            ConstructDataString();
            this.gameObject.SetActive(true);
            //this.GetComponent<CanvasGroup>().alpha = 1f;
        }

        public void Deactivate()
        {
            //this.GetComponent<CanvasGroup>().alpha = 0f;
            this.gameObject.SetActive(false);
            DestructDataString();
        }

        public void ConstructDataString()
        {
            if (null != this._itembase)
                this._itembase.EnterTooltip(this);
            //data = "";
            //if (null != _itembase)
            //{
            //    //data = "<color=#0473f0><b>" + _itembase.Title + "</b></color>\n\n" + _itembase.Description + "\npower" + _itembase.Power;
            //    data = "<color=#0473f0><b>" + _itembase.Title + "</b></color>";
            //}

            //if (0 < this.transform.childCount)
            //    this.transform.GetChild(0).GetComponent<Text>().text = data;


            //_objs.Add(CreateTitle());
            //_objs.Add(CreateCost());
            //_objs.Add(CreateComment());
            //_objs.Add(CreateTotalCost());

            ////GameObject inventoryPanel = Resources.Load<GameObject>("prefab/ui/Slot Panel") as GameObject;
            ////GameObject objPanel = UnityEngine.Object.Instantiate(inventoryPanel);
            ////objPanel.transform.SetParent(this.transform, false);//[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
            ////objPanel.name = "Slot-Panel-";// i.ToString("D2");

            ////if (null != _itembase)
            ////    _itembase.GetToolTip(this);
            ////else
            ////{
            ////    data = "";
            ////    if (0 < this.transform.childCount)
            ////        this.transform.GetChild(0).GetComponent<Text>().text = data;
            ////}
        }
        public void DestructDataString()
        {
            if (null == this._itembase)
                return;

            for (int i = 0; i < this._objs.Count; ++i)
                Managers.Resource.Destroy(this._objs[i].gameObject);
            //this._itembase.LeaveTooltip(this);
            this._itembase = null;

        }
    }
}