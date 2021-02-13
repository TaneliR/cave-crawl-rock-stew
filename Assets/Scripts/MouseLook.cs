using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private Vector3 refVelocity = Vector3.zero; 
    [SerializeField]private Vector3 offset;
    [SerializeField]private Vector2 acceleration;
    [SerializeField]private Vector2 sensitivity;
    [SerializeField]private float maxVerticalAngleFromHorizon;
    [SerializeField]private float maxHorizontalAngleFromHorizon;
    [SerializeField]private float inputLagPeriod;
    [SerializeField]private Player player;
    [SerializeField]private bool smoothFollow = true;


    private bool reseting = false;
    private Vector2 velocity;
    private Vector2 rotation;
    private Vector2 lastInputEvent;
    private Vector3 playerForward;
    private Vector3 cameraForward;
    public float cameraRotationSpeed;
    private float inputLagTimer;
    [SerializeField]
    private float tiltConstant = 50;
    [SerializeField]
    private float cameraThreshold;

    private float ClampAngle(float angle, float max) {
        return Mathf.Clamp(angle, -max, max);;
    }

    private Vector2 GetInput() {
        inputLagTimer += Time.deltaTime;
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );
        if ((Mathf.Approximately(0, input.x) && Mathf.Approximately(0, input.y)) ==  false || inputLagTimer >= inputLagPeriod) {
            lastInputEvent = input;
            inputLagTimer = 0;
        }
        return lastInputEvent;
    }
    private void Start() {
        offset = transform.position - player.transform.position;
        // Debug.Log("Start transform" + gameObject.GetComponent<Transform>().transform);
        // Debug.Log("Start position" + gameObject.GetComponent<Transform>().position);
        // Debug.Log("Start rotation" + gameObject.GetComponent<Transform>().rotation.eulerAngles);
    }
    // Realized I dont need this yet lol
    bool CameraWithinThreshold() {
        playerForward = player.GetComponent<Transform>().forward;
        cameraForward = transform.forward;
        return true;
    }
    

    private IEnumerator ResetCameraAngle(float resetWait) {
        yield return new WaitForSeconds(resetWait);
        playerForward = player.GetComponent<Transform>().forward;
        cameraForward = transform.forward;
        Debug.Log("Player fwd: " + playerForward);
        Debug.Log("Camera fwd: " + cameraForward);
        Debug.Log("Camera rotation: " + gameObject.transform.rotation);
        cameraForward.y =  0;
        playerForward.y = 0;
        Quaternion resetRotation = Quaternion.LookRotation(playerForward, Vector3.up);
        Quaternion cameraRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        Debug.Log("Reset rot: " + resetRotation);
        Debug.Log("Camera rot: " + cameraRotation);
        float elapsed = 0;
        // rotation speed bugged
        while(elapsed < cameraRotationSpeed) {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(cameraRotation, resetRotation, elapsed / cameraRotationSpeed);

            yield return null;
        }
        reseting = false;
    }

    void LateUpdate()
    {   
        if (smoothFollow){
            if (Input.GetMouseButton(1)) {
                Vector2 wantedVelocity = GetInput() * (sensitivity * 2f);
                
                velocity = new Vector2(
                    Mathf.MoveTowards(velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime),
                    Mathf.MoveTowards(velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime)
                );
                
                rotation += velocity * Time.deltaTime;
                Debug.Log("rotaatio" + rotation);
                rotation.y = ClampAngle(rotation.y, maxVerticalAngleFromHorizon);

                transform.eulerAngles = new Vector3(rotation.y + tiltConstant, -rotation.x, 0);
                // Without the offset theres a cool zoom ready to use lol! :D
                Vector3 targetPos = player.transform.position + offset;
                transform.position = Vector3.SmoothDamp(player.transform.position, targetPos, ref refVelocity, 0.2f);
            }
            else { 
                Vector3 targetPos = player.transform.position + offset;
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref refVelocity, 0.2f);
                transform.LookAt(player.transform);
            }
        
        }
        // if (!reseting) {
        //     if (player.actionsRunning)
        //         reseting = true;
        //         StartCoroutine("ResetCameraAngle", 1f);
        // if (Input.GetMouseButton(1)) {
        //     smoothFollow = false;
        //     Vector2 wantedVelocity = GetInput() * (sensitivity * 2f);
            
        //     velocity = new Vector2(
        //         Mathf.MoveTowards(velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime),
        //         Mathf.MoveTowards(velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime)
        //     );
        //     rotation += velocity * Time.deltaTime;
        //     rotation.y = ClampAngle(rotation.y, maxVerticalAngleFromHorizon);

        //     transform.localEulerAngles = new Vector3(rotation.y + tiltConstant, -rotation.x, 0);
        // }
        // }
    }
}
