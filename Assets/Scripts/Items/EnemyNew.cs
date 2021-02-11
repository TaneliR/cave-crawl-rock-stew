using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNew : Interactable {

    private List<Damage> damages;
    private List<DamageType> resistances;
    private List<DamageType> weaknesses;
    
    [SerializeField]
    private Player player;
    
    [SerializeField]
    private float health = 100f;
    private float enemyDamage;

    public override void Interact()
    {
        base.Interact();

        GetHit();
    }
    
    private void Awake() {
        resistances = new List<DamageType>();
        weaknesses = new List<DamageType>();
    }

    private void GetHit() {
        player.GetDamage(out List<DamageType> damageTypes, out float baseDamage);
        float damage = CalculateIncomingDamage(damageTypes,baseDamage);
        Debug.Log($"Attacking {transform.name}.");
        Debug.Log($"With {damage} damage.");
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