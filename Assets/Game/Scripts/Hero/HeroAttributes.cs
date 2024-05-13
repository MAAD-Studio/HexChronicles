using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroAttributes
{
    public string heroName;
    public string description;
    public Sprite avatar;

    [Header("Basic Attributes")]
    public float health = 0;
    public float movementRange = 0;
    public float attackDamage = 0;
    public float attackRange = 0;
    public float defensePercentage = 0;

    [Header("Max Values")]
    public float maxHealth = 0;
    public float maxMovementRange = 0;
    public float maxAttackDamage = 0;
    public float maxAttackRange = 0;
    public float maxDefense = 0;

    [Header("Element")]
    public ElementType elementType;
}
