using UnityEngine;
using System.Collections;
using MyCraft;

public class ScreenShot : MonoBehaviour {
	string format;
	
	// Use this for initialization
	void Start () {
		format = "yyyy-MM-dd-HH-mm-ss"; 
	}
	
	// Update is called once per frame
	void Update () {
        if (Managers.Chat.gameObject.gameObject.activeSelf)
            return; //chatting창 열려있으면 못움직인다.
        
		if (Input.GetKeyDown(KeyCode.Space)){
			ScreenCapture.CaptureScreenshot(Application.dataPath + "/" + System.DateTime.Now.ToString(format) +".png");
		}
	}
}