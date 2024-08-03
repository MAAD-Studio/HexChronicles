using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnInfo : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image weatherBar;

    [SerializeField] private GameObject textIndicator;
    [SerializeField] private TextMeshProUGUI turnText;

    [SerializeField] private Color overColor;
    [SerializeField] private Color currentColor;
    [SerializeField] private Color comingColor;
    [SerializeField] private Color rainColor;

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

    public void SetWeatherTurn()
    {
        isWeather = true;
        bar.color = rainColor;
    }

    public void EndTurn()
    {
        textIndicator.SetActive(false);
        bar.color = overColor;
        tip.HideTooltip();
        tip.enabled = false;

        if (isWeather)
        {
            weatherBar.gameObject.SetActive(false);
        }
    }
}
