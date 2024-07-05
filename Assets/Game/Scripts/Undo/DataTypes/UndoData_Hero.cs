using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoData_Hero : UndoData_Character
{
    #region Variables

    public Hero heroInvolved;
    public ElementType heroType;
    public bool hasMadeDecision;
    public List<BasicUpgrade> upgradeList;

    #endregion
}
