using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileEffect : MonoBehaviour
{
    public GameObject panel;
    public Image icon;
    public TextMeshProUGUI text;

    [SerializeField] private Sprite buffSprite;
    [SerializeField] private Sprite debuffSprite;

    public void SetEffect(ElementType characterType, ElementType tileType)
    {
        if (tileType == ElementType.Base)
        {
            panel.SetActive(false);
        }
        else if (characterType == tileType)
        {
            icon.sprite = buffSprite;
            text.text = "Character can get a Buff here";
            panel.SetActive(true);
        }
        else
        {
            icon.sprite = debuffSprite;
            text.text = "Character can get a Debuff here";
            panel.SetActive(true);
        }
    }
}
