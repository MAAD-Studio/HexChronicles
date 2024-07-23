using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusVFX : MonoBehaviour
{
    protected Character character;
    protected int turns;
    protected bool isPlayer;
    [SerializeField] protected Status.StatusTypes statusType;

    public void Initialize(Character character, int turns)
    {
        this.character = character;
        this.turns = turns;

        character.UpdateStatus.AddListener(UpdateStatusTurns);

        if (isPlayer)
        {
            EventBus.Instance.Subscribe<OnPlayerTurn>(OnNewTurn);
        }
        else
        {
            EventBus.Instance.Subscribe<OnEnemyTurn>(OnNewTurn);
        }
    }

    protected void UpdateStatusTurns()
    {
        foreach (Status status in character.statusList)
        {
            if (status.statusType == statusType)
            {
                turns = status.effectTurns;
                return;
            }
        }

        // Remove if the status is not found
        Remove();
    }

    protected void OnNewTurn(object obj)
    {
        turns--;
        if (turns <= 0)
        {
            Remove();
        }
    }

    protected virtual void Remove()
    {
        Destroy(gameObject);
    }

    protected void OnDestroy()
    {
        UnsubscribeEvents();
    }

    protected void UnsubscribeEvents()
    {
        character.UpdateStatus.RemoveListener(UpdateStatusTurns);
        if (isPlayer)
        {
            EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnNewTurn);
        }
        else
        {
            EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnNewTurn);
        }
    }
}
