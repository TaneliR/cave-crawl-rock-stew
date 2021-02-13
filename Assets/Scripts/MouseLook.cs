using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]private Vector2 acceleration;
    [SerializeField]private Vector2 sensitivity;
    [SerializeField]private float maxVerticalAngleFromHorizon;
    [SerializeField]private float maxHorizontalAngleFromHorizon;
    [SerializeField]private float inputLagPeriod;
    [SerializeField]private Player player;

    private bool reseting = false;
    private Vector2 velocity;
    private Vector2 rotation;
    private Vector2 lastInputEvent;
    private Vector3 playerForward;
    private Vector3 cameraForward;
    public float cameraRotationSpeed;
    private float inputLagTimer;
    [SerializeField]
    private float tiltConstant = 62;
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

    // Realized I dont need this yet lol
    bool CameraWithinThreshold() {
        playerForward = GetComponentInParent<Transform>().forward;
        cameraForward = gameObject.transform.forward;
        return true;
    }
    

    private IEnumerator ResetCameraAngle(float resetWait) {
        yield return new WaitForSeconds(resetWait);
        playerForward = GetComponentInParent<Transform>().forward;
        cameraForward = gameObject.transform.forward;
        cameraForward.y =  0;
        playerForward.y = 0;
        Quaternion resetRotation = Quaternion.LookRotation(playerForward, Vector3.up);
        Quaternion cameraRotation = Quaternion.LookRotation(cameraForward, Vector3.up);

        float elapsed = 0;
        // rotation speed bugged
        while(elapsed < cameraRotationSpeed) {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(cameraRotation, resetRotation, elapsed / cameraRotationSpeed);

            yield return null;
        }
        reseting = false;
    }

    void Update()
    {   
        if (!reseting) {
            if (player.actionsRunning)
                reseting = true;
                StartCoroutine("ResetCameraAngle", 1f);
            if (Input.GetMouseButton(1)) {
                Vector2 wantedVelocity = GetInput() * (sensitivity * 2f);
                
                velocity = new Vector2(
                    Mathf.MoveTowards(velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime),
                    Mathf.MoveTowards(velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime)
                );
                rotation += velocity * Time.deltaTime;
                rotation.y = ClampAngle(rotation.y, maxVerticalAngleFromHorizon);

                transform.localEulerAngles = new Vector3(rotation.y + tiltConstant, -rotation.x, 0);
            }
        }
        
        
    }
}
