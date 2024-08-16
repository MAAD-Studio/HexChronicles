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

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        return base.CalculateMovementValue(tile, enemy, turnManager, closestCharacter);
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        List<Character> charactersToCheck;
        if(!mindControl)
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Player);
        }
        else
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Enemy);
        }

        int valueOfAttack = 0;
        foreach (Character character in charactersToCheck)
        {
            valueOfAttack += 25;

            //Bias towards remaining on current tile
            if (currentTile == characterTile)
            {
                valueOfAttack += 60;
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

        return valueOfAttack;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        base.ExecuteAttack(attackArea, turnManager);

        List<Character> charactersToCheck;
        if (!mindControl)
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Player);
        }
        else
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Enemy);
        }

        foreach (Character character in charactersToCheck)
        {
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

                follower.FollowUpAttack(target);
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

    #region CustomMethods

    private void FollowUpAttack(Character character)
    {
        if(mindControl && character.characterType == TurnEnums.CharacterType.Player)
        {
            return;
        }
        else if(!mindControl && character.characterType == TurnEnums.CharacterType.Enemy)
        {
            return;
        }

        TemporaryMarker.GenerateMarker(followUpText, transform.position, 3f, 0.5f);

        transform.LookAt(character.transform.position);
        character.RotateToFaceCharacter(character);

        // Spawn attack vfx
        GameObject vfx = Instantiate(attackVFX, transform.position, Quaternion.identity);
        vfx.transform.forward = transform.forward;
        Destroy(vfx, 3f);

        character.TakeDamage(attackDamage, elementType);
 
        AudioManager.Instance.PlaySound(enemySO.attributes.basicAttackSFX);

        TemporaryMarker.GenerateMarker(followUpMarker, character.transform.position, 4.5f, 0.5f);
    }

    #endregion
}
