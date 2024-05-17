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

    public Dictionary<Tile, Tile> DetermineAttackFrontier(Tile tile)
    {
        throw new System.NotImplementedException();
    }

    public List<Tile> DetermineMovementFrontier()
    {
        throw new System.NotImplementedException();
    }

    public void TakePath(Tile destination)
    {
        throw new System.NotImplementedException();
    }

    public virtual int CalculateMovementValue(Tile tile)
    {
        return 0;
    }

    public virtual int CalculteAttackValue(AttackArea attackArea)
    {
        return 0;
    }

    public virtual void ExecuteAttack(Tile target)
    {
        
    }

    #endregion
}
