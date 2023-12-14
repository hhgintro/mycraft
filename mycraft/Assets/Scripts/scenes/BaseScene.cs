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

		protected virtual void Init()
        {
            _eventsystem = GameObject.FindObjectOfType(typeof(EventSystem));
            if (_eventsystem == null) Managers.Resource.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";
        }

        public virtual void Clear()
        {
            if (null != _eventsystem)
                Managers.Resource.Destroy(_eventsystem.GameObject());
        }
    }
}