using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyCraft
{
    public abstract class BaseScene : MonoBehaviour
    {
        public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
        protected Object _eventsystem;

        void Awake()
        {
            fnAwake();
        }
        void Start()
        {
            fnStart();
        }
        void Update()
        {
            fnUpdate();
        }

        //private void OnDisable()
        //{
        //    fnDisable();
        //}

        protected virtual void fnAwake()
        {
            _eventsystem = GameObject.FindObjectOfType(typeof(EventSystem));
            if (_eventsystem == null) Managers.Resource.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";
        }
        protected virtual void fnStart() { }
        protected virtual void fnUpdate() { }
        //protected virtual void fnDisable() { }
        public virtual void Clear()
        {
            if (null != _eventsystem)
                Managers.Resource.Destroy(_eventsystem.GameObject());
        }
    }
}