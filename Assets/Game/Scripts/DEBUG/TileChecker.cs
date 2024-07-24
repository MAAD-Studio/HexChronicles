using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChecker : MonoBehaviour
{
    #region Variables

    [SerializeField] private LayerMask tileLayer;

    [Header("Weather Checking: ")]
    [SerializeField] private WeatherManager weatherManager;
    [SerializeField] private Material weatherMaterial;

    #endregion

    #region UnityMethods

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            CheckSpot();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            CheckSpotAndAdjacent();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            ExamineWeather();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            ChangeWeather();
        }
    }

    #endregion

    #region CustomMethods

    private Tile CheckSpot()
    {
        Debug.Log("------------------");
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, tileLayer))
        {
            Tile hitTile = hit.transform.GetComponent<Tile>();
            Debug.Log("Tile Name: " + hitTile.name);
            return hitTile;
        }
        else
        {
            Debug.Log("--- NO TILE DETECTED ---");
            return null;
        }
    }

    private void CheckSpotAndAdjacent()
    {
        CheckAdjacentTiles(CheckSpot());
    }

    private void CheckAdjacentTiles(Tile origin)
    {
        if (origin == null)
        {
            return;
        }

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
                TileObject tileObj = hitTile.objectOnTile;

                Debug.Log(hitTile.name);
            }
        }
    }

    private void ExamineWeather()
    {
        if(weatherManager == null)
        {
            Debug.Log("NO WEATHER MANAGER SET");
            return;
        }

        Tile tile = CheckSpot();
        if (tile == null)
        {
            return;
        }

        Debug.Log("Weather Status: " + tile.underWeatherAffect);
        foreach(WeatherPatch patch in weatherManager.WeatherPatches)
        {
            if(patch.EffectedTiles.Contains(tile))
            {
                Debug.Log("Weather is Effected?: true");
            }
        }
    }

    private void ChangeWeather()
    {
        if(weatherMaterial == null)
        {
            Debug.Log("NO WEATHER MATERIAL SET");
            return;
        }

        Tile tile = CheckSpot();
        if(tile == null)
        {
            return;
        }

        tile.ChangeTileWeather(true, weatherMaterial);
    }

    #endregion
}
