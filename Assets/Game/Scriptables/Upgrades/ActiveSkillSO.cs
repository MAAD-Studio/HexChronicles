using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ActiveSkill;

[CreateAssetMenu(menuName = "Character Attributes/Active Skill")]
public class ActiveSkillSO : ScriptableObject
{
    public ElementType elementType;
    public Sprite icon;
    public string skillName;
    public KeywordDescription description;
    public Sprite skillshape;
    public AttackArea shapeArea;
    public GameObject particleEffect;
    public GameObject soundEffect;

    public SkillEffect skillEffect;
    public int skillEffectValue;

    public Status status = new Status();

    [Header("SFX")]
    public AudioSO attackSFK;
}
