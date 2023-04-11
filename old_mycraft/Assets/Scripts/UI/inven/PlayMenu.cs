using UnityEngine;
using UnityEngine.UI;


namespace MyCraft
{
    public class PlayMenu : MonoBehaviour
    {
        MainMenu mainmenu;

        NewGame newgame;
        LoadGame loadgame;
        private CanvasGroup canvas_ui;


        // Use this for initialization
        void Start()
        {
            this.mainmenu = GameObject.Find("Canvas/MainMenu").GetComponent<MainMenu>();

            this.newgame = GameObject.Find("Canvas/NewGame").GetComponent<NewGame>();
            this.loadgame = GameObject.Find("Canvas/LoadGame").GetComponent<LoadGame>();
            this.canvas_ui = this.transform.GetComponent<CanvasGroup>();

            this.newgame.SetActive(false);
            this.loadgame.SetActive(false);
            this.SetActive(false);

            //locale
            LocaleManager.SetLocale("lobby", this.transform.Find("Title").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("NewGame/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("LoadGame/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Back/Text").GetComponent<Text>());

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

        public void OnNewGame()
        {
            //prev
            this.SetActive(false);
            //next
            this.newgame.SetActive(true);
        }

        public void OnLoadGame()
        {
            //prev
            this.SetActive(false);
            //next
            this.loadgame.SetActive(true);
        }

        public void OnBack()
        {
            //prev
            this.SetActive(false);
            //next
            this.mainmenu.SetActive(true);
        }

    }
}