using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryFramework
{
    public class PowerGrid : MonoBehaviour
    {
        // adjacency list node N: connected to nodes {a,b,c}
        public HashSet<PowerGridComponent> nodes; // List

        private Color _debugColor;

        private void Awake()
        {
            nodes = new HashSet<PowerGridComponent>();

            _debugColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }

        public float Load{
            get{ 
                if (nodes == null) return 0f; 
                return nodes.Where(n => n.basePowerDraw > 0).Sum(n => n.basePowerDraw);
            }
        }
        public float Production{
            get {
                if (nodes == null) return 0f;
                return Mathf.Abs(nodes.Where(n => n.basePowerDraw<0).Sum(n => n.basePowerDraw));
            }
        }
        private float NetPower { get { return nodes.Sum(n => n.basePowerDraw); } }
        public float Efficiency { get { return (Production > 0f) ? Mathf.Clamp01(Production / Load) : 0f; } }

        public bool debug_info = false;

        /// <summary>
        /// Internal connection between the underlying node classes
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(PowerGridComponent node) 
        {
            if (!nodes.Contains(node)) nodes.Add(node);
        }

        public static PowerGrid NewGrid(PowerGridComponent root)
        {
            GameObject gridObj = new GameObject() { name= "Power Grid" };
            gridObj.transform.SetPositionAndRotation(root.transform.position, root.transform.rotation);
            PowerGrid grid = gridObj.AddComponent<PowerGrid>();

            HashSet<PowerGridComponent> visited = new HashSet<PowerGridComponent>();

            grid.AddNode(root);
            visited.Add(root);
            root.grid = grid;
            // rebuild the graph(s) with DFS
            void NewGridHelper(PowerGridComponent parent, PowerGridComponent n)
            {
                if (visited.Contains(n)) return;
                parent.Connect(n);
                visited.Add(n);
                foreach (var connection in n.Connections.ToArray())
                    NewGridHelper(n, connection);
            }
            foreach (var connection in root.Connections.ToArray())
                NewGridHelper(root, connection);

            return grid;

        }
        
        public static bool UnionGraphs(PowerGrid a, PowerGrid b)
        {
            if (a == b) return false;
            // graphs can merge?
            if (a.nodes.Any(n => b.nodes.Contains(n)))
            {
                // keep bigger graph
                if (a.nodes.Count < b.nodes.Count)
                    return UnionGraphs(b, a);
                // a is definitely the bigger graph
                foreach (var n in b.nodes)
                {
                    // add to grid and set the active grid
                    a.AddNode(n);
                    n.grid = a;
                }

                // destroy b
                Destroy(b.gameObject);
                return true;
            } else
            {
                return false;
            }
        }
        

        private void OnDrawGizmosSelected()
        {
            if (!debug_info) return;

            foreach(PowerGridComponent pgc in nodes)
            {
                Gizmos.color = (pgc.basePowerDraw < 0) ? Color.green : (pgc.basePowerDraw == 0) ? Color.black : Color.red;
                Gizmos.DrawSphere(pgc.transform.position, .6f);

                foreach(PowerGridComponent connection in pgc.Connections)
                {
                    Gizmos.color = _debugColor;
                    Gizmos.DrawLine(pgc.transform.position + Vector3.up, connection.transform.position + Vector3.up);
                }
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            s.AppendJoin('\n', nodes.Select(n => n.ToString()));

            return s.ToString();
        }

        private void OnGUI()
        {
            if (!debug_info) return;
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 200,  200, 400, 400));
            GUILayout.Label($"load: {Load}");
            GUILayout.Label($"production: {Production}");
            GUILayout.Label($"Efficiency: {Efficiency}");

            if (GUILayout.Button("Debug ToString"))
            {
                Debug.Log(ToString());
            }

            GUILayout.EndArea();

        }
    }
}