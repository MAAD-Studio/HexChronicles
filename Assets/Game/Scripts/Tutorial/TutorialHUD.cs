using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHUD : MonoBehaviour
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
    [SerializeField] private GameObject playerTurnMessage;
    [SerializeField] private GameObject enemyTurnMessage;

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

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoPanel;
    private TileInfo tileInfo;

    [Header("Buttons")]
    [SerializeField] private EndTurnButton endTurn;
    [SerializeField] private Button pause;

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
            selectedTile = null;
            selectedEnemy = null;
        }
    }

    #endregion


    #region Events
    private void SubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Subscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Subscribe<OnMovementPhase>(RestoreShowingInfos);
            EventBus.Instance.Subscribe<OnAttackPhase>(OnAttackPhase);
            EventBus.Instance.Subscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Subscribe<UpdateCharacterDecision>(OnUpdateCharacterDecision);
            EventBus.Instance.Subscribe<OnRestoreHeroData>(RestoreHeroData);
        }
        TurnManager.OnCharacterDied.AddListener(CharacterDied);
        PauseMenu.EndLevel.AddListener(OnLevelEnded);
        Character.movementComplete.AddListener(RestoreShowingInfos);
        Tutorial_Base.TutorialFullControl.AddListener(TutorialFullControl);
    }

    private void UnsubscribeEvents()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<OnPlayerTurn>(OnPlayerTurn);
            EventBus.Instance.Unsubscribe<OnMovementPhase>(RestoreShowingInfos);
            EventBus.Instance.Unsubscribe<OnAttackPhase>(OnAttackPhase);
            EventBus.Instance.Unsubscribe<OnEnemyTurn>(OnEnemyTurn);
            EventBus.Instance.Unsubscribe<UpdateCharacterDecision>(OnUpdateCharacterDecision);
            EventBus.Instance.Unsubscribe<OnRestoreHeroData>(RestoreHeroData);
        }
        TurnManager.OnCharacterDied.RemoveListener(CharacterDied);
        PauseMenu.EndLevel.RemoveListener(OnLevelEnded);
        Character.movementComplete.RemoveListener(RestoreShowingInfos);
        Tutorial_Base.TutorialFullControl.RemoveListener(TutorialFullControl);
    }

    private void OnNewLevelStart(object obj)
    {
        OnLevelEnded();

        turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "HUDInfo couldn't find the TurnManager Component");
        playerTurn = turnManager.GetComponent<PlayerTurn>();
        Debug.Assert(playerTurn != null, "HUDInfo couldn't find PlayerTurn");

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
        if (gameObject.activeInHierarchy)
        {
            enemyTurnMessage.gameObject.SetActive(true);
            enemyTurnMessage.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.5f).
                OnComplete(() => StartCoroutine(HideTurnMessage(enemyTurnMessage)));
        }

        endTurn.DisableButton();

        foreach (var button in heroButtons)
        {
            button.interactable = false;
        }
        enemyHoverUI.Hide();
    }

    private void OnPlayerTurn(object obj)
    {
        if (gameObject.activeInHierarchy)
        {
            playerTurnMessage.gameObject.SetActive(true);
            playerTurnMessage.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.5f).
                OnComplete(() => StartCoroutine(HideTurnMessage(playerTurnMessage)));
        }

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
        enemyStatus.Hide();
        enemyHoverUI.Hide();
    }

    private void RestoreShowingInfos(object obj)
    {
        showInfos = true;
    }

    private void TutorialFullControl()
    {
        /*showInfos = false;
        selectedTile = null;
        currentTile = null;*/
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

        // Create enemyHoverPrefab:
        GameObject enemyHover = Instantiate(enemyHoverPrefab);
        enemyHover.transform.SetParent(enemyHoverPanel.transform);
        enemyHover.transform.localScale = new Vector3(1, 1, 1);  // for fixing scale difference in different resolutions
        enemyHover.transform.localPosition = new Vector3(0, 0, 0); // for fixing position error
        enemyHoverUI = enemyHover.GetComponent<EnemyHoverUI>();
        enemyHoverUI.Hide();

        // Get Tile Info:
        tileInfo = tileInfoPanel.GetComponent<TileInfo>();
        tileInfo.Hide();
    }

    private void ButtonsAddListener()
    {
        pause.onClick.AddListener(() => EventBus.Instance.Publish(new PauseGame()));

        endTurn.AddEndTurnBtnListener(() =>
        {
            if (selectedCharacter != null && selectedCharacter.moving)
            {
                return;
            }
            //if (activeHeroes == 0)
            //{
                playerTurn.EndTurn();
                endTurn.EndTurnInactive();
            //}
            //else
            //{
            //    endTurn.ShowAskPanel();
            //}
        });

        endTurn.AddConfirmBtnListener(() =>
        {
            playerTurn.EndTurn();
            endTurn.EndTurnInactive();
        });

        question.onClick.AddListener(() => tutorialSummary.gameObject.SetActive(true));
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

        heroes.Clear();
        heroButtons.Clear();
        characterInfoDict.Clear();

        pause.onClick.RemoveAllListeners();
        endTurn.ResetEndTurn();
        question.onClick.RemoveAllListeners();

        availableHeroes = 0;
        activeHeroes = 0;
        selectedHero = null;
        selectedEnemy = null;
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
                    enemyStatus.SetEnemyStats(enemy);
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
    }

    #endregion
}
