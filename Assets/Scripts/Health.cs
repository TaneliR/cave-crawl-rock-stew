using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event Action<Health> OnHealthAdded = delegate { };
    public static event Action<Health> OnHealthRemoved = delegate { };

    [SerializeField]
    private int maxHealth = 100;

    public int CurrentHealth { get; private set; }

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable() {
        CurrentHealth = maxHealth;
        OnHealthAdded(this);
    }

    public void ChangeHealth(int amt) {
        CurrentHealth += amt;

        float currentHealthPct = (float)CurrentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }

    // DEBUG HEALTH
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            ChangeHealth(-12);
        }
    }
    private void OnDisable() {
        OnHealthRemoved(this);
    }
}
