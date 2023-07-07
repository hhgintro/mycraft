using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
	public class Tooltip : MonoBehaviour
	{
		private JSonDatabase _itembase;
		private string data;
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

		//public void SetTitle(string text)
		//{
		//    string title = string.Format($"<color=#0473f0><b>{text}</b></color>"); 
		//    this.transform.GetChild(0).GetComponent<Text>().text = title;
		//}
		public GameObject CreateTitle(string text)
		{
			//GameObject clone = CreateObject(_title, this.transform);
			GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Tooltip-Title", this.transform);
			clone.GetComponentInChildren<Text>().text = string.Format($"<color=#0473f0><b>{text}</b></color>");
			clone.transform.position = Vector3.zero;
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
			GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Skill", null);
			clone.transform.SetParent(parent, false); //[HG2017.05.19]false : Cause Grid layout not scale with screen resolution
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
			if (false == this.gameObject.activeSelf) return;

			float ratio = 120f;	//마우스와 tooltip 이격거리
			Vector3 pos = Input.mousePosition;
			RectTransform rt = (RectTransform)this.gameObject.transform;

			//좌측 상단을 (원점)기준으로
			//x
			if (Screen.width < Input.mousePosition.x + rt.sizeDelta.x + ratio)
				pos -= Vector3.right * (rt.sizeDelta.x + ratio);
			else
				pos += Vector3.right * ratio;

			//y
			if (Input.mousePosition.y < rt.sizeDelta.y)
				pos += Vector3.up * rt.sizeDelta.y;

			//this.gameObject.transform.position = new Vector3(1000, 500);
			this.gameObject.transform.position = pos;
		}

		public void Activate(JSonDatabase itembase)
		{
			if (itembase == this._itembase) return;//동일하면...무시

			//old
			if (null != this._itembase) DestructDataString();
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
			//title
			if (null != this._itembase) this._itembase.EnterTooltip(this);

			//CreateTitle();
			//CreateCost();
			//CreateComment();
			//CreateTotalCost();

			// 부모의 크기를 업데이트합니다.
			UpdateParentSize();
		}
		public void DestructDataString()
		{
			if (null == this._itembase)
				return;

			for (int i = 0; i < this._objs.Count; ++i)
				Managers.Resource.Destroy(this._objs[i].gameObject);
			//this._itembase.LeaveTooltip(this);
			_objs.Clear();
			this._itembase = null;

		}

		private void UpdateParentSize()
		{
			RectTransform parentRectTransform = (RectTransform)this.transform;

			// 현재는 자식 UI 들이 수직으로 배치된다고 가정합니다.
			float maxHeight = 0f;
			float totalWidth = 0f;
			Vector2 padding = new Vector2(20, 20);

			for (int i = 0; i < parentRectTransform.childCount; i++)
			{
				RectTransform childRect = parentRectTransform.GetChild(i).GetComponent<RectTransform>();
				Vector2 childSize = childRect.rect.size;

				if (childRect.gameObject.activeSelf) // 자식 UI가 활성화 상태일 때만 처리합니다.
				{
					maxHeight += childSize.y;
					totalWidth = Mathf.Max(totalWidth, childSize.x);
				}
			}

			// 총 높이에는 padding값도 더합니다.
			maxHeight += padding.y;

			// 부모 UI의 크기를 자식 UI의 크기에 맞춥니다 (여백 값을 추가합니다)
			parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth + padding.x);
			parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxHeight);
		}
	}
}