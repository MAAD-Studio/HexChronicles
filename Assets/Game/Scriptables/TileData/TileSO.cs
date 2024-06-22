using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
public class TileSO : ScriptableObject
{
    public float tileCost;
    public bool walkable = true;

    public ElementType tileType = new ElementType();

    [Header("Elements Debuffed by Tile: ")]
    public List<ElementType> elementsWeakAgainst = new List<ElementType>();

    [Header("Elements Buffed by Tile: ")]
    public List<ElementType> elementsStrongAgainst = new List<ElementType>();

    [Header("Tile Materials:")]
    public Material baseMaterial;
    public Material highlightMaterial;
    public Material reachableMaterial;
    public Material attackableMaterial;
    public Material pathMaterial;
    public Material selectedCharMaterial;

    public Material towerAttackMaterial;
    public Material weatherMaterial;

    public Sprite tileSprite;
    public KeywordDescription tileEffects;
}
