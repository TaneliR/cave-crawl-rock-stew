using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    List<Vector3> wayPoints = new List<Vector3>();

    public LayerMask clickMask;
    public float maxDistance = 100f;
    LineRenderer lineRenderer;

    private NavMeshPath path;
    private float elapsed = 0.0f;

    void Awake() {
        path = new NavMeshPath();
        elapsed = 0.0f;
        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.useWorldSpace = true;  
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 clickPos = -Vector3.one;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxDistance, clickMask)) {
                clickPos = hit.point;
                NavMesh.CalculatePath(this.gameObject.transform.position, clickPos, NavMesh.AllAreas, path);
            }
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPosition(0, currentPos); 
            for (int i = 0; i < path.corners.Length; i++)
                // Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
                lineRenderer.SetPosition(i, path.corners[i]);

        }
    }
}
