using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The skill to be selected in Skill Selection
public class ActiveSkillDisplay : MonoBehaviour
{
    public Button button;
    [SerializeField] private GameObject outline;
    [SerializeField] private Image skillshape;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI description;

    private ActiveSkillSO skill;
    private HeroSkillInfo heroSkill;

    public void Initialize(ActiveSkillSO skill, HeroSkillInfo heroSkill)
    {
        skillshape.sprite = skill.skillshape;
        skillName.text = skill.skillName;
        description.text = skill.description.DisplayKeywordDescription();

        this.heroSkill = heroSkill;
        this.skill = skill;

        button.onClick.AddListener(OnSelected);
        outline.SetActive(false);
    }

    public void OnSelected()
    {
        heroSkill.SkillSelected(skill);

        button.interactable = false;
        outline.SetActive(true);

        // Reset the other skill buttons
        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                child.GetComponent<ActiveSkillDisplay>().Reset();
            }
        }
    }

    public void Reset()
    {
        button.interactable = true;
        outline.SetActive(false);
    }
}
