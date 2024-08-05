using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_SporeStorm : Weather_Base
{
    #region Variables

    [SerializeField] private int healthDebuff = 1;
    [SerializeField] private Tile grassTilePrefab;

    #endregion

    #region UnityMethods

    private void Start()
    {
        weatherName = "SporeStorm";
    }

    #endregion

    #region CustomMethods

    public override void ApplyEffect(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character.elementType == ElementType.Water)
            {
                ApplyStatusToCharacter(character, Status.StatusTypes.MovementReduction);
                character.TakeDamage(healthDebuff, ElementType.Base);
            }
            else if (character.elementType == ElementType.Grass)
            {
               
            }

            if (Status.GrabIfStatusActive(character, primaryEffect) == null && character.elementType != ElementType.Grass)
            {
                ApplyStatusToCharacter(character, primaryEffect);
            }
        }
    }

    public override void ApplyTileEffect(Tile tile, TurnManager turnManager, WeatherPatch patch)
    {
        ElementType type = tile.tileData.tileType;

        if (type == ElementType.Water)
        {
            foreach (Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
            {
                if (adjTile.characterOnTile != null)
                {
                    ApplyStatusToCharacter(adjTile.characterOnTile, Status.StatusTypes.Bound);
                }
            }
        }
        else if (type == ElementType.Fire)
        {
            foreach (Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
            {
                if (adjTile.characterOnTile != null)
                {
                    ApplyStatusToCharacter(adjTile.characterOnTile, Status.StatusTypes.MovementReduction);
                }
            }
        }
        else if (type == ElementType.Grass)
        {
            List<Tile> adjTiles = turnManager.pathfinder.FindAdjacentTiles(tile, true);
            int choice = Random.Range(0, adjTiles.Count);
            Tile newTile = Instantiate(grassTilePrefab, adjTiles[choice].transform.position, Quaternion.identity);
            patch.TileReplaced(adjTiles[choice], newTile);
            adjTiles[choice].ReplaceTileWithNew(newTile);
        }
    }

    #endregion
}
