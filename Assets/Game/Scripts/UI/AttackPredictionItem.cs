using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackPredictionItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI predictionText;
    [SerializeField] private TextMeshProUGUI predictionValue;

    public void SetData(Sprite sprite, string text, string value)
    {
        icon.sprite = sprite;
        predictionText.text = text;
        predictionValue.text = value;
    }
}
