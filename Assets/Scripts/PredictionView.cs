using System.Collections.Generic;
using UnityEngine;

public class PredictionView : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer = null;

    public void SetPredictionData(List<Vector3> path)
    {
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
    }
}
