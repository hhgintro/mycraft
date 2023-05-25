using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class LoadGame : MonoBehaviour
    {
        void Start()
        {
            //locale
            LocaleManager.SetLocale("lobby", this.transform.Find("Title").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Load/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Back/Text").GetComponent<Text>());

        }

        public void OnLoadGame()
        {
            Managers.Game.bNewGame = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OnBack()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            this.transform.parent.GetComponent<Menu>()._playmenu.SetActive(true);
        }

    }
}