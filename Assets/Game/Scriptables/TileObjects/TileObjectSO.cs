using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileObjectData", menuName = "ScriptableObjects/TileObjectData")]
public class TileObjectSO : ScriptableObject
{
    public Sprite avatar;
    public string objectName;
    public KeywordDescription description;
    public float health;
    public float defense;
    public GameObject hitMarker;

    public AudioSO SFX;
}
