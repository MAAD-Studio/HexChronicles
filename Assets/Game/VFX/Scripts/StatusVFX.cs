using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class StatusVFX : MonoBehaviour
{
    protected int turns;
    protected bool isPlayer;

    public void Initialize(int turns, bool isPlayer)
    {
        this.turns = turns;
        this.isPlayer = isPlayer;

        if (isPlayer)
        {
            EventBus.Instance.Subscribe<OnPlayerTurn>(OnUpdate);
        }
        else
        {
            EventBus.Instance.Subscribe<OnEnemyTurn>(OnUpdate);
        }
    }

    protected void OnUpdate(object obj)
    {
        turns--;
        if (turns <= 0)
        {
            Remove();
        }
    }

    protected virtual void Remove()
    {
        if (isPlayer)
        {
            EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnUpdate);
        }
        else
        {
            EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnUpdate);
        }
        Destroy(gameObject);
    }
}
