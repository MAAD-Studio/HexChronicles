using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TurnEnums;

public class Spawner : TileObject
{
    #region Variables

    [Header("Spawning Info: ")]
    [SerializeField] private int numberToSpawn = 1;
    [SerializeField] private List<Enemy_Base> enemyList;

    #endregion

    #region UnityMethods

    public override void Start()
    {
        base.Start();
        Debug.Assert(turnManager != null, "Spawner doesn't have a turnManager provided");
        Debug.Assert(enemyList != null, "Spawner doesn't have any provided enemies for spawning");
    }

    #endregion

    #region CustomMethods

    //Attempts to spawn in new enemies on the surrounding tiles
    public void AttemptSpawn()
    {
        List<Tile> adjacentTiles = turnManager.pathfinder.FindAdjacentTiles(attachedTile, false);
        int enemiesSpawnedIn = 0;

        while (adjacentTiles.Count > 0 && enemiesSpawnedIn < numberToSpawn)
        {
            //Selects what tile to spawn on and what enemy to spawn
            int tileChoice = Random.Range(0, adjacentTiles.Count);
            int enemyChoice = Random.Range(0, enemyList.Count);
            
            //Setsup the spawn position
            Vector3 spawnPosition = adjacentTiles[tileChoice].transform.position;
            spawnPosition.y += 0.5f;

            //Spawns in the enemy and has it attach itself to the selected tile
            Enemy_Base newEnemy = Instantiate(enemyList[enemyChoice].gameObject, spawnPosition, Quaternion.identity).GetComponent<Enemy_Base>();
            newEnemy.FindTile();

            turnManager.enemyList.Add(newEnemy);
            enemiesSpawnedIn++;
            adjacentTiles.Remove(adjacentTiles[tileChoice]);
        }
    }

    public override void TakeDamage(float attackDamage)
    {
        base.TakeDamage(attackDamage);
    }

    #endregion
}
