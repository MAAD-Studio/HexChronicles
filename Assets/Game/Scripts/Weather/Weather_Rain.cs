using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather_Rain : Weather_Base
{
    #region Variables

    [SerializeField] private int healthBoost = 1;
    [SerializeField] private int healthDebuff = 1;
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
                character.currentHealth += healthBoost;
                character.currentHealth = Mathf.Min(character.currentHealth, character.maxHealth);
                character.UpdateHealthBar?.Invoke();
                continue;
            }
            else if(character.elementType == ElementType.Fire)
            {
                character.TakeDamage(healthDebuff, ElementType.Base);
            }

            if (Status.GrabIfStatusActive(character, statusEffect) == null)
            {
                Status newStatus = new Status();
                newStatus.statusType = statusEffect;
                newStatus.effectTurns = effectTurns;

                character.AddStatus(newStatus);
            }

            character.effectedByWeather = true;
        }
    }

    public override void ApplyTileEffect(Tile tile, TurnManager turnManager)
    {
        /*Debug.Log("MAKING WATER TILES");
        foreach(Tile adjTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
        {
            Tile newTile = Instantiate(waterTilePrefab);
            adjTile.ReplaceTileWithNew(newTile);
        }*/
    }

    #endregion
}
