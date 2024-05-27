using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyInterface
{
    public int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager);

    public int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager);

    public void ExecuteAttack(AttackArea attackArea, TurnManager turnManager);
}
