using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    #region Variables

    private const float heightOffset = 0.33f;
    LineRenderer lineRenderer;

    #endregion

    #region UnityMethods

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Debug.Assert(lineRenderer != null, $"{gameObject.name}'s PathIllustrator couldn't find the LineRenderer component.");
    }

    #endregion

    #region BreadthFirstMethods

    //Illustrates the path provided with a line
    public void IllustratePath(Tile[] path)
    {
        lineRenderer.positionCount = path.Length;

        for (int i = 0; i < path.Length; i++)
        {
            Vector3 tileTranform = path[i].transform.position;
            tileTranform.y += heightOffset;
            lineRenderer.SetPosition(i, tileTranform);
        }
    }

    //Illustrates the frontier provided by changing the material of the tiles in the frontier
    public void IllustrateFrontier(List<Tile> frontier, TurnEnums.PathfinderTypes type)
    {
        if(type == TurnEnums.PathfinderTypes.Movement)
        {
            foreach (Tile tile in frontier)
            {
                tile.ChangeTileColor(TileEnums.TileMaterial.frontier);
            }
        }
        else if(type == TurnEnums.PathfinderTypes.EnemyBasicAttack || type == TurnEnums.PathfinderTypes.EnemyMovement)
        {

        }
        else
        {
            foreach (Tile tile in frontier)
            {
                tile.ChangeTileColor(TileEnums.TileMaterial.attackable);
            }
        }
    }

    //Clears any active line illustrations
    public void ClearIllustrations()
    {
        lineRenderer.positionCount = 0;
    }

    #endregion
}
