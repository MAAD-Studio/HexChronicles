using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDInfo : MonoBehaviour
{
    private TurnManager turnManager;
    private PlayerTurn playerTurn;
    private List<Enemy_Base> enemies;
    private List<Hero> heroes = new List<Hero>();
    private Character selectedCharacter;
    private Tile currentTile;

    [Header("Turn Info")]
    [SerializeField] private TextMeshProUGUI currentTurn;
    [SerializeField] private TextMeshProUGUI turnNumber;

    [Header("Hero Info")]
    [SerializeField] private GameObject heroListPanel;
    [SerializeField] private GameObject heroInfoPrefab;
    [SerializeField] private GameObject heroStatusPrefab;
    private CharacterStatsUI selectHeroStatus;

    [Header("Enemy Info")]
    [SerializeField] private GameObject enemyInfoPanel;
    private Image enemyImage;
    private TextMeshProUGUI enemyName;
    private TextMeshProUGUI enemyBehavior;

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoPanel;
    private Image tileImage;
    private TextMeshProUGUI tileName;
    private TextMeshProUGUI tileEffects;

    [Header("Buttons")]
    [SerializeField] private Button endTurn;
    [SerializeField] private Button undo;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();

        InstantiateUIElements();
        GetUIComponents();
        ButtonsAddListener();

        enemies = turnManager.enemyList;
    }

    private void Update()
    {
        if (turnManager == null)
        {
            return;
        }
        UpdateTurnInfo();
        UpdateSelectedCharacterInfo();
        UpdateEnemyInfo();
        UpdateTileInfo();
    }

    #region Initialization Methods

    private void InstantiateUIElements()
    {
        foreach (Character character in turnManager.characterList)
        {
            Hero hero = (Hero)character;
            heroes.Add(hero);

            // Create heroInfoPrefab in Character List:
            GameObject gameObject = Instantiate(heroInfoPrefab);
            gameObject.transform.SetParent(heroListPanel.transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            // Display Hero Info:
            TextMeshProUGUI heroName = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            heroName.text = hero.heroSO.attributes.name;
            Image avatar = gameObject.transform.GetChild(1).GetComponent<Image>();
            avatar.sprite = hero.heroSO.attributes.avatar;
            Image skillIcon = gameObject.transform.GetChild(2).GetComponent<Image>();
            skillIcon.sprite = hero.heroSO.activeSkill.icon;
        }

        // Create heroInfoPrefab in Character List:
        GameObject statusGO = Instantiate(heroStatusPrefab);
        statusGO.transform.SetParent(heroListPanel.transform);
        statusGO.transform.localScale = new Vector3(1, 1, 1);
        selectHeroStatus = statusGO.GetComponent<CharacterStatsUI>();
    }

    private void GetUIComponents()
    {
        enemyImage = enemyInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        enemyName = enemyInfoPanel.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        enemyBehavior = enemyInfoPanel.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();

        tileImage = tileInfoPanel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        tileName = tileInfoPanel.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        tileEffects = tileInfoPanel.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
    }

    private void ButtonsAddListener()
    {
        selectHeroStatus.moveBtn.onClick.AddListener(() => playerTurn.SwitchToMovement());
        selectHeroStatus.attackBtn.onClick.AddListener(() => playerTurn.SwitchToBasicAttack());
        selectHeroStatus.skillBtn.onClick.AddListener(() => playerTurn.SwitchToActiveSkill());
        endTurn.onClick.AddListener(() => playerTurn.EndTurn());
        //undo.onClick.AddListener(() => playerTurn.UndoLastAction());
        undo.interactable = false;
    }
    #endregion

    #region Update Methods

    private void UpdateTurnInfo()
    {
        if (turnManager.CurrentTurn is PlayerTurn)
        {
            currentTurn.text = "PLAYER TURN";
            endTurn.interactable = true;
        }
        else if (turnManager.CurrentTurn is EnemyTurn)
        {
            currentTurn.text = "ENEMY TURN";
            endTurn.interactable = false;
        }

        turnNumber.text = turnManager.TurnNumber.ToString();
    }

    private void UpdateSelectedCharacterInfo()
    {
        selectedCharacter = playerTurn.SelectedCharacter;
        if (selectedCharacter != null)
        {
            selectHeroStatus.gameObject.SetActive(true);
            Hero hero = selectedCharacter as Hero;
            //if CannotAttack or Freezed
            if (hero.statusList.Exists(x => x.statusType == Status.StatusTypes.CannotAttack)
                || hero.statusList.Exists(x => x.statusType == Status.StatusTypes.Freezed))
            {
                selectHeroStatus.attackBtn.interactable = false;
            }
            else
            {
                selectHeroStatus.attackBtn.interactable = true;
            }

            //if CannotMove or Freezed
            if (hero.statusList.Exists(x => x.statusType == Status.StatusTypes.CannotMove)
                || hero.statusList.Exists(x => x.statusType == Status.StatusTypes.Freezed))
            {
                selectHeroStatus.moveBtn.interactable = false;
            }
            else
            {
                selectHeroStatus.moveBtn.interactable = true;
            }

            //if CannotUseSkill or has SkillCD
            if (hero.statusList.Exists(x => x.statusType == Status.StatusTypes.CannotUseSkill)
                || hero.currentSkillCD > 0)
            {
                selectHeroStatus.skillBtn.interactable = false;
            }
            else
            {
                selectHeroStatus.skillBtn.interactable = true;
            }
            selectHeroStatus.avatar.sprite = hero.heroSO.attributes.avatar;
            selectHeroStatus.textName.text = hero.heroSO.attributes.name;
            selectHeroStatus.textType.text = "Type: " + selectedCharacter.elementType;
            selectHeroStatus.textHP.text = "HP: " + selectedCharacter.currentHealth + " / " + selectedCharacter.maxHealth;
            selectHeroStatus.textMovement.text = "MOV: " + selectedCharacter.moveDistance;
            selectHeroStatus.textAttack.text = "ATK: " + selectedCharacter.attackDamage;
            selectHeroStatus.textDef.text = "Def: " + selectedCharacter.defensePercentage;
            selectHeroStatus.skillInfo.text = hero.heroSO.activeSkill.DisplaySkillDetail();
            selectHeroStatus.skillInfo.ForceMeshUpdate();
            selectHeroStatus.textStatus.text = "Status: " + GetStatusTypes(selectedCharacter);

        }
        else
        {
            selectHeroStatus.gameObject.SetActive(false);
        }
    }

    private string GetStatusTypes(Character character)
    {
        string statusList = "";
        foreach (var status in character.statusList)
        {
            statusList += status.statusType.ToString() + ", ";
        }
        return statusList;
    }

    private void UpdateEnemyInfo()
    {
        currentTile = playerTurn.CurrentTile;

        if (currentTile != null && currentTile.characterOnTile != null && currentTile.characterOnTile is Enemy_Base)
        {
            enemyInfoPanel.gameObject.SetActive(true);
            Enemy_Base enemy = currentTile.characterOnTile as Enemy_Base;
            enemyImage.sprite = enemy.enemySO.attributes.avatar;
            enemyName.text = enemy.enemySO.attributes.name;
            enemyBehavior.text = enemy.enemySO.attributes.description;
        }
        else
        {
            enemyInfoPanel.gameObject.SetActive(false);
        }
    }

    private void UpdateTileInfo()
    {
        currentTile = playerTurn.CurrentTile;
        if (currentTile != null)
        {
            tileInfoPanel.gameObject.SetActive(true);
            tileImage.sprite = currentTile.tileData.tileSprite;
            tileName.text = currentTile.tileData.name;
            tileEffects.text = currentTile.tileData.tileEffects;
        }
        else
        {
            tileInfoPanel.gameObject.SetActive(false);
        }
    }
    #endregion
}
