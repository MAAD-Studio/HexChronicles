using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Maker : Enemy_Base
{

    #region Variables

    [SerializeField] GameObject deathTilePrefab;

    #endregion

    #region InterfaceMethods

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

    #endregion

    #region CustomMethods

    public override void Died()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.pathfinder.PathTilesInRange(characterTile, 0, 1, true, false);

        List<Tile> potentialTiles = new List<Tile>(turnManager.pathfinder.frontier);

        foreach (Tile tile in potentialTiles)
        {
            ConvertToDeathTile(tile, true);
        }

        base.Died();
    }

    protected override Tile WalkOntoTileEffect(Tile tile)
    {
        return ConvertToDeathTile(tile, false);
    }

    private Tile ConvertToDeathTile(Tile tile, bool death)
    {
        if(tile.tileData.tileType == ElementType.death)
        {
            return tile;
        }
        GameObject deathTile = Instantiate(deathTilePrefab, tile.transform.position, Quaternion.identity);
        DeathTile deathTileObj = deathTile.GetComponent<DeathTile>();

        if(death)
        {
            UndoManager.Instance.StoreTile(deathTileObj, tile.tileData.tileType);
        }

        tile.ReplaceTileWithNew(deathTileObj);

        return deathTile.GetComponent<DeathTile>();
    }

    public override Character LikelyTarget()
    {
        return null;
    }

        #endregion
  }
