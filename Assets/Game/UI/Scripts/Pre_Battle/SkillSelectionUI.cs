using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    public Transform skillList;
    public GameObject skillDisplayPrefab;

    private List<HeroSkillInfo> heroSkillList;
    private HeroAttributesSO currentHeroSO;
    private HeroSkillInfo currentHeroSkill;

    public void Initialize(List<HeroSkillInfo> heroSkillList)
    {
        this.heroSkillList = heroSkillList;
        foreach (HeroSkillInfo heroSkill in heroSkillList)
        {
            heroSkill.OnHeroSelected += UpdateSkillList;
        }
    }

    // Reset the list and get new skills when a new hero is selected
    private void UpdateSkillList(HeroSkillInfo heroSkill, HeroAttributesSO heroSO)
    {
        currentHeroSkill = heroSkill;
        currentHeroSO = heroSO;

        foreach (HeroSkillInfo heroSkillInfo in heroSkillList)
        {
            if (heroSkillInfo != currentHeroSkill)
            {
                heroSkillInfo.Reset();
            }
        }

        ResetSkillList();
        DisplaySkills(ActiveSkillCollection.Instance.GetPlayerSkills(heroSO.attributes.elementType));
    }

    // Display every skills from the collection list, and bind the button to change skill
    private void DisplaySkills(List<ActiveSkillSO> skills)
    {
        ResetSkillList();

        foreach (ActiveSkillSO skill in skills)
        {
            GameObject skillDisplay = Instantiate(skillDisplayPrefab, skillList);
            ActiveSkillDisplay skillButton = skillDisplay.GetComponent<ActiveSkillDisplay>();
            skillButton.Initialize(skill, currentHeroSkill);

            // Select the current hero's skill
            if (currentHeroSO.activeSkillSO == skill)
            {
                skillButton.OnSelected();
            }

            // Refresh the list panel layout group
            LayoutRebuilder.ForceRebuildLayoutImmediate(skillList.GetComponent<RectTransform>());
        }
    }

    private void ResetSkillList()
    {
        foreach (Transform child in skillList)
        {
            Destroy(child.gameObject);
        }
    }
}
