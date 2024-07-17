using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_HeatWave : Weather_Base
{
    #region Variables

    [SerializeField] private int healthDebuff = 1;
    [SerializeField] private Tile fireTilePrefab;

    #endregion

    #region UnityMethods

    private void Start()
    {
        weatherName = "HeatWave";
    }

    #endregion

    #region CustomMethods

    public override void ApplyEffect(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            if (character.elementType == ElementType.Fire)
            {
                ApplyStatusToCharacter(character, secondaryEffect);
            }
            else if (character.elementType == ElementType.Grass)
            {
                ApplyStatusToCharacter(character, Status.StatusTypes.MovementReduction);
                character.TakeDamage(healthDebuff, ElementType.Base);
            }

            if (Status.GrabIfStatusActive(character, primaryEffect) == null && character.elementType != ElementType.Fire)
            {
                ApplyStatusToCharacter(character, primaryEffect);
            }
        }
    }

    public override void ApplyTileEffect(Tile tile, TurnManager turnManager)
    {
        ElementType type = tile.tileData.tileType;

        if (type == ElementType.Water)
        {
            foreach (Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
            {
                if (adjTile.characterOnTile != null)
                {
                    ApplyStatusToCharacter(adjTile.characterOnTile, Status.StatusTypes.MovementReduction);
                }
            }
        }
        else if (type == ElementType.Fire)
        {
            foreach (Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
            {
                Tile newTile = Instantiate(fireTilePrefab, adjTile.transform.position, Quaternion.identity);
                adjTile.ReplaceTileWithNew(newTile);
            }
        }
        else if (type == ElementType.Grass)
        {
            
        }
    }

    #endregion
}
