using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_MasterJelly : Enemy_Base
{
    #region Variables

    [SerializeField] private GameObject kingJellyPrefab;
    [SerializeField] public GameObject combineText;

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int valueOfMovement = -100;

        foreach (Enemy_Base enemyChar in turnManager.enemyList)
        {
            if (enemyChar == this)
            {
                continue;
            }

            Enemy_SoloJelly soloJelly = enemyChar.GetComponent<Enemy_SoloJelly>();

            if (soloJelly != null)
            {
                int distanceTile = (int)Vector3.Distance(tile.transform.position, enemyChar.transform.position);
                int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, enemyChar.transform.position);
                int tileValue = distanceEnemy - distanceTile;

                if (valueOfMovement < tileValue)
                {
                    valueOfMovement = tileValue * 2;
                }
            }
            else
            {
                int distanceTile = (int)Vector3.Distance(tile.transform.position, closestCharacter.transform.position);
                int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, closestCharacter.transform.position);
                int tileValue = distanceEnemy - distanceTile;

                if (valueOfMovement < tileValue)
                {
                    valueOfMovement = tileValue * 2;
                }
            }
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
                soloJelly.CombineJelly(turnManager);
                CombineJelly(turnManager);
                break;
            }
        }

        return false;
    }

    #endregion

    #region CustomMethods

    public void CombineJelly(TurnManager turnManager)
    {
        Vector3 spawnPoint = transform.position;
        spawnPoint.y += 1.1f;

        GameObject newObject = Instantiate(kingJellyPrefab, spawnPoint, Quaternion.identity);
        Enemy_KingJelly newKingJelly = newObject.GetComponent<Enemy_KingJelly>();
        newKingJelly.FindTile();

        turnManager.enemyList.Add(newKingJelly);
        DestroySelfEnemy(turnManager);

        Debug.Log("MASTER JELLY HAS COMBINED WITH A SOLO JELLY TO PRODUCE A KING");
        TemporaryMarker.GenerateMarker(combineText, transform.localPosition, 4f, 1f);
    }

    #endregion
}
