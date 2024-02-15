using MyCraft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Member;

namespace FactoryFramework
{
	[RequireComponent(typeof(SerializationReference))]
	public class PowerGridComponent : MonoBehaviour
	{
		protected GlobalLogisticsSettings settings { get { return ConveyorLogisticsUtils.settings; } }
		//protected SerializationReference _sRef;

		/// <summary>
		/// Power grid we are connected to
		/// </summary>
		public PowerGrid grid;
		/// <summary>
		/// How much power is normally while processing something and connected to a grid.
		/// </summary>
		public float basePowerDraw = 0f;
		private float _powerDraw;
		/// <summary>
		/// Visual Anchor for connections
		/// </summary>
		public Vector3 connectionPoint;
		/// <summary>
		/// Should this be a radius-connection or a direct single connection
		/// </summary>
		public bool useConnectionRadius;
		/// <summary>
		/// Radius to connect to other PGCs if useConnectionRadius is enabled. 
		/// </summary>
		public float connectionRadius;
		/// <summary>
		/// if we use the connection radius, this is an optional visual object we can set
		/// </summary>
		public Transform radiusVisual;
		/// <summary>
		/// Adjacency List. Connections in the graph
		/// </summary>
		public HashSet<PowerGridComponent> Connections;// { get; internal set; }
		/// <summary>
		/// Event to fire when this is disconnected from a grid (destroyed)
		/// </summary>
		public UnityEvent OnDisconnect;
		public UnityEvent<PowerGridComponent> OnRemoveConnection;
		public UnityEvent<PowerGridComponent> OnAddConnection;

		private LogisticComponent _logisticComponent;
		public System.Guid GUID { get { return _logisticComponent.GUID; } }

		public void Init()
		{
			Connections.Clear();

			//grid = null;
			//GameObject pGrid = new GameObject("Power Grid");
			//pGrid.transform.SetPositionAndRotation(transform.position, transform.rotation);
			//grid = pGrid.AddComponent<PowerGrid>();
			grid = Managers.Resource.Instantiate(Managers.Game.PowerGrid).GetComponent<PowerGrid>();
			grid.transform.parent = Common.ParentPool(Managers.Game.Placement.transform, Managers.Game.PowerGrid.name);
			grid.Clear(this);
		}
		//건물이 취소되거나 파괴될 때...호출
		public void Clear()
		{
			//RemoveFromGrid();   //삭제하는 개체를 grid에서 빼준다.
			//for (int i = Connections.Count - 1; i >= 0; --i)
			//{
			//	PowerGridComponent source = Connections.ElementAt(i);
			//	source.Disconnect(this);
			//	Disconnect(source);
			//}
			////grid?.Clear();
			//grid?.AddNode(this);//불러오기할때 자신을 다시 추가해 준다.

			this.grid.RemoveNode(this);    //끊어지면 node에서 빼야. 실시간 체크가 가능하다.
			this.grid = null;

			//서로 연결된 것은 하나로 남기고,
			//분리된 것은 새로운 grid로 등록합니다.
			List<PowerGridComponent> sources = new List<PowerGridComponent>();
			int max_count = 0;
			//연결부터 끊고.
			for (int i = Connections.Count - 1; i >= 0; --i)
			{
				PowerGridComponent pgc = Connections.ElementAt(i);
				pgc.Disconnect(this);
				Disconnect(pgc);

				//sources.Add(pgc);

				//연결수가 가장큰 것만 앞으로 온다.(내림차순 아님)
				int cnt = ConnectCount(pgc);
				if (cnt <= max_count)
					sources.Add(pgc);
				else
				{
					sources.Insert(0, pgc);
					max_count = cnt;
					//Debug.Log($"({max_count})개 맨앞으로 이동");
				}
			}

			//this 가 소멸됨에 따라.
			//연결되어 있던 노드 A,B가 서로 연결이 되어 있는지 체크합니다.
			if (2 <= sources.Count) //2이상 연결되어 있을때.
			{
				for (int i = 0; i < sources.Count - 1; ++i)
				{
					PowerGridComponent A = sources.ElementAt(i);
					for (int j = i + 1; j < sources.Count; ++j)
					{
						PowerGridComponent B = sources.ElementAt(j);
						if (true == IsConnected(A, B)) continue;    //다른쪽에서 연결되어 있으므로...무시
						if (A.grid != B.grid) continue;   //끊어져서 다른 grid로 설정되었으므로...무시

						//새로운 grid를 설정해 줍니다.
						PowerGrid.NewGrid_1(A, B);
					}
				}
			}
		}

