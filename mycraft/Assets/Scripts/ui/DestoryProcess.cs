//using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;


public class DestoryProcess : MonoBehaviour
{
	IPlacement _owner;
	GameObject _target;
    Image _progress;
    float _time = 1f;//(단위:s)

    void Awake()
    {
        _progress = this.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        this._progress.fillAmount = 0;
    }


    public void SetProgress(IPlacement owner, GameObject target, float time=2f)
    {
		//target이 바꿨어면 처음부터...
        if (_target != target)
        {
			this._owner     = owner;
			this._target    = target;
            this._time      = time;
            this._progress.fillAmount = 1f;
		}
        if (null == _target) return;

        this._progress.fillAmount -= Time.deltaTime / _time;
		//Debug.Log($"fill: {this._progress.fillAmount}");
		if (this._progress.fillAmount <= 0f)
        {
            //Debug.Log($"{_target.name} 철거");
            _owner.DestroyBuilding(_target);
            ////this._progress.fillAmount = 0f;
            //this._progress.fillAmount = 1f;
        }
    }
}
