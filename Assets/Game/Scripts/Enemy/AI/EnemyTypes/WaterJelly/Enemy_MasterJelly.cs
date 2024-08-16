using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MasterJelly : Jelly_Base
{
    #region Variables

    [SerializeField] private GameObject kingJellyPrefab;
    [SerializeField] public GameObject combineText;
    Enemy_Base closestJelly;

    #endregion

    #region InterfaceMethods

    public override void PreCalculations(TurnManager turnManager)
    {
        base.PreCalculations(turnManager);

        float distanceToJelly = 1000f;
        foreach (Enemy_Base enemy in turnManager.enemyList)
        {
            float newDistance;
            if (enemy.GetComponent<Enemy_SoloJelly>() != null)
            {
                newDistance = Vector3.Distance(transform.position, enemy.transform.position);

                if (newDistance < distanceToJelly)
                {
                    distanceToJelly = newDistance;
                    closestJelly = enemy;
                }
            }
        }
    }

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int valueOfMovement = -100;

        if (closestJelly != null)
        {
            int distanceTile = (int)Vector3.Distance(tile.transform.position, closestJelly.transform.position);
            int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, closestJelly.transform.position);
            int tileValue = distanceEnemy - distanceTile;

            valueOfMovement = tileValue;
        }

        return valueOfMovement * 2;
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        return base.CalculteAttackValue(attackArea, turnManager, currentTile);
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        base.ExecuteAttack(attackArea, turnManager);
    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        foreach (Tile tile in turnManager.pathfinder.FindAdjacentTiles(characterTile, true))
        {
            if (!tile.tileOccupied)
            {
                continue;
            }

            Character characterOnTile = tile.characterOnTile;
            Enemy_SoloJelly soloJelly = characterOnTile.GetComponent<Enemy_SoloJelly>();

            if (soloJelly != null)
            {
                soloJelly.CombineJelly(this, true);
                break;
            }
        }

        return false;
    }

    public override void ActionCleanup()
    {
        base.ActionCleanup();

        closestJelly = null;
    }

    #endregion

    #region CustomMethods

    public void CombineJelly(TurnManager turnManager)
    {
        Vector3 spawnPoint = transform.position;
        spawnPoint.y += 3.2f;

        GameObject newObject = Instantiate(kingJellyPrefab, spawnPoint, Quaternion.identity);
        Enemy_KingJelly newKingJelly = newObject.GetComponent<Enemy_KingJelly>();
        newKingJelly.characterTile = characterTile;
        characterTile.characterOnTile = newKingJelly;
        newKingJelly.transform.eulerAngles = new Vector3(0f, 180f, 0f);

        turnManager.enemyList.Add(newKingJelly);

        Debug.Log("MASTER JELLY HAS COMBINED WITH A SOLO JELLY TO PRODUCE A KING");
        TemporaryMarker.GenerateMarker(combineText, transform.localPosition, 4f, 1f);

        turnManager.enemyList.Remove(this);
        Destroy(gameObject);
    }

    public override Character LikelyTarget()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        Enemy_Base closestEnemy = null;
        float distance = 1000f;
        foreach (Enemy_Base enemy in turnManager.enemyList)
        {
            if(enemy.GetComponent<Enemy_SoloJelly>() == null)
            {
                continue;
            }

            float newDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    #endregion
}
