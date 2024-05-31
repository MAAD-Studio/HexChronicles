using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    [Header("Hero Specific:")]
    public HeroAttributesSO heroSO;

    // Active skill
    [HideInInspector] public ActiveSkill activeSkill;
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

        basicAttackArea = heroSO.attackArea;

        activeSkill = heroSO.activeSkill.Clone();
        activeSkill.thisCharacter = this;
        activeSkillArea = activeSkill.shapeArea;
        currentSkillCD = skillCD;

        buffModifiers = new List<BuffModifier>();
    }

    public override void EnterNewTurn()
    {
        base.EnterNewTurn();

        ReduceSkillCD(1);
    }

    public void ReduceSkillCD(int value)
    {
        currentSkillCD = Mathf.Clamp(currentSkillCD - value, 0, skillCD);
    }

    public override void TakeDamage(float damage)
    {
        int hitNum = UnityEngine.Random.Range(0, 101);
        if (hitNum > heroSO.attributes.defensePercentage)
        {
            base.TakeDamage(damage);
        }
        else
        {
            TemporaryMarker.GenerateMarker(heroSO.attributes.missText, gameObject.transform.position, 4f, 0.5f);
        }
    }

    public override void Heal(float heal)
    {
        base.Heal(heal);
    }

    public override void PerformBasicAttack(List<Character> targets)
    {
        base.PerformBasicAttack(targets);

        foreach (var target in targets)
        {
            target.TakeDamage(attackDamage);
            target.PreviewDamage(0);
        }
    }

    public override void ReleaseActiveSkill(List<Character> targets)
    {
        base.ReleaseActiveSkill(targets);

        activeSkill.Release(targets);
        currentSkillCD = skillCD;
    }

    public override void PerformBasicAttackObjects(List<TileObject> targets)
    {
        foreach(TileObject target in targets)
        {
            target.TakeDamage(attackDamage);
            target.PreviewDamage(0);
        }
    }

    public void ApplyPassiveSkill()
    {
        //passiveSkill.Apply();
    }

    #region Modifiers
    public void AddModifier(BuffModifier mod)
    {
        buffModifiers.Add(mod);

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

    public void RemoveModifier(BuffModifier mod)
    {
        buffModifiers.Remove(mod);

        switch (mod.attributeType)
        {
            case BasicAttributeType.AttackDamage:
                attackDamage -= mod.value;
                break;
            /*case BasicAttributeType.AttackRange:
                heroSO.attributes.attackRange -= mod.value;
                break;*/
            case BasicAttributeType.Health:
                currentHealth -= mod.value;
                if (currentHealth < 0)
                    currentHealth = 0;
                break;
            case BasicAttributeType.DefensePercentage:
                defensePercentage -= mod.value;
                break;
            case BasicAttributeType.MovementRange:
                moveDistance -= (int)mod.value;
                break;
        }
    }

    public void ClearModifiers()
    {
        foreach (var mod in buffModifiers)
        {
            RemoveModifier(mod);
        }
        buffModifiers.Clear();
    }

    #endregion Modifiers

}