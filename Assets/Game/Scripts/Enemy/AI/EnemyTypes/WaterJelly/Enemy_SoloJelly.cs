using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SoloJelly : Jelly_Base
{
    #region Variables

    [SerializeField] public GameObject combineText;
    Enemy_Base closestJelly = null;

    #endregion

    #region InterfaceMethods

    public override void PreCalculations(TurnManager turnManager)
    {
        base.PreCalculations(turnManager);

        bool foundMaster = false;
        float distanceToJelly = 1000f;
        foreach (Enemy_Base enemy in turnManager.enemyList)
        {
            float newDistance;
            bool examineEnemy = false;
            if(enemy.GetComponent<Enemy_MasterJelly>() != null)
            {
                examineEnemy = true;
                foundMaster = true;
            }
            else if(!foundMaster && enemy.GetComponent<Enemy_KingJelly>())
            {
                examineEnemy = true;
            }

            if(examineEnemy)
            {
                newDistance = Vector3.Distance(transform.position, enemy.transform.position);

                if(newDistance < distanceToJelly)
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

        if(closestJelly != null)
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
        //Solo Jellies can't attack
        return 0;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        //Solo Jellies can't attack
    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        Enemy_MasterJelly pickedMaster = null;
        Enemy_KingJelly pickedKing = null;

        if(archFinished)
        {
            foreach (Tile tile in turnManager.pathfinder.FindAdjacentTiles(characterTile, true))
            {
                if (!tile.tileOccupied)
                {
                    continue;
                }

                Character characterOnTile = tile.characterOnTile;
                Enemy_MasterJelly masterJelly = characterOnTile.GetComponent<Enemy_MasterJelly>();
                Enemy_KingJelly kingJelly = characterOnTile.GetComponent<Enemy_KingJelly>();

                if (masterJelly != null)
                {
                    pickedMaster = masterJelly;
                    break;
                }
                else if (kingJelly != null)
                {
                    pickedKing = kingJelly;
                }
            }

            if (pickedMaster != null)
            {
                CombineJelly(pickedMaster, true);
            }
            else if (pickedKing != null)
            {
                CombineJelly(pickedKing, false);
            }
        }

        while(!archFinished)
        {
            return true;
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

    public void CombineJelly(Jelly_Base combineTarget, bool isMaster)
    {
        selfDestruct = true;
        if(!isMaster)
        {
            InitiateArch(combineTarget.transform.position, 2f, 2f, (Enemy_KingJelly)combineTarget);
        }
        else
        {
            InitiateArch(combineTarget.transform.position, 2f, 2f, (Enemy_MasterJelly)combineTarget);
        }
    }

    public override Character LikelyTarget()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        Enemy_Base closestEnemy = null;
        float distance = 1000f;
        bool foundMaster = false;
        foreach (Enemy_Base enemy in turnManager.enemyList)
        {
            float newDistance;
            bool examineEnemy = false;
            if (enemy.GetComponent<Enemy_MasterJelly>() != null)
            {
                examineEnemy = true;
                foundMaster = true;
            }
            else if (!foundMaster && enemy.GetComponent<Enemy_KingJelly>())
            {
                examineEnemy = true;
            }

            if (examineEnemy)
            {
                newDistance = Vector3.Distance(transform.position, enemy.transform.position);

                if (newDistance < distance)
                {
                    distance = newDistance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    #endregion
}
