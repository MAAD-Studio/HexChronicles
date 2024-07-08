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
        Tile newTile = Instantiate(conversionTile, currentTile.transform.position, Quaternion.identity);
        currentTile.ReplaceTileWithNew(newTile);
    }

    #endregion
}
