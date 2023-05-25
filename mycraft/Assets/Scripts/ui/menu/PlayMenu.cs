using UnityEngine;
using UnityEngine.UI;


namespace MyCraft
{
    public class PlayMenu : MonoBehaviour
    {
        void Start()
        {
            //locale
            LocaleManager.SetLocale("lobby", this.transform.Find("Title").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("NewGame/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("LoadGame/Text").GetComponent<Text>());
            LocaleManager.SetLocale("lobby", this.transform.Find("Back/Text").GetComponent<Text>());

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