using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class LocaleManager
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(string section, string key, int def, string filePath);


        public StringBuilder _locale { get; private set; }
        Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();

        public void Init(string pathConfig, string pathLocale)
        {
            if (null != _locale) return;
            //locale
            _locale = new StringBuilder(255);
            GetPrivateProfileString("common", "locale", "(empty)", _locale, 255, pathConfig);
            if(true == string.IsNullOrEmpty(_locale.ToString()))
            {
                Debug.LogError("Fail : Not Read Locale");
                _locale = null;
                return;
            }

            Open(pathLocale + _locale.ToString() + "/ui.cfg");
        }

        void Open(string filename)
        {
            //this.cfg.Open(Application.streamingAssetsPath + "/locale" + "/ko" + "/locale.cfg");
            //this.cfg.Open(filename);
            //foreach(var v in cfg.values)
            //    Debug.Log("[" + v.Key.ToString() + ":" + v.Value.ToString() + "]");

            //clear
            sections.Clear();

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs, false))
                    {
                        string strLineValue = null;
                        string[] tmp = null;

                        Dictionary<string, string> values = null;
                        while ((strLineValue = sr.ReadLine()) != null)
                        {
                            if (string.IsNullOrEmpty(strLineValue)) continue;
                            //section
                            if (true == strLineValue.Contains("[") && true == strLineValue.Contains("]"))
                            {
                                int start = strLineValue.IndexOf("[");
                                int end = strLineValue.IndexOf("]");
                                string key = strLineValue.Substring(start+1, end-start-1);
                                //Debug.Log("key: " + key);
                                if(true == sections.ContainsKey(key))
                                {
                                    Debug.LogError($"critical: same key({key})");
                                    continue;
                                }

                                sections.Add(key, values = new Dictionary<string, string>());
                                continue;
                            }
                            //key / value
                            if (false == strLineValue.Contains("=")) continue;

                            tmp = strLineValue.Split('=');
                            values.Add(tmp[0], tmp[1]);
                        }
                        fs.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public string GetLocale(string section, string key)
        {
            if (false == sections.ContainsKey(section))         return key;
            if (false == sections[section].ContainsKey(key))    return key;
            return sections[section][key];
        }

        public void SetLocale(string section, Text title)
        {
            try { title.text = GetLocale(section, title.text); } catch(Exception e) { Debug.LogError(e.Message); }
        }

    }
}
