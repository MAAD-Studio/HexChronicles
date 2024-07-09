using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_PoisonGenerator :TileReporter_TempObjectMaker
{
    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        base.ExecuteAddOnEffect();

        Pathfinder pathFinder = GameObject.Find("MapNavigators").GetComponentInChildren<Pathfinder>();

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        foreach (Tile tile in pathFinder.FindAdjacentTiles(currentTile, true))
        {
            if(tile.objectOnTile || tile.tileOccupied && tile.characterOnTile.elementType == ElementType.Grass 
                && tile.characterOnTile.characterType == TurnEnums.CharacterType.Player)
            {
                return;
            }

            turnManager.temporaryTileObjects.Add(Instantiate(objectToSpawn, tile.transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity));
        }
    }

    #endregion
}
