using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour {

    #region singleton
    private static StageManager _instance;
    public static StageManager Instance
    {
        get
        {
            //if(null == _instance)
            //{
            //    _instance = FindObjectOfType(typeof(StageManager)) as StageManager;
            //    //_instance = this;
            //    if (null == _instance)
            //        Debug.Log("There's no active StageManager object");
            //}
            return _instance;
        }
    }
    #endregion


    #region NonSerialized Members
    //NonSerialized를 빼면. InGame씬만 독립으로 실행할때 이름이 비어있는 현상발견됨.
    [System.NonSerialized]
    public string bundlename = "stage_guitar2";
    [System.NonSerialized]
    public string audio = "기타2";            //AudioClip 이름
    [System.NonSerialized]
    public string track = "Guitar2_Track";    //Track 이름
    #endregion


    void Awake()
    {
        if (_instance != null)
            Destroy(_instance);
        _instance = this;

        DontDestroyOnLoad(this);
    }

    //// Use this for initialization
    //void Start()
    //{
    //    audio = "기타2";
    //    track = "Guitar2_Track";
    //}

    //// Update is called once per frame
    //void Update () {

    //   }

    //public void Log()
    //{
    //    Debug.Log("called StageManager by " + SceneManager.GetActiveScene().name);
    //}


}
