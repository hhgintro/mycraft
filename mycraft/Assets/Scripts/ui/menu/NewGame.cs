﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class NewGame : MonoBehaviour
    {
        void Start()
        {
            //locale
            Managers.Locale.SetLocale("lobby", this.transform.Find("Title").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby", this.transform.Find("Create/Text").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby", this.transform.Find("Back/Text").GetComponent<Text>());

        }

        public void OnNewGame()
        {
            Managers.Game.bNewGame = true;
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