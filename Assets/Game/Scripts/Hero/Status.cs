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
        Burning, // Damage 1
        Bound, // Skip this turn
        Blessing, // Heal 2
        Hurt, // Take +1 damage
        CannotMove,
        CannotAttack,
        Wet,
        Haste
    }
    public StatusTypes statusType;
    public int effectTurns;
    public int damageAddOn = 0;

    public void Apply(Character character)
    {
        switch (statusType)
        {
            case StatusTypes.Burning:
                character.TakeDamage(1 + damageAddOn, ElementType.Poison);
                break;

            case StatusTypes.Bound:
                character.canMove = false;
                character.canAttack = false;
                break;

            case StatusTypes.Blessing:
                character.Heal(2);
                break;

            case StatusTypes.Hurt:
                character.isHurt = true;
                break;

            case StatusTypes.CannotMove:
                character.canMove = false;
                break;

            case StatusTypes.CannotAttack:
                character.canAttack = false;
                break;

            case StatusTypes.Wet:
                character.movementThisTurn += 1;
                break;

            case StatusTypes.Haste:
                Debug.Log("STATUS MOVEMENT BEFORE: " + character.movementThisTurn);
                character.movementThisTurn -= 2;
                Debug.Log("STATUS MOVEMENT AFTER: " + character.movementThisTurn);
                break;

            default:
                break;
        }
        effectTurns -= 1;
    }

    public static Status GrabIfStatusActive(Character character, Status.StatusTypes type)
    {
        foreach (Status status in character.statusList)
        {
            if (status.statusType == type)
            {
                return status;
            }
        }
        return null;
    }
}
