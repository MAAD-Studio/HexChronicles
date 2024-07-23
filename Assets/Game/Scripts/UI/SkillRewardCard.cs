using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// The skill card that will be displayed in Victory Screen
// Responsible for adding the skill to the ActiveSkillCollection
public class SkillRewardCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;

    [SerializeField] private Image skillshape;
    [SerializeField] private Image element;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI description;

    private ActiveSkillSO skill;
    private bool isSelectable = true;

    private void Start()
    {
        button.onClick.AddListener(() => OnSelected());
    }

    public void SetSkillDisplay(ActiveSkillSO skill)
    {
        this.skill = skill;
        element.sprite = Config.Instance.GetElementSprite(skill.elementType);
        skillshape.sprite = skill.skillshape;
        skillName.text = skill.skillName;
        description.text = skill.description.DisplayKeywordDescription();
    }

    private void OnSelected()
    {
        ActiveSkillCollection.Instance.PlayerAddSkill(skill);

        transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);

        // Make all skill cards Unselectable
        foreach (Transform child in transform.parent)
        {
            //if (child != transform)
            //{
                child.GetComponent<SkillRewardCard>().OnUnselectable();
            //}
        }
    }

    public void OnUnselectable()
    {
        isSelectable = false;
        button.interactable = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelectable)
        {
            transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelectable)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
