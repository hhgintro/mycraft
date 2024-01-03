using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
	public class CenterGridManager
	{
		int _index;
		GameObject _prefab;
		Transform _root;	//grid를 모아둘 곳.


		float duration = 10f;
		const int MAX_GRID = 4;
		CenterGrid[] _centerGrid = new CenterGrid[MAX_GRID];

		public CenterGridManager ()
		{
			_index = 0;// Random.Range(0, 1000);

		}

		public void Init(Transform owner)
		{
			if (null == _prefab)
			{
				_prefab = Managers.Resource.Load<GameObject>("prefabs/ui/CenterGrid");
				//Debug.Log($"CenterGridManager::Init({_index}:{_prefab})");

				_root = owner.Find($"Pool_{_prefab.name}") ?? (new GameObject($"Pool_{_prefab.name}") { transform = { parent = owner } }).transform;
			}
		}
		public void Clear()
		{
			for (int i = 0; i < MAX_GRID; ++i)
			{
				if (null == _centerGrid[i]) continue;

				Managers.Resource.Destroy(_centerGrid[i].gameObject);
				_centerGrid[i] = null;
			}

			Managers.Resource.Destroy(_prefab);
			_prefab = null;
			Debug.Log($"CenterGridManager::Clear({_index}:{_prefab})");
		}

		public void Place(GameObject current, ref Vector3 pos)
		{
			//Vector3 from = pos + Vector3.up * 0.5f;   //0.5f:기준이 바닥면이라서 조금 높은곳에서 Raycast() 호출한다.
			Vector3 size = current.GetComponent<BoxCollider>().size;
			//Debug.Log($"size:{size}");

			int layerMask = 1 << LayerMask.NameToLayer("Building");
			for (int i = 0; i < MAX_GRID; ++i)
			{
				//dir방향을 기준으로 시계방향(90도)으로 회전한다.
				//Vector3 direction = Quaternion.Eular(0, 0, -90) * dir;
				Vector3 direction = Quaternion.AngleAxis(90 * i, Vector3.up) * current.transform.forward;

				//간격: ray를 여러번 쏴준다.(한번 쏘니 안맞는 경우 있어서, 촘촘하게 쏴준다)
				float interval = 0.5f;

				//디버깅을 위한 (충돌확인)선을 그린다.
				DrawLine(pos, direction, size.y, interval);

				//center-grid를 그린다.
				DrawCenterGrid(i, pos, direction, size.y, interval, layerMask);
			}

			//center-grid위에 마우스가 위치하면 current를 위에 올려 놓는다.
			pos = Projection(pos);

		}
		public void Stop()
		{
			//Debug.LogError($"CenterGridManager::Stop({_index}:{_prefab})");
			//prefab을 로드할때 centerGrid를 받아오니, 이것만 체크해도 된다.
			if (null == _prefab) return;

			for (int i = 0; i < MAX_GRID; ++i)
			{
				if (null == _centerGrid[i]) continue;
				_centerGrid[i].Stop();
			}
		}

		//디버깅을 위한 (충돌확인)선을 그린다.
		//height : BoxCollider의 높이
		//interval: height를 n등분한다.
		void DrawLine(Vector3 pos, Vector3 direction, float height, float interval)
		{
			int j = 0;
			while ((j * interval) < height)
			{
				Vector3 from = pos + Vector3.up * j * interval;//간격만큼 높이를 높인다.
				Vector3 to = from + direction * this.duration;
				//Debug.Log($"GRID: ({from}) ==> ({to})");
				Debug.DrawLine(from, to, Color.red);
				j++;
			}
		}

		//height : BoxCollider의 높이
		//interval: height를 n등분한다.
		void DrawCenterGrid(int index, Vector3 pos, Vector3 direction, float height, float interval, int layerMask)
		{
			if (null == _prefab) return;

			int j = 0;
			while ((j * interval) < height)
			{
				Vector3 from = pos + Vector3.up * j * interval;//간격만큼 높이를 높인다.
			   //Debug.DrawLine(from, from + direction * this.duration, Color.red);

				RaycastHit hit;
				if (Physics.Raycast(from, direction, out hit, this.duration, layerMask))
				{
					if (null == _centerGrid[index])
					{
						_centerGrid[index] = Managers.Resource.Instantiate(_prefab).GetComponent<CenterGrid>();
						_centerGrid[index].transform.parent = _root;
					}
					//direction: -1 ( hit대상에서 부터 설치할 current의 방향 )
					_centerGrid[index].Play(hit.collider.gameObject.transform.position, -direction, this.duration);
					break;
				}

				if(_centerGrid[index]) _centerGrid[index].Stop();
				j++;
			}
		}

		//center-grid위에 마우스가 위치하면 current를 위에 올려 놓는다.
		Vector3 Projection(Vector3 pos)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			int layerMask = 1 << LayerMask.NameToLayer("CenterGrid");
			if(Physics.Raycast(ray, out hit, Common.MAX_RAY_DISTANCE, layerMask))
			{
				//Debug.Log($"{hit.transform.name}");
				Vector3 tmp = pos;
				pos = Projection(hit.transform, hit.point);
				pos.y = tmp.y;  //높이값은 그대로 유지
				//***중요***
				//x,z축을 조금 이동한다.Look vector가 zero라면서 warning이 뜨면서
				//생선된 건물에 conveyor를 설치하면 conveyor가 먹통이 되어 버린다.
				pos += new Vector3(0.005f, 0, 0.005f);
			}
			return pos;
		}

		//center grid에 만나는 점을 리턴한다.
		Vector3 Projection(Transform hit, Vector3 point)
		{
			//Gizmos.DrawLine(c, Vector3.Project(c - a, b - a) + a);
			Vector3 a = hit.position;
			Vector3 b = hit.position + hit.forward;
			return Vector3.Project(point - a, b - a) + a;
			//return Vector3.Project(point - a, hit.forward) + a;\

			//Vector3 A = hit.position;
   //         Vector3 B = hit.position + hit.forward;

   //         Vector3 line1 = A - B;
   //         Vector3 line2 = point - B;

   //         float cos = Vector3.Dot(line1, line2) / (line1.magnitude * line2.magnitude);
   //         float lengthBD = Vector3.Distance(B, point);
   //         float projectionLength = lengthBD * cos;

   //         Vector3 normalAB = (B - A).normalized;

   //         return (B - normalAB * projectionLength);
        }

	}
}
