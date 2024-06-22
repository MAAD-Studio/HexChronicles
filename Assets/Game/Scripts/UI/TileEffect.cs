using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileEffect : MonoBehaviour
{
    public GameObject panel;
    public Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI detail;

    [SerializeField] private Sprite buffSprite;
    [SerializeField] private Sprite debuffSprite;

    [Header("Tile Buff/Debuff")]
    [SerializeField] private KeywordDescription waterTileBuff;
    [SerializeField] private KeywordDescription waterTileDebuff;
    [SerializeField] private KeywordDescription fireTileBuff;
    [SerializeField] private KeywordDescription fireTileDebuff;
    [SerializeField] private KeywordDescription grassTileBuff;
    [SerializeField] private KeywordDescription grassTileDebuff;
    [SerializeField] private KeywordDescription poisonTileDebuff;


    public void SetEffect(ElementType characterType, ElementType tileType)
    {
        if (tileType == ElementType.Base)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);

            switch (tileType)
            {
                case ElementType.Water:
                    SetWaterEffect(characterType);
                    break;
                case ElementType.Fire:
                    SetFireEffect(characterType);
                    break;
                case ElementType.Grass:
                    SetGrassEffect(characterType);
                    break;
                case ElementType.Poison:
                    SetPoisonEffect();
                    break;
            }

            detail.ForceMeshUpdate();
        }
    }

    private void SetPoisonEffect()
    {
        icon.sprite = debuffSprite;
        text.text = "Poison";
        detail.text = poisonTileDebuff.DisplayKeywordDescription();
    }

    private void SetWaterEffect(ElementType characterType)
    {
        if (characterType == ElementType.Water)
        {
            icon.sprite = buffSprite;
            text.text = "Buff";
            detail.text = waterTileBuff.DisplayKeywordDescription();
        }
        else
        {
            icon.sprite = debuffSprite;
            text.text = "Debuff";
            detail.text = waterTileDebuff.DisplayKeywordDescription();
        }
    }

    private void SetFireEffect(ElementType characterType)
    {
        if (characterType == ElementType.Fire)
        {
            icon.sprite = buffSprite;
            text.text = "Buff";
            detail.text = fireTileBuff.DisplayKeywordDescription();
        }
        else
        {
            icon.sprite = debuffSprite;
            text.text = "Debuff";
            detail.text = fireTileDebuff.DisplayKeywordDescription();
        }
    }

    private void SetGrassEffect(ElementType characterType)
    {
        if (characterType == ElementType.Grass)
        {
            icon.sprite = buffSprite;
            text.text = "Buff";
            detail.text = grassTileBuff.DisplayKeywordDescription();
        }
        else
        {
            icon.sprite = debuffSprite;
            text.text = "Debuff";
            detail.text = grassTileDebuff.DisplayKeywordDescription();
        }
    }
}
