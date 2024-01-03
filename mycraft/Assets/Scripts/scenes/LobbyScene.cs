using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class LobbyScene : BaseScene
    {
        protected override void fnAwake()
        {
            base.fnAwake();
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