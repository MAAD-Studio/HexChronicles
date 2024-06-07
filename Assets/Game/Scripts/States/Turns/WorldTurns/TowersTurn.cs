using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowersTurn : WorldTurnBase
{
    #region Variables

    [Header("Spawning Information: ")]
    [SerializeField] private int turnsTillSpawn = 2;
    [SerializeField] private List<Spawner> spawners = new List<Spawner>();

    bool updateCalled = false;
    bool updateDone = false;

    #endregion

    #region UnityMethods

    protected override void Start()
    {
        base.Start();
        if(spawners.Count > 0)
        {
            TileObject.objectDestroyed.AddListener(SpawnerDestroyed);
        }
    }

    #endregion

    #region StateInterfaceMethods

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
        updateCalled = false;
        updateDone = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (turnManager.TurnNumber % turnsTillSpawn == 0)
        {
            if (!updateCalled)
            {
                updateCalled = true;
                //weatherManager.UpdateWeather();
                StartCoroutine(UpdateSpawners());
            }
            else if(updateDone)
            {
                turnManager.SwitchState(TurnEnums.TurnState.PlayerTurn);
            }
        }
        else
        {
            //weatherManager.UpdateWeather();
            turnManager.SwitchState(TurnEnums.TurnState.PlayerTurn);
        }
    }

    private IEnumerator UpdateSpawners()
    {
        if (turnManager.TurnNumber % turnsTillSpawn == 0)
        {
            foreach (Spawner spawner in spawners)
            {
                turnManager.mainCameraController.MoveToTargetPosition(transform.position, true);

                yield return new WaitForSeconds(0.5f);
                spawner.AttemptSpawn();
                yield return new WaitForSeconds(0.5f);
            }
        }

        updateDone = true;
    }

    private void SpawnerDestroyed(TileObject tileObj)
    {
        Spawner spawnerToDestroy = null;

        foreach(Spawner spawner in spawners)
        {
            if(spawner == tileObj)
            {
                spawnerToDestroy = spawner;
                break;
            }
        }

        if(spawnerToDestroy != null)
        {
            spawners.Remove(spawnerToDestroy);
            if(spawners.Count <= 0)
            {
                TileObject.objectDestroyed.RemoveListener(SpawnerDestroyed);
            }
        }
    }

    #endregion
}