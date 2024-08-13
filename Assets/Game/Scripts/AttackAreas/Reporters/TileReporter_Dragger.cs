using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileReporter_Dragger : TileReporter
{
    #region Variables

    [SerializeField] private int dragRange;
    [SerializeField] private int damage;
    [SerializeField] private GameObject effect;

    #endregion

    #region CustomMethods

    public override void ExecuteAddOnEffect()
    {
        if(currentTile == null)
        {
            return;
        }

        Pathfinder pathFinder = GameObject.Find("MapNavigators").GetComponentInChildren<Pathfinder>();
        pathFinder.PathTilesInRange(currentTile, 0, dragRange, true, false);
        List<Tile> dragTile = new List<Tile>(pathFinder.frontier);

        Destroy(Instantiate(effect, currentTile.transform.position, Quaternion.identity), 2f);

        int delay = 0;
        foreach(Tile tile in dragTile)
        {
            Character character = tile.characterOnTile;
            if(character != null && character.characterType == TurnEnums.CharacterType.Enemy)
            {
                UndoManager.Instance.StoreEnemy((Enemy_Base)character, false);
                character.DragTowards(currentTile, damage);
                delay++;
            }
        }
    }

    #endregion
}
