using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The skill card that will be displayed in Victory Screen
// Responsible for adding the skill to the ActiveSkillCollection
public class SkillRewardCard : MonoBehaviour
{
    public Button button;

    [SerializeField] private Image skillshape;
    [SerializeField] private Image element;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private CharacterUIConfig characterUIConfig;

    private ActiveSkillSO skill;

    private void Start()
    {
        button.onClick.AddListener(() => OnSelected());
    }

    public void SetSkillDisplay(ActiveSkillSO skill)
    {
        this.skill = skill;
        element.sprite = characterUIConfig.GetElementSprite(skill.elementType);
        skillshape.sprite = skill.skillshape;
        skillName.text = skill.skillName;
        description.text = skill.description.DisplayKeywordDescription();
    }

    private void OnSelected()
    {
        ActiveSkillCollection.Instance.PlayerAddSkill(skill);

        button.interactable = false;
    }
}
