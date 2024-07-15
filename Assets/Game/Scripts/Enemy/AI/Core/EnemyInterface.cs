using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyInterface
{
    public void PreCalculations(TurnManager turnManager);

    public int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter);

    public int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile);

    public void ExecuteAttack(AttackArea attackArea, TurnManager turnManager);

    public bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager);

    public void ActionCleanup();

    public Character LikelyTarget();
}
