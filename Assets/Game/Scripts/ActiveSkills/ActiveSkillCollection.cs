using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkillCollection : Singleton<ActiveSkillCollection>
{
    [SerializeField] private List<ActiveSkillSO> allFireSkills;
    [SerializeField] private List<ActiveSkillSO> allGrassSkills;
    [SerializeField] private List<ActiveSkillSO> allWaterSkills;

    [SerializeField] private List<ActiveSkillSO> playerFireSkills;
    [SerializeField] private List<ActiveSkillSO> playerGrassSkills;
    [SerializeField] private List<ActiveSkillSO> playerWaterSkills;

    private List<ActiveSkillSO> remainFireSkills;
    private List<ActiveSkillSO> remainGrassSkills;
    private List<ActiveSkillSO> remainWaterSkills;

    private void Start()
    {
        remainFireSkills = new List<ActiveSkillSO>(allFireSkills);
        remainGrassSkills = new List<ActiveSkillSO>(allGrassSkills);
        remainWaterSkills = new List<ActiveSkillSO>(allWaterSkills);

        // Add the first skills to heroes
        PlayerAddSkill(allFireSkills[0]);
        PlayerAddSkill(allGrassSkills[0]);
        PlayerAddSkill(allWaterSkills[0]);
    }

    public ActiveSkillSO GetRandomSkillReward(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                if (remainFireSkills.Count == 0)
                {
                    return null;
                }
                return remainFireSkills[Random.Range(0, remainFireSkills.Count)];
            case ElementType.Grass:
                if (remainGrassSkills.Count == 0)
                {
                    return null;
                }
                return remainGrassSkills[Random.Range(0, remainGrassSkills.Count)];
            case ElementType.Water:
                if (remainWaterSkills.Count == 0)
                {
                    return null;
                }
                return remainWaterSkills[Random.Range(0, remainWaterSkills.Count)];
            default:
                return null;
        }
    }

    public void PlayerAddSkill(ActiveSkillSO skill)
    {
        switch (skill.elementType)
        {
            case ElementType.Fire:
                if (playerFireSkills.Contains(skill) == false)
                {
                    playerFireSkills.Add(skill);
                    remainFireSkills.Remove(skill);
                }
                break;
            case ElementType.Grass:
                if (playerGrassSkills.Contains(skill) == false)
                {
                    playerGrassSkills.Add(skill);
                    remainGrassSkills.Remove(skill);
                }
                break;
            case ElementType.Water:
                if (playerWaterSkills.Contains(skill) == false)
                {
                    playerWaterSkills.Add(skill);
                    remainWaterSkills.Remove(skill);
                }
                break;
        }
    }

    public List<ActiveSkillSO> GetPlayerSkills(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                return playerFireSkills;
            case ElementType.Grass:
                return playerGrassSkills;
            case ElementType.Water:
                return playerWaterSkills;
            default:
                return null;
        }
    }

    public List<ActiveSkillSO> PlayerSkills()
    {
        List<ActiveSkillSO> playerSkills = new List<ActiveSkillSO>(playerFireSkills);
        playerSkills.AddRange(playerGrassSkills);
        playerSkills.AddRange(playerWaterSkills);
        return playerSkills;
    }

    public void LoadPlayerSkills(List<ActiveSkillSO> playerSkills)
    {
        foreach (ActiveSkillSO skill in playerSkills)
        {
            PlayerAddSkill(skill);
        }
    }
}
