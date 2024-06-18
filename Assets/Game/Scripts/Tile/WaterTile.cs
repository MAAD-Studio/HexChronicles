using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : Tile
{
    #region Variables

    #endregion

    #region CustomMethods

    public override void OnTileStay(Character character)
    {
        base.OnTileStay(character);
        Status status = null;

        if (characterTimeOnTile >= 2)
        {
            status = Status.GrabIfStatusActive(character, Status.StatusTypes.Wet);
            if (status == null)
            {
                status = new Status();
                status.effectTurns = 2;
                status.statusType = Status.StatusTypes.Wet;
                characterOnTile.AddStatus(status);
            }
        }

        if(characterTimeOnTile >= 3 && status != null)
        {
            characterOnTile.movementThisTurn += 2;
            characterOnTile.TakeDamage(1, ElementType.Base);
        }
    }

    #endregion
}
