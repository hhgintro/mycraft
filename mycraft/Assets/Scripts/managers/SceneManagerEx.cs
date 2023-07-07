using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyCraft
{
    public class SceneManagerEx
    {
        public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

        public void LoadScene(Define.Scene type)
        {
            Managers.Clear(false);

            SceneManager.LoadScene(GetSceneName(type));
        }

        string GetSceneName(Define.Scene type)
        {
            string name = System.Enum.GetName(typeof(Define.Scene), type);
            return name;
        }

        public void Clear()
        {
            CurrentScene?.Clear();
        }
    }
}