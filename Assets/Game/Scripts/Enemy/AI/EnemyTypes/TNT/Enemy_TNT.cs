using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TNT : Enemy_Base
{
    #region Variables

    [SerializeField] private int closeRangeDMG = 10;
    [SerializeField] private int farRangeDMG = 5;

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        return base.CalculateMovementValue(tile, enemy, turnManager, closestCharacter);
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        return base.CalculteAttackValue(attackArea, turnManager, currentTile);
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        base.ExecuteAttack(attackArea, turnManager);
    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    #endregion

    #region CustomMethods

    public override void Died()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.pathfinder.PathTilesInRange(characterTile, 0, 2, true, false);

        List<Tile> potentialTiles = new List<Tile>(turnManager.pathfinder.frontier);

        foreach (Tile tile in potentialTiles)
        {
            if (tile.tileOccupied && tile.characterOnTile != this && tile.characterOnTile.currentHealth > 0)
            {
                Character characterOnTile = tile.characterOnTile;
                if(characterOnTile.characterType == TurnEnums.CharacterType.Player)
                {
                    UndoManager.Instance.StoreHero((Hero)characterOnTile);
                }
                else
                {
                    UndoManager.Instance.StoreEnemy((Enemy_Base)characterOnTile, false);
                }

                if (tile.cost == 1)
                {
                    characterOnTile.TakeDamage(closeRangeDMG, elementType);
                }
                else
                {
                    characterOnTile.TakeDamage(farRangeDMG, elementType);
                }
            }

            TemporaryMarker.GenerateMarker(enemySO.attributes.hitMarker, tile.transform.position, 2f, 0.5f);
        }

        base.Died();
    }

    #endregion
}
