using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Weather Specific")]
    [TextArea(3, 10)]
    [SerializeField] private string rainExplanation;
    [TextArea(3, 10)]
    [SerializeField] private string heatWaveExplanation;
    [TextArea(3, 10)]
    [SerializeField] private string sporeStormExplanation;

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
            SetWeatherInfo(tile.weatherOnTile);
        }
        else
        {
            weatherPanel.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    private void SetWeatherInfo(WeatherType weatherOnTile)
    {
        weatherPanel.SetActive(true);
        weatherIcon.sprite = Config.Instance.GetWeatherSprite(weatherOnTile);
        switch (weatherOnTile)
        {
            case WeatherType.rain:
                weatherTitle.text = "Raining Here";
                weatherExplanation.text = rainExplanation;
                break;
            case WeatherType.sporeStorm:
                weatherTitle.text = "Spore Storm Here";
                weatherExplanation.text = sporeStormExplanation;
                break;
            case WeatherType.heatWave:
                weatherTitle.text = "Heat Wave Here";
                weatherExplanation.text = heatWaveExplanation;
                break;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
