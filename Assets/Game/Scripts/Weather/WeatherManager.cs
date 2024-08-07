using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    #region Variables

    [Header("Setup Info:")]
    [SerializeField] private GameObject gridParent;

    private List<Tile> tilesOnMap = new List<Tile>();
    private List<WeatherPatch> weatherPatches = new List<WeatherPatch>();
    public List<WeatherPatch> WeatherPatches
    {
        get { return weatherPatches; }
    }

    private bool weatherActive = false;

    [Header("Weather Spawn Controls: ")]
    [Range(0,100)]
    [SerializeField] private int weatherChance = 25;
    [SerializeField] private int numberOfPatches = 3;
    [SerializeField] private int turnsToStay = 3;
    public int TurnsToStay
    {
        get { return turnsToStay; }
    }
    private int turnsActive = 0;

    [Header("Weather Generation Controls: ")]
    [SerializeField] private bool effectEntireMap = false;
    [SerializeField] private int maxSpread = 4;
    [SerializeField] private int movementPerTurn = 2;

    [Header("Weather to Create: ")]
    [SerializeField] private Weather_Base weather;
    public WeatherType WeatherType
    {
        get { return weather.WeatherType; }
    }

    private TurnManager turnManager;

    //Tutorial
    public bool isTutorial = false;

    #endregion

    #region UnityMethods

    void Start()
    {
        //Creates a list of all the tiles on the map for future reference
        foreach (Tile tile in gridParent.GetComponentsInChildren<Tile>())
        {
            tilesOnMap.Add(tile);
        }

        for(int i = 0; i < numberOfPatches; i++)
        {
            WeatherPatch newPatch = new WeatherPatch();
            weatherPatches.Add(newPatch);
        }

        Debug.Assert(weather != null, "WeatherManager doesn't have a weather affect to use");
        Tile.tileReplaced.AddListener(TileReplaced);

        turnManager = FindObjectOfType<TurnManager>();
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
                patch.SetWeatherPatchInfo(tilesOnMap[tileChoice], effectEntireMap, maxSpread, movementPerTurn, weather);
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
                StartCoroutine(patch.UpdateAndEffect(turnManager));
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

            foreach(Tile tile in tilesOnMap)
            {
                if(tile.WeatherActive)
                {
                    tile.ChangeTileWeather(false, WeatherType.none);
                }
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
        tilesOnMap.Remove(oldTile);
        tilesOnMap.Add(newTile);

        foreach(WeatherPatch patch in weatherPatches)
        {
            patch.TileReplaced(oldTile, newTile);
        }
    }

    public ElementType GetWeatherElementType()
    {
        return weather.Element;
    }

    #endregion

}
