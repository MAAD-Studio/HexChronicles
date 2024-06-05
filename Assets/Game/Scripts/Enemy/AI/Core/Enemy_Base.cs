using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : Character, EnemyInterface
{
    #region Variables

    [Header("Enemy Specific:")]
    public EnemyAttributesSO enemySO;

    #endregion

    #region UnityMethods
    protected override void Start()
    {
        base.Start();
        moveDistance = enemySO.attributes.movementRange;
        attackDamage = enemySO.attributes.attackDamage;
        defensePercentage = enemySO.attributes.defensePercentage;
        elementType = enemySO.attributes.elementType;
        elementWeakAgainst = enemySO.attributes.elementWeakAgainst;
        elementStrongAgainst = enemySO.attributes.elementStrongAgainst;

        maxHealth = enemySO.attributes.health;
        currentHealth = maxHealth;

        basicAttackArea = enemySO.attackArea;
    }

    #endregion

    #region InterfaceMethods

    public virtual int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        return 0;
    }

    public virtual int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        return 0;
    }

    public virtual void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        animator.SetTrigger("attack");
    }

    public virtual bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    #endregion

    #region CustomMethods

    public override void TakeDamage(float damage, ElementType type)
    {
        int hitResult = Random.Range(0, 101);

        if(elementWeakAgainst == type && hitResult <= defensePercentage)
        {
            base.TakeDamage(damage + 1, type);
        }
        else if(elementStrongAgainst == type)
        {
            base.TakeDamage(damage - 1, type);
        }
        else
        {
            base.TakeDamage(damage, type);
        }
    }

    #endregion
}
