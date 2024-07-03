using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowersTurn : WorldTurnBase
{
    #region Variables

    [Header("Spawning Information: ")]
    [SerializeField] private int turnsTillSpawn = 2;
    [SerializeField] private List<Tower> towers = new List<Tower>();

    [HideInInspector] public static UnityEvent<TileObject> StoreTileObject = new UnityEvent<TileObject>();

    public bool HasTowers
    {
        get { return towers.Count > 0; }
    }

    bool updateCalled = false;
    bool updateDone = false;

    #endregion

    #region UnityMethods

    protected override void Start()
    {
        base.Start();
        if(towers.Count > 0)
        {
            TileObject.objectDestroyed.AddListener(TowerDestroyed);
            TileObject.objectCreated.AddListener(TowerCreated);
            StoreTileObject.AddListener(StoreTowerData);
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

        if (!updateCalled)
        {
            updateCalled = true;
            StartCoroutine(UpdateTowers());
        }
        else if(updateDone)
        {
            turnManager.SwitchState(TurnEnums.TurnState.WeatherTurn);
        }
    }

    public override void ResetState()
    {
        base.ResetState();

    }

    private IEnumerator UpdateTowers()
    {
        foreach (Tower tower in towers)
        {
            bool holdOnEnd = false;

            if (turnManager.TurnNumber % turnsTillSpawn == 0)
            {
                turnManager.mainCameraController.MoveToTargetPosition(tower.transform.position, true);
                yield return new WaitForSeconds(0.5f);
                tower.AttemptSpawn();
                holdOnEnd = true;
            }

            if(tower.CanAttack())
            {
                yield return new WaitForSeconds(0.5f);
                tower.AttemptAttack();
                holdOnEnd = true;
            }

            //If something happens the camera holds for a second to show the action
            if(holdOnEnd)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        updateDone = true;
    }

    private void TowerDestroyed(TileObject tileObj)
    {
        Tower towerToDestroy = null;

        foreach(Tower tower in towers)
        {
            if(tower == tileObj)
            {
                towerToDestroy = tower;
                break;
            }
        }

        if(towerToDestroy != null)
        {
            towers.Remove(towerToDestroy);
            if(towers.Count <= 0)
            {
                TileObject.objectDestroyed.RemoveListener(TowerDestroyed);
                TileObject.objectCreated.RemoveListener(TowerCreated);
                StoreTileObject.RemoveListener(StoreTowerData);
                Victory.Invoke();
            }
        }
    }

    private void TowerCreated(TileObject tileObj)
    {
        if(tileObj.objectType == ObjectType.Tower)
        {
            towers.Add((Tower)tileObj);
        }
    }

    private void StoreTowerData(TileObject tileObj)
    {
        List<TileObject> referenceList = new List<TileObject>(towers);
        //UndoManager.Instance.StoreTileObject(tileObj, referenceList);
    }

    #endregion
}