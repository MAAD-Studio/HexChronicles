using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Rain : Weather_Base
{
    #region Variables

    [SerializeField] private Tile waterTilePrefab;

    #endregion

    #region UnityMethods

    private void Start()
    {
        weatherName = "Rain";

        Debug.Assert(waterTilePrefab != null, "Rain Weather hasn't been provided a Water Tile Prefab");
    }

    #endregion

    #region CustomMethods

    public override void ApplyEffect(List<Character> characters)
    {
        foreach(Character character in characters)
        {
            if(character.elementType == ElementType.Water)
            {
                character.Heal(healAffect);
            }
            else if(character.elementType == ElementType.Fire)
            {
                character.TakeDamage(damageAffect, ElementType.Base);
            }

            if (Status.GrabIfStatusActive(character, primaryEffect) == null && character.elementType != ElementType.Water)
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
                Tile newTile = Instantiate(waterTilePrefab, adjTile.transform.position, Quaternion.identity);
                patch.TileReplaced(adjTile, newTile);
                adjTile.ReplaceTileWithNew(newTile);
            }
        }
        else if(type == ElementType.Fire)
        {
            
        }
        else if(type == ElementType.Grass)
        {
            foreach (Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
            {
                if (adjTile.characterOnTile != null)
                {
                    ApplyStatusToCharacter(adjTile.characterOnTile, Status.StatusTypes.Bound);
                }
            }
        }
    }

    #endregion
}
