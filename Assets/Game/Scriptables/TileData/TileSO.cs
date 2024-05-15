using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
public class TileSO : ScriptableObject
{
    public float tileCost;
    public TileEnums.TileType tileType = TileEnums.TileType.lava;
    public bool walkable = true;
}
