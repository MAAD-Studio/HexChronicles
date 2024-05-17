using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : Character, EnemyInterface
{
    #region Variables

    #endregion

    #region UnityMethods

    #endregion

    #region InterfaceMethods

    public virtual int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager)
    {
        return 0;
    }

    public virtual int CalculteAttackValue(AttackArea attackArea)
    {
        return 0;
    }

    public virtual void ExecuteAttack(AttackArea attackArea)
    {
        
    }

    #endregion
}
