using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillCollection : Singleton<ActiveSkillCollection>
{
    public List<ActiveSkillSO> allFireSkills;
    public List<ActiveSkillSO> allGrassSkills;
    public List<ActiveSkillSO> allWaterSkills;

    public List<ActiveSkillSO> fireSkills;
    public List<ActiveSkillSO> grassSkills;
    public List<ActiveSkillSO> waterSkills;

    public ActiveSkillSO GetRandomSkill(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return allFireSkills[Random.Range(0, allFireSkills.Count)];
            case ElementType.Grass:
                return allGrassSkills[Random.Range(0, allGrassSkills.Count)];
            case ElementType.Water:
                return allWaterSkills[Random.Range(0, allWaterSkills.Count)];
            default:
                return null;
        }
    }
}
