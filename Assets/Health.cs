using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable() {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amt) {
        currentHealth += amt;

        float currentHealthPct = (float)currentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            ChangeHealth(-12);
        }
    }
}
