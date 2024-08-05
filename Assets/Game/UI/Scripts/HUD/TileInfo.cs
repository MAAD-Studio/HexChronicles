using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileInfo : MonoBehaviour
{
    public Image tileImage;
    public Image tileElement;
    public TextMeshProUGUI tileName;
    public TextMeshProUGUI tileEffects;

    public void SetTileInfo(Tile tile)
    {
        tileImage.sprite = tile.tileData.tileSprite;
        tileElement.sprite = Config.Instance.GetElementSprite(tile.tileData.tileType);
        if (tileElement.sprite == null) { tileElement.gameObject.SetActive(false); }
        else { tileElement.gameObject.SetActive(true); }
        tileName.text = tile.tileData.name;
        tileEffects.text = tile.tileData.tileEffects.DisplayKeywordDescription();
        tileEffects.ForceMeshUpdate();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
