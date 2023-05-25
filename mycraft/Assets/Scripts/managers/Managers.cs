using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class Managers : MonoBehaviour
    {
        static Managers s_instance; // 유일성이 보장된다
        static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

        //DataManager _data = new DataManager();
        GameManager _game = new GameManager();
        InputManager _input = new InputManager();
        PoolManager _pool = new PoolManager();
        ResourceManager _resource = new ResourceManager();
        SceneManagerEx _scene = new SceneManagerEx();
        SoundManager _sound = new SoundManager();
        //UIManager _ui = new UIManager();

        GameObject _systemmenu;
        //ChatManager _chat_manager;

        private static Coordinates _coodinates;  //좌표


        //public static DataManager Data { get { return Instance._data; } }
        public static GameManager Game { get { return Instance._game; } }
        public static InputManager Input { get { return Instance._input; } }
        public static PoolManager Pool { get { return Instance._pool; } }
        public static ResourceManager Resource { get { return Instance._resource; } }
        public static SceneManagerEx Scene { get { return Instance._scene; } }
        public static SoundManager Sound { get { return Instance._sound; } }
        //public static UIManager UI { get { return Instance._ui; } }

        public static GameObject SystemMenu { get { if (null == Instance._systemmenu) Instance._systemmenu = GameObject.Find("Canvas/SystemMenu"); return Instance._systemmenu; } }
        //public static ChatManager Chat { get { if (null == Instance._chat_manager) Instance._chat_manager = GameObject.Find("Canvas/Chatting").GetComponent<ChatManager>(); return Instance._chat_manager; } }

        public static Coordinates Coordinates
        {
            get
            {
                if (null == _coodinates)
                    _coodinates = GameObject.Find("Gameplay UI/Coordinates").GetComponent<Coordinates>();
                return _coodinates;
            }
        }


        void Start()
        {
            Init();
        }

        void Update()
        {
            //_input.OnUpdate();
        }

        static void Init()
        {
            if (s_instance == null)
            {
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                s_instance = go.GetComponent<Managers>();

                s_instance._game.Init();
                //s_instance._data.Init();
                s_instance._pool.Init();
                s_instance._sound.Init();
            }
        }

        public static void Clear()
        {
            Game.Clear();
            Input.Clear();
            Sound.Clear();
            Scene.Clear();
            //UI.Clear();
            Pool.Clear();
        }
    }
}//..namespace MyCraft
