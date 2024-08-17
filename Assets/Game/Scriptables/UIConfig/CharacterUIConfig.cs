using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CharacterUIConfig", menuName = "ScriptableObjects/CharacterUIConfig")]
public class CharacterUIConfig : ScriptableObject
{
    [Header("Stats Sprites")]
    public Sprite health;
    public Sprite attack;
    public Sprite movement;

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
    public Sprite attackBoost;
    public Sprite movementReduction;

    [Header("Status VFX")]
    public GameObject burningVFX;
    public GameObject wetVFX;
    //public GameObject hasteVFX;
    public GameObject boundVFX;
    public GameObject shieldVFX;
    public GameObject mindControlVFX;

    [Header("Tile Effect VFX")]
    public GameObject fireBuffVFX;
    public GameObject grassBuffVFX;
    public GameObject waterBuffVFX;
    public GameObject debuffVFX;

    [Header("Tile Effect Special")]
    public GameObject fireBurnVFX;

    [Header("Tile Effect Short")]
    public GameObject shortFireBuffVFX;
    public GameObject shortGrassBuffVFX;
    public GameObject shortWaterBuffVFX;
    public GameObject shortDebuffVFX;

    [Header("Status Description")]
    [TextArea(3, 10)] public string burningDetail;
    [TextArea(3, 10)] public string wetDetail;
    [TextArea(3, 10)] public string hasteDetail;
    [TextArea(3, 10)] public string boundDetail;
    [TextArea(3, 10)] public string shieldDetail;
    [TextArea(3, 10)] public string attackBoostDetail;
    [TextArea(3, 10)] public string movementReductionDetail;

    [Header("Weather Sprites")]
    public Sprite rain;
    public Sprite sporeStorm;
    public Sprite heatWave;
}
