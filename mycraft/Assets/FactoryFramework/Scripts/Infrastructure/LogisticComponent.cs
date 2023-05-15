using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FactoryFramework
{
    /// <summary>
    /// Base class used for all things that create/process/destroy/transfer materials
    /// </summary>
    /// 
    [RequireComponent(typeof(SerializationReference))]
    public abstract class LogisticComponent : MonoBehaviour
    {
        protected GlobalLogisticsSettings settings { get { return ConveyorLogisticsUtils.settings; } }
        protected SerializationReference _sRef;
        public System.Guid GUID { get { 
                if (_sRef == null) _sRef ??= GetComponent<SerializationReference>();
                return _sRef.GUID; } } // set { _sRef.GUID = value; }

        protected PowerGridComponent _powerGridComponent;
        protected float PowerEfficiency 
        { get 
            { 
                if (this._powerGridComponent?.basePowerDraw > 0)
                {
                    return (_powerGridComponent?.grid?.Efficiency) ?? 0f;

                } else
                return 1f;
            } 
        }

        public bool IsWorking { get; protected set; }

        private void Awake()
        {
            _powerGridComponent ??= GetComponent<PowerGridComponent>();
            _sRef ??= GetComponent<SerializationReference>();
        }

        private void OnValidate()
        {
            _powerGridComponent ??= GetComponent<PowerGridComponent>();
            _sRef ??= GetComponent<SerializationReference>();
        }

        public virtual void ProcessLoop() { }

        

    }
}