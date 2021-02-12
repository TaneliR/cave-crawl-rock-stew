using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event Action<Health> OnHealthAdded = delegate { };
    public static event Action<Health> OnHealthRemoved = delegate { };

    [SerializeField]
    private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable() {
        CurrentHealth = maxHealth;
        OnHealthAdded(this);
    }

    public void ChangeHealth(float amt) {
        CurrentHealth += amt;

        float currentHealthPct = (float)CurrentHealth / (float)maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }

    private void OnDisable() {
        OnHealthRemoved(this);
    }
}
