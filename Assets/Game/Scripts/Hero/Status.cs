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
        _________BelowNotUsed,
        Freezed,
        Poisoned,
        Taunt,
        Chaos,
        CannotMove,
        CannotAttack,
        CannotUseSkill,
        CannotBeSelected
    }
    public StatusTypes statusType;
    public int effectTurns;

    public void Apply(Character character)
    {
        switch (statusType)
        {
            case StatusTypes.Burning:
                character.TakeDamage(1);
                break;

            case StatusTypes.Bound:
                character.movementThisTurn = 0;
                break;

            case StatusTypes.Blessing:
                character.Heal(2);
                break;

            case StatusTypes.Hurt:
                character.isHurt = true;
                break;

            case StatusTypes.Poisoned:
                break;

            case StatusTypes.Freezed:
                break;

            case StatusTypes.Chaos:
                break;

            case StatusTypes.Taunt:
                break;
        }
        effectTurns -= 1;
    }
}
