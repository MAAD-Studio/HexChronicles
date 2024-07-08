using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter : MonoBehaviour
{
    #region Variables

    [HideInInspector] public Tile currentTile;

    public List<TileReporter> children = new List<TileReporter>();

    #endregion

    #region UnityMethods

    public virtual void Start()
    {

    }

    public void CheckBlockages(bool parentNull)
    {
        bool tileNull = false;
        if(parentNull)
        {
            currentTile = null;
            tileNull = true;
        }
        else if(currentTile == null)
        {
            tileNull = true;
        }

        foreach (TileReporter tile in children)
        {
            tile.CheckBlockages(tileNull);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Tile newTile = other.GetComponent<Tile>();
        currentTile = newTile;

        if(currentTile != null && currentTile.tileHasObject && currentTile.objectOnTile.objectType == ObjectType.General)
        {
            currentTile = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentTile = null;
    }

    public virtual void ExecuteAddOnEffect()
    {
        
    }

    #endregion
}
