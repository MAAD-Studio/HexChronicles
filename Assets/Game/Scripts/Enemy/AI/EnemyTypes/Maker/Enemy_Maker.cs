using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Maker : Enemy_Base
{

    #region Variables

    [SerializeField] GameObject deathTilePrefab;

    #endregion

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int movementValue = 0;

        if(tile == characterTile)
        {
            movementValue -= 5;
        }

        if(tile.tileData.tileType != ElementType.death)
        {
            movementValue += 4;
        }

        foreach(Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
        {
            if(adjTile.tileData.tileType != ElementType.death)
            {
                movementValue += 1;
            }
        }

        return movementValue;
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        //Can't attack
        return 0;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        //Can't attack
    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    public override void Died()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.pathfinder.PathTilesInRange(characterTile, 0, 1, true);

        List<Tile> potentialTiles = new List<Tile>(turnManager.pathfinder.frontier);

        foreach (Tile tile in potentialTiles)
        {
            ConvertToDeathTile(tile);
        }

        base.Died();
    }

    protected override Tile WalkOntoTileEffect(Tile tile)
    {
        return ConvertToDeathTile(tile);
    }

    private Tile ConvertToDeathTile(Tile tile)
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();

        GameObject deathTile = Instantiate(deathTilePrefab, tile.transform.position, Quaternion.identity);
        DeathTile deathTileObj = deathTile.GetComponent<DeathTile>();

        deathTileObj.name = tile.name;
        deathTileObj.transform.parent = tile.transform.parent;
        deathTileObj.tileOccupied = tile.tileOccupied;
        deathTileObj.characterOnTile = tile.characterOnTile;
        deathTileObj.cost = tile.cost;
        deathTileObj.tileHasObject = tile.tileHasObject;
        deathTileObj.objectOnTile = tile.objectOnTile;
        deathTileObj.underWeatherAffect = tile.underWeatherAffect;
        deathTileObj.weatherCost = tile.weatherCost;
        deathTileObj.inFrontier = tile.inFrontier;
        if(deathTileObj.inFrontier)
        {
            turnManager.pathfinder.frontier.Add(deathTileObj);
        }
        deathTileObj.parentTile = tile.parentTile;

        turnManager.pathfinder.frontier.Remove(tile);

        Destroy(tile.gameObject);

        return deathTile.GetComponent<DeathTile>();
    }
}
