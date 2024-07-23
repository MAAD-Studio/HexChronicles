using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_Converter : TileReporter
{
    #region Variables

    [SerializeField] private Tile conversionTile;

    #endregion

    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        if(currentTile == null)
        {
            return;
        }

        Tile newTile = Instantiate(conversionTile, currentTile.transform.position, Quaternion.identity);
        UndoManager.Instance.StoreTile(newTile, currentTile.tileData.tileType);
        currentTile.ReplaceTileWithNew(newTile);
    }

    #endregion
}
