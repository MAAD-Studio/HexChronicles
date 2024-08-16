using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_VFXSpawner : TileReporter
{
    #region Variables

    [SerializeField] protected GameObject VFX;

    #endregion

    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        if(currentTile == null)
        {
            return;
        }

        Destroy(Instantiate(VFX, currentTile.transform.position, Quaternion.identity), 2f);
    }

    #endregion
}
