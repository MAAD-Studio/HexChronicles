using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HUDInfo : MonoBehaviour
{
    private TurnManager turnManager;
    private PlayerTurn playerTurn;
    private List<Hero> heroes = new List<Hero>();
    private Character selectedCharacter;
    private Tile currentTile;
    private Tile selectedTile;
    private bool showInfos = true;

    [Header("Tutorial")]
    [SerializeField] private UIPanel tutorialSummary;
    [SerializeField] private Button question;

    [Header("Turn Info")]
    [SerializeField] private TextMeshProUGUI primaryObjective;
    [SerializeField] private TextMeshProUGUI currentTurn;
    [SerializeField] private GameObject playerTurnMessage;
    [SerializeField] private GameObject enemyTurnMessage;
    [SerializeField] private TurnIndicator turnIndicator;
    [SerializeField] private WeatherIndicatorWindow weatherWindow;

    [Header("Hero Info")]
    [SerializeField] private GameObject heroListPanel;
    [SerializeField] private GameObject heroInfoPrefab;
    private Dictionary<string, CharacterInfo> characterInfoDict = new Dictionary<string, CharacterInfo>();
    private List<Button> heroButtons = new List<Button>();
    private Hero selectedHero;
    private int availableHeroes;
    private int activeHeroes;

    [Header("Enemy Info")]
    [SerializeField] private GameObject enemyInfoGO;
    [SerializeField] private GameObject enemyHoverPanel;
    [SerializeField] private GameObject enemyHoverPrefab;
    private Enemy_Base selectedEnemy;
    private EnemyStatsUI enemyStats;
    private EnemyHoverUI enemyHoverUI;

    [Header("Tile Object Info")]
    [SerializeField] private GameObject objectInfoGO;
    [SerializeField] private GameObject objectHoverPrefab;
    private TileObject selectedObject;
    private EnemyStatsUI objectStats;
    private EnemyHoverUI objectHoverUI;

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoGO;
    private TileInfo tileInfo;

    [Header("Buttons")]
    [SerializeField] private EndTurnButton endTurn;
    [SerializeField] private Button pause;
    [SerializeField] private Button undo;
    [SerializeField] private Button fast;

    private ButtonChangeNotifier undoNotifier;

    #region Unity Methods

    private void Awake()
    {
        EventBus.Instance.Subscribe<OnNewLevelStart>(OnNewLevelStart);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<OnNewLevelStart>(OnNewLevelStart);
    }

    private void Update()
    {
        // From current Tile:
        currentTile = playerTurn.CurrentTile;

        if (currentTile != null)
        {
            CheckCurrentTile();
        }

        // From selected Character:
        selectedCharacter = playerTurn.SelectedCharacter;
        if (selectedCharacter != null)
        {
            Hero hero = selectedCharacter as Hero;
            HeroSelected(hero);
        }

        // Right Click to hide everything
        if (Input.GetMouseButtonDown(1))
        {
            tileInfo.Hide();
            enemyStats.Hide();
            objectStats.Hide();
            selectedTile = null;
            selectedEnemy = null;
            selectedObject = null;
        }
    }

    #endregion

    #region Events
    private void SubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            //EventBus.Instance.Subscribe<OnNewLevelStart>(OnNewLevelStart);
            EventBus.Instance.Subscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Subscribe<OnMovementPhase>(RestoreShowingInfos);
            EventBus.Instance.Subscribe<OnAttackPhase>(OnAttackPhase);
            EventBus.Instance.Subscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Subscribe<OnWeatherSpawn>(SetWeather);
            EventBus.Instance.Subscribe<UpdateCharacterDecision>(OnUpdateCharacterDecision);
            EventBus.Instance.Subscribe<OnRestoreHeroData>(RestoreHeroData);
        }
        TurnManager.OnCharacterDied.AddListener(CharacterDied);
        WorldTurnBase.Victory.AddListener(OnLevelEnded);
        TurnManager.LevelVictory.AddListener(OnLevelEnded);
        TurnManager.LevelDefeat.AddListener(OnLevelEnded);
        PauseMenu.EndLevel.AddListener(OnLevelEnded);
        Character.movementComplete.AddListener(RestoreShowingInfos);
        UndoManager.Instance.UndoDataAvailable.AddListener(UpdateUndoButton);
    }

    private void UnsubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            //EventBus.Instance.Unsubscribe<OnNewLevelStart>(OnNewLevelStart);
            EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Unsubscribe<OnMovementPhase>(RestoreShowingInfos);
            EventBus.Instance.Unsubscribe<OnAttackPhase>(OnAttackPhase);
            EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Unsubscribe<OnWeatherSpawn>(SetWeather);
            EventBus.Instance.Unsubscribe<UpdateCharacterDecision>(OnUpdateCharacterDecision);
            EventBus.Instance.Unsubscribe<OnRestoreHeroData>(RestoreHeroData);
        }
        TurnManager.OnCharacterDied.RemoveListener(CharacterDied);
        WorldTurnBase.Victory.RemoveListener(OnLevelEnded);
        TurnManager.LevelVictory.RemoveListener(OnLevelEnded);
        TurnManager.LevelDefeat.RemoveListener(OnLevelEnded);
        PauseMenu.EndLevel.RemoveListener(OnLevelEnded);
        Character.movementComplete.RemoveListener(RestoreShowingInfos);
        UndoManager.Instance.UndoDataAvailable.RemoveListener(UpdateUndoButton);
    }

    private void OnNewLevelStart(object obj)
    {
        OnLevelEnded();

        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();
        Debug.Assert(playerTurn != null, "HUDInfo couldn't find PlayerTurn");

        primaryObjective.text = GameManager.Instance.levelDetails[GameManager.Instance.CurrentLevelIndex].primaryObjective;
        turnIndicator.Initialize();
        weatherWindow.Start();

        SubscribeEvents();
        InstantiateUIElements();
        ButtonsAddListener();
    }

    private void OnLevelEnded()
    {
        ResetHUD();
        UnsubscribeEvents();
    }

    private void SetWeather(object obj)
    {
        WeatherManager weatherManager = FindObjectOfType<WeatherManager>();
        weatherWindow.ShowWeather(weatherManager.WeatherType);
        
        int turns = weatherManager.TurnsToStay;
        turnIndicator.SetWeatherTurn(weatherManager.WeatherType, turns);
    }

    private void OnEnemyTurn(object obj)
    {
        if (gameObject.activeInHierarchy)
        {
            enemyTurnMessage.gameObject.SetActive(true);
            enemyTurnMessage.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.5f).
                OnComplete(() =>StartCoroutine(HideTurnMessage(enemyTurnMessage)));
        }

        currentTurn.text = "ENEMY TURN";
        endTurn.DisableButton();

        foreach (var button in heroButtons)
        {
            button.interactable = false;
        }
        enemyHoverUI.Hide();
        objectHoverUI.Hide();
    }

    private void OnPlayerTurn(object obj)
    {
        if (gameObject.activeInHierarchy)
        {
            playerTurnMessage.gameObject.SetActive(true);
            playerTurnMessage.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.5f).
                OnComplete(() => StartCoroutine(HideTurnMessage(playerTurnMessage)));
        }

        turnIndicator.SetCurrentTurn(turnManager.TurnNumber);

        //turnNumber.text = (turnManager.objectiveTurnNumber - turnManager.TurnNumber + 1).ToString();
        currentTurn.text = "PLAYER TURN";
        endTurn.EnableButton();

        foreach (var button in heroButtons)
        {
            button.interactable = true;
        }

        activeHeroes = availableHeroes;
    }

    private IEnumerator HideTurnMessage(GameObject turnMessage)
    {
        yield return new WaitForSeconds(0.3f);
        turnMessage.transform.DOScale(0.5f, 0.3f).SetEase(Ease.InBack).OnComplete(() => turnMessage.SetActive(false));
    }

    private void OnAttackPhase(object obj)
    {
        // Disable some UI elements
        showInfos = false;
        enemyStats.Hide();
        objectStats.Hide();
        enemyHoverUI.Hide();
    }

    private void RestoreShowingInfos(object obj)
    {
        showInfos = true;
    }

    // Used for update info after character has made decision
    private void OnUpdateCharacterDecision(object obj)
    {
        UpdateCharacterDecision data = (UpdateCharacterDecision)obj;
        Hero hero = (Hero)data.character;
        if (characterInfoDict.TryGetValue(hero.heroSO.name, out var info))
        {
            info.SetNoActionState();
        }
        activeHeroes--;

        if (activeHeroes == 0)
        {
            endTurn.EndTurnActive();
        }
    }

    // Used for update info while restoring hero data
    // If the hero is already dead and respawned, the characterinfo should be updated
    private void RestoreHeroData(object obj) 
    {
        OnRestoreHeroData data = (OnRestoreHeroData)obj;
        Hero hero = data.hero;

        if (characterInfoDict.TryGetValue(hero.heroSO.name, out var info))
        {
            info.SetRestoreState(hero);
            if (hero.hasMadeDecision == false)
            {
                activeHeroes++;
            }
        }

        if (activeHeroes == 0)
        {
            endTurn.EndTurnActive();
        }
        else
        {
            endTurn.EndTurnInactive();
        }
    }

    private void CharacterDied(Character arg0)
    {
        Hero hero = (Hero)arg0;
        if (characterInfoDict.TryGetValue(hero.heroSO.name, out var info))
        {
            info.SetDeadState();
        }

        heroButtons.Remove(heroButtons.Find(x => x.name == hero.heroSO.name));

        availableHeroes--;
    }


    private void UpdateUndoButton(bool arg0)
    {
        if (arg0)
        {
            undo.interactable = true;
        }
        else
        {
            undo.interactable = false;
        }
        undoNotifier.onButtonChange?.Invoke();
    }
    #endregion


    #region Initialization and Reset

    private void InstantiateUIElements()
    {
        playerTurnMessage.gameObject.SetActive(false);
        enemyTurnMessage.gameObject.SetActive(false);
        tutorialSummary.Initialize();

        // Create Characters Info:
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

            // Add Button Listener for select Character:
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                EventBus.Instance.Publish(new CharacterSelected { character = character });
            });
            heroButtons.Add(button);

            // Add Hero Info:
            CharacterInfo info = gameObject.GetComponent<CharacterInfo>();
            characterInfoDict.Add(hero.heroSO.name, info);

            info.InitializeInfo(hero);
            
            info.attackBtn.onClick.AddListener(() => 
            {
                info.AttackMode();
                playerTurn.SwitchToBasicAttack();
            });
            info.skillBtn.onClick.AddListener(() => 
            {
                info.SkillMode();
                playerTurn.SwitchToSpecialAttack();
            });
        }

        // Get Enemy Stats Info:
        enemyStats = enemyInfoGO.GetComponent<EnemyStatsUI>();
        enemyStats.Hide();

        objectStats = objectInfoGO.GetComponent<EnemyStatsUI>();
        objectStats.Hide();

        // Create enemyHoverPrefab:
        GameObject enemyHover = Instantiate(enemyHoverPrefab);
        enemyHover.transform.SetParent(enemyHoverPanel.transform);
        enemyHover.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyHover.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyHoverUI = enemyHover.GetComponent<EnemyHoverUI>();
        enemyHoverUI.Hide();

        // Create objectHoverPrefab:
        GameObject objectHover = Instantiate(objectHoverPrefab);
        objectHover.transform.SetParent(enemyHoverPanel.transform);
        objectHover.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        objectHover.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        objectHoverUI = objectHover.GetComponent<EnemyHoverUI>();
        objectHoverUI.Hide();

        // Get Tile Info:
        tileInfo = tileInfoGO.GetComponent<TileInfo>();
        tileInfo.Hide();
    }

    private void ButtonsAddListener()
    {
        pause.onClick.AddListener(() => EventBus.Instance.Publish(new PauseGame()));
        undo.onClick.AddListener(() => playerTurn.UndoAction());
        undoNotifier = undo.GetComponent<ButtonChangeNotifier>();
        undoNotifier.onButtonChange?.Invoke();
        question.onClick.AddListener(() => tutorialSummary.FadeIn());

        endTurn.AddEndTurnBtnListener(() =>
        {
            if (selectedCharacter != null && !playerTurn.AllowSelection)
            {
                return;
            }
            if (activeHeroes == 0)
            {
                playerTurn.EndTurn();
                endTurn.EndTurnInactive();
            }
            else
            {
                endTurn.ShowAskPanel();
            }
        });

        endTurn.AddConfirmBtnListener(() =>
        {
            playerTurn.EndTurn();
            endTurn.EndTurnInactive();
        });

        fast.onClick.AddListener(() => 
        {
            if (GameManager.Instance.IsFast)
            {
                GameManager.Instance.DecreaseGameSpeed();
                fast.GetComponentInChildren<TextMeshProUGUI>().text = "1x";
            }
            else
            {
                GameManager.Instance.IncreaseGameSpeed();
                fast.GetComponentInChildren<TextMeshProUGUI>().text = ">>2x";
            }
        });
    }

    private void ResetHUD()
    {
        foreach (Transform child in heroListPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in enemyHoverPanel.transform)
        {
            Destroy(child.gameObject);
        }

        heroes.Clear();
        heroButtons.Clear();
        characterInfoDict.Clear();

        pause.onClick.RemoveAllListeners();
        undo.onClick.RemoveAllListeners();
        undo.interactable = false;
        question.onClick.RemoveAllListeners();

        endTurn.ResetEndTurn();
        fast.onClick.RemoveAllListeners();

        turnIndicator.ResetTurn();

        availableHeroes = 0;
        activeHeroes = 0;

        selectedHero = null;
        selectedEnemy = null;
        selectedObject = null;
        selectedTile = null;
        showInfos = true;
    }
    #endregion

    #region Custom Methods

    private void HeroSelected(Hero hero)
    {
        if (characterInfoDict.TryGetValue(hero.heroSO.name, out var newInfo))
        {
            newInfo.SetSelectedState();
        }

        // While changing hero in the list, set the previous selected hero to default state
        if (selectedHero != null && selectedHero != hero)
        {
            if (characterInfoDict.TryGetValue(selectedHero.heroSO.name, out var info))
            {
                info.SetDefaultState();
            }
        }

        selectedHero = hero;
    }

    private void CheckCurrentTile()
    {
        // Hero Info:
        if (currentTile.characterOnTile != null && currentTile.characterOnTile is Hero)
        {
            Hero hero = currentTile.characterOnTile as Hero;

            if (characterInfoDict.TryGetValue(hero.heroSO.name, out var info))
            {
                info.SetHoverState();
            }
        }
        else
        {
            foreach (var info in characterInfoDict.Values)
            {
                info.SetDefaultState();
            }
        }

        // Clicked on tile: 
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedTile != currentTile)
            {
                tileInfo.SetTileInfo(currentTile);
                selectedTile = currentTile;
            }
            else
            {
                tileInfo.Hide();
                selectedTile = null;
            }
        }

        

        // Enemy Info:
        if (showInfos && currentTile.characterOnTile != null && currentTile.characterOnTile is Enemy_Base)
        {
            Enemy_Base enemy = currentTile.characterOnTile as Enemy_Base;
            enemyHoverUI.SetEnemyStats(enemy);
            enemyHoverUI.gameObject.transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position + new Vector3(0, 2.5f, 0));

            // Scale based on enemy distance from camera, referenced from LookAtCamera
            float distance = Vector3.Distance(enemy.transform.position, Camera.main.transform.position);
            float scale = distance * 0.02f;
            enemyHoverUI.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 2.0f, Vector3.one * 0.3f, scale);

            // Click to show status panel
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedEnemy != enemy)
                {
                    enemyStats.SetEnemyStats(enemy);
                    objectStats.Hide();
                    selectedEnemy = enemy;
                }
                else
                {
                    enemyStats.Hide();
                    selectedEnemy = null;
                }
            }
        }
        else
        {
            enemyHoverUI.Hide();
        }

        // Object Info:
        if (showInfos && currentTile.tileHasObject)
        {
            TileObject tileObject = currentTile.objectOnTile;
            objectHoverUI.SetObjectStats(tileObject);
            objectHoverUI.gameObject.transform.position = Camera.main.WorldToScreenPoint(tileObject.transform.position + new Vector3(0, 3.5f, 0));

            // Scale based on enemy distance from camera, referenced from LookAtCamera
            float distance = Vector3.Distance(tileObject.transform.position, Camera.main.transform.position);
            float scale = distance * 0.02f;
            objectHoverUI.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 2.0f, Vector3.one * 0.3f, scale);
            
            // Click to show status panel
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedObject != tileObject)
                {
                    objectStats.SetObjectStats(tileObject);
                    enemyStats.Hide();
                    selectedObject = tileObject;
                }
                else
                {
                    objectStats.Hide();
                    selectedObject = null;
                }
            }
        }
        else
        {
            objectHoverUI.Hide();
        }
    }

    #endregion
}
