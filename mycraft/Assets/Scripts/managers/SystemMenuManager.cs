using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class SystemMenuManager : MonoBehaviour
    {
		private GameObject _load_game;
		private GameObject _save_game;

        void Start()
        {
			_load_game = this.transform.GetChild(0).GetChild(2).gameObject;
			_save_game = this.transform.GetChild(0).GetChild(3).gameObject;
			_load_game.SetActive(false);
			_save_game.SetActive(false);


			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Title").GetComponent<Text>());
			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Buttons/Continue/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Buttons/Retry/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Buttons/Load/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Buttons/Save/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Buttons/Option/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("systemmenu", this.transform.Find("Menu/Buttons/Exit/Text").GetComponent<Text>());

		}

		private void OnEnable()
		{
			_load_game?.SetActive(false);
			_save_game?.SetActive(false);

			Time.timeScale = 0.0f;
			AudioListener.pause = true;		//음악정지
		}

		private void OnDisable()
		{
			Time.timeScale = 1.0f;
			AudioListener.pause = false;
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
            //Managers.Game.bNewGame = true;
            Managers.Game._load_filename = null;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        public void OnLoad()
        {
			this._load_game.SetActive(true);
			//this.gameObject.SetActive(false);
		}

		public void OnSave()
        {
			this._save_game.SetActive(true);
		}

		public void OnOption()
        {

        }

        public void OnExit()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

    }
}