using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class WeatherIndicatorWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weatherName;
    [SerializeField] private TextMeshProUGUI weatherInfo;
    [SerializeField] private Button closeBtn;

    [Header("Rain")]
    [SerializeField] private Color rainColor;
    [SerializeField] private KeywordDescription rainInfo;

    [Header("Spore Storm")]
    [SerializeField] private Color sporeStormColor;
    [SerializeField] private KeywordDescription sporeStormInfo;

    [Header("Heat Wave")]
    [SerializeField] private Color heatWaveColor;
    [SerializeField] private KeywordDescription heatWaveInfo;

    // Hide this window at start
    public void Start()
    {
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    public void ShowWeather(WeatherType weatherType)
    {
        weatherName.text = weatherType.ToString();

        switch (weatherType)
        {
            case WeatherType.rain:
                weatherName.color = rainColor;
                weatherInfo.text = rainInfo.DisplayKeywordDescription();
                break;
            case WeatherType.sporeStorm:
                weatherName.color = sporeStormColor;
                weatherInfo.text = sporeStormInfo.DisplayKeywordDescription();
                break;
            case WeatherType.heatWave:
                weatherName.color = heatWaveColor;
                weatherInfo.text = heatWaveInfo.DisplayKeywordDescription();
                break;
            default:
                weatherInfo.text = "No weather info available";
                break;
        }

        gameObject.SetActive(true);
    }
}
