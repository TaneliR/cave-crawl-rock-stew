using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Health))]
public class EnemyNew : Interactable {

    [SerializeField]
    private Transform popup;
    
    [SerializeField]
    private Transform parent;

    private List<Damage> damages;
    private List<DamageType> resistances;
    private List<DamageType> weaknesses;
    private Health health;
    
    private float enemyDamage;

    public override void Interact()
    {
        base.Interact();

        GetHit();
    }
    
    private void Start() {
        parent = GameObject.FindGameObjectWithTag("DamagePopup").transform;
        health = GetComponent<Health>();
        resistances = new List<DamageType>();
        weaknesses = new List<DamageType>();
    }

    private void GetHit() {
        player.GetDamage(out List<DamageType> damageTypes, out float baseDamage);
        float damage = CalculateIncomingDamage(damageTypes,baseDamage);
        Debug.Log($"Attacking {transform.name}.");
        Debug.Log($"With {damage} damage.");
        health.ChangeHealth(-damage);
        Transform damagePopup = Instantiate(popup, transform.position, Quaternion.identity);
        damagePopup.GetComponent<DamagePopup>().Setup(transform, damage);
        
        damagePopup.SetParent(parent);
    }

    private float CalculateIncomingDamage(List<DamageType> damageTypes, float baseDamage) {
        float modifiers = 1;
        foreach(DamageType type in damageTypes) {
            modifiers += CalculateDamageModifiers(type);
        }
        return baseDamage * modifiers;
    }

    private float CalculateDamageModifiers(DamageType damageType) {
        float modifier = 0;
        if (weaknesses.Contains(damageType)) {
            modifier += 2f;
        } else if (resistances.Contains(damageType)) {
            modifier -= 2f;
        }
        return modifier;
    }
}

struct Damage {
    public float damageAmount;
    public List<DamageType> damageType;
    public Damage(float _damageAmount,  List<DamageType> _damageTypes)
    {
        this.damageAmount = _damageAmount;
        this.damageType = _damageTypes;
    }
}

public enum DamageType { Acid, Bludgeoning, Cold, Fire, Force, Lightning, Necrotic, Piercing, Poison, Psychic, Radiant, Slashing, Thunder }