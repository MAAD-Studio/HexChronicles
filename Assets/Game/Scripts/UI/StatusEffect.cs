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

    public void Initialize(Status status)
    {
        transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions

        icon.sprite = Config.Instance.GetStatusSprite(status);
        effectTurns.text = status.effectTurns.ToString();
        effectDetail.text = Config.Instance.GetStatusExplain(status);
    }
}
