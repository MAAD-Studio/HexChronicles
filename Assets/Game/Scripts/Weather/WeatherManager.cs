using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    #region Variables

    [Header("Setup Info:")]
    [SerializeField] private GameObject gridParent;
    private List<Tile> tilesOnMap = new List<Tile>();

    [SerializeField] private LayerMask tileLayer;

    private List<WeatherPatch> weatherPatches = new List<WeatherPatch>();

    private bool weatherActive = false;

    [Header("Weather Spawn Controls: ")]
    [Range(0,100)]
    [SerializeField] private int weatherChance = 25;
    [SerializeField] private int numberOfPatches = 3;
    [SerializeField] private int turnsToStay = 3;
    private int turnsActive = 0;

    [Header("Weather Generation Controls: ")]
    [SerializeField] private bool effectEntireMap = false;
    [SerializeField] private int maxSpread = 4;
    [SerializeField] private int movementPerTurn = 2;
    [SerializeField] private Weather_Base weather;

    #endregion

    #region UnityMethods

    void Start()
    {
        foreach (Tile tile in gridParent.GetComponentsInChildren<Tile>())
        {
            tilesOnMap.Add(tile);
        }

        for(int i = 0; i < numberOfPatches; i++)
        {
            WeatherPatch newPatch = new WeatherPatch();
            newPatch.weather = weather;
            weatherPatches.Add(newPatch);
            
        }

        Debug.Assert(weather != null, "WeatherManager doesn't have a weather affect to use");
        Tile.tileReplaced.AddListener(TileReplaced);
    } 

    #endregion

    #region CustomMethods

    public void AttemptWeatherSpawn()
    {
        int result = Random.Range(0, 100);
        if(result <= weatherChance)
        {
            weatherActive = true;

            foreach(WeatherPatch patch in weatherPatches)
            {
                int tileChoice = Random.Range(0, tilesOnMap.Count);
                patch.origin = tilesOnMap[tileChoice];
                patch.DetermineAreaOfAffect(effectEntireMap, maxSpread, movementPerTurn, tileLayer);
                patch.ColourArea();
            }

            EventBus.Instance.Publish(new OnWeatherSpawn());
        }
        else
        {
            Debug.Log("FAILED TO SPAWN");
        }
    }

    public void UpdateWeather()
    {
        if(!weatherActive)
        {
            AttemptWeatherSpawn();
        }
        else if(turnsActive < turnsToStay)
        {
            foreach(WeatherPatch patch in weatherPatches)
            {
                patch.EffectCharacters();
                patch.MoveOrigin();
                patch.DetermineAreaOfAffect(effectEntireMap, maxSpread, movementPerTurn, tileLayer);
                patch.ColourArea();
            }
            turnsActive++;
        }
        else
        {
            turnsActive = 0;
            weatherActive = false;

            foreach(WeatherPatch patch in weatherPatches)
            {
                patch.ResetWeatherTiles();
            }

            EventBus.Instance.Publish(new OnWeatherEnded());
        }
    }

    public void FullReset()
    {
        turnsActive = 0;
        foreach (WeatherPatch patch in weatherPatches)
        {
            patch.ResetWeatherTiles();
        }
    }

    private void TileReplaced(Tile oldTile, Tile newTile)
    {
        foreach(WeatherPatch patch in weatherPatches)
        {
            patch.TileReplaced(oldTile, newTile);
        }
    }

    #endregion

}
