using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Hero : Character
{
    [Header("Hero Specific:")]
    public HeroAttributesSO heroSO;

    // Active skill
    public int skillCD = 3;
    public int currentSkillCD;

    public List<BuffModifier> buffModifiers; // Used in battle to modify stats

    protected override void Start()
    {
        base.Start();
        moveDistance = heroSO.attributes.movementRange;
        attackDamage = heroSO.attributes.attackDamage;
        defensePercentage = heroSO.attributes.defensePercentage;
        elementType = heroSO.attributes.elementType;

        maxHealth = heroSO.attributes.health;
        currentHealth = maxHealth;
        currentSkillCD = skillCD;
        buffModifiers = new List<BuffModifier>();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void Heal(float heal)
    {
        base.Heal(heal);
    }

    public override void PerformBasicAttack(List<Character> targets)
    {
        foreach (var target in targets)
        {
            target.TakeDamage(heroSO.attributes.attackDamage);
        }
    }

    public override void ReleaseActiveSkill(List<Character> targets)
    {
        heroSO.activeSkill.Release(targets);
    }

    public void ApplyPassiveSkill()
    {
        //passiveSkill.Apply();
    }

    #region Modifiers
    public void AddModifier(BuffModifier mod)
    {
        buffModifiers.Add(mod);
        CalculateCurrentStats(mod);
    }

    public void RemoveModifier(BuffModifier mod)
    {
        buffModifiers.Remove(mod);
        CalculateCurrentStats(mod);
    }

    public void ClearModifiers()
    {
        foreach (var mod in buffModifiers)
        {
            RemoveModifier(mod);
        }
        buffModifiers.Clear();
    }

    private void CalculateCurrentStats(BuffModifier mod)
    {
        switch (mod.attributeType)
        {
            case BasicAttributeType.AttackDamage:
                attackDamage += mod.value;
                break;
            /*case BasicAttributeType.AttackRange:
                heroSO.attributes.attackRange += mod.value;
                break;*/
            case BasicAttributeType.Health:
                currentHealth += mod.value;
                if (currentHealth >= maxHealth)
                    currentHealth = maxHealth;
                break;
            case BasicAttributeType.DefensePercentage:
                defensePercentage += mod.value;
                break;
            case BasicAttributeType.MovementRange:
                moveDistance += (int)mod.value;
                break;
        }
    }
    #endregion Modifiers

}