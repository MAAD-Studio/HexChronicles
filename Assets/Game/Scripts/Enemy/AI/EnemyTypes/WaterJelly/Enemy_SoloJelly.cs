using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_SoloJelly : Enemy_Base
{
    #region Variables

    [SerializeField] public GameObject combineText;

    #endregion

    #region InterfaceMethods

    public override int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
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

        foreach (Tile tile in turnManager.pathfinder.FindAdjacentTiles(characterTile, true))
        {
            if(!tile.tileOccupied)
            {
                continue;
            }

            Character characterOnTile = tile.characterOnTile;
            Enemy_MasterJelly masterJelly = characterOnTile.GetComponent<Enemy_MasterJelly>();
            Enemy_KingJelly kingJelly = characterOnTile.GetComponent<Enemy_KingJelly>();

            if(masterJelly != null)
            {
                pickedMaster = masterJelly;
                break;
            }
            else if(kingJelly != null)
            {
                pickedKing = kingJelly;
            }
        }

        if (pickedMaster != null)
        {
            pickedMaster.CombineJelly(turnManager);
            DestroySelfEnemy(turnManager);
        }
        else if (pickedKing != null)
        {
            pickedKing.CombineJelly();
            DestroySelfEnemy(turnManager);
        }

        return false;
    }

    #endregion

    #region CustomMethods

    public void CombineJelly(TurnManager turnManager)
    {
        DestroySelfEnemy(turnManager);
    }

    #endregion
}
