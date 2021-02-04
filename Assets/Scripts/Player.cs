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
    private Vector3 newDestination;
    private NavMeshPath path;
    private NavMeshAgent playerAgent;
    private Rigidbody playerBody;
    private float speed = 1f;
    private bool moving = false;
    private bool actionsRunning = false;
    private bool turnEnded = false;
    const int distanceConstant = 4;
    private int actionsToMake = 0;
    private int currentActionIndex = 0;
    void Awake() {
        path = new NavMeshPath();
        playerBody = GetComponent<Rigidbody>();
        playerAgent = GetComponent<NavMeshAgent>();
        nextPosition = this.gameObject.transform.position;
    }

    void Start() {
        TickManager.OnTick += TickManager_OnTick;
    }
    private void TickManager_OnTick(object sender, TickManager.OnTickEventArgs  e) {
        actionsToMake++;
        if (!actionsRunning) {
            StartCoroutine("DoActions");
        }
    }
    // TODO with every tick add to queue tms. do the moves one at a time with delay
    IEnumerator DoActions() {
        Debug.Log("Actions: " + actionsToMake);
        actionsRunning = true;
        while(actionsToMake > 0) {
            // playerAgent.destination = gizmoPositions[currentActionIndex];
            newDestination = gizmoPositions[currentActionIndex];
            currentActionIndex += 1;
            Debug.Log("Next pos: " + nextPosition);
            actionsToMake--;
            yield return new WaitForSeconds(2f);
        }
        actionsRunning = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && actionsToMake == 0) {
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 clickPos = -Vector3.one;
            gizmoPositions = new List<Vector3>();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxDistance, clickMask)) {
                actionsToMake = 0;
                currentActionIndex = 0;
                clickPos = hit.point;
                playerBody.isKinematic = true;
                NavMesh.CalculatePath(this.gameObject.transform.position, clickPos, NavMesh.AllAreas, path);
                float currentLength = 0;
                for (int i = 0; i < path.corners.Length - 1; i++) {
                    currentLength += Vector3.Distance(path.corners[i], path.corners[i+1]);
                    // Lets chop the length to next corner to players distance per tick
                    if (currentLength > GetMaxDistanceFromSpeed(speed)) {
                        int iterations = Mathf.FloorToInt(currentLength / GetMaxDistanceFromSpeed(speed));
                        for (int j = 1; j < iterations; j++) {
                            Vector3 maxPoint = Vector3.Lerp(path.corners[i], path.corners[i+1], (GetMaxDistanceFromSpeed(speed)*j+1f)/currentLength);
                            // Debug.Log("maxPoint " + maxPoint);
                            gizmoPositions.Add(maxPoint);
                        }
                        Debug.Log("ITEMS IN POSITION LIST " + gizmoPositions.Count);
                        
                        turnEnded = true;
                        tickManager.Act(iterations);
                        currentLength = 0;
                        Debug.Log(gameObject.transform.position);
                        Debug.Log(nextPosition);
                    }
                    
                }
            
            }
            

        }
    }

    void FixedUpdate() {
        if (newDestination != transform.position) {
            float step = speed * 4f * Time.fixedDeltaTime;
            Vector3 targetDirection = newDestination - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, newDestination, step);
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    float GetMaxDistanceFromSpeed(float speed) {
        return speed * distanceConstant;
    }

    // void OnDrawGizmos()
    // {
    //     if (gizmoPositions.Count > 0) {
    //         for(int i = 0; i < gizmoPositions.Count; i++) {
    //             Gizmos.color = gizmoColors[i % gizmoColors.Length];
    //             Gizmos.DrawSphere(gizmoPositions[i], 2);
    //         }
    //     }
    // }
}
