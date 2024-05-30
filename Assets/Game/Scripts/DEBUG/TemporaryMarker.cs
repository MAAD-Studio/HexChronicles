using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryMarker : MonoBehaviour
{
    public static GameObject GenerateMarker(GameObject marker, Vector3 location, float height)
    {
        Vector3 position = location;
        position.y += height;

        return Instantiate(marker, position, marker.transform.rotation);
    }

    public static void GenerateMarker(GameObject marker, Vector3 location, float height, float timeToDestroy)
    {
        Destroy(GenerateMarker(marker, location, height), timeToDestroy);
    }
}
