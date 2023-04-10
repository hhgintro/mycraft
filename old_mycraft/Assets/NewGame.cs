using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class NewGame : MonoBehaviour
    {
        PlayMenu playmenu;
        private CanvasGroup canvas_ui;

        // Use this for initialization
        void Start()
        {
            this.playmenu = GameObject.Find("Canvas/PlayMenu").GetComponent<PlayMenu>();
            this.canvas_ui = this.transform.GetComponent<CanvasGroup>();

            this.SetActive(false);

            //locale
            LocaleManager.SetLocale("lobby", this.transform.Find("Title").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Create/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Back/Text").GetComponent<Text>());

        }

        //   // Update is called once per frame
        //   void Update () {

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

        public void OnNewGame()
        {
            LobbyManager.Instance().bNewGame = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OnBack()
        {
            //prev
            this.SetActive(false);
            //next
            this.playmenu.SetActive(true);
        }



    }
}