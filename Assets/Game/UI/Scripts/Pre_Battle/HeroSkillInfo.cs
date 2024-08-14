using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSkillInfo : MonoBehaviour
{
    [SerializeField] public Button heroSkillBtn;
    [SerializeField] private Image avatar;
    [SerializeField] private Image element;
    [SerializeField] private Image skillShape;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI movement;
    [SerializeField] private TextMeshProUGUI health;

    [SerializeField] private HeroAttributesSO heroSO;

    private ActiveSkillSO selectedActiveSkill;

    public event Action<HeroSkillInfo,HeroAttributesSO> OnHeroSelected;

    void Start()
    {
        SetUIDisplay();
        heroSkillBtn.onClick.AddListener(OnSelected);
    }

    private void SetUIDisplay()
    {
        avatar.sprite = heroSO.attributes.avatar;
        element.sprite = Config.Instance.GetElementSprite(heroSO.attributes.elementType);
        skillShape.sprite = heroSO.activeSkillSO.skillshape;
        nameText.text = heroSO.attributes.name;
        attack.text = heroSO.attributes.attackDamage.ToString();
        movement.text = heroSO.attributes.movementRange.ToString();
        health.text = heroSO.attributes.health.ToString();
    }

    // When this hero is selected, pass the heroSO to the SkillSelectionUI
    public void OnSelected()
    {
        heroSkillBtn.interactable = false;

        // Reset the other hero buttons
        foreach (Transform child in transform.parent)
        {
            if (child != transform)
            {
                child.GetComponent<HeroSkillInfo>().heroSkillBtn.interactable = true;
            }
        }

        OnHeroSelected?.Invoke(this, heroSO);
    }

    public void SkillSelected(ActiveSkillSO skill)
    {
        selectedActiveSkill = skill;
        skillShape.sprite = skill.skillshape;
        heroSO.SetActiveSkill(skill);
    }
}
