using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private Transform target;
    private float offsetSpeed = 0;
    private float increaseScaleAmount = 2f;
    private TextMeshProUGUI textMesh;
    private Color textColor;

    private const float DISAPPEAR_TIMER_MAX = 0.5f;
    private float disappearTimer;

    private void Awake() {
        textMesh = GetComponent<TextMeshProUGUI>();
        textColor = textMesh.color;
    }

    public void Setup(Transform _target, float damage) {
        target = _target;
        disappearTimer = DISAPPEAR_TIMER_MAX;
        textMesh.SetText(damage.ToString());
    }
    // Camera.main.WorldToScreenPoint(target.position + Vector3.up * 5.5f + Vector3.right * 1.1f + (new Vector3(0, moveYSpeed) * Time.deltaTime));
    private void LateUpdate() {
        offsetSpeed += 1f * Time.deltaTime;
        Vector3 offset = new Vector3(0, offsetSpeed);
        transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 5f + Vector3.right * 1f + offset);
        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            // First make larger
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        } else {
            // Then smaller
            transform.localScale -= Vector3.one * increaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;

        if (disappearTimer <= 0) {
            textColor.a -= 3f * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }
}
