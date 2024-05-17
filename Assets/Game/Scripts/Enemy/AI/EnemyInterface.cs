using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyInterface
{
    public Dictionary<Tile,Tile> DetermineAttackFrontier(Tile tile);

    public List<Tile> DetermineMovementFrontier();

    public void TakePath(Tile destination);

    public int CalculateMovementValue(Tile tile);

    public int CalculteAttackValue(AttackArea attackArea);

    public void ExecuteAttack(Tile target);
}
