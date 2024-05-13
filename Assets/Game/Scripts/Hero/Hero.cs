using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hero : MonoBehaviour
{
    public HeroAttributesSO heroSO;
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
        originalHealth = heroSO.attributes.health;
        statModifiers = new List<StatModifier>();
    }

    public void TakeDamage(float damage)
    {
        heroSO.attributes.health -= damage;

        if (heroSO.attributes.health < 0)
        {
            heroSO.attributes.health = 0;
            // ToDo: HeroDied();
            OnDeath?.Invoke(this, EventArgs.Empty);
        }

        OnDamage?.Invoke(this, EventArgs.Empty); // ? is checking if OnDamage is null
    }

    public void Heal(float heal)
    {
        heroSO.attributes.health += heal;

        if (heroSO.attributes.health > heroSO.attributes.maxHealth)
        {
            heroSO.attributes.health = heroSO.attributes.maxHealth;
        }

        OnHeal?.Invoke(this, EventArgs.Empty);
    }

    public void ReleaseActiveSkill(Hero target)
    {
        //activeSkill.Release();
        target.TakeDamage(heroSO.attributes.attackDamage);
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
                heroSO.attributes.attackDamage += mod.value;
                if (heroSO.attributes.attackDamage >= heroSO.attributes.maxAttackDamage)
                {
                    heroSO.attributes.attackDamage = heroSO.attributes.maxAttackDamage;
                    Debug.Log("Attack Damage Reached Max Value");
                }
                break;
            case BasicAttributeType.AttackRange:
                heroSO.attributes.attackRange += mod.value;
                if (heroSO.attributes.attackRange >= heroSO.attributes.maxAttackRange)
                    heroSO.attributes.attackRange = heroSO.attributes.maxAttackRange;
                break;
            case BasicAttributeType.Health:
                heroSO.attributes.health += mod.value;
                if (heroSO.attributes.health >= heroSO.attributes.maxHealth)
                    heroSO.attributes.health = heroSO.attributes.maxHealth;
                break;
            case BasicAttributeType.DefensePercentage:
                heroSO.attributes.defensePercentage += mod.value;
                if (heroSO.attributes.defensePercentage >= heroSO.attributes.maxDefense)
                    heroSO.attributes.defensePercentage = heroSO.attributes.maxDefense;
                break;
            case BasicAttributeType.MovementRange:
                heroSO.attributes.movementRange += mod.value;
                if (heroSO.attributes.movementRange >= heroSO.attributes.maxMovementRange)
                    heroSO.attributes.movementRange = heroSO.attributes.maxMovementRange;
                break;
        }
    }
    #endregion StatModifiers

    // ToDo: Save and Load HeroAttributes

}