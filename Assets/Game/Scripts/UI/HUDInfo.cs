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

    [Header("Tile Object Info")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private GameObject objectStatusPrefab;
    private EnemyStatsUI objectStatus;

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
        CheckCurrentHover();

        selectedCharacter = playerTurn.SelectedCharacter;
        if (selectedCharacter != null)
        {
            Hero hero = selectedCharacter as Hero;
            UpdateSelectedHeroInfo(hero);
        }
    }

    private void CheckCurrentHover()
    {
        currentTile = playerTurn.CurrentTile;

        if (currentTile == null)
        {
            tileInfoPanel.gameObject.SetActive(false);
        }
        else
        {
            UpdateTileInfo();

            if (currentTile.characterOnTile != null)
            {
                if (currentTile.characterOnTile is Hero)
                {
                    Hero hero = currentTile.characterOnTile as Hero;
                    UpdateSelectedHeroInfo(hero);
                }
                else
                {
                    selectHeroStatus.gameObject.SetActive(false);
                }

                if (currentTile.characterOnTile is Enemy_Base)
                {
                    Enemy_Base enemy = currentTile.characterOnTile as Enemy_Base;
                    UpdateEnemyInfo(enemy);
                }
                else
                {
                    enemyInfoPanel.gameObject.SetActive(false);
                }
            }
            else
            {
                selectHeroStatus.gameObject.SetActive(false);
                enemyInfoPanel.gameObject.SetActive(false);
            }

            if (currentTile.tileHasObject)
            {
                TileObject tileObject = currentTile.objectOnTile;

                UpdateObjectInfo(tileObject);
            }
            else
            {
                objectInfoPanel.gameObject.SetActive(false);
            }
        }
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

        // Create objectInfoPrefab:
        GameObject objectUI = Instantiate(objectStatusPrefab);
        objectUI.transform.SetParent(objectInfoPanel.transform);
        objectUI.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        objectStatus = objectUI.GetComponent<EnemyStatsUI>();
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

    #region Update Stats

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

        turnNumber.text = (8 - turnManager.TurnNumber + 1).ToString();
    }

    private void UpdateSelectedHeroInfo(Hero hero)
    {
        selectHeroStatus.gameObject.SetActive(true);

        //selectHeroStatus.attackBtn.interactable = hero.canAttack;
        //selectHeroStatus.moveBtn.interactable = hero.canMove;

        if (hero.currentSkillCD > 0)
        {
            selectHeroStatus.skillBtn.interactable = false;
            selectHeroStatus.skillCD.text = $"(On Cooldown - {hero.currentSkillCD} turns)";
            selectHeroStatus.attackBtn.interactable = false; // temp
        }
        else
        {
            selectHeroStatus.skillBtn.interactable = true;
            selectHeroStatus.skillCD.gameObject.SetActive(false);
            selectHeroStatus.attackBtn.interactable = true; // temp
        }

        // Display Status:
        selectHeroStatus.avatar.sprite = hero.heroSO.attributes.avatar;
        selectHeroStatus.skillShape.sprite = hero.heroSO.activeSkill.skillshape;
        selectHeroStatus.element.sprite = GetElementSprite(hero.elementType);
        selectHeroStatus.textName.text = hero.heroSO.attributes.name;
        selectHeroStatus.textHP.text = $"{hero.currentHealth} / {hero.maxHealth}";
        selectHeroStatus.textMovement.text = $"{hero.moveDistance}";
        selectHeroStatus.textAttack.text = $"{hero.attackDamage}";
        selectHeroStatus.textDef.text = $"{hero.defensePercentage}%";

        selectHeroStatus.attackShape.sprite = hero.heroSO.attackShape;
        selectHeroStatus.attackInfo.text = hero.heroSO.attackInfo.DisplayKeywordDescription();
        selectHeroStatus.attackInfo.ForceMeshUpdate();

        selectHeroStatus.skillInfo.text = hero.heroSO.activeSkill.description.DisplayKeywordDescription();
        selectHeroStatus.skillInfo.ForceMeshUpdate();
        selectHeroStatus.textStatus.text = GetStatusTypes(hero).ToString();
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

    private void UpdateEnemyInfo(Enemy_Base enemy)
    {
        enemyInfoPanel.gameObject.SetActive(true);

        enemyStatus.avatar.sprite = enemy.enemySO.attributes.avatar;
        enemyStatus.element.sprite = GetElementSprite(enemy.elementType);
        enemyStatus.textName.text = enemy.enemySO.attributes.name;
        enemyStatus.enemyInfo.text = enemy.enemySO.attributes.description.DisplayKeywordDescription();
        enemyStatus.enemyInfo.ForceMeshUpdate();
        enemyStatus.textHP.text = $"{enemy.currentHealth} / {enemy.maxHealth}";
        enemyStatus.textMovement.text = $"{enemy.moveDistance}";
        enemyStatus.textAttack.text = $"{enemy.attackDamage}";
        enemyStatus.textRange.text = $"{enemy.attackDistance}%";
        enemyStatus.textDef.text = $"{enemy.defensePercentage}%";
        enemyStatus.textStatus.text = GetStatusTypes(enemy).ToString(); 
    }

    private void UpdateObjectInfo(TileObject tileObject)
    {
        objectInfoPanel.gameObject.SetActive(true);

        objectStatus.avatar.sprite = tileObject.tileObjectData.avatar;
        objectStatus.textName.text = tileObject.tileObjectData.objectName;
        objectStatus.enemyInfo.text = tileObject.tileObjectData.description.DisplayKeywordDescription();
        objectStatus.enemyInfo.ForceMeshUpdate();
        objectStatus.textHP.text = $"{tileObject.currentHealth} / {tileObject.tileObjectData.health}";
        objectStatus.textDef.text = $"{tileObject.tileObjectData.defense}%";
        //objectStatus.textStatus.text = GetStatusTypes(tileObject).ToString();
        objectStatus.textStatus.gameObject.SetActive(false);
    }

    private void UpdateTileInfo()
    {
        tileInfoPanel.gameObject.SetActive(true);
        tileImage.sprite = currentTile.tileData.tileSprite;
        tileElement.sprite = GetElementSprite(currentTile.tileData.tileType);
        if (tileElement.sprite == null) { tileElement.gameObject.SetActive(false); }
        else { tileElement.gameObject.SetActive(true); }
        tileName.text = currentTile.tileData.name;
        tileEffects.text = currentTile.tileData.tileEffects.DisplayKeywordDescription();
        tileEffects.ForceMeshUpdate();
    }
    #endregion
}
