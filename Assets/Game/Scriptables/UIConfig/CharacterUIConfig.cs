using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CharacterUIConfig", menuName = "ScriptableObjects/CharacterUIConfig")]
public class CharacterUIConfig : ScriptableObject
{
    [Header("Element Sprites")]
    public Sprite fire;
    public Sprite water;
    public Sprite grass;
    public Sprite poison;
    
    [Header("Status Sprites")]
    public Sprite burning;
    public Sprite wet;
    public Sprite haste;
    public Sprite bound;
    public Sprite shield;

    [Header("Status VFX")]
    public GameObject burningVFX;
    //public GameObject wetVFX;
    //public GameObject hasteVFX;
    public GameObject boundVFX;
    //public GameObject shieldVFX;

    [Header("Status Description")]
    [TextArea(3, 10)] public string burningDetail;
    [TextArea(3, 10)] public string wetDetail;
    [TextArea(3, 10)] public string hasteDetail;
    [TextArea(3, 10)] public string boundDetail;
    [TextArea(3, 10)] public string shieldDetail;
}
