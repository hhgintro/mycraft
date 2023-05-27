using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MyCraft
{
    public class LobbyScene : BaseScene
    {
        void Awake()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
            SceneType = Define.Scene.Lobby;

            //미리읽어온다.
            JSonParser<ItemBase> itembases =  Managers.Game.ItemBases;
            JSonParser<TechBase> techbases =  Managers.Game.TechBases;
            JSonParser<Category> categories =  Managers.Game.Categories;
        }

        public override void Clear()
        {
            Debug.Log("Lobby Scene Clear!");
        }
    }
}