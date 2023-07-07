using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
    public class MainMenu : MonoBehaviour
    {
        void Start()
        {
            //locale
            Managers.Locale.SetLocale("lobby-main-menu", this.transform.Find("Title").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-main-menu", this.transform.Find("PlayButton/Text").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-main-menu", this.transform.Find("MapEditButton/Text").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-main-menu", this.transform.Find("OptionButton/Text").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-main-menu", this.transform.Find("Exit/Text").GetComponent<Text>());
        }

        public void OnPlayMenu()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            this.transform.parent.GetComponent<Menu>()._playmenu.SetActive(true);
        }

        public void OnMapEditMenu()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            //this.mapeditmenu.SetActive(true);
        }

        public void OnOptionMenu()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            //this.optionmenu.SetActive(true);
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}