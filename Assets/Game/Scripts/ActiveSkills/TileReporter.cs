using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter : MonoBehaviour
{
    #region Variables

    [HideInInspector] public Tile currentTile;

    #endregion

    #region UnityMethods

    private void OnTriggerStay(Collider other)
    {
        Tile newTile = other.GetComponent<Tile>();
        if(newTile != null)
        {
            currentTile = newTile;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentTile = null;
    }

    #endregion
}
