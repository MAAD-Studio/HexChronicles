using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

// The skill card that will be displayed in Victory Screen
// Responsible for adding the skill to the ActiveSkillCollection
public class SkillRewardCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;

    [SerializeField] private Image skillshape;
    [SerializeField] private Image element;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Vector3 scaledUpSize = new Vector3(1.2f, 1.2f, 1.2f);

    private VictoryReward victoryReward;
    private ActiveSkillSO skill;
    private bool isSelectable = true;

    public ActiveSkillSO Skill => skill;

    private void Start()
    {
        button.onClick.AddListener(() => OnSelected());
    }

    public void SetSkillDisplay(ActiveSkillSO skill, VictoryReward victoryReward)
    {
        this.skill = skill;
        this.victoryReward = victoryReward;

        element.sprite = Config.Instance.GetElementSprite(skill.elementType);
        skillshape.sprite = skill.skillshape;
        skillName.text = skill.skillName;
        description.text = skill.description.DisplayKeywordDescription();

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void OnSelected()
    {
        victoryReward.OnSkillSelected(skill, this);

        button.interactable = false;
        isSelectable = false;
        transform.DOScale(scaledUpSize, 0.2f).SetEase(Ease.OutBack).From(new Vector3(1.15f, 1.15f, 1.15f));
    }

    public void OnUnselected()
    {
        button.interactable = true;
        isSelectable = true;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelectable)
        {
            transform.DOScale(scaledUpSize, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelectable)
        {
            transform.DOScale(Vector3.one, 0.2f);
        }
    }
}
