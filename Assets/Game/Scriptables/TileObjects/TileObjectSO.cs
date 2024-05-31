using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjectData", menuName = "ScriptableObjects/TileObjectData")]
public class TileObjectSO : ScriptableObject
{
    public Sprite avatar;
    public string objectName;
    public string description;
    public float health;
    public float defense;
    public GameObject missText;
    public GameObject hitMarker;
}
