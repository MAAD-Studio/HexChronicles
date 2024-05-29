using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Gangsta : Enemy_Base
{
    #region Variables

    List<FollowUpCombo> followUpComboList = new List<FollowUpCombo>();
    int followUpsPerformed = 0;

    public GameObject followUpText;
    public GameObject followUpMarker;

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
        base.ExecuteAttack(attackArea, turnManager);

        foreach (Character character in attackArea.CharactersHit(TurnEnums.CharacterType.Player))
        {
            transform.LookAt(character.transform.position);

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
                    FollowUpCombo newCombo = new FollowUpCombo();
                    newCombo.follower = otherEnemy;
                    newCombo.target = character;
                    followUpComboList.Add(newCombo);
                }
            }
        }
    }

    public void FollowUpAttack(Character character)
    {
        transform.LookAt(character.transform.position);
        character.TakeDamage(attackDamage);
    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        if(followUpsPerformed < followUpComboList.Count)
        {
            if (followUpComboList[followUpsPerformed].target != null)
            {
                Character target = followUpComboList[followUpsPerformed].target;
                Enemy_Gangsta follower = followUpComboList[followUpsPerformed].follower;

                transform.LookAt(target.transform);
                follower.FollowUpAttack(target);

                Vector3 textPos = follower.transform.position;
                textPos.y += 2;

                Vector3 markerPos = target.transform.position;
                markerPos.y += 4;

                Instantiate(followUpText, textPos, Quaternion.identity);
                Instantiate(followUpMarker, markerPos, Quaternion.identity);
            }

            followUpsPerformed++;

            return true;
        }
        else
        {
            followUpsPerformed = 0;
            followUpComboList.Clear();
            return false;
        }
    }

    #endregion
}
