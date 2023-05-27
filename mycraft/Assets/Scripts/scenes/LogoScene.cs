﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MyCraft
{
    public class LogoScene : BaseScene
    {
        [SerializeField] float load_delay;

        void Awake()
        {
            Init();
        }

        void Start()
        {
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
            Managers.Locale.Init(Application.dataPath + "/../config/config.ini", Application.streamingAssetsPath + "/locale/");
        }

        public override void Clear()
        {
            Debug.Log("Logo Scene Clear!");
        }
    }
}