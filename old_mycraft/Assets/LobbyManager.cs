using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;


namespace MyCraft
{
    public class LobbyManager : MonoBehaviour
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string section, string key, int def, string filePath);




        public bool bNewGame { get; set; }

        public StringBuilder _locale;

        private static LobbyManager _instance;
        public static LobbyManager Instance()
        {
            if (null == _instance)
                _instance = new LobbyManager();
            return _instance;
        }

        private void Awake()
        {
            //this.bNewGame = true;
            _instance = this;

            //load ini
            Load("/../config/config.ini");
            LocaleManager.Open(Application.streamingAssetsPath + "/locale/" + _locale.ToString() + "/ui.cfg");

            DontDestroyOnLoad(this);
        }

        private void Load(string filepath)
        {
            string Path = Application.dataPath + filepath;
            //locale
            _locale = new StringBuilder(255);
            GetPrivateProfileString("common", "locale", "(empty)", _locale, 255, Path);
        }

        //// Use this for initialization
        //private void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update () {

        //}
    }
}