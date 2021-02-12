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
    private List<Vector3> playerWayPoints = new List<Vector3>();
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
    private float playerBaseDamage = 7f;
    private DamageType baseDamageType = DamageType.Bludgeoning;
    private List<DamageType> playerDamageTypes;
    private float speed = 4f;
    private bool moving = false;
    public bool actionsRunning = false;
    private bool turnEnded = false;
    private int actionsToMake = 0;
    private int currentActionIndex = 0;
    public float maxDistance = 100f;
    private float moveLeft;

    void Awake() {
        // MOCK DMG MODIFIERS BASEDMG MOCK ACID MODIFIER
        playerDamageTypes = new List<DamageType>() {DamageType.Acid};
        playerDamageTypes.Add(baseDamageType);
        // ENDMOCK
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
            newDestination = playerWayPoints[currentActionIndex];
            currentActionIndex += 1;
            actionsToMake--;
            yield return new WaitForSeconds(2f);
        }
        actionsRunning = false;
    }

    IEnumerator WaitUntilStopped(Vector3 destination) {
        Debug.Log("YAY!");
        Debug.Log("Waypoints first :" + playerWayPoints.Count);
        Debug.Log("Playerpos :" + transform.position);
        Debug.Log("Destination :" + destination);
        moving = true;
        playerAgent.SetDestination(destination);
        Debug.Log(transform.position != destination);
        if(transform.position != destination) {
            yield return new WaitForSeconds(0.2f);
        } 
        playerWayPoints.Remove(playerWayPoints[0]);
        Debug.Log("Waypoints left :" + playerWayPoints.Count);
        moving = false;
    }

    void Update()
    {
        if (playerWayPoints.Count > 0 && transform.position != playerWayPoints[0] && !moving) {
            StartCoroutine(WaitUntilStopped(playerWayPoints[0]));
        }
        // if (EventSystem.current.IsPointerOverGameObject()) {
        //     return;
        // }
        if (Input.GetMouseButtonDown(0) && actionsToMake == 0) {
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 clickPos = -Vector3.one;
            gizmoPositions = new List<Vector3>();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxDistance, clickMask)) {
                
                                        // WHAT I WANT TO DO:
                                        // If length to the clicked point is equal or less than the speed (max distance an entity can go), go to that point and finish turn
                                        // If length is greater than the speed, create a List of points, where first point = cur position -> new point with known direction...
                                        // ... and magnitude of player speed
                                        // If the path has corners iterate every vector from point to point, if added length is less than player speed, add point to List and iterate
                                        // when the movement left is less than the length of point to point in iteration, chop it to the left movement, go there and end the turn.
                    
                    
                    
                    // Lets say 
                    // pathLength = 13f, 
                    // speed = 4f, 
                    // path.corners.Lengt = 2;
                    // currentLength = 0;
                    // path is straight so theres 2 corners. (start point and finish point)
                    
                    // for (int i = 0; i < (path.corners.Length - 1); i++) {
                    //     // First: i = 0 DO I NEED THIS?!

                        // if  (path.corners.Length == 2) {
                        //     // First Add the distance (13f) to currentLength
                        //     // First currentLength 13f
                            
                        // }
                        
                        // First true, since 13f > 4f
                        // if (currentLength > speed) {
                            // Chop the length into as many iterations  (turns needed to get to destination) as it should take actions to get there
                            // 13 / 4, ceiled = 4
                            // First: turnsNeeded = 4
                    RemoveFocus();
                    actionsToMake = 0;
                    currentActionIndex = 0;
                    clickPos = hit.point;
                    playerBody.isKinematic = true;
                    NavMesh.CalculatePath(this.gameObject.transform.position, clickPos, NavMesh.AllAreas, path);
                    float currentLength = 0;
                    float pathLength = CalculateDistanceFromPath(path);
                    if (pathLength > speed) {
                        // TODO ADD FUNCTIONALITY FOR OTHER USE CASES THAN STRAIGHT LINE LOL
                        if  (path.corners.Length == 2) {
                            currentLength += Vector3.Distance(path.corners[0], path.corners[1]);
                            int turnsNeeded = Mathf.CeilToInt(currentLength / speed);
                            Debug.Log("Cur length: " + currentLength);
                            Debug.Log("Turns needed : " + turnsNeeded);
                            // CHANGE THIS. CHECKS IF ITS A STRAIGHT PATH
                            // Get as many waypoints as there are turnsNeeded = turns
                            while(currentLength > speed) {
                                Vector3 wayPoint = Vector3.Lerp(path.corners[0], path.corners[1], (speed/currentLength));
                                playerWayPoints.Add(wayPoint);
                                currentLength -= speed;
                                for (int i = 0; i < turnsNeeded; i++) {
                                    // Point in the first turn the player can go
                                    Vector3 nextWayPoint = Vector3.Lerp(playerWayPoints[i], path.corners[1], (speed/currentLength));
                                    playerWayPoints.Add(nextWayPoint);
                                    currentLength -= speed; 
                                }
                                turnEnded = true;
                                tickManager.Act(turnsNeeded);


                                // Change the moving logic, its really confusing
                                moveLeft -= currentLength;
                            }
                            
                        }
                    }
                    currentLength = 0;
                    // }
                // }
                
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

    float GetMaxDistanceFromSpeed(float speed) {
        return speed * speedConstant;
    }

    float CalculateDistanceFromPath(NavMeshPath path) {
        // If no corners the point is players transform, so the length is 0;
        if (path.corners.Length == 1) {
            return 0;
        }
        // If 2 corners, it's a straigth path, so just get the only length.
        if (path.corners.Length == 2) {
            return Vector3.Distance(path.corners[0], path.corners[1]);
        }
        // Else, iterate through the path, add the lengths and return it. 
        else {
            float lengthAccumulator = 0;
            for (int i = 0;  i < (path.corners.Length - 1); i++) {
                lengthAccumulator += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }
            return lengthAccumulator;
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

        newFocus.OnFocused(this);
    }

    public void GetDamage(out List<DamageType> damageTypes, out float baseDamage) {
        damageTypes = playerDamageTypes;
        baseDamage = playerBaseDamage;
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

    // void FixedUpdate() {
    //     // Do something with this, it can't stay here
    //     if (newDestination != transform.position && newDestination != null && actionsToMake > 0) {
    //         float step = speed * 4f * Time.fixedDeltaTime;
    //         Vector3 targetDirection = newDestination - transform.position;
    //         transform.position = Vector3.MoveTowards(transform.position, newDestination, step);
    //         Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);
    //         transform.rotation = Quaternion.LookRotation(newDir);
    //     }
    // }

    
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
