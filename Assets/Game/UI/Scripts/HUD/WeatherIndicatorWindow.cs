using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class WeatherIndicatorWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI weatherName;
    [SerializeField] private Button closeBtn;

    // Hide this window at start
    public void Start()
    {
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    public void ShowWeather(WeatherManager weatherManager)
    {
        weatherName.text = weatherManager.WeatherName;
        // also set text color here

        gameObject.SetActive(true);
    }
}
