using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowersTurn : WorldTurnBase
{
    #region Variables

    [SerializeField] private int turnsbetweenSpawn = 2;
    [SerializeField] private List<Spawner> spawners = new List<Spawner>();

    #endregion

    #region UnityMethods

    #endregion

    #region StateInterfaceMethods

    public override void EnterState(TurnManager manager)
    {
        base.EnterState(manager);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (turnManager.TurnNumber % turnsbetweenSpawn == 0)
        {
            foreach (Spawner spawner in spawners)
            {
                spawner.AttemptSpawn();
            }
        }

        turnManager.SwitchState(TurnEnums.TurnState.PlayerTurn);
    }

    #endregion
}