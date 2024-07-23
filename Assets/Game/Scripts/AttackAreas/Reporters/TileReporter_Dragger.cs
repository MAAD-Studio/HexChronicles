using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_Dragger : TileReporter
{
    #region Variables

    [SerializeField] private int dragRange;
    [SerializeField] private int damage;

    #endregion

    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        Pathfinder pathFinder = GameObject.Find("MapNavigators").GetComponentInChildren<Pathfinder>();
        pathFinder.PathTilesInRange(currentTile, 0, dragRange, true, false);
        List<Tile> dragTile = new List<Tile>(pathFinder.frontier);

        foreach(Tile tile in dragTile)
        {
            Character character = tile.characterOnTile;
            if(character != null && character.characterType == TurnEnums.CharacterType.Enemy)
            {
                UndoManager.Instance.StoreEnemy((Enemy_Base)character);
                character.DragTowards(currentTile, damage);
            }
        }
    }

    #endregion
}
