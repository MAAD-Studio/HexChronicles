using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileCoordGizmo : MonoBehaviour
{
    public bool showTileCoord = false;

    private void OnDrawGizmos()
    {
        if (showTileCoord)
        {
            foreach (Transform child in transform)
            {
                Vector3 pos = child.position;
                pos.y += 0.2f;
                Handles.Label(pos, $"{child.name}");
            }
        }
    }
}
