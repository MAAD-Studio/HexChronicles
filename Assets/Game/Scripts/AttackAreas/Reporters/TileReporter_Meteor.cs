using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_Meteor : TileReporter
{
    #region Variables

    [Header("Meteor Controls: ")]
    [SerializeField] private Meteor meteorObject;
    [SerializeField] private float spawnHeight;

    #endregion

    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        if (meteorObject == null)
        {
            Debug.Log("Meteor Tile Reporter not provided a meteor to spawn");
            return;
        }
        else if (currentTile == null)
        {
            return;
        }

        Meteor meteor = Instantiate(meteorObject, currentTile.transform.position + new Vector3(0, spawnHeight, 0), Quaternion.identity);
        meteor.tileToEffect = currentTile;
    }

    #endregion
}
