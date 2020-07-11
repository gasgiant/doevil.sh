using System.Collections.Generic;
using UnityEngine;

public class PredictionView : MonoBehaviour
{
    [SerializeField]
    LineRenderer linePrefab = null;

    [SerializeField]
    GameObject dotPrefab = null;

    List<GameObject> allTheStuff = new List<GameObject>();

    public void SetPredictionData(List<Vector3> path)
    {
        foreach (var item in allTheStuff)
        {
            Destroy(item);
        }
        allTheStuff.Clear();

        if (path.Count < 1) return;

        GameObject dot = Instantiate(dotPrefab);
        allTheStuff.Add(dot);
        dot.transform.SetParent(transform);
        dot.transform.position = path[0];

        for (int i = 1; i < path.Count; i++)
        {
            LineRenderer lineRenderer = Instantiate(linePrefab);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, path[i - 1]);
            lineRenderer.SetPosition(1, path[i]);
            lineRenderer.transform.SetParent(transform);
            allTheStuff.Add(lineRenderer.gameObject);

            dot = Instantiate(dotPrefab);
            allTheStuff.Add(dot);
            dot.transform.SetParent(transform);
            dot.transform.position = path[i];
        }
    }
}
