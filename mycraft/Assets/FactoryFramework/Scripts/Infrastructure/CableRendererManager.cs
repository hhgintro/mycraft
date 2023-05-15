using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    /// <summary>
    /// World Manager for cables used to create cables for an undirected graph (powergrid) without overdraw 
    /// or unecessary duplication of cables between nodes
    /// </summary>
    public class CableRendererManager : MonoBehaviour
    {
        public static CableRendererManager instance;
        protected GlobalLogisticsSettings settings { get { return ConveyorLogisticsUtils.settings; } }

        /// <summary>
        /// Dict of A-B: cable where A and B are PGConnectorss ordered by distance from world origin (0,0,0)
        /// </summary>
        private Dictionary<(PowerGridComponent, PowerGridComponent), GameObject> _cables;

        private void Awake()
        {
            instance = this;
            _cables = new Dictionary<(PowerGridComponent, PowerGridComponent), GameObject>();
        }
        private void OnDestroy()
        {
            instance = null;
        }

        public void Clear()
        {
            foreach (var go in _cables.Values) Destroy(go);
            _cables.Clear();
        }

        /// <summary>
        /// Sort Components so that we always return (Closest,Furthest) from origin
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public (PowerGridComponent, PowerGridComponent) Sort(PowerGridComponent A, PowerGridComponent B)
        {
            if (A == null || B == null) return (null, null);
            float distA = Vector3.SqrMagnitude(A.transform.position);
            float distB = Vector3.SqrMagnitude(B.transform.position);
            return (distB < distA) ? (B, A) : (A, B);
        }

        /// <summary>
        /// Create a Cable GameObject that looks like it connects two PowerGridComponents A and B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        public void AddCable(PowerGridComponent A, PowerGridComponent B)
        {
            if (A == B)
            {
                Debug.LogError($"Cannot connect a PowerGridComponent to itself: {A.name}");
                return;
            }
            (A, B) = Sort(A, B);
            if (_cables.ContainsKey((A, B)))
                return;
            GameObject cable = SpawnLineRenderCable(A, B);
            _cables.Add((A, B), cable);
        }
        /// <summary>
        /// Remove cable associated with PowerGridcomponents A and B if it exists
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        public void RemoveCable(PowerGridComponent A, PowerGridComponent B)
        {
            if (A == B)
            {
                Debug.LogError($"Cannot disconnect a PowerGridComponent to itself: {A.name}");
                return;
            }
            (A, B) = Sort(A, B);
            if (_cables.TryGetValue((A,B), out GameObject cable)) {
                Destroy(cable.gameObject);
                _cables.Remove((A, B));
            }
            
        }

        /// <summary>
        /// Create a GameObject to visually link two PowerGridComponents
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        protected GameObject SpawnLineRenderCable(PowerGridComponent A, PowerGridComponent B)
        {
            GameObject obj = new GameObject("Cable", new System.Type[] { typeof(LineRenderer), typeof(CableRenderer) });
            obj.transform.SetPositionAndRotation(transform.position, transform.rotation);

            LineRenderer lr = obj.GetComponent<LineRenderer>();
            lr.material = settings.CABLE_MATERIAL;
            lr.widthMultiplier = settings.CABLE_THICKNESS;

            CableRenderer cr = obj.GetComponent<CableRenderer>();
            cr.droop = settings.CABLE_DROOP;
            cr.segments = settings.CABLE_RESOLUTION;
            cr.SetAnchors(
                A.transform.TransformPoint(A.connectionPoint), 
                B.transform.TransformPoint(B.connectionPoint));

            return obj;
        }
    }
}