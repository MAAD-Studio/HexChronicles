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
    private List<Hero> heroes = new List<Hero>();
    private Character selectedCharacter;
    private Tile currentTile;

    [Header("Turn Info")]
    [SerializeField] private TextMeshProUGUI currentTurn;
    [SerializeField] private TextMeshProUGUI turnNumber;
    [SerializeField] private GameObject turnMessage;
    [SerializeField] private WeatherIndicator weatherIndicator;

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

    #region Unity Methods

    private void Awake()
    {
        EventBus.Instance.Subscribe<OnNewLevelStart>(OnNewLevelStart);
    }

    private void Update()
    {
        // From current Tile:
        currentTile = playerTurn.CurrentTile;
        if (currentTile == null)
        {
            tileInfo.Hide();
        }
        else
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
    }
    #endregion

    #region Events
    private void SubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            //EventBus.Instance.Subscribe<OnNewLevelStart>(OnNewLevelStart);
            EventBus.Instance.Subscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Subscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Subscribe<CharacterHasMadeDecision>(OnCharacterMadeDecision);
        }
        TurnManager.OnCharacterDied.AddListener(CharacterDied);
        WorldTurnBase.Victory.AddListener(OnLevelEnded);
        TurnManager.LevelDefeat.AddListener(OnLevelEnded);
        PauseMenu.EndLevel.AddListener(OnLevelEnded);
    }

    private void UnsubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            //EventBus.Instance.Unsubscribe<OnNewLevelStart>(OnNewLevelStart);
            EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Unsubscribe<CharacterHasMadeDecision>(OnCharacterMadeDecision);
        }
        TurnManager.OnCharacterDied.RemoveListener(CharacterDied);
        WorldTurnBase.Victory.RemoveListener(OnLevelEnded);
        TurnManager.LevelDefeat.RemoveListener(OnLevelEnded);
        PauseMenu.EndLevel.RemoveListener(OnLevelEnded);
    }

    private void OnNewLevelStart(object obj)
    {
        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();
        Debug.Assert(playerTurn != null, "HUDInfo couldn't find PlayerTurn");
        
        turnNumber.text = (turnManager.objectiveTurnNumber - turnManager.TurnNumber + 1).ToString();

        WeatherManager weatherManager = turnManager.GetComponent<WeatherManager>();
        weatherIndicator.Initialize(weatherManager);

        SubscribeEvents();
        InstantiateUIElements();
        ButtonsAddListener();
    }

    private void OnLevelEnded()
    {
        ResetHUD();
        UnsubscribeEvents();
    }

    private void OnEnemyTurn(object obj)
    {
        currentTurn.text = "ENEMY TURN";
        endTurn.interactable = false;

        foreach (var button in heroButtons)
        {
            button.interactable = false;
        }
    }

    private void OnPlayerTurn(object obj)
    {
        if (gameObject.activeInHierarchy)
        {
            turnMessage.gameObject.SetActive(true);
            StartCoroutine(HideTurnMessage());
        }

        currentTurn.text = "PLAYER TURN";
        endTurn.interactable = true;

        foreach (var button in heroButtons)
        {
            button.interactable = true;
        }

        activeHeroes = availableHeroes;
        turnNumber.text = (turnManager.objectiveTurnNumber - turnManager.TurnNumber + 1).ToString();
    }

    private IEnumerator HideTurnMessage()
    {
        yield return new WaitForSeconds(0.5f);
        turnMessage.gameObject.SetActive(false);
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

    #region Initialization and Reset

    private void InstantiateUIElements()
    {
        turnMessage.gameObject.SetActive(false);

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
            characterInfoDict.Add(gameObject.name, info);

            info.InitializeInfo(hero);
            info.attackBtn.onClick.AddListener(() => playerTurn.SwitchToBasicAttack());
            info.skillBtn.onClick.AddListener(() => playerTurn.SwitchToSpecialAttack());
        }

        // Create enemyInfoPrefab:
        GameObject enemyUI = Instantiate(enemyDetailPrefab);
        enemyUI.transform.SetParent(enemyInfoPanel.transform);
        enemyUI.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyUI.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyStatus = enemyUI.GetComponent<EnemyStatsUI>();
        enemyStatus.gameObject.SetActive(false);

        // Create objectInfoPrefab:
        GameObject objectUI = Instantiate(objectStatusPrefab);
        objectUI.transform.SetParent(objectInfoPanel.transform);
        objectUI.transform.localScale = new Vector3(1, 1, 1); 
        objectUI.transform.localPosition = new Vector3(0, 0, 0); 
        objectStatus = objectUI.GetComponent<EnemyStatsUI>();
        objectStatus.gameObject.SetActive(false);

        // Create enemyHoverPrefab:
        GameObject enemyHover = Instantiate(enemyHoverPrefab);
        enemyHover.transform.SetParent(enemyHoverPanel.transform);
        enemyHover.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyHover.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyHoverUI = enemyHover.GetComponent<EnemyHoverUI>();
        enemyHoverUI.gameObject.SetActive(false);

        // Get Tile Info:
        tileInfo = tileInfoPanel.GetComponent<TileInfo>();
    }

    private void ButtonsAddListener()
    {
        if (endTurn == null)
        {
            Debug.LogError("endTurn is null");
        }
        if (endTurn.onClick == null)
        {
            Debug.LogError("endTurn.onClick is null");
        }
        endTurn.onClick.AddListener(() =>
        {
            playerTurn.EndTurn();
            endTurn.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        });
        //undo.onClick.AddListener(() => playerTurn.UndoLastAction());
        undo.interactable = false;
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
        availableHeroes = 0;
        activeHeroes = 0;
        selectedHero = null;
        endTurn.onClick.RemoveAllListeners();
    }
    #endregion

    #region Custom Methods

    private void HeroSelected(Hero hero)
    {
        if (characterInfoDict.TryGetValue(hero.name, out var newInfo))
        {
            newInfo.SetSelectedState();
        }

        // While changing hero in the list, set the previous selected hero to default state
        if (selectedHero != null && selectedHero != hero)
        {
            if (characterInfoDict.TryGetValue(selectedHero.name, out var info))
            {
                info.SetDefaultState();
            }
        }

        selectedHero = hero;
    }

    private void CheckCurrentTile()
    {
        tileInfo.SetTileInfo(currentTile);

        // Hero Info:
        if (currentTile.characterOnTile != null && currentTile.characterOnTile is Hero)
        {
            Hero hero = currentTile.characterOnTile as Hero;

            if (characterInfoDict.TryGetValue(hero.name, out var info))
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

        // Enemy Info:
        if (currentTile.characterOnTile != null && currentTile.characterOnTile is Enemy_Base)
        {
            Enemy_Base enemy = currentTile.characterOnTile as Enemy_Base;

            enemyHoverUI.SetStats(enemy);
            enemyHoverUI.gameObject.transform.position = Camera.main.WorldToScreenPoint(enemy.transform.position + new Vector3(0, 2.5f, 0));

            // Scale based on enemy distance from camera, referenced from LookAtCamera
            float distance = Vector3.Distance(enemy.transform.position, Camera.main.transform.position);
            float scale = distance * 0.02f;
            enemyHoverUI.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * 2.0f, Vector3.one * 0.3f, scale);

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

        // Object Info:
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

    #endregion
}
