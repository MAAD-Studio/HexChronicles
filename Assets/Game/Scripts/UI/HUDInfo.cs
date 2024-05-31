using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDInfo : MonoBehaviour
{
    [SerializeField] private GameObject tempIntro;
    [SerializeField] private Button tempButton;

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
    [SerializeField] private GameObject enemyStatusPrefab;
    private EnemyStatsUI enemyStatus;

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoPanel;
    [SerializeField] private Image tileImage;
    [SerializeField] private Image tileElement;
    [SerializeField] private TextMeshProUGUI tileName;
    [SerializeField] private TextMeshProUGUI tileEffects;

    [Header("Buttons")]
    [SerializeField] private Button endTurn;
    [SerializeField] private Button undo;

    [Header("Element Icons")]
    [SerializeField] private Sprite[] elementSprites;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();

        InstantiateUIElements();
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
        GameObject heroUI = Instantiate(heroStatusPrefab);
        heroUI.transform.SetParent(heroListPanel.transform);
        selectHeroStatus = heroUI.GetComponent<CharacterStatsUI>();

        // Create enemyInfoPrefab:
        GameObject enemyUI = Instantiate(enemyStatusPrefab);
        enemyUI.transform.SetParent(enemyInfoPanel.transform);
        enemyUI.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyStatus = enemyUI.GetComponent<EnemyStatsUI>();
    }

    private void ButtonsAddListener()
    {
        //selectHeroStatus.moveBtn.onClick.AddListener(() => playerTurn.SwitchToMovement());
        selectHeroStatus.attackBtn.onClick.AddListener(() => playerTurn.SwitchToBasicAttack());
        selectHeroStatus.skillBtn.onClick.AddListener(() => playerTurn.SwitchToSpecialAttack());
        endTurn.onClick.AddListener(() => playerTurn.EndTurn());
        //undo.onClick.AddListener(() => playerTurn.UndoLastAction());
        undo.interactable = false;
        tempButton.onClick.AddListener(() => tempIntro.SetActive(false));
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

            selectHeroStatus.attackBtn.interactable = hero.canAttack;
            selectHeroStatus.moveBtn.interactable = hero.canMove;

            if (hero.currentSkillCD > 0)
            {
                selectHeroStatus.skillBtn.interactable = false;
                selectHeroStatus.skillCD.text = $"(On Cooldown - {hero.currentSkillCD} turns)";
            }
            else
            {
                selectHeroStatus.skillBtn.interactable = true;
                selectHeroStatus.skillCD.gameObject.SetActive(false);
            }

            // Display Status:
            selectHeroStatus.avatar.sprite = hero.heroSO.attributes.avatar;
            selectHeroStatus.skillShape.sprite = hero.heroSO.activeSkill.skillshape;
            selectHeroStatus.element.sprite = GetElementSprite(selectedCharacter.elementType);
            selectHeroStatus.textName.text = hero.heroSO.attributes.name;
            selectHeroStatus.textHP.text = $"{selectedCharacter.currentHealth} / {selectedCharacter.maxHealth}";
            selectHeroStatus.textMovement.text = $"{selectedCharacter.moveDistance}";
            selectHeroStatus.textAttack.text = $"{selectedCharacter.attackDamage}";
            selectHeroStatus.textDef.text = $"{selectedCharacter.defensePercentage}%";

            selectHeroStatus.attackShape.sprite = hero.heroSO.attackShape;
            selectHeroStatus.attackInfo.text = hero.heroSO.attackInfo.ToString();

            selectHeroStatus.skillInfo.text = hero.heroSO.activeSkill.DisplaySkillDetail();
            selectHeroStatus.skillInfo.ForceMeshUpdate();
            selectHeroStatus.textStatus.text = GetStatusTypes(selectedCharacter).ToString();
        }
        else
        {
            selectHeroStatus.gameObject.SetActive(false);
        }
    }

    private Sprite GetElementSprite(ElementType element)
    {
        if (element == ElementType.Fire)
        {
            return elementSprites[0];
        }
        else if (element == ElementType.Water)
        {
            return elementSprites[1];
        }
        else if (element == ElementType.Grass)
        {
            return elementSprites[2];
        }
        else
        {
            return null;
        }
    }

    private string GetStatusTypes(Character character)
    {
        if (character.statusList.Count != 0)
        {
            string statusList = "Status: ";
            foreach (var status in character.statusList)
            {
                statusList += status.statusType.ToString() + ", ";
            }
            return statusList;
        }
        
        return "";
    }

    private void UpdateEnemyInfo()
    {
        currentTile = playerTurn.CurrentTile;

        if (currentTile != null && currentTile.characterOnTile != null && currentTile.characterOnTile is Enemy_Base)
        {
            enemyInfoPanel.gameObject.SetActive(true);
            Enemy_Base enemy = currentTile.characterOnTile as Enemy_Base;

            // Display Status:
            enemyStatus.avatar.sprite = enemy.enemySO.attributes.avatar;
            enemyStatus.element.sprite = GetElementSprite(enemy.elementType);
            enemyStatus.textName.text = enemy.enemySO.attributes.name;
            enemyStatus.enemyInfo.text = enemy.enemySO.attributes.description;
            enemyStatus.textHP.text = $"{enemy.currentHealth} / {enemy.maxHealth}";
            enemyStatus.textMovement.text = $"{enemy.moveDistance}";
            enemyStatus.textAttack.text = $"{enemy.attackDamage}";
            enemyStatus.textRange.text = $"{enemy.attackDistance}%";
            enemyStatus.textDef.text = $"{enemy.defensePercentage}%";
            enemyStatus.textStatus.text = GetStatusTypes(enemy).ToString();
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
            tileElement.sprite = GetElementSprite(currentTile.tileData.tileType);
            if (tileElement.sprite == null) { tileElement.gameObject.SetActive(false); }
            else { tileElement.gameObject.SetActive(true); }
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
