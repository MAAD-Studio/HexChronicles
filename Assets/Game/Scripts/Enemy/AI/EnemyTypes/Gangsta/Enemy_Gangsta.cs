using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Gangsta : Enemy_Base
{
    #region Variables

    private List<FollowUpCombo> followUpComboList = new List<FollowUpCombo>();
    private int followUpsPerformed = 0;

    [SerializeField] private GameObject followUpText;
    [SerializeField] private GameObject followUpMarker;

    #endregion

    #region UnityMethods

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int distanceTile = (int)Vector3.Distance(tile.transform.position, closestCharacter.transform.position);
        int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, closestCharacter.transform.position);
        int tileValue = distanceEnemy - distanceTile;
        
        return tileValue * 2;
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        int valueOfAttack = 0;
        foreach (Character character in attackArea.CharactersHit(TurnEnums.CharacterType.Player))
        {
            valueOfAttack += 5;

            //Bias towards remaining on current tile
            if (currentTile == characterTile)
            {
                valueOfAttack += 30;
            }

            //Bias towards hitting targets surrounded by other gangstas
            foreach (Tile tile in turnManager.pathfinder.FindAdjacentTiles(character.characterTile, true))
            {
                if(tile.tileOccupied && tile.characterOnTile.GetComponent<Enemy_Gangsta>() != null)
                {
                    valueOfAttack += 15;
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

            character.TakeDamage(attackDamage, elementType);

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

                TemporaryMarker.GenerateMarker(followUpText, follower.transform.position, 2f, 0.5f);
                TemporaryMarker.GenerateMarker(followUpMarker, target.transform.position, 4f, 0.5f);
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

    private void FollowUpAttack(Character character)
    {
        transform.LookAt(character.transform.position);
        character.TakeDamage(attackDamage, elementType);
    }

    #endregion
}
