using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Drainer : Enemy_Base
{
    #region Variables

    [SerializeField] GameObject basePrefab;

    #endregion

    #region CustomMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int movementValue = 0;

        if (tile.tileData.tileType != ElementType.Base)
        {
            movementValue += 15;
        }

        foreach (Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
        {
            if (adjTile.tileData.tileType != ElementType.Base)
            {
                movementValue += 15;
            }
        }

        return movementValue;
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
        characterTile = ConvertToBaseTile(characterTile);

        foreach(Tile tile in turnManager.pathfinder.FindAdjacentTiles(characterTile, true))
        {
            ConvertToBaseTile(tile);
        }

        return false;
    }

    private Tile ConvertToBaseTile(Tile tile)
    {
        if(tile.tileData.tileType == ElementType.Base)
        {
            return tile;
        }

        TurnManager turnManager = FindObjectOfType<TurnManager>();

        GameObject baseTile = Instantiate(basePrefab, tile.transform.position, Quaternion.identity);
        Tile baseTileObj = baseTile.GetComponent<Tile>();

        baseTileObj.name = tile.name;
        baseTileObj.transform.parent = tile.transform.parent;
        baseTileObj.tileOccupied = tile.tileOccupied;
        baseTileObj.characterOnTile = tile.characterOnTile;
        baseTileObj.cost = tile.cost;
        baseTileObj.tileHasObject = tile.tileHasObject;
        baseTileObj.objectOnTile = tile.objectOnTile;
        baseTileObj.underWeatherAffect = tile.underWeatherAffect;
        baseTileObj.weatherCost = tile.weatherCost;
        baseTileObj.parentTile = tile.parentTile;

        turnManager.pathfinder.frontier.Remove(tile);

        Destroy(tile.gameObject);

        return baseTileObj;
    }

    #endregion
}
