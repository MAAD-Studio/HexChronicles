using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI effectTurns;
    [SerializeField] private TextMeshProUGUI effectDetail;
    [SerializeField] private CharacterUIConfig characterUIConfig;

    public void Initialize(Status status)
    {
        icon.sprite = characterUIConfig.GetStatusSprite(status);
        effectTurns.text = status.effectTurns.ToString();
        effectDetail.text = characterUIConfig.GetStatusExplain(status);
    }
}
