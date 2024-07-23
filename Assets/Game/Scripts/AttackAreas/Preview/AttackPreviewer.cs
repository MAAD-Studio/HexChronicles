using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackPreviewer : Singleton<AttackPreviewer>
{
    #region Variables

    private List<Tile> checkedTiles = new List<Tile>();
    private List<GameObject> tileTops = new List<GameObject>();

    private List<Tile> tileObjCheckedTiles = new List<Tile>();
    private List<GameObject> tileObjTileTops = new List<GameObject>();

    [Header("Spawn Info: ")]
    [SerializeField] private LayerMask tileLayer;
    [Range(0.1f, 2f)]
    [SerializeField] private float spawnHeight = 1f;

    [Header("Top Prefabs: ")]
    [SerializeField] private GameObject movementTop;
    [SerializeField] private GameObject attackTop;

    [Header("Marker Prefabs: ")]
    [SerializeField] private GameObject targetMarker;
    private GameObject spawnedMarker;

    #endregion

    #region UnityMethods

    private void Start()
    {
        Debug.Assert(movementTop != null, "The AttackPreviewer has not been provided a MovementTop Prefab");
        Debug.Assert(attackTop != null, "The AttackPreviewer has not been provided a AttackTop Prefab");
    }

    #endregion

    #region CustomMethods

    //Spawns in tops to visualize the area an Enemy can Move or Attack during its next turn
    public void PreviewMoveAttackArea(Enemy_Base enemy)
    {
        ClearAttackArea();

        PreviewOrigin previewArea = null;
        List<TileReporter> pointsToCheck = null;

        if (enemy.attackAreaPreview != null)
        {
            previewArea = Instantiate(enemy.attackAreaPreview);
            pointsToCheck = previewArea.GetComponentsInChildren<TileReporter>().ToList();
        }

        //Grabs the movement tiles for the Enemy
        List<Tile> tiles = new List<Tile>(Pathfinder.Instance.ReturnMovementTiles(enemy));
        checkedTiles = new List<Tile>(tiles);

        foreach(Tile tile in tiles)
        {
            tileTops.Add(Instantiate(movementTop, tile.transform.position + new Vector3(0, spawnHeight, 0), Quaternion.identity));

            //If a movement tile is on the edge of the movement range it checks for attacking tops
            if(tile.cost >= enemy.moveDistance && previewArea != null)
            {
                CheckAttackSpawns(tile, previewArea, pointsToCheck);
            }
        }

        //Spawns an indicator over the cloest enemy to display who the enemy is likely to target
        Character closestCharacter = enemy.LikelyTarget();

        if(closestCharacter != null)
        {
            spawnedMarker = Instantiate(targetMarker, closestCharacter.transform.position, Quaternion.identity);
        }
    }

    //Checks for any tiles that can be attacked from the provided edge tile
    private void CheckAttackSpawns(Tile tile, PreviewOrigin previewArea, List<TileReporter> pointsToCheck)
    {
        previewArea.transform.position = tile.transform.position + new Vector3(0, 1, 0);
        float rotation = 0;

        //Checks for attacking from each rotation on a hexagon
        for (int i = 0; i < 6; i++)
        {
            previewArea.transform.eulerAngles = new Vector3(0, rotation, 0);

            foreach(TileReporter point in pointsToCheck)
            {
                //Checks if the tile at the point needs an attack top
                ConfirmAttackTopSpawn(point);
            }
            //Spawns in the attack tops
            SpawnAttacks(previewArea, pointsToCheck);

            rotation += 60;
        }
    }

    //Uses a Ray to detect if the current point is over a tile needing an attack top
    private void ConfirmAttackTopSpawn(TileReporter point)
    {
        if (Physics.Raycast(point.transform.position, Vector3.down, out RaycastHit hit, spawnHeight + 0.9f, tileLayer))
        {
            Tile hitTile = hit.transform.GetComponent<Tile>();

            if (!hitTile.tileHasObject && !checkedTiles.Contains(hitTile))
            {
                point.currentTile = hitTile;
            }
        }
    }

    //Spawns in the attack tops and checks for any bloackages from tileObjects interupting the enemies attack
    private void SpawnAttacks(PreviewOrigin previewArea, List<TileReporter> pointsToCheck)
    {
        //Checks for any blockages
        previewArea.originTile.CheckBlockages(false);

        //Spawns tops
        foreach (TileReporter point in pointsToCheck)
        {
            if (point.currentTile != null)
            {
                Vector3 position = point.transform.position;
                position.y = spawnHeight;

                tileTops.Add(Instantiate(attackTop, position, Quaternion.identity));
                checkedTiles.Add(point.currentTile);
            }
        }

        //Resets the TileReporters
        previewArea.originTile.CheckBlockages(true);
    }

    public void ClearAttackArea()
    {
        foreach(GameObject top in tileTops)
        {
            Destroy(top);
        }

        checkedTiles.Clear();
        tileTops.Clear();

        if (spawnedMarker != null)
        {
            Destroy(spawnedMarker);
        }
    }

    public void PreviewAttackAreaTower(Tower tileObj)
    {
        List<Tile> edgeTiles;
        List<Tile> nextSet = new List<Tile>();

        tileObjCheckedTiles.Add(tileObj.AttachedTile);
        edgeTiles = Pathfinder.Instance.FindAdjacentTiles(tileObj.AttachedTile, false);

        for(int i = 0; i < tileObj.TileRange; i++)
        {
            foreach(Tile tile in edgeTiles)
            {
                Vector3 position = tile.transform.position;
                position.y = spawnHeight;

                tileObjTileTops.Add(Instantiate(attackTop, position, Quaternion.identity));
                tileObjCheckedTiles.Add(tile);

                foreach(Tile adjTile in Pathfinder.Instance.FindAdjacentTiles(tile, false))
                {
                    if(!tileObjCheckedTiles.Contains(adjTile))
                    {
                        nextSet.Add(adjTile);
                    }
                }
            }

            edgeTiles.Clear();
            edgeTiles = new List<Tile>(nextSet);
            nextSet.Clear();
        }
    }

    public void ClearAttackAreaTower()
    {
        foreach (GameObject top in tileObjTileTops)
        {
            Destroy(top);
        }

        tileObjCheckedTiles.Clear();
        tileObjTileTops.Clear();
    }

    #endregion
}
