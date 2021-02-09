using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    // Constants (move to another file?)
    const int speedConstant = 4;

    // For gizmos (maybe change later for waypoints?)
    Color[] gizmoColors = {Color.blue, Color.yellow, Color.green, Color.red};
    private List<Vector3> gizmoPositions = new List<Vector3>();

    // NavMesh stuff
    private Transform target;
    private Vector3 newDestination;
    private NavMeshPath path;
    private NavMeshAgent playerAgent;
    List<Vector3> wayPoints = new List<Vector3>();

    // Player references
    private Rigidbody playerBody;
    private Camera playerCamera;
    public Interactable playerFocus;
    public LayerMask clickMask;
    public TickManager tickManager;

    //Properties
    private float speed = 4f;
    private bool moving = false;
    public bool actionsRunning = false;
    private bool turnEnded = false;
    private int actionsToMake = 0;
    private int currentActionIndex = 0;
    public float maxDistance = 100f;
    private float moveLeft;

    void Awake() {
        moveLeft = speed *  speedConstant;
        path = new NavMeshPath();
        playerCamera = GetComponentInChildren<Camera>();
        playerBody = GetComponent<Rigidbody>();
        playerAgent = GetComponent<NavMeshAgent>();
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
            actionsToMake--;
            yield return new WaitForSeconds(2f);
        }
        actionsRunning = false;
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        if (Input.GetMouseButtonDown(0) && actionsToMake == 0) {
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 clickPos = -Vector3.one;
            gizmoPositions = new List<Vector3>();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxDistance, clickMask)) {
                RemoveFocus();
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
                        turnEnded = true;
                        tickManager.Act(iterations);
                        // Change the moving logic, its really confusing
                        moveLeft -= currentLength;
                        currentLength = 0;
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            // Ray to the mouse poistion
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // If the ray hits
            if (Physics.Raycast(ray, out hit, 100)) {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null) {
                    SetFocus(interactable);
                }
            }
        }

        if (target != null) {
            playerAgent.SetDestination(target.position);
            FaceTarget();
        }
    }

    void SetFocus(Interactable newFocus) {
        // Do something with the focused Interactable?
        if (newFocus != playerFocus) {
            if (playerFocus != null) {
                playerFocus.OnDefocused();
            }
            playerFocus = newFocus;
            FollowTarget(newFocus);
        }

        newFocus.OnFocused(transform);
    }

    void RemoveFocus() {
        if (playerFocus != null) {
            playerFocus.OnDefocused();
        }
        playerFocus = null;
        StopFollowingTarget();
    }

    public void FollowTarget(Interactable newTarget) {
        playerAgent.stoppingDistance = newTarget.radius * 0.8f;
        playerAgent.updateRotation = false;
        target = newTarget.interactionTransform;
    }

    public void StopFollowingTarget() {
        playerAgent.stoppingDistance = 0;
        playerAgent.updateRotation = true;
        target = null;
    }

    private void FaceTarget() {
        Vector3 dir =  (target.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
    }

    void FixedUpdate() {
        // Do something with this, it can't stay here
        if (newDestination != transform.position && newDestination != null && actionsToMake > 0) {
            float step = speed * 4f * Time.fixedDeltaTime;
            Vector3 targetDirection = newDestination - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, newDestination, step);
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    float GetMaxDistanceFromSpeed(float speed) {
        return speed * speedConstant;
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
