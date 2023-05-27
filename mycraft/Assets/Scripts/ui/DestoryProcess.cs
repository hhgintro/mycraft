using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DestoryProcess : MonoBehaviour
{
    GameObject _target;
    Image _progress;
    float _time = 1f;//(����:s)

    void Awake()
    {
        _progress = this.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        this._progress.fillAmount = 0;
    }


    public void SetProgress(GameObject target)
    {
        //target�� �ٲ��� ó������...
        if (_target != target)
        {
            _target = target;
            this._progress.fillAmount = 0;
        }
        if (null == _target) return;

        this._progress.fillAmount += Time.smoothDeltaTime / _time;
        if (1f <= this._progress.fillAmount)
        {
            //Debug.Log($"{_target} ������");
            //GameManager.GetTerrainManager().DeleteBlock(_target, true);
            this._progress.fillAmount = 0;
        }
    }
}
