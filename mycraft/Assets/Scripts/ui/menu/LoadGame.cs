using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
	public class LoadGame : SaveContext
	{

		protected override void fnStart()
		{
			//locale
			Managers.Locale.SetLocale("load-game", this.transform.Find("Title").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Load File/Load File Info/File Detail/map version").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Load File/Load File Info/File Detail/scenario").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Load File/Load File Info/File Detail/difficult").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Load File/Load File Info/File Detail/play time").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Load File/Load File Info/File Detail/mode").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Load/Text").GetComponent<Text>());
			Managers.Locale.SetLocale("load-game", this.transform.Find("Back/Text").GetComponent<Text>());
		}

		protected override void OnSelectSaveFile(GameObject go)
		{
			base.OnSelectSaveFile(go); 
			Managers.Game._load_filename = go.name;
		}

		public void OnLoadGame()
		{
			if (string.IsNullOrEmpty(Managers.Game._load_filename)) return;
			//Managers.Game.bNewGame = false;

			Managers.Input.Clear();

			//HG_TODO:[통합방법모색] lobby에 호출될 때와 world에서 호출될 때. 각각 다른값을 호출하고 있다,
			//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			//SceneManager.LoadScene("30.World");
			Managers.Scene.LoadScene(Define.Scene.World);
		}

		//public void OnBack()
		//{
		//	//prev
		//	this.gameObject.SetActive(false);

		//	//HG_TODO:[통합방법모색] lobby에 호출될 때와 world에서 호출될 때. 각각 다른값을 호출하고 있다,
		//	//next(lobby)
		//	this.transform.parent.GetComponent<Menu>()?._playmenu.SetActive(true);  //(lobby)Menu창
		//	//next(world)
		//	this.transform.parent.parent.GetComponent<SystemMenuManager>()?.gameObject.SetActive(true); //(world)SystemMenu창
		//}
	}
}