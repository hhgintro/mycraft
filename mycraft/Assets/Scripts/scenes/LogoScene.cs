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


		void Awake()
        {
            Init();
			warning.gameObject.SetActive(false);    //hide
		}

        void Start()
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

        IEnumerator LoadNextScene1()
        {
            yield return new WaitForSeconds(load_delay);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }


        protected override void Init()
        {
            base.Init();
            SceneType = Define.Scene.Logo;

			//load ini
			Managers.Locale.Init(Application.dataPath + "/../config/config.ini", Application.dataPath + "/../config/locale/");
    	}

        public override void Clear()
        {
            Debug.Log("Logo Scene Clear!");
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