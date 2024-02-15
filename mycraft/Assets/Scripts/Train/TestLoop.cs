using Dreamteck.Splines;
using Dreamteck.Splines.Examples;
using MyCraft;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestLoop : MonoBehaviour
{
	[SerializeField]
	private Dreamteck.Splines.SplineComputer subLoop;
	[SerializeField]
	private Dreamteck.Splines.SplineComputer testLoop;

	// Start is called before the first frame update
	void Start()
	{
		//Dreamteck.Splines.SplinePoint[] points = testLoop.GetPoints();

		////1. 위치변경 테스트(o)
		//Debug.Log($"{points[2].position}");
		//points[2].position.y = 10;
		//splineComputer.SetPoints(points);

		//2.중간삽입(o)
		//Dreamteck.Splines.SplinePoint[] points1 = new Dreamteck.Splines.SplinePoint[points.Length + 1];
		////{
		////	points[2].position.y = 10;
		////}
		//for(int i=0; i<points.Length; ++i)
		//	points1[i] = points[i];
		////points1[points.Length] = points1[points.Length - 1];
		//points1[points.Length].SetPosition(new Vector3(0, 10, 0));
		//splineComputer.SetPoints(points1);\
		//AddPoint(new Vector3(0, 10, 0));


		//AddPoint(new Vector3(27, 7, 78), Vector3.up);
		//AddPoint(new Vector3(27, 7, 58), Vector3.up);
		//AddPoint(new Vector3(7, 7, 58), Vector3.up);
		//AddPoint(new Vector3(7, 9, 78), Vector3.up);
		//splineComputer.Close();

		//AddPoint_1(testLoop, new Vector3(30.3f, 3.89f, 54.3f), -1, subLoop, 3);	//junction(subLoop의 3번과 연결)
		//AddPoint_1(testLoop, new Vector3(21.8f, 5.6f, 69.0f));
		//AddPoint_1(testLoop, new Vector3(12.8f, 8.5f, 74.0f));
		//AddPoint_1(testLoop, new Vector3(4.7f, 11.7f, 68.8f));
		//AddPoint_1(testLoop, new Vector3(6.3f, 16.0f, 56.0f));
		//AddPoint_1(testLoop, new Vector3(15.4f, 15.0f, 47.6f), 3);	//point 중간삽입
		//AddPoint_1(testLoop, new Vector3(26.6f, 11.0f, 53.2f));
		//AddPoint_1(testLoop, new Vector3(32.5f, 4.78f, 73.16f), -1, subLoop, 4);

		//AddPoint_1(testLoop, new Vector3(28.16f, 3.32f, 67.10f));
		//AddPoint_1(testLoop, new Vector3(10.36f, 5.90f, 55.07f));
		//AddPoint_1(testLoop, new Vector3(-12.49f, 3.37f, 44.88f));

		Debug.Log($"{testLoop.name} length: {testLoop.CalculateLength()}");
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE).OrderBy(h => h.distance).ToArray();
			//RaycastHit[] hits = Physics.RaycastAll(ray, Common.MAX_RAY_DISTANCE);
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
				{
					//중간에 삽입
					if (Input.GetKey(KeyCode.LeftShift))
					{
						double percent = testLoop.spline.Project(hit.point);
						Vector3 pos = testLoop.spline.EvaluatePosition(percent);
						//SplineSample sample = testLoop.spline.Evaluate(percent);
						//Debug.Log($"{hit.transform.name}/{percent} => {pos} : sample({sample.position})");
						Debug.Log($"{percent} => hit:{hit.point} / spline:{pos}");

						for (int i = 0; i < testLoop.pointCount; ++i)
						{
							double tmp = testLoop.GetPointPercent(i);
							Debug.Log($"	index({i}): {tmp} percent");
						}

						for (int i = 0; i < testLoop.pointCount; ++i)
						{
							double tmp = testLoop.GetPointPercent(i);
							if (percent < tmp)
							{
								AddPoint_1(testLoop, pos, i);  //point 중간삽입
								Debug.Log($"new index({i}): {percent} percent");
								break;
							}
						}
					}
					//끝에 계속 연결
					else
					{
						Vector3 pos = MyCraft.Common.Floor(hit.point + Vector3.up);  //건물위치간격보간
						AddPoint_1(testLoop, pos);
					}
				}
				//if (hit.transform.tag != "Safe-Footing")  //안전발판
			}
		}
	}

	void AddPoint(SplineComputer sourceComputer, Vector3 point, int sourceIndex = -1, SplineComputer destComputer = null, int destIndex = -1)
	{
		Dreamteck.Splines.SplinePoint[] points = sourceComputer.GetPoints();
		if (sourceIndex < 0 || points.Length < sourceIndex) sourceIndex = points.Length;  //bad index는 맨끝에 넣어준다.

		int pos = 0; //새 point를 넣을 위치를 비우기 위해 따로 관리한다.
		Dreamteck.Splines.SplinePoint[] temp = new Dreamteck.Splines.SplinePoint[points.Length + 1];
		//+1: point가 중간에 들어갈꺼를 예상해서.(끝에 들어갈 경우는 continue에서 빠진다.)
		for (int i = 0; i < points.Length + 1; ++i)
		{
			if (sourceIndex == i) continue;//이 자리는 신규point로 채울예정이므로 비워둔다.
			temp[i] = points[pos++];
		}
		//new point
		temp[sourceIndex].SetPosition(point);
		temp[sourceIndex].normal = Vector3.up;
		temp[sourceIndex].size = 1;
		sourceComputer.SetPoints(temp);//삽입
		sourceComputer.CalculateLength();

		//junction(sourceIndex를 destIndex와 결합합니다.)
		if (null != destComputer && 0 <= destIndex)
			Connect(sourceComputer, sourceIndex, destComputer, destIndex);
	}
	void AddPoint_1(SplineComputer sourceComputer, Vector3 point, int sourceIndex = -1, SplineComputer destComputer = null, int destIndex = -1)
	{
		if (sourceIndex < 0 || sourceComputer.GetPoints().Length < sourceIndex) sourceIndex = sourceComputer.GetPoints().Length;  //bad index는 맨끝에 넣어준다.

		sourceComputer.AddPointPosition_1(sourceIndex, point);//삽입
		//computer.CalculateLength();

		//junction(sourceIndex를 destIndex와 결합합니다.)
		if (null != destComputer && 0 <= destIndex)
			Connect(sourceComputer, sourceIndex, destComputer, destIndex);
	}

	void Connect(SplineComputer sourceComputer, int sourceIndex, SplineComputer destComputer, int destIndex)
	{
		SplineSample sample = destComputer.Evaluate(destIndex);
		GameObject go = Managers.Resource.Instantiate("prefabs/Splines/Junction/LoopNode", destComputer.transform);
		go.transform.parent = destComputer.transform;
		go.transform.position = sample.position;
		if (destComputer.is2D)
		{
			go.transform.rotation = sample.rotation * Quaternion.Euler(90, -90, 0);
		}
		else
		{
			go.transform.rotation = sample.rotation;
		}

		Dreamteck.Splines.Node node4 = go.GetComponent<Dreamteck.Splines.Node>();
		destComputer.ConnectNode(node4, destIndex);
		sourceComputer.ConnectNode(node4, sourceIndex);
		
		//bridge
		node4.GetComponent<JunctionSwitch>().bridges.Add(new JunctionSwitch.Bridge
		{
			active = true,
			a = 0, aDirection = JunctionSwitch.Bridge.Direction.Forward,
			b = 1, bDirection = JunctionSwitch.Bridge.Direction.Forward,
		});
	}
}
