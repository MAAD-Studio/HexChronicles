using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAttributes
{
    public Sprite avatar;
    public string name;
    public KeywordDescription description;

    [Header("Basic Attributes")]
    public float health = 0;
    public int movementRange = 0;
    public float attackDamage = 0;
    public int attackRange = 0;
    public float defensePercentage = 0;
    [SerializeField] public TurnEnums.CharacterType characterType;
    public Vector3 defaultScale = Vector3.one;

    [Header("Elements:")]
    public ElementType elementType;
    public ElementType elementWeakAgainst;
    public ElementType elementStrongAgainst;

    [Header("Area Prefabs:")]
    public PreviewOrigin attackAreaPreview;
    [HideInInspector] public AttackArea basicAttackArea;
    [HideInInspector] public AttackArea activeSkillArea;

    [Header("Marker Prefabs:")]
    public GameObject hitMarker;
    public GameObject buffPrefab;

    [Header("Temporary Markers")]
    public GameObject fireMarker;
    public GameObject healText;

    [Header("SFX: ")]
    public AudioSO basicAttackSFX;

    /*[Header("Max Upgrade")]
    public float maxHealth = 0;
    public float maxMovementRange = 0;
    public float maxAttackDamage = 0;
    public float maxAttackRange = 0;
    public float maxDefense = 0;*/

    //[Header("Upgrades")]
    //public List<StatModifier> statModifiers = new List<StatModifier>();
}