using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MyCraft;
using Unity.VisualScripting;

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

		public float Load{  //요구량
			get{ 
				if (nodes == null) return 0f; 
				return nodes.Where(n => n.basePowerDraw > 0).Sum(n => n.basePowerDraw);
			}
		}
		public float Production{    //생산량
			get {
				if (nodes == null) return 0f;
				return Mathf.Abs(nodes.Where(n => n.basePowerDraw<0).Sum(n => n.basePowerDraw));
			}
		}
		private float NetPower { get { return nodes.Sum(n => n.basePowerDraw); } }
		public float Efficiency { get { return (Production > 0f) ? Mathf.Clamp01(Production / Load) : 0f; } }

		public bool debug_info = false;

		public void Clear(PowerGridComponent self)
		{
			nodes.Clear();
			AddNode(self);
		}

		/// <summary>
		/// Internal connection between the underlying node classes
		/// </summary>
		/// <param name="node"></param>
		public void AddNode(PowerGridComponent node) 
		{
			if(null == node) return;
			if (!nodes.Contains(node)) nodes.Add(node);
		}

		public void RemoveNode(PowerGridComponent node)
		{
			nodes.Remove(node);
			if (nodes.Count <= 0) MyCraft.Managers.Resource.Destroy(this.gameObject);
		}
		//public static PowerGrid NewGrid(PowerGridComponent root)
		//{
		//	GameObject gridObj = new GameObject() { name= "Power Grid" };
		//	gridObj.transform.SetPositionAndRotation(root.transform.position, root.transform.rotation);
		//	PowerGrid grid = gridObj.AddComponent<PowerGrid>();

		//	HashSet<PowerGridComponent> visited = new HashSet<PowerGridComponent>();

		//	grid.AddNode(root);
		//	visited.Add(root);
		//	root.grid = grid;
		//	// rebuild the graph(s) with DFS
		//	void NewGridHelper(PowerGridComponent parent, PowerGridComponent n)
		//	{
		//		if (visited.Contains(n)) return;
		//		parent.Connect(n);
		//		visited.Add(n);
		//		foreach (var connection in n.Connections.ToArray())
		//			NewGridHelper(n, connection);
		//	}
		//	foreach (var connection in root.Connections.ToArray())
		//		NewGridHelper(root, connection);

		//	return grid;
		//}

		public static void NewGrid_1(PowerGridComponent root, PowerGridComponent source)
		{
			//GameObject gridObj = new GameObject() { name = "Power Grid" };
			//gridObj.transform.SetPositionAndRotation(source.transform.position, source.transform.rotation);
			//PowerGrid grid = gridObj.AddComponent<PowerGrid>();
			PowerGrid grid = Managers.Resource.Instantiate(Managers.Game.PowerGrid).GetComponent<PowerGrid>();
			grid.transform.parent = Common.ParentPool(Managers.Game.Placement.transform, Managers.Game.PowerGrid.name);
			grid.Clear(null);

			//재귀
			NewGrid_2(root, source, grid);
		}

		private static void NewGrid_2(PowerGridComponent root, PowerGridComponent source, PowerGrid grid)
		{
			source.grid = grid;
			grid.AddNode(source);
			root.grid.RemoveNode(source);
			//root와 연결된 개체도 동일한 grid로 묶어준다.
			foreach (var conn in source.Connections)
			{
				if (conn.grid == source.grid) continue;
				NewGrid_2(root, conn, grid);
			}
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
				//Destroy(b.gameObject);
				Managers.Resource.Destroy(b.gameObject);
				b = null;
				return true;
			}
			return false;
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
			GUILayout.Label($"Count : {nodes.Count}");
			GUILayout.Label($"load: {Load}");
			GUILayout.Label($"production: {Production}");
			GUILayout.Label($"Efficiency: {Efficiency}");

			if (GUILayout.Button("Debug ToString"))
				Debug.Log(ToString());
			GUILayout.EndArea();
		}
	}
}