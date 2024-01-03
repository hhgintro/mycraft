using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class LogoScene : BaseScene
    {
        [SerializeField] float load_delay;
		[SerializeField] Text warning;  //경고문구(경로는 영문으로)


        protected override void fnAwake()
        {
            base.fnAwake();
            SceneType = Define.Scene.Logo;
            warning.gameObject.SetActive(false);    //hide

            //load ini
            Managers.Locale.Init(Application.dataPath + "/../config/config.ini", Application.dataPath + "/../config/locale/");
		}

        protected override void fnStart()
        {
            MyCraft.Managers.Input.KeyAction -= OnKeyDown_LogoScene;
			MyCraft.Managers.Input.KeyAction += OnKeyDown_LogoScene;
			
            //"(empty)"이면 진행을 못하도록 막는다.
			if (0 == string.Compare(Managers.Locale._locale.ToString(), "(empty)"))
            {
				warning.gameObject.SetActive(true);    //show
				return;
            }
            StartCoroutine(LoadNextScene1());
        }

        public override void Clear()
        {
            Debug.Log("Logo Scene Clear!");
        }

        IEnumerator LoadNextScene1()
        {
            yield return new WaitForSeconds(load_delay);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Managers.Scene.LoadScene(Define.Scene.Lobby);
        }

		void OnKeyDown_LogoScene()
		{
			//ESC
			if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(warning && true == warning.gameObject.activeSelf)
                    Application.Quit();
            }
		}

	}
}