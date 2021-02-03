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
    public TickManager tickManager;
    
    
    Color[] gizmoColors = {Color.blue, Color.yellow, Color.green, Color.red};
    private List<Vector3> gizmoPositions = new List<Vector3>();
    private Vector3 nextPosition;
    private bool turnEnded = false;
    private NavMeshPath path;
    private float speed = 1f;
    const int distanceConstant = 4;
    private int actionsToMake = 0;
    void Awake() {
        path = new NavMeshPath();
        nextPosition = this.gameObject.transform.position;
        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.useWorldSpace = true;  
    }

    void Start() {
        TickManager.OnTick += TickManager_OnTick;

    }
    private void TickManager_OnTick(object sender, TickManager.OnTickEventArgs  e) {
        actionsToMake++;
    }
    // TODO with every tick add to queue tms. do the moves one at a time with delay
    // IEnumerable DoActions(int actions) {
    //     while(actionsToMake > 0) {
    //         nextPosition = gizmoPositions[actionsToMake];
    //     }
    // }
    void Update()
    {
        if (transform.position != nextPosition) {
            transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * distanceConstant * Time.deltaTime);
        }
        if (Input.GetMouseButtonDown(0)) {
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 clickPos = -Vector3.one;
            gizmoPositions = new List<Vector3>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxDistance, clickMask)) {
                clickPos = hit.point;
                NavMesh.CalculatePath(this.gameObject.transform.position, clickPos, NavMesh.AllAreas, path);
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPosition(0, currentPos);
                float currentLength = 0;
                for (int i = 0; i < path.corners.Length - 1; i++) {
                    currentLength += Vector3.Distance(path.corners[i], path.corners[i+1]);
                    // Lets chop the length to next corner to players distance per tick
                    if (currentLength > GetMaxDistanceFromSpeed(speed)) {
                        int iterations = Mathf.FloorToInt(currentLength / GetMaxDistanceFromSpeed(speed));
                        Debug.Log("DIVIDE SPEED ITERATIONS " + iterations);
                        for (int j = 0; j < iterations; j++) {
                            Vector3 maxPoint = Vector3.Lerp(path.corners[i], path.corners[i+1], (GetMaxDistanceFromSpeed(speed)*j+1f)/currentLength);
                            Debug.Log("maxPoint " + maxPoint);
                            gizmoPositions.Add(maxPoint);
                        }
                        
                        turnEnded = true;
                        tickManager.Act(iterations);
                        currentLength = 0;
                    }
                    
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
                    lineRenderer.SetPosition(i+1, path.corners[i+1]);
                }
            
            }
            

        }
    }

    float GetMaxDistanceFromSpeed(float speed) {
        return speed * distanceConstant;
    }

    void OnDrawGizmos()
    {
        if (gizmoPositions.Count > 0) {
            for(int i = 0; i < gizmoPositions.Count; i++) {
                // Gizmos.color = gizmoColors[i % gizmoColors.Length];
                // Gizmos.DrawSphere(gizmoPositions[i], 2);
            }
        }
    }
}
