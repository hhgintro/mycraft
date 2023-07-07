using UnityEngine;
using UnityEngine.UI;


namespace MyCraft
{
    public class PlayMenu : MonoBehaviour
    {
        void Start()
        {
            //locale
            Managers.Locale.SetLocale("lobby-play-menu", this.transform.Find("Title").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-play-menu", this.transform.Find("NewGame/Text").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-play-menu", this.transform.Find("LoadGame/Text").GetComponent<Text>());
            Managers.Locale.SetLocale("lobby-play-menu", this.transform.Find("Back/Text").GetComponent<Text>());

        }

        public void OnNewGame()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            this.transform.parent.GetComponent<Menu>()._newgame.SetActive(true);
        }

        public void OnLoadGame()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            this.transform.parent.GetComponent<Menu>()._loadgame.SetActive(true);
        }

        public void OnBack()
        {
            //prev
            this.gameObject.SetActive(false);
            //next
            this.transform.parent.GetComponent<Menu>()._mainmenu.SetActive(true);
        }

    }
}