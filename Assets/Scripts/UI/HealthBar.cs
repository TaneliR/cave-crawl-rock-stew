﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBarImage;
    [SerializeField]
    private float updateSpeedSeconds = .1f;
    [SerializeField]
    private float positionOffset;

    private Health health;

    public void SetHealth(Health health) {
        this.health = health;
        health.OnHealthPctChanged += HandleHealthChanged;
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
        healthBarImage.fillAmount = pct;
    }
    
    // Look at camera
    private void LateUpdate() {
        transform.position = Camera.main.WorldToScreenPoint(health.transform.position +  Vector3.up * positionOffset);
    }
    
    private void OnDestroy() {
        health.OnHealthPctChanged -= HandleHealthChanged;    
    }
    
}
