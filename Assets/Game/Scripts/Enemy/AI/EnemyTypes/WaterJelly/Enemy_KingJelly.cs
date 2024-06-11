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
        return base.CalculateMovementValue(tile, enemy, turnManager, closestCharacter);
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        return base.CalculteAttackValue(attackArea, turnManager, currentTile);
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

    #endregion

    #region CustomMethods

    public override void Died()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.pathfinder.PathTilesInRange(characterTile, 0, 5, false);

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
        maxHealth = currentHealth;
        base.InvokeUpdateHealthBar();
    }

    #endregion
}
