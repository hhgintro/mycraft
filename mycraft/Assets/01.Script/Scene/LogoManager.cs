using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoManager : MonoBehaviour {

    public float showtime = 1f;
	// Use this for initialization
	void Start () {
        StartCoroutine(NextScene());
	}

    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(showtime);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
