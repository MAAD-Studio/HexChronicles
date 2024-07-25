using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KingJelly : Jelly_Base
{
    #region Variables

    [SerializeField] public int slimeCount = 2;
    [SerializeField] public GameObject masterJellyPrefab;
    [SerializeField] public GameObject soloJellyPrefab;
    [SerializeField] public GameObject combineText;

    [Header("New Jelly Launch Controls")]
    [Range(0.1f, 10f)]
    [SerializeField] public float launchHeight = 5f;

    [Range(0.05f, 2f)]
    [SerializeField] public float launchSpeed = 0.5f;

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
        turnManager.pathfinder.PathTilesInRange(characterTile, 0, 5, false, false);

        bool masterJellySpawned = false;
        int slimesSpawned = 0;
        List<Tile> potentialTiles = new List<Tile>(turnManager.pathfinder.frontier);

        while (slimesSpawned < slimeCount && potentialTiles.Count > 0)
        {
            int choice = UnityEngine.Random.Range(0, potentialTiles.Count);
            Vector3 spawnPoint = potentialTiles[choice].transform.position;

            if (masterJellySpawned)
            {
                GameObject newObject = Instantiate(soloJellyPrefab, transform.position, Quaternion.identity);
                Enemy_SoloJelly newSoloJelly = newObject.GetComponent<Enemy_SoloJelly>();
                turnManager.enemyList.Add(newSoloJelly);
                UndoManager.Instance.StoreEnemy(newSoloJelly, true);

                newSoloJelly.InitiateArch(spawnPoint + new Vector3(0, 1, 0), launchSpeed, launchHeight);
            }
            else
            {
                GameObject newObject = Instantiate(masterJellyPrefab, transform.position, Quaternion.identity);
                Enemy_MasterJelly newMasterJelly = newObject.GetComponent<Enemy_MasterJelly>();
                masterJellySpawned = true;
                turnManager.enemyList.Add(newMasterJelly);
                UndoManager.Instance.StoreEnemy(newMasterJelly, true);

                newMasterJelly.InitiateArch(spawnPoint, launchSpeed, launchHeight);
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
        UpdateHealthBar?.Invoke();
        TemporaryMarker.GenerateMarker(combineText, transform.localPosition, 4f, 1f);
    }

    #endregion
}
