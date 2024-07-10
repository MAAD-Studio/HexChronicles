using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("States")]
    [SerializeField] private GameObject defaultState;
    [SerializeField] private GameObject selectedState;
    [SerializeField] private GameObject noActionState;
    [SerializeField] private GameObject deadState;

    [SerializeField] private Color buffColor = new Color(0f, 0.8f, 0f);
    [SerializeField] private Color debuffColor = Color.red;

    [Header("Hero Info")]
    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<Image> avatars;
    [SerializeField] private List<Image> elements;
    [SerializeField] private List<TextMeshProUGUI> textMovement;
    [SerializeField] private List<TextMeshProUGUI> textAttack;
    [SerializeField] private List<TextMeshProUGUI> textDef;
    [SerializeField] private GameObject statusField;
    [SerializeField] private GameObject statusPrefab;
    private Hero hero;

    [Header("HealthBar")]
    [SerializeField] private List<TextMeshProUGUI> textHP;
    [SerializeField] private List<Image> health;

    [Header("Attack and Skill")]
    [SerializeField] private Image attackShape;
    [SerializeField] private TextMeshProUGUI attackInfo;

    [SerializeField] private Image skillShape;
    [SerializeField] private TextMeshProUGUI skillInfo;
    [SerializeField] private TextMeshProUGUI skillCD;

    [Header("Buttons")]
    public Button attackBtn;
    public Button skillBtn;

    private bool interactable = true;
    private bool heroDead = false;

    [Header("StartValue")]
    private float startAttack;
    private int startMovement;


    private void Start()
    {
        SetDefaultState();
    }

    public void InitializeInfo(Hero hero)
    {
        AssignHero(hero);

        startAttack = hero.heroSO.attributes.attackDamage;
        startMovement = hero.heroSO.attributes.movementRange;

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
            element.sprite = Config.Instance.GetElementSprite(hero.heroSO.attributes.elementType);
        }

        attackShape.sprite = hero.heroSO.attackShape;
        attackInfo.text = hero.heroSO.attackInfo.DisplayKeywordDescription();
        attackInfo.ForceMeshUpdate();

        skillShape.sprite = hero.heroSO.activeSkillSO.skillshape;
        skillInfo.text = hero.heroSO.activeSkillSO.description.DisplayKeywordDescription();
        skillInfo.ForceMeshUpdate();
    }

    public void AssignHero(Hero hero)
    {
        this.hero = hero;
        SubscribeEvents();
    }

    #region Update Info
    private void UpdateAttributes()
    {
        foreach (TextMeshProUGUI movement in textMovement)
        {
            int newMove = hero.moveDistance - hero.movementThisTurn;
            if (newMove > startMovement) // Show buff text in green
            {
                string buffValue = $"<color=#{ColorUtility.ToHtmlStringRGBA(buffColor)}> + {newMove - startMovement}</color>";
                movement.text = $"{startMovement}{buffValue}";
            }
            else if (newMove < startMovement) // Show debuff text in red
            {
                string deBuffValue = $"<color=#{ColorUtility.ToHtmlStringRGBA(debuffColor)}> - {startMovement - newMove}</color>";
                movement.text = $"{startMovement}{deBuffValue}";
            }
            else
            {
                movement.text = newMove.ToString();
            }
        }

        foreach (TextMeshProUGUI attack in textAttack)
        {
            float newAttack = hero.attackDamage;
            if (newAttack > startAttack)
            {
                string buffValue = $"<color=#{ColorUtility.ToHtmlStringRGBA(buffColor)}> + {newAttack - startAttack}</color>";
                attack.text = $"{startAttack}{buffValue}";
            }
            else if (newAttack < startAttack)
            {
                string deBuffValue = $"<color=#{ColorUtility.ToHtmlStringRGBA(debuffColor)}> - {startAttack - newAttack}</color>";
                attack.text = $"{startAttack}{deBuffValue}";
            }
            else
            {
                attack.text = newAttack.ToString();
            }
        }
        foreach (TextMeshProUGUI def in textDef)
        {
            def.text = hero.defensePercentage.ToString() + "%";
        }
    }

    private void UpdateStatus()
    {
        List<Status> newStatus = hero.statusList;

        foreach (Transform child in statusField.transform)
        {
            Destroy(child.gameObject);
        }

        if (newStatus != null)
        {
            foreach (var status in newStatus)
            {
                GameObject statusObject = Instantiate(statusPrefab);
                statusObject.transform.SetParent(statusField.transform);

                StatusEffect statusEffect = statusObject.GetComponent<StatusEffect>();
                statusEffect.Initialize(status);
            }
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
    #endregion

    #region Events
    private void SubscribeEvents()
    {
        EventBus.Instance.Subscribe<OnPlayerTurn>(OnPlayerTurn);
        EventBus.Instance.Subscribe<OnEnemyTurn>(OnEnemyTurn);
        hero.UpdateHealthBar.AddListener(UpdateHealthBar);
        hero.UpdateAttributes.AddListener(UpdateAttributes);
        hero.UpdateStatus.AddListener(UpdateStatus);
    }

    private void UnsubscribeEvents()
    {
        EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnPlayerTurn);
        EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnEnemyTurn);
        hero.UpdateHealthBar.RemoveListener(UpdateHealthBar);
        hero.UpdateAttributes.RemoveListener(UpdateAttributes);
        hero.UpdateStatus.RemoveListener(UpdateStatus);
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
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
        if (hero.CurrentSkillCD > 0)
        {
            skillBtn.interactable = false;
            skillCD.text = $"(On Cooldown - {hero.CurrentSkillCD} turns)";
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
    #endregion


    #region States
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

    public void SetRestoreState(Hero hero)
    {
        if (heroDead) // Assign dead first to prevent the return
        {
            heroDead = false;
            UnsubscribeEvents();
            AssignHero(hero);
        }

        interactable = !hero.hasMadeDecision;
        if (interactable)
        {
            SetDefaultState();
        }
        else
        {
            SetNoActionState();
        }
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
        statusField.SetActive(false);
        defaultState.SetActive(false);
        selectedState.SetActive(false);
        noActionState.SetActive(false);
        deadState.SetActive(true);
    }
    #endregion
}
