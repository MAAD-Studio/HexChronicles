using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hero : MonoBehaviour
{
    public HeroAttributesSO heroAttributes;
    public float originalHealth = 0;

    // ToDo: different heroes has different activeskills, but same basic attributes??
    //public ActiveSkill activeSkill;
    //public PassiveSkill passiveSkill;

    public List<StatModifier> statModifiers; // Used in battle to modify stats

    // Events
    public event EventHandler OnDamage;
    public event EventHandler OnHeal;
    public event EventHandler OnDeath;

    private void Start()
    {
        originalHealth = heroAttributes.health;
        statModifiers = new List<StatModifier>();
    }

    public void TakeDamage(float damage)
    {
        heroAttributes.health -= damage;

        if (heroAttributes.health < 0)
        {
            heroAttributes.health = 0;
            // ToDo: HeroDied();
            OnDeath?.Invoke(this, EventArgs.Empty);
        }

        OnDamage?.Invoke(this, EventArgs.Empty); // ? is checking if OnDamage is null
    }

    public void Heal(float heal)
    {
        heroAttributes.health += heal;

        if (heroAttributes.health > heroAttributes.maxHealth)
        {
            heroAttributes.health = heroAttributes.maxHealth;
        }

        OnHeal?.Invoke(this, EventArgs.Empty);
    }

    public void ReleaseActiveSkill(Hero target)
    {
        //activeSkill.Release();
        target.TakeDamage(heroAttributes.attackDamage);
    }

    public void ApplyPassiveSkill()
    {
        //passiveSkill.Apply();
    }

    #region StatModifiers
    public void AddStatModifiers(StatModifier mod)
    {
        statModifiers.Add(mod);
        CalculateCurrentStats(mod);
    }

    public void RemoveStatModifiers(StatModifier mod)
    {
        statModifiers.Remove(mod);
        CalculateCurrentStats(mod);
    }

    public void ClearStatModifiers()
    {
        foreach (var mod in statModifiers)
        {
            RemoveStatModifiers(mod);
        }
        statModifiers.Clear();
    }

    private void CalculateCurrentStats(StatModifier mod)
    {
        switch (mod.attributeType)
        {
            case BasicAttributeType.AttackDamage:
                heroAttributes.attackDamage += mod.value;
                if (heroAttributes.attackDamage >= heroAttributes.maxAttackDamage)
                {
                    heroAttributes.attackDamage = heroAttributes.maxAttackDamage;
                    Debug.Log("Attack Damage Reached Max Value");
                }
                break;
            case BasicAttributeType.AttackRange:
                heroAttributes.attackRange += mod.value;
                if (heroAttributes.attackRange >= heroAttributes.maxAttackRange)
                    heroAttributes.attackRange = heroAttributes.maxAttackRange;
                break;
            case BasicAttributeType.Health:
                heroAttributes.health += mod.value;
                if (heroAttributes.health >= heroAttributes.maxHealth)
                    heroAttributes.health = heroAttributes.maxHealth;
                break;
            case BasicAttributeType.DefensePercentage:
                heroAttributes.defensePercentage += mod.value;
                if (heroAttributes.defensePercentage >= heroAttributes.maxDefense)
                    heroAttributes.defensePercentage = heroAttributes.maxDefense;
                break;
            case BasicAttributeType.MovementRange:
                heroAttributes.movementRange += mod.value;
                if (heroAttributes.movementRange >= heroAttributes.maxMovementRange)
                    heroAttributes.movementRange = heroAttributes.maxMovementRange;
                break;
        }
    }
    #endregion StatModifiers

    // ToDo: Save and Load HeroAttributes

}