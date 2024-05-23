using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowersTurn : WorldTurnBase
{
    #region Variables

    [Header("Spawning Information: ")]
    [SerializeField] private int turnsTillSpawn = 2;
    [SerializeField] private List<Spawner> spawners = new List<Spawner>();

    #endregion

    #region UnityMethods

    #endregion

    #region StateInterfaceMethods

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (turnManager.TurnNumber % turnsTillSpawn == 0)
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