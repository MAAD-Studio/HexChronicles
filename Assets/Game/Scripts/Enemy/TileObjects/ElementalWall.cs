using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalWall : TileObject
{
    #region Variables

    public ElementType elementType;
    public int turnsToStay = 2;
    private int turnOfCreation;

    #endregion

    #region UnityMethods

    public override void Start()
    {
        base.Start();

        turnOfCreation = turnManager.TurnNumber;
        turnManager.temporaryTileObjects.Add(this);
    }

    #endregion

    #region CustomMethods

    public override bool CheckDestruction()
    {
        if(turnManager.TurnNumber - turnOfCreation >= turnsToStay)
        {
            return true;
        }
        return false;
    }

    #endregion
}
