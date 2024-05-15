using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
{
    #region Variables

    [HideInInspector] public List<Tile> affectedTiles = new List<Tile>();

    #endregion

    public void ResetTiles()
    {
        foreach(Tile tile in affectedTiles)
        {
            tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            tile.inFrontier = false;
        }
    }

    public static GameObject SpawnSelf(ActiveSkill activeSkill)
    {
        return Instantiate(activeSkill.gameObject, new Vector3(100, 100, 100), Quaternion.identity);
    }

    public void DestroySelf()
    {
        ResetTiles();
        Destroy(this.gameObject);
    }
}
