using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Basic : Enemy_Base
{
    #region Variables

    #endregion

    #region UnityMethods

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile)
    {
        return 0;
    }

    public override int CalculteAttackValue(AttackArea attackArea)
    {
        return 0;
    }

    public override void ExecuteAttack(Tile target)
    {

    }

    #endregion
}
