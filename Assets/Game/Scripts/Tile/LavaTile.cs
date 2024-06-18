using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTile : Tile
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
            status = Status.GrabIfStatusActive(character, Status.StatusTypes.Burning);
            if (status == null)
            {
                status = new Status();
                status.effectTurns = 2;
                status.statusType = Status.StatusTypes.Burning;
                characterOnTile.AddStatus(status);
            }
        }

        if (characterTimeOnTile >= 3 && status != null)
        {
            status.damageAddOn += 1;
            characterOnTile.TakeDamage(2, ElementType.Fire);
        }
    }

    #endregion
}