		//서로 연결된 것은 true를 리턴합니다.(재귀)
		private bool IsConnected(PowerGridComponent A, PowerGridComponent B)
		{
			List<PowerGridComponent> temp = new List<PowerGridComponent>();
			if (true == IsConnected(A, B, ref temp))
			{
				Debug.Log("** connected(IsConnected()) !!!");
				return true;
			}
			Debug.Log($"** not connected ({temp.Count}) !!!");
			return false;
		}

		private bool IsConnected(PowerGridComponent A, PowerGridComponent B, ref List<PowerGridComponent> temp)
		{
			if (null == A || null == B) return false;
			foreach (var pgcB in B.Connections)
			{
				if (A == pgcB) return true;
				if (true == temp.Contains(pgcB)) continue;
				temp.Add(pgcB);	//IsConnected()보다 먼저 있어야...무한루프 방지
				if (true == IsConnected(A, pgcB, ref temp))
					return true;
			}
			return false;
		}

		private int ConnectCount(PowerGridComponent A)
		{
			List<PowerGridComponent> temp = new List<PowerGridComponent>();

			foreach(var pgc in A.Connections)
			{
				if (true == temp.Contains(pgc))
					continue;
				temp.Add(pgc);
			}
			return temp.Count;
		}

		private void ConnectGrid(PowerGridComponent other)
		{
			this.Connections.Add(other);
			other.Connections.Add(this);
			OnAddConnection?.Invoke(other);


			if (this.grid == null) this.grid = other.grid;
			if (other.grid == null) other.grid = this.grid;
			grid.AddNode(other);
			other.grid.AddNode(this);
			// union if possible
			Debug.Log($"nodes count: this({this.grid.nodes.Count}), other({other.grid.nodes.Count})");
			if (PowerGrid.UnionGraphs(this.grid, other.grid))
			{
				//Debug.Log("Graphs Consolidated");
			}

			// add line render cable
			CableRendererManager.instance?.AddCable(this, other);
		}

		public bool Connect(PowerGridComponent other = null)
		{ 
			if (this.useConnectionRadius)
			{
				// check if grids==null because that means we're rebuilding the graph after a node-delete. Otherwise this triggers unecessarily
				//if (other != null && (this.grid!=null && other.grid!=null))
				//    Debug.LogWarning($"You do not need to provide a power grid component to connect to if you use the connection radius: {other.gameObject.name}");
				return ConnectRadiusHelper();
			} else
			{
				if (other == null) return false;
				this.ConnectGrid(other);
				return true;
			}
		}
		private bool ConnectRadiusHelper()
		{
			if (this == null) return false;
			bool foundConnections = false;
			foreach (Collider c in Physics.OverlapCapsule(
				transform.position - Vector3.up * 5f,
				transform.position + Vector3.up * 5f,
				connectionRadius))
			{
				if (c.gameObject == this.gameObject) 
					continue;
				if (c.gameObject.TryGetComponent(out PowerGridComponent other))
				{
					this.ConnectGrid(other);
					foundConnections = true;

					// add line render
					CableRendererManager.instance?.AddCable(this, other);
				}
			}
			return foundConnections;
		}

		public void Disconnect(PowerGridComponent other)
		{
			this.Connections.Remove(other);
			OnRemoveConnection?.Invoke(other);
			//grid.nodes.Remove(other);//(여기서하면 안됨)밖에서 처리해라.
			// remove line render
			CableRendererManager.instance?.RemoveCable(this, other);
		}

		//public void RemoveFromGrid()
		//{
		//	if (grid == null || grid.nodes == null || Connections == null) return;
		//	// remove self from grid, split grids if necessary
		//	grid.nodes.Remove(this);
		//	foreach (var node in grid.nodes)
		//	{
		//		//node.grid = null;
		//		node.Disconnect(this);
		//	}
		//	//Destroy(grid.gameObject);

