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
        MovementReduction,
        AttackBoost,
        Wet,
        Haste,
        Shield,
        MindControl
    }
    public StatusTypes statusType;
    public int effectTurns;
    [HideInInspector] public int damageAddOn = 0;

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
                character.movementThisTurn = character.moveDistance;
                break;

            case StatusTypes.CannotAttack:
                character.canAttack = false;
                break;

            case StatusTypes.MovementReduction:
                character.movementThisTurn += 1;
                break;

            case StatusTypes.Wet:
                character.movementThisTurn += 1;
                break;

            case StatusTypes.Haste:
                if(character.movementThisTurn < character.moveDistance)
                {
                    character.movementThisTurn -= 2;
                }
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
