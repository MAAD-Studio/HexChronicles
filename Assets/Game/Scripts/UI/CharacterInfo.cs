using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject defaultState;
    public GameObject selectedState;
    public GameObject noActionState;
    public GameObject deadState;
    public CharacterUIConfig characterUIConfig;

    public List<TextMeshProUGUI> names;
    public List<Image> avatars;
    public List<Image> elements;
    public List<TextMeshProUGUI> textMovement;
    public List<TextMeshProUGUI> textAttack;
    public List<TextMeshProUGUI> textDef;
    public List<TextMeshProUGUI> textStatus;
    [HideInInspector] public Hero hero;

    [Header("HealthBar")]
    public List<TextMeshProUGUI> textHP;
    public List<Image> health;

    [Header("Hero")]
    public Image attackShape;
    public TextMeshProUGUI attackInfo;

    public Image skillShape;
    public TextMeshProUGUI skillInfo;
    public TextMeshProUGUI skillCD;

    [Header("Buttons")]
    public Button attackBtn;
    public Button skillBtn;

    private void Start()
    {
        hero.OnUpdateHealthBar += UpdateHealthBar;

        SetDefaultState();
        UpdateHealthBar(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        hero.OnUpdateHealthBar -= UpdateHealthBar;
    }

    private void UpdateHealthBar(object sender, EventArgs e)
    {
        foreach (Image hp in health)
        {
            hp.fillAmount = hero.currentHealth / hero.maxHealth;
        }
        foreach (TextMeshProUGUI hp in textHP)
        {
            hp.text = $"{hero.currentHealth} / {hero.maxHealth} HP";
        }
    }

    public void InitializeInfo()
    {
        foreach (TextMeshProUGUI name in names)
        {
            name.text = hero.heroSO.attributes.name;
        }
        foreach (Image avatar in avatars)
        {
            avatar.sprite = hero.heroSO.attributes.avatar;
        }
        
        attackShape.sprite = hero.heroSO.attackShape;
        attackInfo.text = hero.heroSO.attackInfo.DisplayKeywordDescription();
        attackInfo.ForceMeshUpdate();

        skillShape.sprite = hero.heroSO.activeSkill.skillshape;
        skillInfo.text = hero.heroSO.activeSkill.description.DisplayKeywordDescription();
        skillInfo.ForceMeshUpdate();
    }

    public void UpdateInfo()
    {
        foreach (TextMeshProUGUI movement in textMovement)
        {
            movement.text = hero.moveDistance.ToString();
        }
        foreach (TextMeshProUGUI attack in textAttack)
        {
            attack.text = hero.attackDamage.ToString();
        }
        foreach (TextMeshProUGUI def in textDef)
        {
            def.text = hero.defensePercentage.ToString() + "%";
        }

        if (hero.currentSkillCD > 0)
        {
            skillBtn.interactable = false;
            skillCD.text = $"(On Cooldown - {hero.currentSkillCD} turns)";
            attackBtn.interactable = false;
        }
        else
        {
            skillBtn.interactable = true;
            skillCD.gameObject.SetActive(false);
            attackBtn.interactable = true;
        }
        //selectHeroStatus.attackBtn.interactable = hero.canAttack;
        //selectHeroStatus.moveBtn.interactable = hero.canMove;
    }

    public void SetDefaultState()
    {
        Debug.Log("DefaultState");
        defaultState.SetActive(true);
        defaultState.GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
        defaultState.GetComponent<Outline>().enabled = false;
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetHoverState()
    {
        Debug.Log("HoverState");
        defaultState.SetActive(true);
        defaultState.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        defaultState.GetComponent<Outline>().enabled = true;
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetNoActionState()
    {
        Debug.Log("NoActionState");
        defaultState.SetActive(false);
        selectedState.SetActive(false);
        noActionState.SetActive(true);
        deadState.SetActive(false);
    }

    public void SetSelectedState()
    {
        Debug.Log("SelectedState");
        defaultState.SetActive(false);
        selectedState.SetActive(true);
        noActionState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetDeadState()
    {
        Debug.Log("DeadState");
        defaultState.SetActive(false);
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(true);
    }
}
