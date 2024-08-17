using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TileEnums;

public class TileInfo : MonoBehaviour
{
    public Image tileImage;
    public Image tileElement;
    public TextMeshProUGUI tileName;
    public TextMeshProUGUI tileEffects;
    private RectTransform rectTransform;

    [Header("Weather Info")]
    [SerializeField] private GameObject weatherPanel;
    [SerializeField] private Image weatherIcon;
    [SerializeField] private TextMeshProUGUI weatherTitle;
    [SerializeField] private TextMeshProUGUI weatherExplanation;

    [Header("Rain Specific")]
    [TextArea(2, 10)]
    [SerializeField] private string rainOnFireTile;
    [TextArea(2, 10)]
    [SerializeField] private string rainOnGrassTile; 
    [TextArea(2, 10)]
    [SerializeField] private string rainOnWaterTile;

    [Header("Heat Wave Specific")]
    [TextArea(2, 10)]
    [SerializeField] private string heatWaveOnFireTile;
    [TextArea(2, 10)]
    [SerializeField] private string heatWaveOnGrassTile;
    [TextArea(2, 10)]
    [SerializeField] private string heatWaveOnWaterTile;

    [Header("Spore Storm Specific")]
    [TextArea(2, 10)]
    [SerializeField] private string sporeStormOnFireTile;
    [TextArea(2, 10)]
    [SerializeField] private string sporeStormOnGrassTile;
    [TextArea(2, 10)]
    [SerializeField] private string sporeStormOnWaterTile;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetTileInfo(Tile tile)
    {
        tileImage.sprite = tile.tileData.tileSprite;
        tileElement.sprite = Config.Instance.GetElementSprite(tile.tileData.tileType);
        if (tileElement.sprite == null) { tileElement.gameObject.SetActive(false); }
        else { tileElement.gameObject.SetActive(true); }
        tileName.text = tile.tileData.name;
        tileEffects.text = tile.tileData.tileEffects.DisplayKeywordDescription();
        tileEffects.ForceMeshUpdate();

        gameObject.SetActive(true);

        if (tile.underWeatherAffect)
        {
            SetWeatherInfo(tile.weatherOnTile, tile.tileData.tileType);
        }
        else
        {
            weatherPanel.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private void SetWeatherInfo(WeatherType weatherOnTile, ElementType tileType)
    {
        weatherPanel.SetActive(true);
        weatherIcon.sprite = Config.Instance.GetWeatherSprite(weatherOnTile);
        switch (weatherOnTile)
        {
            case WeatherType.rain:
                weatherTitle.text = "Raining Here";
                if (tileType == ElementType.Fire)
                {
                    weatherExplanation.text = rainOnFireTile;
                }
                else if (tileType == ElementType.Grass)
                {
                    weatherExplanation.text = rainOnGrassTile;
                }
                else if (tileType == ElementType.Water)
                {
                    weatherExplanation.text = rainOnWaterTile;
                }
                else
                {
                    weatherExplanation.text = "";
                }
                break;
            case WeatherType.sporeStorm:
                weatherTitle.text = "Spore Storm Here";
                if (tileType == ElementType.Fire)
                {
                    weatherExplanation.text = sporeStormOnFireTile;
                }
                else if (tileType == ElementType.Grass)
                {
                    weatherExplanation.text = sporeStormOnGrassTile;
                }
                else if (tileType == ElementType.Water)
                {
                    weatherExplanation.text = sporeStormOnWaterTile;
                }
                else
                {
                    weatherExplanation.text = "";
                }
                break;
            case WeatherType.heatWave:
                weatherTitle.text = "Heat Wave Here";
                if (tileType == ElementType.Fire)
                {
                    weatherExplanation.text = heatWaveOnFireTile;
                }
                else if (tileType == ElementType.Grass)
                {
                    weatherExplanation.text = heatWaveOnGrassTile;
                }
                else if (tileType == ElementType.Water)
                {
                    weatherExplanation.text = heatWaveOnWaterTile;
                }
                else
                {
                    weatherExplanation.text = "";
                }
                break;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
