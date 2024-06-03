using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MasterJelly : Enemy_Base
{
    #region Variables

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager)
    {
        return 0;
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager)
    {
        return 0;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {

    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    #endregion
}
