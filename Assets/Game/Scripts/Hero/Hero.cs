using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Character
{
    [Header("Hero Specific:")]
    public HeroAttributesSO heroSO;

    [Header("Active skill:")]
    [HideInInspector] public ActiveSkill activeSkill = new ActiveSkill();
    public int skillCD = 2;
    private int currentSkillCD = 2;
    public int CurrentSkillCD
    {
        get { return currentSkillCD; }
        set { currentSkillCD = value; }
    }

    [Header("Upgrades:")]
    public List<BasicUpgrade> upgradeList;

    private List<Character> currentTargets;

    protected override void Start()
    {
        base.Start();
        moveDistance = heroSO.attributes.movementRange;
        attackDamage = heroSO.attributes.attackDamage;
        defensePercentage = heroSO.attributes.defensePercentage;
        elementType = heroSO.attributes.elementType;
        elementWeakAgainst = heroSO.attributes.elementWeakAgainst;
        elementStrongAgainst = heroSO.attributes.elementStrongAgainst;

        maxHealth = heroSO.attributes.health;
        currentHealth = maxHealth;

        basicAttackArea = heroSO.attackArea;
        defaultScale = heroSO.attributes.defaultScale;

        activeSkill.thisCharacter = this;
        activeSkill.Initialize(heroSO.activeSkillSO);
        activeSkillArea = heroSO.activeSkillSO.shapeArea;
        currentSkillCD = (skillCD - 1);

        upgradeList = new List<BasicUpgrade>();

        UpdateHealthBar?.Invoke();
        UpdateAttributes?.Invoke();
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

    public override void TakeDamage(float damage, ElementType type)
    {
        int hitResult = UnityEngine.Random.Range(0, 101);

        Status shield = Status.GrabIfStatusActive(this, Status.StatusTypes.Shield);
        if (shield != null)
        {
            Debug.Log("SHIELDED FROM 5 DAMAGE");
            damage -= 5;
            MathF.Max(0, damage);
            RemoveStatus(shield);
        }

        if (elementWeakAgainst == type)// && hitResult <= defensePercentage)
        {
            base.TakeDamage(damage + 1, type);
        }
        else if (elementStrongAgainst == type)
        {
            base.TakeDamage(damage - 1, type);
        }
        else
        {
            base.TakeDamage(damage, type);
        }
    }

    public override void Heal(float heal)
    {
        base.Heal(heal);
    }

    public override void PerformBasicAttack(List<Character> targets)
    {
        if (characterTile.tileData.tileType == elementType)
        {
            SpawnTileVFX(transform.position, true);
        }
        else if (characterTile.tileData.tileType != ElementType.Base)
        {
            SpawnTileVFX(transform.position, false);
        }

        if (targets.Count != 0)
        {
            currentTargets = targets;
            animator.SetTrigger("attack");

            StartCoroutine(SpawnVFXAndAttack());
        }
    }

    private IEnumerator SpawnVFXAndAttack()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (var target in currentTargets)
        {
            SpawnAttackVFX(target);
        }
    }

    private void SpawnAttackVFX(Character target)
    {
        GameObject vfx = null;
        if (elementType == ElementType.Fire)
        {
            vfx = Instantiate(attackVFX, transform.position, Quaternion.identity);
            vfx.transform.rotation = Quaternion.LookRotation(transform.forward);
        }
        else if (elementType == ElementType.Water)
        {
            vfx = Instantiate(attackVFX, target.transform.position, Quaternion.identity);
        }
        else if (elementType == ElementType.Grass)
        {
            vfx = Instantiate(attackVFX, transform.position, Quaternion.identity);
            vfx.transform.LookAt(target.transform.position);
        }

        AttackVFX attack = vfx.GetComponent<AttackVFX>();
        attack.Initialize(target, ActualBasicAttack);
    }

    // Called by AttackVFX when the VFX hits the target
    private void ActualBasicAttack(Character target)
    {
        int actualDamage = (int)attackDamage;
        if (Status.GrabIfStatusActive(this, Status.StatusTypes.AttackBoost) != null)
        {
            attackDamage += 1;
        }

        target.TakeDamage(actualDamage, elementType);

        base.PerformBasicAttack(currentTargets);
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

    #region Upgrade
    public void AddUpgrade(BasicUpgrade upgrade)
    {
        upgradeList.Add(upgrade);

        switch (upgrade.attributeType)
        {
            case BasicAttributeType.Health:
                maxHealth += upgrade.value;
                break;
            case BasicAttributeType.MovementRange:
                moveDistance += upgrade.value;
                break;
            case BasicAttributeType.AttackDamage:
                attackDamage += upgrade.value;
                break;
            case BasicAttributeType.DefensePercentage:
                defensePercentage += upgrade.value;
                break;
        }
    }

    public void RemoveUpgrade(BasicUpgrade upgrade)
    {
        upgradeList.Remove(upgrade);

        switch (upgrade.attributeType)
        {
            case BasicAttributeType.Health:
                maxHealth -= upgrade.value;
                break;
            case BasicAttributeType.MovementRange:
                moveDistance -= upgrade.value;
                break;
            case BasicAttributeType.AttackDamage:
                attackDamage -= upgrade.value;
                break;
            case BasicAttributeType.DefensePercentage:
                defensePercentage -= upgrade.value;
                break;
        }
    }

    public void ClearUpgrades()
    {
        foreach (var upgrade in upgradeList)
        {
            RemoveUpgrade(upgrade);
        }
        upgradeList.Clear();
    }

    #endregion 

}