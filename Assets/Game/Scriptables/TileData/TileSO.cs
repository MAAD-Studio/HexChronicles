using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
public class TileSO : ScriptableObject
{
    public float tileCost;
    public TileEnums.TileType tileType = TileEnums.TileType.lava;
    public bool walkable = true;

    public List<ElementType> buffTypes = new List<ElementType>();
    public List<ElementType> debuffTypes = new List<ElementType>();
}
