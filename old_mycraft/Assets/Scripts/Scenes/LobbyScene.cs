using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MyCraft
{
    public class LobbyScene : BaseScene
    {
        protected override void Init()
        {
            base.Init();

            SceneType = Define.Scene.Lobby;
        }

        public override void Clear()
        {
            Debug.Log("Lobby Scene Clear!");
        }
    }
}