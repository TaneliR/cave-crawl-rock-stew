using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    private NavMeshPath path;
    private NavMeshAgent enemyAgent;
    private Animator enemyAnimator;
    private List<Vector3> wayPoints = new List<Vector3>();
    private float speed = 1f;
    const int distanceConstant = 8;
    private bool actionsRunning = false;
    private bool attackAfterMove = false;
    private int actionsToMake = 0;
    private int currentActionIndex = 0;

    [SerializeField]
    private GameObject player;
    void Awake()
    {
        path = new NavMeshPath(); 
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
    }

    void Start() {
        TickManager.OnTick += TickManager_OnTick;
        
    }
    private void TickManager_OnTick(object sender, TickManager.OnTickEventArgs  e) {
        Debug.Log(e.tick);
        if (e.tick >= distanceConstant)
        {
            actionsToMake++;
            currentActionIndex = 0;
            if (!actionsRunning) StartCoroutine("DoActions");
        }
    }
    IEnumerator DoActions() {
        actionsRunning = true;
        while(actionsToMake > 0) {
            Debug.Log("ENEMY ACTION");
            CalculatePathToPlayer();
            Debug.Log("ENEMY WAYPOINTS:" + wayPoints.Count);
            Debug.Log("ENEMY CUR ACTION IDX:" + currentActionIndex);
            Debug.Log("GOING TOWARDS POINT: " + wayPoints[currentActionIndex]);
            enemyAgent.destination = wayPoints[currentActionIndex];
            enemyAnimator.SetBool("Moving", true);
            currentActionIndex += 1;
            actionsToMake--;
            yield return new WaitForSeconds(2f);
        }
        actionsRunning = false;
        enemyAnimator.SetBool("Moving", false);
    }
    float GetMaxDistanceFromSpeed(float speed) {
        return speed * distanceConstant;
    }
    
    void Update()
    {
        if (attackAfterMove && GetDistanceToPlayer() < 2f) {
            HitPlayer();
        }
    }
    private float GetDistanceToPlayer() {
        float distance = Vector3.Distance(gameObject.transform.position, player.transform.position);
        return distance;
    }

    private void HitPlayer() {
        Debug.Log("HIYAA!");
        enemyAnimator.SetTrigger("Attack");
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.isKinematic = false;
        playerBody.AddForce((gameObject.transform.position + player.transform.position) + Vector3.forward * 30f);
        attackAfterMove = false;
    }

    private void CalculatePathToPlayer() {
        wayPoints = new List<Vector3>();
        NavMesh.CalculatePath(gameObject.transform.position, player.transform.position, NavMesh.AllAreas, path);
        float currentLength = 0;
        for (int i = 0; i < path.corners.Length - 1; i++) {
            currentLength += Vector3.Distance(path.corners[i], path.corners[i+1]);
            int iterations = Mathf.FloorToInt(currentLength / GetMaxDistanceFromSpeed(speed));
            if (path.corners.Length <= 2) {
                Vector3 maxPoint = Vector3.Lerp(gameObject.transform.position, player.transform.position, (GetMaxDistanceFromSpeed(speed)/currentLength));
                wayPoints.Add(maxPoint);
                if (Vector3.Distance(gameObject.transform.position, maxPoint) < GetMaxDistanceFromSpeed(speed)) {
                    attackAfterMove = true;
                    Debug.Log("SHOULD ATTACK");
                }
            } else {
                for (int j = 0; j < iterations; j++) {
                    Vector3 maxPoint = Vector3.Lerp(path.corners[j], path.corners[j+1], (GetMaxDistanceFromSpeed(speed)*j+1f)/currentLength);
                    wayPoints.Add(maxPoint);
                }
                currentLength -= GetMaxDistanceFromSpeed(speed);
            }
        }
    }
}
