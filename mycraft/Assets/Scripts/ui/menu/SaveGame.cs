using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class SaveGame : SaveContext
    {
        public InputField _new_name;  //다른이름으로저장(할 파일명)

        void Start()
        {
            base.Init();
                
            //locale
			Managers.Locale.SetLocale("save-game", this.transform.Find("Title").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Load File/Save Other Name/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Load File/Load File Info/File Detail/map version").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Load File/Load File Info/File Detail/scenario").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Load File/Load File Info/File Detail/difficult").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Load File/Load File Info/File Detail/play time").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Load File/Load File Info/File Detail/mode").GetComponent<Text>());
            Managers.Locale.SetLocale("save-game", this.transform.Find("Save/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("save-game", this.transform.Find("Back/Text").GetComponent<Text>());
        }

        private void OnEnable()
        {
            //refresh save files
            base.RefreshContext();
        }

        private IEnumerator SaveCoroutine()
        {
            yield return new WaitForEndOfFrame();

			Managers.Game.Save(this._new_name.text);
			this.gameObject.SetActive(false);//save창
			this.transform.parent.parent.GetComponent<SystemMenuManager>()?.gameObject.SetActive(true); //SystemMenu창
		}

		protected override void OnSelectSaveFile(string filename)
        {
            base.OnSelectSaveFile(filename);

            this._new_name.text = filename;
        }

        public void OnSaveGame()
        {
            if (string.IsNullOrEmpty(this._new_name.text)) return;
            Debug.Log($"save:{this._new_name.text}");
            
			StartCoroutine(SaveCoroutine());
		}

		public void OnBack()
        {
            //prev
            this.gameObject.SetActive(false);

            //HG_TODO:[통합방법모색] lobby에 호출될 때와 world에서 호출될 때. 각각 다른값을 호출하고 있다,
            //next(lobby)
            this.transform.parent.GetComponent<Menu>()?._playmenu.SetActive(true);
            //next(world)
            this.transform.parent.parent.GetComponent<SystemMenuManager>()?.gameObject.SetActive(true);
        }
    }
}