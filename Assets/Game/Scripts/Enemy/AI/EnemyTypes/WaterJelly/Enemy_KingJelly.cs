using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KingJelly : Enemy_Base
{
    #region Variables

    [SerializeField] public int slimeCount = 2;
    [SerializeField] public GameObject masterJellyPrefab;
    [SerializeField] public GameObject soloJellyPrefab;

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
        }
    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    public override void Died()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.pathfinder.FindPaths(this);

        bool masterJellySpawned = false;
        int slimesSpawned = 0;
        List<Tile> potentialTiles = new List<Tile>(turnManager.pathfinder.frontier);

        while (slimesSpawned < slimeCount && potentialTiles.Count > 0)
        {
            int choice = UnityEngine.Random.Range(0, potentialTiles.Count);
            Vector3 spawnPoint = potentialTiles[choice].transform.position;
            spawnPoint.y += 1.1f;

            if (masterJellySpawned)
            {
                GameObject newObject = Instantiate(soloJellyPrefab, spawnPoint, Quaternion.identity);
                Enemy_SoloJelly newSoloJelly = newObject.GetComponent<Enemy_SoloJelly>();
                newSoloJelly.FindTile();
                turnManager.enemyList.Add(newSoloJelly);
            }
            else
            {
                GameObject newObject = Instantiate(masterJellyPrefab, spawnPoint, Quaternion.identity);
                Enemy_MasterJelly newMasterJelly = newObject.GetComponent<Enemy_MasterJelly>();
                newMasterJelly.FindTile();
                masterJellySpawned = true;
                turnManager.enemyList.Add(newMasterJelly);
            }

            slimesSpawned++;
            potentialTiles.Remove(potentialTiles[choice]);
        }

        base.Died();
    }

    public void CombineJelly()
    {
        slimeCount++;
        attackDamage += 1;
        currentHealth += 1;
        //ADD HEALTH UPDATE LATER
    }

    #endregion
}
