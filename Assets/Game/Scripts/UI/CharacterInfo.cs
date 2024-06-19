using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    private bool interactable = true;
    private bool heroDead = false;

    private void Start()
    {
        SetDefaultState();
    }

    private void UpdateAttributes()
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
    }

    private void UpdateStatus()
    {
        string statusString = characterUIConfig.GetStatusTypes(hero).ToString();
        foreach (TextMeshProUGUI status in textStatus)
        {
            status.text = statusString;
        }
    }

    private void UpdateHealthBar()
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

    public void InitializeInfo(Hero hero)
    {
        this.hero = hero;
        SubscribeEvents();

        foreach (TextMeshProUGUI name in names)
        {
            name.text = hero.heroSO.attributes.name;
        }
        foreach (Image avatar in avatars)
        {
            avatar.sprite = hero.heroSO.attributes.avatar;
        }
        foreach (Image element in elements)
        {
            element.sprite = characterUIConfig.GetElementSprite(hero.elementType);
        }

        attackShape.sprite = hero.heroSO.attackShape;
        attackInfo.text = hero.heroSO.attackInfo.DisplayKeywordDescription();
        attackInfo.ForceMeshUpdate();

        skillShape.sprite = hero.heroSO.activeSkill.skillshape;
        skillInfo.text = hero.heroSO.activeSkill.description.DisplayKeywordDescription();
        skillInfo.ForceMeshUpdate();
    }

    private void SubscribeEvents()
    {
        EventBus.Instance.Subscribe<OnPlayerTurn>(OnPlayerTurn);
        EventBus.Instance.Subscribe<OnEnemyTurn>(OnEnemyTurn);
        hero.UpdateHealthBar.AddListener(UpdateHealthBar);
        hero.UpdateAttributes.AddListener(UpdateAttributes);
        hero.UpdateStatus.AddListener(UpdateStatus);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnPlayerTurn);
        EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnEnemyTurn);
        hero.UpdateHealthBar.RemoveListener(UpdateHealthBar);
        hero.UpdateAttributes.RemoveListener(UpdateAttributes);
        hero.UpdateStatus.RemoveListener(UpdateStatus);
    }

    private void OnPlayerTurn(object obj)
    {
        interactable = true;
        UpdateButton();
        SetDefaultState();
    }

    private void OnEnemyTurn(object obj)
    {
        interactable = false;
        SetNoActionState();
    }

    public void UpdateButton()
    {
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHoverState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultState();
    }

    public void SetDefaultState()
    {
        if (!interactable || heroDead)
        {
            return;
        }
        defaultState.SetActive(true);
        defaultState.GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
        defaultState.GetComponent<Outline>().enabled = false;
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetHoverState()
    {
        if (!interactable || heroDead)
        {
            return;
        }
        defaultState.SetActive(true);
        defaultState.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        defaultState.GetComponent<Outline>().enabled = true;
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetNoActionState()
    {
        if (heroDead)
        {
            return;
        }
        interactable = false;
        defaultState.SetActive(false);
        selectedState.SetActive(false);
        noActionState.SetActive(true);
        deadState.SetActive(false);
    }

    public void SetSelectedState()
    {
        if (!interactable || heroDead)
        {
            return;
        }
        defaultState.SetActive(false);
        selectedState.SetActive(true);
        noActionState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetDeadState()
    {
        heroDead = true;
        defaultState.SetActive(false);
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(true);
    }
}
