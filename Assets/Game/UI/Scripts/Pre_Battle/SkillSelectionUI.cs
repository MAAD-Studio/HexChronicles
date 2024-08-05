using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    public Transform skillList;
    public GameObject skillDisplayPrefab;

    private HeroAttributesSO currentHeroSO;
    private HeroSkillInfo currentHeroSkill;

    public void Initialize(List<HeroSkillInfo> heroSkillList)
    {
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

            // Disable the button of the current hero's skill
            if (currentHeroSO.activeSkillSO == skill)
            {
                skillButton.button.interactable = false;
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
