using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_TempObjectMaker : TileReporter
{
    #region Variables

    [SerializeField] protected TileObject objectToSpawn;

    #endregion

    #region UnityMethods

    public override void Start()
    {
        base.Start();

        Debug.Assert(objectToSpawn != null, "TileReporter_ObjectMaker on object " + name + " with parent " + transform.parent.name + " doesn't have an object to spawn.");
    }

    #endregion

    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        if (currentTile == null)
        {
            return;
        }

        if (currentTile.tileHasObject == false && currentTile.tileOccupied == false)
        {
            TurnManager turnManager = FindObjectOfType<TurnManager>();

            TileObject newObj = Instantiate(objectToSpawn, currentTile.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
            turnManager.temporaryTileObjects.Add(newObj);

            UndoManager.Instance.StoreTileObject(newObj, true);
        }
    }

    #endregion
}
