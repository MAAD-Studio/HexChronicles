using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Status
{
    public enum StatusTypes
    {
        None,
        // elemental
        Burning,
        Freezed,
        Poisoned,

        // physical
        Bound,
        Taunt,
        Chaos,
        CannotMove,
        CannotAttack,
        CannotUseSkill,
        CannotBeSelected,
    }
    public StatusTypes statusType;
    public int effectTurns;

    public void Apply(Character character)
    {
        switch (statusType)
        {
            case StatusTypes.Burning:
                BurningStatus(character);
                break;
            case StatusTypes.Poisoned:
                PoisonStatus(character);
                break;
            case StatusTypes.Freezed:
                FreezeStatus(character);
                break;
            case StatusTypes.Bound:
                BoundStatus(character);
                break;
            case StatusTypes.Chaos:
                ChaosStatus(character);
                break;
            case StatusTypes.Taunt:
                TauntStatus(character);
                break;
        }
        effectTurns -= 1;
    }

    private void BurningStatus(Character character)
    {
        Debug.Log("Burning");
        character.TakeDamage(1);
    }

    private void PoisonStatus(Character character)
    {
        Debug.Log("Poisoned");
    }

    private void FreezeStatus(Character character)
    {
        Debug.Log("Freezed");
    }

    private void BoundStatus(Character character)
    {
        character.movementThisTurn = 0;
        // Attack?
    }

    private void ChaosStatus(Character character)
    {
        Debug.Log("Chaos");
    }

    private void TauntStatus(Character character)
    {
        Debug.Log("Taunt");
    }
}
