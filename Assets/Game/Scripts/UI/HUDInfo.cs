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
    private Dictionary<string, CharacterInfo> characterInfoDict = new Dictionary<string, CharacterInfo>();
    private List<Button> heroButtons = new List<Button>();
    private Hero selectedHero;
    private int availableHeroes;
    private int activeHeroes;

    [Header("Enemy Info")]
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private GameObject enemyDetailPrefab;
    [SerializeField] private GameObject enemyHoverPrefab;
    private EnemyStatsUI enemyStatus;
    private EnemyHoverUI enemyHoverUI;

    [Header("Tile Object Info")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private GameObject objectStatusPrefab;
    private EnemyStatsUI objectStatus;

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoPanel;
    private TileInfo tileInfo;

    [Header("Buttons")]
    [SerializeField] private Button endTurn;
    [SerializeField] private Button undo;

    [Header("Element Icons")]
    [SerializeField] private Sprite[] elementSprites;

    #region Unity Methods

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();

        turnNumber.text = (turnManager.objectiveTurnNumber - turnManager.TurnNumber + 1).ToString();

        SubscribeEvents();
        InstantiateUIElements();
        ButtonsAddListener();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        CheckCurrentHover();

        selectedCharacter = playerTurn.SelectedCharacter;
        if (selectedCharacter != null)
        {
            Hero hero = selectedCharacter as Hero;
            HeroSelected(hero);
            //UpdateSelectedHeroInfo(hero);
        }
        else if (selectedCharacter == null && selectedHero != null)
        {
            if (characterInfoDict.TryGetValue(selectedHero.name, out var info))
            {
                info.SetDefaultState();
            }
        }
    }
    #endregion

    #region Events
    private void SubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Subscribe<OnPlayerTurn>(OnPlayerTurn);
            Debug.Log("Subscribed to OnPlayerTurn");
            EventBus.Instance.Subscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Subscribe<CharacterHasMadeDecision>(OnCharacterMadeDecision);
        }
        TurnManager.OnCharacterDied.AddListener(CharacterDied);
    }

    private void UnsubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Unsubscribe<CharacterHasMadeDecision>(OnCharacterMadeDecision);
        }
        TurnManager.OnCharacterDied.RemoveListener(CharacterDied);
    }

    private void OnEnemyTurn(object obj)
    {
        currentTurn.text = "ENEMY TURN";
        endTurn.interactable = false;
    }

    private void OnPlayerTurn(object obj)
    {
        currentTurn.text = "PLAYER TURN";
        endTurn.interactable = true;

        activeHeroes = availableHeroes;
        turnNumber.text = (turnManager.objectiveTurnNumber - turnManager.TurnNumber + 1).ToString();
    }

    private void OnCharacterMadeDecision(object obj)
    {
        CharacterHasMadeDecision decisionData = (CharacterHasMadeDecision)obj;
        if (characterInfoDict.TryGetValue(decisionData.character.name, out var info))
        {
            info.SetNoActionState();
        }
        activeHeroes--;

        if (activeHeroes == 0)
        {
            endTurn.GetComponent<Image>().color = new Color(1, 0.88f, 0, 1);
        }
    }

    private void CharacterDied(string arg0)
    {
        if (characterInfoDict.TryGetValue(arg0, out var info))
        {
            info.SetDeadState();
        }

        heroButtons.Remove(heroButtons.Find(x => x.name == arg0));

        availableHeroes--;
    }

    #endregion

    #region Initialization Methods

    private void InstantiateUIElements()
    {
        foreach (Character character in turnManager.characterList)
        {
            Hero hero = (Hero)character;
            heroes.Add(hero);
            availableHeroes++;
            activeHeroes++;

            // Create heroInfoPrefab in Character List:
            GameObject gameObject = Instantiate(heroInfoPrefab);
            gameObject.transform.SetParent(heroListPanel.transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.name = hero.name;

            // Add Button Listener:
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                EventBus.Instance.Publish(new CharacterSelected { character = character });
            });
            heroButtons.Add(button);

            // Add Hero Info:
            CharacterInfo info = gameObject.GetComponent<CharacterInfo>();
            info.hero = hero;
            characterInfoDict.Add(gameObject.name, info);

            // Set Hero Info:
            info.InitializeInfo();
            foreach (Image element in info.elements)
            {
                element.sprite = info.characterUIConfig.GetElementSprite(hero.elementType);
            }

            info.attackBtn.onClick.AddListener(() => playerTurn.SwitchToBasicAttack());
            info.skillBtn.onClick.AddListener(() => playerTurn.SwitchToSpecialAttack());

            // Update Hero Info:
            info.UpdateInfo();
            foreach (TextMeshProUGUI status in info.textStatus)
            {
                status.text = info.characterUIConfig.GetStatusTypes(hero).ToString();
            }
        }

        // Create enemyInfoPrefab:
        GameObject enemyUI = Instantiate(enemyDetailPrefab);
        enemyUI.transform.SetParent(enemyInfoPanel.transform);
        enemyUI.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyUI.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error

        // Create objectInfoPrefab:
        GameObject objectUI = Instantiate(objectStatusPrefab);
        objectUI.transform.SetParent(objectInfoPanel.transform);
        objectUI.transform.localScale = new Vector3(1, 1, 1); 
        objectUI.transform.localPosition = new Vector3(0, 0, 0); 
        objectStatus = objectUI.GetComponent<EnemyStatsUI>();
        enemyStatus = enemyUI.GetComponent<EnemyStatsUI>();
        
        // Create enemyHoverPrefab:
        GameObject enemyHover = Instantiate(enemyHoverPrefab);
        enemyHover.transform.SetParent(this.transform);
        enemyHover.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyHover.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyHoverUI = enemyHover.GetComponent<EnemyHoverUI>();
        enemyHoverUI.gameObject.SetActive(false);

        // Get Tile Info:
        tileInfo = tileInfoPanel.GetComponent<TileInfo>();
    }

    private void ButtonsAddListener()
    {
        endTurn.onClick.AddListener(() =>
        {
            playerTurn.EndTurn();
            endTurn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        });
        //undo.onClick.AddListener(() => playerTurn.UndoLastAction());
        undo.interactable = false;
    }

    
    #endregion

    #region Custom Methods

    private void HeroSelected(Hero hero)
    {
        if (characterInfoDict.TryGetValue(hero.name, out var newInfo))
        {
            newInfo.SetSelectedState();
        }

        // While changing hero in the list
        if (selectedHero != null && selectedHero != hero)
        {
            if (characterInfoDict.TryGetValue(selectedHero.name, out var info))
            {
                info.SetDefaultState();
            }
        }

        selectedHero = hero;
    }

    private void CheckCurrentHover()
    {
        currentTile = playerTurn.CurrentTile;

        if (currentTile == null)
        {
            tileInfo.Hide();
        }
        else
        {
            tileInfo.SetTileInfo(currentTile);

            if (currentTile.characterOnTile != null)
            {
                if (currentTile.characterOnTile is Hero)
                {
                    Hero hero = currentTile.characterOnTile as Hero;
                    //UpdateSelectedHeroInfo(hero);
                    if (hero != selectedHero && characterInfoDict.TryGetValue(hero.name, out var info))
                    {
                        info.SetHoverState();
                    }
                }

                if (currentTile.characterOnTile is Enemy_Base)
                {
                    Enemy_Base enemy = currentTile.characterOnTile as Enemy_Base;

                    enemyHoverUI.SetStats(enemy);
                    enemyHoverUI.gameObject.transform.position = (Camera.main.WorldToScreenPoint(enemy.transform.position) + new Vector3(0,24,0));

                    if (Input.GetMouseButtonDown(0))
                    {
                        enemyStatus.SetEnemyStats(enemy);
                    }
                }
                else
                {
                    enemyStatus.Hide();
                    enemyHoverUI.Hide();
                }
            }
            else
            {
                enemyStatus.Hide();
            }

            if (currentTile.tileHasObject)
            {
                TileObject tileObject = currentTile.objectOnTile;
                objectStatus.SetObjectStats(tileObject);
            }
            else
            {
                objectStatus.Hide();
            }
        }
    }
    #endregion
}
