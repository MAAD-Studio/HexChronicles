using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeatherPatch
{
    #region Variables

    public List<Tile> tilesUnderAffect = new List<Tile>();
    public List<Tile> tilesToIgnore = new List<Tile>();
    public List<Tile> tilesMoveable = new List<Tile>();
    public List<Character> effectedCharacters = new List<Character>();

    public Tile origin;

    public bool spawned = false;

    public Weather_Base weather;

    #endregion

    #region CustomMethods

    public void DetermineAreaOfAffect(bool entireMapEffected, int maxSpread, int movementPerTurn, LayerMask tileLayer)
    {
        ResetWeatherTiles();

        //Grabs and sets the origin tile
        Queue<Tile> openTiles = new Queue<Tile>();
        openTiles.Enqueue(origin);
        tilesUnderAffect.Add(origin);

        origin.weatherCost = 0f;

        //While we have tiles to investigate
        while (openTiles.Count > 0)
        {
            Tile currentTile = openTiles.Dequeue();

            //Checks every adjacent tile to the current tile we are investigating
            foreach (Tile adjacentTile in FindAdjacentTiles(currentTile, tileLayer))
            {
                float newCost;

                if (!entireMapEffected)
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
                else
                {
                    newCost = 0f;
                }

                if (openTiles.Contains(adjacentTile) || tilesToIgnore.Contains(adjacentTile))
                {
                    continue;
                }

                adjacentTile.weatherCost = newCost;

                //Checks if the character can travel to the adjacent tile, if they can it adds its data into the list to investigate
                if (adjacentTile.weatherCost < maxSpread)
                {
                    adjacentTile.parentTile = currentTile;
                    openTiles.Enqueue(adjacentTile);
                    tilesUnderAffect.Add(adjacentTile);
                    if (adjacentTile.weatherCost <= movementPerTurn)
                    {
                        tilesMoveable.Add(adjacentTile);
                    }
                    tilesToIgnore.Add(adjacentTile);
                }
                else
                {
                    tilesToIgnore.Add(adjacentTile);
                }
            }
        }
    }

    public List<Tile> FindAdjacentTiles(Tile origin, LayerMask tileLayer)
    {
        List<Tile> adjacentTiles = new List<Tile>();

        Vector3 direction = Vector3.forward;
        float rayLength = 50f;
        float rayHeightOffset = 1f;

        //Checks in all 6 direction for an adjacent tile
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = origin.transform.position + direction;
            aboveTilePos.y += rayHeightOffset;

            //If we hit a tile we add it to the list of adjacents
            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileLayer))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                adjacentTiles.Add(hitTile);
            }
        }

        return adjacentTiles;
    }

    public void ResetWeatherTiles()
    {
        foreach (Tile tile in tilesUnderAffect)
        {
            tile.underWeatherAffect = false;
            tile.ChangeTileWeather(TileEnums.TileWeather.disabled);
        }

        tilesUnderAffect.Clear();
        tilesToIgnore.Clear();
        tilesMoveable.Clear();

        foreach(Character character in effectedCharacters)
        {
            character.effectedByWeather = false;
        }
        effectedCharacters.Clear();
    }

    public void EffectCharacters()
    {
        foreach (Tile tile in tilesUnderAffect)
        {
            if (tile.tileOccupied && !tile.characterOnTile.effectedByWeather)
            {
                effectedCharacters.Add(tile.characterOnTile);
            }
        }

        weather.ApplyEffect(effectedCharacters);
    }

    public void MoveOrigin()
    {
        if (tilesMoveable.Count > 0)
        {
            int tileChoice = Random.Range(0, tilesMoveable.Count);
            origin = tilesMoveable[tileChoice];
        }
    }

    public void ColourArea()
    {
        foreach(Tile tile in tilesUnderAffect)
        {
            tile.ChangeTileWeather(TileEnums.TileWeather.rain);
        }
    }

    public void TileReplaced(Tile oldTile, Tile newTile)
    {
        tilesUnderAffect.Remove(oldTile);
        tilesUnderAffect.Add(newTile);

        tilesMoveable.Remove(oldTile);
        tilesMoveable.Add(newTile);

        tilesToIgnore.Remove(oldTile);
        tilesToIgnore.Add(newTile);
    }

    #endregion
}
