using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Basic : Enemy_Base
{
    #region Variables

    #endregion

    #region UnityMethods

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int valueOfMovement = -100;
        foreach (Character character in turnManager.characterList)
        {
            int distanceTile = (int)Vector3.Distance(tile.transform.position, character.transform.position);
            int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, character.transform.position);
            int tileValue = distanceEnemy - distanceTile;

            if (valueOfMovement < tileValue)
            {
                valueOfMovement = tileValue;
            }
        }
        return valueOfMovement * 2;
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        int valueOfAttack = 0;
        foreach(Character character in attackArea.CharactersHit(TurnEnums.CharacterType.Player))
        {
            valueOfAttack += 5;
        }

        foreach(Character character in attackArea.CharactersHit(characterType))
        {
            valueOfAttack -= 2;
        }

        return valueOfAttack;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        Debug.Log("~~** WE DO BE EXECUTING AN ATTACK **~~");
    }

    #endregion
}
