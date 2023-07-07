using MyCraft;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
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
        LocaleManager _locale = new LocaleManager();
        PoolManager _pool = new PoolManager();
        ResourceManager _resource = new ResourceManager();
        SceneManagerEx _scene = new SceneManagerEx();
        SoundManager _sound = new SoundManager();
        //UIManager _ui = new UIManager();



        //public static DataManager Data { get { return Instance._data; } }
        public static GameManager Game { get { return Instance._game; } }
        public static InputManager Input { get { return Instance._input; } }
        public static LocaleManager Locale {  get { return Instance._locale; } }
        public static PoolManager Pool { get { return Instance._pool; } }
        public static ResourceManager Resource { get { return Instance._resource; } }
        public static SceneManagerEx Scene { get { return Instance._scene; } }
        public static SoundManager Sound { get { return Instance._sound; } }
        //public static UIManager UI { get { return Instance._ui; } }


        public static BuildingPlacement _buildingPlacement;
        public static ConveyorPlacement _conveyorPlacement;
        public static GameObject _choiced_building = null;

        void Start()
        {
            Init();


        }

        void Update()
        {
            _input.OnUpdate();
        }

		private void OnDestroy()
		{
            Debug.Log("Manager.Clear()");
            Clear(true);
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

        public static void Clear(bool destory)
        {
            Game.Clear();
            Input.Clear();
            Sound.Clear();
            Scene.Clear();
            //UI.Clear();
            if(true == destory) Pool.Clear(); //불러오기를 위해서 삭제하지 않는다.
        }
    }
}//..namespace MyCraft
