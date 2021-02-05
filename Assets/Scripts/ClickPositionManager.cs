using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPositionManager : MonoBehaviour
{
    public LayerMask clickMask;
    public float maxDistance = 100f;

    
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 currentPos = this.gameObject.transform.position;
            Vector3 clickPos = -Vector3.one;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, maxDistance, clickMask)) {
                clickPos = hit.point;
            }
            
        }
    }
}
