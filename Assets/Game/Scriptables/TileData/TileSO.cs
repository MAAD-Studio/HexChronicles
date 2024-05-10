using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData")]
public class TileSO : ScriptableObject
{
    public enum TileType
    {
        neutral,
        lava,
        water,
        snow,
        mountain,
        swamp
    }

    public float tileCost;
    public TileType tileType = TileType.lava;
    public bool walkable = true;
}
