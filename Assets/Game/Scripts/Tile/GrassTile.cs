using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTile : Tile
{
    #region Variables

    #endregion

    #region CustomMethods

    public override void OnTileStay(Character character)
    {
        base.OnTileStay(character);
        Status status = null;

        if (character.elementType != tileData.tileType)
        {
            if (characterTimeOnTile >= 2)
            {
                status = Status.GrabIfStatusActive(character, Status.StatusTypes.Bound);
                if (status == null)
                {
                    status = new Status();
                    status.effectTurns = 2;
                    status.statusType = Status.StatusTypes.Bound;
                    characterOnTile.AddStatus(status);
                }
            }

            if (characterTimeOnTile >= 3 && status != null)
            {

            }
        }
    }

    #endregion
}
