using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private TabGroup tutorialSummary;
    [SerializeField] private Button question;

    [Header("Turn Info")]
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
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private GameObject enemyDetailPrefab;
    [SerializeField] private GameObject enemyHoverPanel;
    [SerializeField] private GameObject enemyHoverPrefab;
    private Enemy_Base selectedEnemy;
    private EnemyStatsUI enemyStatus;
    private EnemyHoverUI enemyHoverUI;

    [Header("Tile Object Info")]
    [SerializeField] private GameObject objectInfoPanel;
    [SerializeField] private GameObject objectStatusPrefab;
    [SerializeField] private GameObject objectHoverPrefab;
    private TileObject selectedObject;
    private EnemyStatsUI objectStatus;
    private EnemyHoverUI objectHoverUI;

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoPanel;
    private TileInfo tileInfo;

    [Header("Buttons")]
    [SerializeField] private Button pause;
    [SerializeField] private Button endTurn;
    [SerializeField] private Button undo;
    [SerializeField] private Button fast;

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
            enemyStatus.Hide();
            objectStatus.Hide();
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
    }

    private void OnNewLevelStart(object obj)
    {
        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();
        Debug.Assert(playerTurn != null, "HUDInfo couldn't find PlayerTurn");

        turnIndicator.Initialize(turnManager.objectiveTurnNumber);
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
        weatherWindow.ShowWeather(weatherManager);
        
        int turns = weatherManager.TurnsToStay;
        turnIndicator.SetWeatherTurn(turns);
    }

    private void OnEnemyTurn(object obj)
    {
        if (gameObject.activeInHierarchy)
        {
            enemyTurnMessage.gameObject.SetActive(true);
            StartCoroutine(HideTurnMessage(enemyTurnMessage));
        }

        currentTurn.text = "ENEMY TURN";
        endTurn.interactable = false;
        
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
            StartCoroutine(HideTurnMessage(playerTurnMessage));
        }

        turnIndicator.SetCurrentTurn(turnManager.TurnNumber);

        //turnNumber.text = (turnManager.objectiveTurnNumber - turnManager.TurnNumber + 1).ToString();
        currentTurn.text = "PLAYER TURN";
        endTurn.interactable = true;

        foreach (var button in heroButtons)
        {
            button.interactable = true;
        }

        activeHeroes = availableHeroes;
    }

    private IEnumerator HideTurnMessage(GameObject turnMessage)
    {
        yield return new WaitForSeconds(0.5f);
        turnMessage.gameObject.SetActive(false);
    }

    private void OnAttackPhase(object obj)
    {
        // Disable some UI elements
        showInfos = false;
        enemyStatus.Hide();
        objectStatus.Hide();
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
            endTurn.GetComponent<Image>().color = new Color(1, 0.88f, 0, 1);
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
            endTurn.GetComponent<Image>().color = new Color(1, 0.88f, 0, 1);
        }
        else
        {
            endTurn.GetComponent<Image>().color = Color.white;
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
    #endregion


    #region Initialization and Reset

    private void InstantiateUIElements()
    {
        playerTurnMessage.gameObject.SetActive(false);
        enemyTurnMessage.gameObject.SetActive(false);
        tutorialSummary.gameObject.SetActive(false);

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
            Outline attackOutline = info.attackBtn.GetComponent<Outline>();
            Outline skillOutline = info.skillBtn.GetComponent<Outline>();
            attackOutline.enabled = false;
            skillOutline.enabled = false;

            info.attackBtn.onClick.AddListener(() => 
            {
                attackOutline.enabled = true;
                skillOutline.enabled = false;
                playerTurn.SwitchToBasicAttack();
            });
            info.skillBtn.onClick.AddListener(() => 
            {
                skillOutline.enabled = true;
                attackOutline.enabled = false;
                playerTurn.SwitchToSpecialAttack();
            });
        }

        // Create enemyInfoPrefab:
        GameObject enemyUI = Instantiate(enemyDetailPrefab);
        enemyUI.transform.SetParent(enemyInfoPanel.transform);
        enemyUI.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyUI.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyStatus = enemyUI.GetComponent<EnemyStatsUI>();
        enemyStatus.Hide();

        // Create objectInfoPrefab:
        GameObject objectUI = Instantiate(objectStatusPrefab);
        objectUI.transform.SetParent(objectInfoPanel.transform);
        objectUI.transform.localScale = new Vector3(1, 1, 1); 
        objectUI.transform.localPosition = new Vector3(0, 0, 0); 
        objectStatus = objectUI.GetComponent<EnemyStatsUI>();
        objectStatus.Hide();

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
        tileInfo = tileInfoPanel.GetComponent<TileInfo>();
        tileInfo.Hide();
    }

    private void ButtonsAddListener()
    {
        pause.onClick.AddListener(() => EventBus.Instance.Publish(new PauseGame()));
        
        endTurn.onClick.AddListener(() =>
        {
            if (selectedCharacter != null && selectedCharacter.moving)
            {
                return;
            }
            playerTurn.EndTurn();
            endTurn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        });
        undo.onClick.AddListener(() => playerTurn.UndoAction());
        question.onClick.AddListener(() => tutorialSummary.gameObject.SetActive(true));
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
        foreach (Transform child in enemyInfoPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in enemyHoverPanel.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in objectInfoPanel.transform)
        {
            Destroy(child.gameObject);
        }

        heroes.Clear();
        heroButtons.Clear();
        characterInfoDict.Clear();

        endTurn.onClick.RemoveAllListeners();
        endTurn.GetComponent<Image>().color = Color.white;
        turnIndicator.ResetTurn();

        availableHeroes = 0;
        activeHeroes = 0;
        selectedHero = null;
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
                    enemyStatus.SetEnemyStats(enemy);
                    objectStatus.Hide();
                    selectedEnemy = enemy;
                }
                else
                {
                    enemyStatus.Hide();
                    selectedEnemy = null;
                }
            }
        }
        else
        {
            enemyHoverUI.Hide();
        }

        // Object Info:
        if (currentTile.tileHasObject)
        {
            TileObject tileObject = currentTile.objectOnTile;
            objectHoverUI.SetObjectStats(tileObject);
            objectHoverUI.gameObject.transform.position = Camera.main.WorldToScreenPoint(tileObject.transform.position + new Vector3(0, 3.5f, 0));

            // Scale based on enemy distance from camera, referenced from LookAtCamera
            float distance = Vector3.Distance(tileObject.transform.position, Camera.main.transform.position);
            float scale = distance * 0.02f;
            objectHoverUI.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 2.0f, Vector3.one * 0.3f, scale);
            
            // Click to show status panel
            if (showInfos && Input.GetMouseButtonDown(0))
            {
                if (selectedObject != tileObject)
                {
                    objectStatus.SetObjectStats(tileObject);
                    enemyStatus.Hide();
                    selectedObject = tileObject;
                }
                else
                {
                    objectStatus.Hide();
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
