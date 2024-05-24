using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Gangsta : Enemy_Base
{
    #region Variables

    #endregion

    #region UnityMethods

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager)
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

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager)
    {
        int valueOfAttack = 0;
        foreach (Character character in attackArea.CharactersHit(TurnEnums.CharacterType.Player))
        {
            valueOfAttack += 5;
            foreach(Tile tile in turnManager.pathfinder.FindAdjacentTiles(character.characterTile, true))
            {
                if(tile.tileOccupied && tile.characterOnTile.GetComponent<Enemy_Gangsta>() != null)
                {
                    valueOfAttack += 10;
                    break;
                }
            }
        }

        foreach (Character character in attackArea.CharactersHit(characterType))
        {
            valueOfAttack -= 2;
        }

        return valueOfAttack;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        foreach(Character character in attackArea.CharactersHit(TurnEnums.CharacterType.Player))
        {
            character.TakeDamage(attackDamage);

            List<Tile> adjacentTiles = turnManager.pathfinder.FindAdjacentTiles(character.characterTile, true);

            foreach(Tile tile in adjacentTiles)
            {
                if(!tile.tileOccupied)
                {
                    continue;
                }

                Enemy_Gangsta otherEnemy = tile.characterOnTile.GetComponent<Enemy_Gangsta>();
                if(otherEnemy != null && otherEnemy.gameObject != gameObject)
                {
                    otherEnemy.FollowUpAttack(character);
                }
            }
        }
    }

    public void FollowUpAttack(Character character)
    {
        transform.LookAt(character.transform.position);

        character.TakeDamage(attackDamage);
    }

    #endregion
}