		//	// brute force, maybe a better way to do it
		//	// foreach connection, make a new power grid
		//	// connect those grids to available nodes
		//	// then union those grids

		//	// create subgrids for each connection now that this is removed
		//	List<PowerGrid> subgrids = new List<PowerGrid>();
		//	HashSet<PowerGridComponent> visited = new HashSet<PowerGridComponent>();
		//	int i = 0;
		//	foreach (var pgc in Connections)
		//	{
		//		// can be null if it was just destroyed
		//		if (pgc == null) continue;
		//		// remove line render
		//		CableRendererManager.instance?.RemoveCable(this, pgc);

		//		if (visited.Contains(pgc))
		//		{
		//			//Debug.Log($"Skipping {pgc.gameObject.name}");
		//		} else
		//		{
		//			PowerGrid nGrid = PowerGrid.NewGrid(pgc);
		//			subgrids.Add(nGrid);
		//			foreach (var node in nGrid.nodes)
		//				visited.Add(node);
		//		}
		//		i++;
		//	}

		//	OnDisconnect?.Invoke();
		//}	   

		private void Awake()
		{
			//_sRef ??= GetComponent<SerializationReference>();

			_powerDraw = basePowerDraw;
			Connections = new HashSet<PowerGridComponent>();

			_logisticComponent ??= GetComponent<LogisticComponent>();

			// create our own power grid until we connect to something
			//if (null == grid)
			//{
			//	//GameObject pGrid = new GameObject("Power Grid");
			//	//pGrid.transform.SetPositionAndRotation(transform.position, transform.rotation);

			//	//grid = pGrid.AddComponent<PowerGrid>();
			//	//grid.AddNode(this);
			//	Init();
			//}

			// set the visual size
			if (useConnectionRadius)
			{
				if (radiusVisual)
					radiusVisual.transform.localScale = Vector3.one * connectionRadius;
			}
		}

		private void OnValidate()
		{
			_logisticComponent ??= GetComponent<LogisticComponent>();
			//_sRef ??= GetComponent<SerializationReference>();
		}

		private void Update()
		{
			ManagePowerDraw();
		}

		private void ManagePowerDraw()
		{
			if (_logisticComponent == null) return;
			//basePowerDraw = (_logisticComponent._IsWorking) ? _powerDraw : 0f;
			basePowerDraw = _powerDraw; // _IsWorking을 체크할 필요가 없을꺼 같아서.
		}

		public override string ToString()
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();

			//s.Append($"{this._sRef.GUID}:");
			//s.AppendJoin(',', this.Connections.Select(n => n._sRef.GUID));
			s.Append($"{this._logisticComponent.GUID}:");
			s.AppendJoin(',', this.Connections.Select(n => n._logisticComponent.GUID));
			return s.ToString();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.black;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(connectionPoint, Vector3.one * 0.5f);
#if UNITY_EDITOR
			if (useConnectionRadius)
			{
				Color fillColor = Color.green;
				fillColor.a = 0.15f;
				UnityEditor.Handles.color = fillColor;
				UnityEditor.Handles.matrix = transform.localToWorldMatrix;
				UnityEditor.Handles.DrawSolidDisc(Vector3.zero, Vector3.up, connectionRadius);
			}      
#endif
		}

		#region RADIUS_VISUAL
		private float _activeAlpha = 0.35f;
		public void ShowRadiusVisual()
		{
			if (!useConnectionRadius || radiusVisual == null) return;

			if (radiusVisual.TryGetComponent(out MeshRenderer mr)){
				mr.material.SetFloat("_Opacity", _activeAlpha);
				
			}
		}
		public void HideRadiusVisual()
		{
			if (!useConnectionRadius || radiusVisual == null) return;

			if (radiusVisual.TryGetComponent(out MeshRenderer mr))
			{
				mr.material.SetFloat("_Opacity", 0f);
			}
		}

		#endregion

		//private bool IsQuitting;
		//void OnApplicationQuit()
		//{
		//	IsQuitting = true;
		//}
		//private void OnDestroy()
		//{
		//	if (IsQuitting) return;
		//	RemoveFromGrid();
		//}

	}
}