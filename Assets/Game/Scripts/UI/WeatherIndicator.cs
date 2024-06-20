using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeatherIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Initialize(WeatherManager weatherManager)
    {
        gameObject.SetActive(false);
    }
}
