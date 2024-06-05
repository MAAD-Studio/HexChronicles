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

        maxHealth = enemySO.attributes.health;
        currentHealth = maxHealth;

        basicAttackArea = enemySO.attackArea;
    }

    #endregion

    #region InterfaceMethods

    public virtual int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager)
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
        if(elementWeakAgainst == type)
        {
            base.TakeDamage(damage, type);
        }
        else
        {
            int hitNum = Random.Range(0, 101);
            if (hitNum > enemySO.attributes.defensePercentage)
            {
                base.TakeDamage(damage, type);
            }
            else
            {
                TemporaryMarker.GenerateMarker(enemySO.attributes.missText, gameObject.transform.position, 4f, 0.5f);
            }
        }
    }

    #endregion
}
