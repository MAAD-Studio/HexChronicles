using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SoloJelly : Enemy_Base
{
    #region Variables

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager)
    {
        int valueOfMovement = -100;

        foreach(Enemy_Base enemyChar in turnManager.enemyList)
        {
            if(enemyChar == this)
            {
                continue;
            }

            Enemy_MasterJelly masterJelly = enemyChar.GetComponent<Enemy_MasterJelly>();
            Enemy_KingJelly kingJelly = enemyChar.GetComponent<Enemy_KingJelly>();

            if(masterJelly != null || kingJelly != null)
            {
                int distanceTile = (int)Vector3.Distance(tile.transform.position, enemyChar.transform.position);
                int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, enemyChar.transform.position);
                int tileValue = distanceEnemy - distanceTile;

                if(masterJelly != null)
                {
                    tileValue += 2;
                }

                if (valueOfMovement < tileValue)
                {
                    valueOfMovement = tileValue;
                }
            }
        }

        return valueOfMovement * 2;
    }

    public override int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager)
    {
        return 0;
    }

    public override void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {

    }

    public override bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    #endregion
}
