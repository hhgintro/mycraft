using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class SystemMenuManager : MonoBehaviour
    {
        void Start()
        {

            LocaleManager.SetLocale("systemmenu", this.transform.Find("Menu/Title").GetComponent<Text>());
            LocaleManager.SetLocale("systemmenu", this.transform.Find("Menu/Continue/Text").GetComponent<Text>());
            LocaleManager.SetLocale("systemmenu", this.transform.Find("Menu/Retry/Text").GetComponent<Text>());
            LocaleManager.SetLocale("systemmenu", this.transform.Find("Menu/Exit/Text").GetComponent<Text>());

        }

        //public bool GetActive_1()
        //{
        //    if (null == canvas_ui || 1f != canvas_ui.alpha)
        //        return false;
        //    return true;
        //}
        //public void SetActive_1(bool active)
        //{
        //    if (null == canvas_ui) return;

        //    if (true == active)
        //    {
        //        canvas_ui.alpha = 1f;
        //        canvas_ui.blocksRaycasts = true;
        //        return;
        //    }

        //    canvas_ui.alpha = 0f;
        //    canvas_ui.blocksRaycasts = false;
        //}

        public void OnContinue()
        {
            this.gameObject.SetActive(false);
        }

        public void OnRetry()
        {
            //HG_TODO : 새게임하기로 다시 시작하도록 해야 합니다.
            //      현재는 OnContinue()와 동일한 기능으로 구현합니다.
            //this.SetActive(false);
            Managers.Game.bNewGame = true;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        public void OnExit()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

    }
}