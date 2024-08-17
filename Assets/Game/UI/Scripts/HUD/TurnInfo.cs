using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnInfo : MonoBehaviour
{
    [SerializeField] private Image bar;

    [SerializeField] private GameObject textIndicator;
    [SerializeField] private TextMeshProUGUI turnText;

    [SerializeField] private Color overColor;
    [SerializeField] private Color currentColor;
    [SerializeField] private Color comingColor;

    [Header("Info based on Weather")]
    [SerializeField] private Image weatherBar;
    [SerializeField] private Image weatherIcon;
    [SerializeField] private TextMeshProUGUI weatherTitle;
    [SerializeField] private TextMeshProUGUI weatherExplanation;

    [Header("Weather Specific")]
    [SerializeField] private Color rainColor;
    [TextArea(3, 10)]
    [SerializeField] private string rainExplanation;

    [SerializeField] private Color heatWaveColor;
    [TextArea(3, 10)]
    [SerializeField] private string heatWaveExplanation;

    [SerializeField] private Color sporeStormColor;
    [TextArea(3, 10)]
    [SerializeField] private string sporeStormExplanation;

    [HideInInspector] public bool isWeather = false;
    private UITip tip;

    public void Initialize(int turn, int totalTurns)
    {
        bar.color = comingColor;
        turnText.text = $"Turn {turn}/{totalTurns}";

        textIndicator.SetActive(false);
        weatherBar.gameObject.SetActive(false);

        tip = GetComponent<UITip>();
        tip.enabled = false;
    }

    public void CurrentTurn()
    {
        textIndicator.SetActive(true);
        bar.color = currentColor;

        if (isWeather)
        {
            weatherBar.gameObject.SetActive(true);
            tip.enabled = true;
        }
    }

    public void SetWeatherTurn(WeatherType weatherType)
    {
        isWeather = true;

        weatherIcon.sprite = Config.Instance.GetWeatherSprite(weatherType);
        switch (weatherType)
        {
            case WeatherType.rain:
                bar.color = rainColor;
                weatherBar.color = rainColor;
                weatherTitle.text = "Raining Now";
                weatherExplanation.text = rainExplanation;
                break;
            case WeatherType.sporeStorm:
                bar.color = sporeStormColor;
                weatherBar.color = sporeStormColor;
                weatherTitle.text = "Spore Storm";
                weatherExplanation.text = sporeStormExplanation;
                break;
            case WeatherType.heatWave:
                bar.color = heatWaveColor;
                weatherBar.color = heatWaveColor;
                weatherTitle.text = "Heat Wave";
                weatherExplanation.text = heatWaveExplanation;
                break;
        }
    }

    public void EndTurn()
    {
        textIndicator.SetActive(false);
        bar.color = overColor;
        tip.rectTransform.gameObject.SetActive(false);
        tip.enabled = false;

        if (isWeather)
        {
            weatherBar.gameObject.SetActive(false);
        }
    }
}
