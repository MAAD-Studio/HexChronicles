using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherPatch
{
    #region Variables

    private List<Tile> effectedTiles = new List<Tile>();
    private List<Character> effectedCharacters = new List<Character>();

    private Tile origin;
    private Weather_Base weather;

    private bool entireMapEffected = false;
    private int maxSpread = 1;
    private int movementPerTurn = 1;

    #endregion

    #region CustomMethods

    public void SetWeatherPatchInfo(Tile tile, bool effectEntireMap, int spread, int movement, Weather_Base weatherType)
    {
        origin = tile;

        entireMapEffected = effectEntireMap;
        maxSpread = spread;
        movementPerTurn = movement;
        weather = weatherType;

        DetermineAreaOfAffect();
        IllustrateAreaOfEffect();
    }

    //Determines the Tiles the Weather will effect
    private void DetermineAreaOfAffect()
    {
        ResetWeatherTiles();

        Queue<Tile> openTiles = new Queue<Tile>();
        openTiles.Enqueue(origin);
        effectedTiles.Add(origin);

        origin.weatherCost = 0f;

        while(openTiles.Count > 0)
        {
            Tile currentTile = openTiles.Dequeue();

            foreach(Tile adjacentTile in Pathfinder.Instance.FindAdjacentTiles(currentTile, true))
            {
                if(openTiles.Contains(adjacentTile) || effectedTiles.Contains(adjacentTile) || adjacentTile.underWeatherAffect)
                {
                    continue;
                }

                //Determines how much the tile should cost
                float newCost = 0f;
                if(!entireMapEffected)
                {
                    if(maxSpread > 2)
                    {
                        newCost = currentTile.weatherCost + Random.Range(1, maxSpread - 1);
                    }
                    else
                    {
                        newCost = currentTile.weatherCost + Random.Range(1, maxSpread);
                    }
                }
                adjacentTile.weatherCost = newCost;

                if(adjacentTile.weatherCost < maxSpread)
                {
                    openTiles.Enqueue(adjacentTile);
                    effectedTiles.Add(adjacentTile);
                    adjacentTile.underWeatherAffect = true;
                }
            }
        }
    }

    //Moves the Weather patch to a new location
    private void MoveOrigin()
    {
        List<Tile> moveableTiles = new List<Tile>();
        foreach(Tile tile in effectedTiles)
        {
            if(tile.weatherCost < movementPerTurn)
            {
                moveableTiles.Add(tile);
            }
        }

        if(moveableTiles.Count > 0)
        {
            int tileChoice = Random.Range(0, moveableTiles.Count);
            origin = moveableTiles[tileChoice];
        }
    }

    private void IllustrateAreaOfEffect()
    {
        foreach (Tile tile in effectedTiles)
        {
            tile.ChangeTileWeather(true, weather.weatherMaterial);
        }
    }

    public void UpdateAndEffect(TurnManager turnManager)
    {
        ApplyWeatherEffect(turnManager);
        MoveOrigin();
        DetermineAreaOfAffect();
        IllustrateAreaOfEffect();
    }

    private void ApplyWeatherEffect(TurnManager turnManager)
    {
        List<Tile> potentialEffectTiles = new List<Tile>();
        foreach(Tile tile in effectedTiles)
        {
            //Grabs the characters in the area of effect for the Weather
            if(tile.tileOccupied && !tile.characterOnTile.effectedByWeather)
            {
                effectedCharacters.Add(tile.characterOnTile);
                tile.characterOnTile.effectedByWeather = true;
            }

            //Grabs the elemental tiles in the area of effect for the Weather
            if(tile.tileData.tileType != ElementType.Base)
            {
                potentialEffectTiles.Add(tile);
            }
        }

        weather.ApplyEffect(effectedCharacters);
        //If any elemental tiles are in the area of effect it attempts to apply its effects a randomly selected one
        if(potentialEffectTiles.Count > 0)
        {
            int tileChoice = Random.Range(0, potentialEffectTiles.Count);
            weather.ApplyTileEffect(potentialEffectTiles[tileChoice], turnManager);
        }

    }

    public void ResetWeatherTiles()
    {
        foreach (Tile tile in effectedTiles)
        {
            tile.underWeatherAffect = false;
            tile.ChangeTileWeather(false, null);
        }

        foreach (Character character in effectedCharacters)
        {
            character.effectedByWeather = false;
        }

        effectedTiles.Clear();
        effectedCharacters.Clear();
    }

    public void TileReplaced(Tile oldTile, Tile newTile)
    {
        if(effectedTiles.Remove(oldTile))
        {
            effectedTiles.Add(newTile);
        }
    }

    #endregion
}
