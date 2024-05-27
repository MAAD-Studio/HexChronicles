using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
public class TileSO : ScriptableObject
{
    public float tileCost;
    public bool walkable = true;

    public ElementType tileType = new ElementType();
    public List<ElementType> elementsWeakAgainst = new List<ElementType>();
    public List<ElementType> elementsStrongAgainst = new List<ElementType>();

    public Sprite tileSprite;
    public string tileEffects;
}
