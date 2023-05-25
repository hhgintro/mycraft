using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject _mainmenu;
    public GameObject _playmenu;
    public GameObject _newgame;
    public GameObject _loadgame;

    void Start()
    {
        _mainmenu   = this.transform.Find("MainMenu").gameObject;
        _playmenu   = this.transform.Find("PlayMenu").gameObject;
        _newgame    = this.transform.Find("NewGame").gameObject;
        _loadgame   = this.transform.Find("LoadGame").gameObject;

        this._mainmenu.SetActive(true);
        this._playmenu.SetActive(false);
        this._newgame.SetActive(false);
        this._loadgame.SetActive(false);
    }
}
