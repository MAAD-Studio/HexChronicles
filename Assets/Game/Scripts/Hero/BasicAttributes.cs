using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAttributes
{
    public string name;
    public string description;
    public Sprite avatar;

    [Header("Basic Attributes")]
    public float health = 0;
    public int movementRange = 0;
    public float attackDamage = 0;
    public int attackRange = 0;
    public float defensePercentage = 0;

    [Header("Max Upgrade")]
    public float maxHealth = 0;
    public float maxMovementRange = 0;
    public float maxAttackDamage = 0;
    public float maxAttackRange = 0;
    public float maxDefense = 0;

    [Header("Element")]
    public ElementType elementType;

    //[Header("Upgrades")]
    //public List<StatModifier> statModifiers = new List<StatModifier>();
}