using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class MainMenu : MonoBehaviour
    {

        private PlayMenu playmenu;
        //MainMenu mapeditmenu;
        //OptionMenu optionmenu;

        private CanvasGroup canvas_ui;

        // Use this for initialization
        void Start()
        {
            this.canvas_ui = this.transform.GetComponent<CanvasGroup>();

            this.playmenu = GameObject.Find("Canvas/PlayMenu").GetComponent<PlayMenu>();
            //this.mapeditmenu = GameObject.Find("Canvas/MapEditMenu").gameObject;
            //this.optionmenu = GameObject.Find("Canvas/OptionMenu").gameObject;

            this.playmenu.SetActive(false);
            //this.mapeditmenu.SetActive(false);
            //this.optionmenu.SetActive(false);

            //self
            this.SetActive(true);


            //locale
            LocaleManager.SetLocale("lobby", this.transform.Find("Title").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("PlayButton/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("MapEditButton/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("OptionButton/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Exit/Text").GetComponent<Text>());
        }

        //// Update is called once per frame
        //void Update () {

        //}

        public bool GetActive()
        {
            if (null == canvas_ui || 1f != canvas_ui.alpha)
                return false;
            return true;
        }
        public void SetActive(bool active)
        {
            if (null == canvas_ui) return;

            if (true == active)
            {
                canvas_ui.alpha = 1f;
                canvas_ui.blocksRaycasts = true;
                return;
            }

            canvas_ui.alpha = 0f;
            canvas_ui.blocksRaycasts = false;
        }

        public void OnPlayMenu()
        {
            //prev
            this.SetActive(false);
            //next
            this.playmenu.SetActive(true);
        }

        public void OnMapEditMenu()
        {
            //prev
            this.SetActive(false);
            //next
            //this.mapeditmenu.SetActive(true);
        }

        public void OnOptionMenu()
        {
            //prev
            this.SetActive(false);
            //next
            //this.optionmenu.SetActive(true);
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}