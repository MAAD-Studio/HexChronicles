using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class WeatherIndicator : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private Button closeBtn;

    public void Initialize(WeatherManager weatherManager)
    {
        gameObject.SetActive(false);
        window.SetActive(false);

        EventBus.Instance.Subscribe<OnWeatherSpawn>(ShowWeather);
        EventBus.Instance.Subscribe<OnWeatherEnded>(EndWeather);

        closeBtn.onClick.AddListener(() => window.SetActive(false));
    }

    private void ShowWeather(object obj)
    {
        gameObject.SetActive(true);
        window.SetActive(true);
    }

    private void EndWeather(object obj)
    {
        gameObject.SetActive(false);
    }

    public void ResetWeather()
    {
        EventBus.Instance.Unsubscribe<OnWeatherSpawn>(ShowWeather);
        EventBus.Instance.Unsubscribe<OnWeatherEnded>(EndWeather);
    }
}
