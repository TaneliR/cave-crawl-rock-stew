using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBarImage;
    [SerializeField]
    private float updateSpeedSeconds = 0.5f;
    void Awake()
    {
        GetComponentInParent<Health>().OnHealthPctChanged += HandleHealthChanged;
    }

    
    private void HandleHealthChanged(float pct) {
        StartCoroutine(ChangeToPct(pct));
    }
    
    private IEnumerator ChangeToPct(float pct) {
        float preChangePct = healthBarImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds) {
            elapsed += Time.deltaTime;
            healthBarImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }
    }
    
    // Look at camera
    private void LateUpdate() {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
        
    
}
