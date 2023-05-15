using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    [System.Serializable]
    public abstract class Socket : MonoBehaviour
    {
        /// <summary>
        /// What building or conveyor belt this is connected to
        /// </summary>
        public LogisticComponent _logisticComponent;
        [SerializeField] protected GameObject _visualIndicator;


        public virtual void Connect(Socket sock) {}
        public virtual void Disconnect() {}
        public virtual bool IsOpen() { return false; }

        // TODO add a item filter. Maybe whitelist/blacklist List<Item>
        
    }
}