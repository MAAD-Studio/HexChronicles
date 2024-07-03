using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    [Header("UI")]
    public Transform skillList;
    public GameObject skillDisplayPrefab;

    [Header("HeroSkillInfo Parent")]
    [SerializeField] private GameObject heroList;

    private HeroAttributesSO heroSO;

    private void Start()
    {
        // Get every HeroSkillInfo from heroList:
        List<HeroSkillInfo> heroSkillList = new List<HeroSkillInfo>(heroList.GetComponentsInChildren<HeroSkillInfo>());
        foreach (HeroSkillInfo heroSkill in heroSkillList)
        {
            heroSkill.OnHeroSelected += UpdateSkillList;
        }

        // Invoke the first hero in the list as the default display
        heroSkillList[0].OnSelected();
    }

    // Reset the list and get new skills when a new hero is selected
    private void UpdateSkillList(HeroAttributesSO heroSO)
    {
        if (this.heroSO != heroSO)
        {
            this.heroSO = heroSO;
            ResetSkillList();
            GetSkillsFromCollection(heroSO.attributes.elementType);
        }
    }

    private void GetSkillsFromCollection(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Fire:
                DisplaySkills(ActiveSkillCollection.Instance.fireSkills);
                break;
            case ElementType.Grass:
                DisplaySkills(ActiveSkillCollection.Instance.grassSkills);
                break;
            case ElementType.Water:
                DisplaySkills(ActiveSkillCollection.Instance.waterSkills);
                break;
        }
    }

    // Display every skills from the collection list, and bind the button to change skill
    private void DisplaySkills(List<ActiveSkillSO> skills)
    {
        foreach (ActiveSkillSO skill in skills)
        {
            GameObject skillDisplay = Instantiate(skillDisplayPrefab, skillList);
            ActiveSkillDisplay skillButton = skillDisplay.GetComponent<ActiveSkillDisplay>();
            skillButton.SetSkillDisplay(skill);

            skillButton.button.onClick.AddListener(() =>
            {
                heroSO.SetActiveSkill(skill);
                skillButton.OnSelected();
            });

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
