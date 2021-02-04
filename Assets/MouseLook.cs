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

    private Vector2 velocity;
    private Vector2 rotation;
    private Vector2 lastInputEvent;
    private float inputLagTimer;
    [SerializeField]
    private float tiltConstant = 62;

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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
