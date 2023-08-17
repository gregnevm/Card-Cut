using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathCreator : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float horizontalCurveFactor = 1.0f;

    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField, Range(1, 100)] private int _numPoints = 20;
    public float CurrentHorizontalCurveFactor { get; private set; }

    private List<Vector3> _trajectoryPositions;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        UpdateLine(horizontalCurveFactor);
        DisableLine();
    }

    public void DisableLine()
    {
        _lineRenderer.enabled = false;
    }

    public void EnableLine()
    {
        _lineRenderer.enabled = true;
    }

    public List<Vector3> GetPositions()
    {
        return _trajectoryPositions;
    }

    public Quaternion GetEndAngle()
    {
        Vector3 direction = _lineRenderer.GetPosition(_numPoints - 1) - _lineRenderer.GetPosition(_numPoints - 2);
        endPoint.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-20f, 0f, 0f);
        return endPoint.rotation;
    }

    public void ResetEndAngle()
    {
        endPoint.rotation = Quaternion.identity;
    }

    public void UpdateLine(float curveFactor)
    {
        CurrentHorizontalCurveFactor = curveFactor;
        _trajectoryPositions = new List<Vector3>();
        _lineRenderer.positionCount = _numPoints;

        for (int i = 0; i < _numPoints; i++)
        {
            float t = i / (float)(_numPoints - 1);
            Vector3 position = CalculateCubicBezierPoint(t,
                startPoint.position,
                startPoint.position + startPoint.right * curveFactor,
                endPoint.position + endPoint.right * curveFactor,
                endPoint.position);
            _trajectoryPositions.Add(position);
            _lineRenderer.SetPosition(i, position);
            
        }
    }
    public void RedrawLine()
    {
        UpdateLine(CurrentHorizontalCurveFactor);
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0 + 3 * uu * t * p1 + 3 * u * tt * p2 + ttt * p3;
        return point;
    }
}
