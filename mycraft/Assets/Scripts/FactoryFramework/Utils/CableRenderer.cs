using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CableRenderer : MonoBehaviour
{
    [SerializeField]
    private Vector3 startPoint;
    [SerializeField]
    private Vector3 endPoint;

    [Range(1,10)]
    public int segments = 4;

    public float droop = 0f;

    LineRenderer _lineRenderer;
    public LineRenderer LineRender { 
        get {
            if(_lineRenderer == null)
                _lineRenderer = GetComponent<LineRenderer>();
            return _lineRenderer; 
        } 
    }
    public void RefreshCable()
    {
        if(startPoint == endPoint)
        {
            LineRender.positionCount = 0;
            LineRender.SetPositions(new Vector3[0]);
            return;
        }

        List<Vector3> points = new List<Vector3>() { startPoint };

        float perSegment = 1f / segments;
        for (int i = 0; i < segments; i++)
        {
            float xPer = (i + 1) * perSegment;
            points.Add(GetPointAtXPercent(xPer));
        }

        LineRender.positionCount = points.Count;
        LineRender.SetPositions(points.ToArray());
    }

    public void SetAnchors(Vector3 start, Vector3 end)
    {
        if (start == end)
        {
            Debug.LogWarning($"Warning: Cable will be rendered with same start and end point {start} {end}");
        }
        startPoint = start;
        endPoint = end;
        RefreshCable();
    }

    public Vector3 GetPointAtXPercent(float xPer)
    {
        Vector3 linearDelta = endPoint - startPoint;

        float depression = Mathf.Sin(xPer * Mathf.PI) * droop * linearDelta.magnitude; // We want droop to be scaled by length of cable
        Vector3 y = depression * GetUpDir() * -1;
        return startPoint + new Vector3(linearDelta.x * xPer, linearDelta.y*xPer, linearDelta.z * xPer) + y;
    }

    private Vector3 GetLinearDelta() => endPoint - startPoint;

    private Vector3 GetMidPoint() => (startPoint + endPoint) / 2f;

    private Vector3 GetUpDir() => Vector3.Cross(Vector3.Cross(GetLinearDelta().normalized, Vector3.up), GetLinearDelta().normalized); //is.. is this smart?

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(startPoint, endPoint);
    }

    private void OnValidate()
    {
        RefreshCable();
    }

}
