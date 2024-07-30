using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("Character Movement Info:")]
    [SerializeField] public float moveSpeed = 0.4f;
    [SerializeField] public int moveDistance = 2;
    [HideInInspector] public int movementThisTurn = 0;
    [HideInInspector] public bool canMove = true;

    [Header("Character Attack Info:")]
    [SerializeField] public int attackDistance = 2;
    [SerializeField] public TurnEnums.CharacterType characterType;
    [HideInInspector] public AttackArea basicAttackArea;
    [HideInInspector] public AttackArea activeSkillArea;
    [HideInInspector] public bool canAttack = true;

    [Header("Character Attributes:")]
    [HideInInspector] public float currentHealth = 0f;
    [HideInInspector] public float maxHealth = 0f;
    [HideInInspector] public float attackDamage = 0;
    [HideInInspector] public float defensePercentage = 0;
    [HideInInspector] protected Animator animator;
    [SerializeField] public HealthBar healthBar;
    [HideInInspector] public ElementType elementType;
    [HideInInspector] public ElementType elementWeakAgainst;
    [HideInInspector] public ElementType elementStrongAgainst;
    [HideInInspector] public Vector3 defaultScale;

    [Header("Tile LayerMask:")]
    [SerializeField] protected LayerMask tileLayer;

    [HideInInspector] public bool moving = false;
    [HideInInspector] public Tile characterTile;

    [Header("Prefabs:")]
    [SerializeField] protected GameObject attackVFX;
    [SerializeField] protected GameObject damagePrefab;

    [Header("Character Status:")]
    public List<Status> statusList = new List<Status>();
    private List<Status> statusToRemove = new List<Status>();
    [HideInInspector] public bool isHurt = false;

    [HideInInspector] public bool effectedByWeather = false;
    [HideInInspector] public bool hasMadeDecision = false;

    [HideInInspector] public static UnityEvent<Character> movementComplete = new UnityEvent<Character>();

    [HideInInspector] public UnityEvent<int> DamagePreview = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<Status> AddStatusPreview = new UnityEvent<Status>();
    [HideInInspector] public UnityEvent<Status, int> ChangeStatusPreview = new UnityEvent<Status, int>();
    [HideInInspector] public UnityEvent<Status> RemoveStatusPreview = new UnityEvent<Status>();
    [HideInInspector] public UnityEvent DonePreview = new UnityEvent();
    [HideInInspector] public UnityEvent UpdateHealthBar = new UnityEvent();
    [HideInInspector] public UnityEvent UpdateAttributes = new UnityEvent();
    [HideInInspector] public UnityEvent UpdateStatus = new UnityEvent();

    private GameObject buffPreview;
    private GameObject tileVFX;
    private TurnManager turnManager;
    #endregion

    #region UnityMethods

    protected virtual void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Can not find Animatior Component on Character");

        Debug.Assert(attackVFX != null, $"Can not find attackVFX on {name}");

        FindTile();
    }

    void Update()
    {

    }

    #endregion


    #region AttackMethods

    public virtual void PerformBasicAttack(List<Character> targets)
    {
        //animator.SetTrigger("attack");
        if (characterTile.tileData.tileType == elementType)
        {
            ApplyStatusAttackArea(targets);
            ApplyStatusElementalTile();
        }
    }

    public virtual void ReleaseActiveSkill(List<Character> targets)
    {
        animator.SetTrigger("skill");
    }

    public virtual void PerformBasicAttackObjects(List<TileObject> targets) { }

    #endregion 


    #region Turn Methods

    public virtual void EnterNewTurn()
    {
        if (statusList.Count > 0)
        {
            ApplyStatus();
        }
        UpdateAttributes?.Invoke();
    }

    public void ApplyStatus()
    {
        foreach (Status status in statusList)
        {
            status.Apply(this);

            if (status.effectTurns == 0)
            {
                statusToRemove.Add(status);
            }
        }
        UpdateStatus?.Invoke();
    }

    public virtual void EndTurn()
    {
        foreach (Status status in statusToRemove)
        {
            RemoveStatus(status);
        }
        statusToRemove.Clear();

        movementThisTurn = 0;
        canMove = true;
    }
    #endregion


    #region Elemental Effects

    // Elemental Tile Buffs
    public void ApplyBuffCharacter()
    {
        Hero thisHero = null;
        Enemy_Base thisEnemy = null;

        if (characterType == TurnEnums.CharacterType.Player)
        {
            thisHero = (Hero)this;
        }
        else
        {
            thisEnemy = (Enemy_Base)this;
        }

        if (elementType == ElementType.Fire)
        {
            turnManager.pathfinder.PathTilesInRange(characterTile, 0, 2, true, false);
            List<Tile> tiles = new List<Tile>(turnManager.pathfinder.frontier);
            List<Character> charactersToHit = new List<Character>();

            foreach (Tile tile in tiles)
            {
                Character characterOnTile = tile.characterOnTile;
                if (characterOnTile != null && characterOnTile.characterType != characterType)
                {
                    charactersToHit.Add(tile.characterOnTile);
                }

                if(characterType == TurnEnums.CharacterType.Player)
                {
                    if(thisHero != null)
                    {
                        TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.fireMarker, tile.transform.position, 2f, 0.5f);
                    }
                    else
                    {
                        TemporaryMarker.GenerateMarker(thisEnemy.enemySO.attributes.fireMarker, tile.transform.position, 2f, 0.5f);
                    }
                }
            }
            ApplyStatusAttackArea(charactersToHit);
            MouseTip.Instance.ShowTip(transform.position, $"AOE Burn damage", false);
        }
        else if (elementType == ElementType.Water)
        {
            currentHealth += 4;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            UpdateHealthBar?.Invoke();
            MouseTip.Instance.ShowTip(transform.position, $"Restore full health", false);

            if (characterType == TurnEnums.CharacterType.Player)
            {
                if(thisHero != null)
                {
                    TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.healText, transform.position, 4f, 0.5f);
                }
                else
                {
                    TemporaryMarker.GenerateMarker(thisEnemy.enemySO.attributes.healText, transform.position, 4f, 0.5f);
                }
            }
        }
        else
        {
            AttemptStatusApply(this, Status.StatusTypes.Haste, 1);
            MouseTip.Instance.ShowTip(transform.position, $"Got a Haste", false);
        }
    }

    // AttemptApply Status to targets within attack area
    public void ApplyStatusAttackArea(List<Character> targets)
    {
        foreach (Character target in targets)
        {
            if (elementType == ElementType.Fire)
            {
                AttemptStatusApply(target, Status.StatusTypes.Burning, 2);
            }
            else if (elementType == ElementType.Water)
            {
                AttemptStatusApply(target, Status.StatusTypes.Wet, 2);
            }
            else
            {
                AttemptStatusApply(target, Status.StatusTypes.Bound, 2);
            }
        }
    }

    // AttemptApply Status to everyone on the same ElementType tiles
    public void ApplyStatusElementalTile()
    {
        Status.StatusTypes chosenType;
        List<Tile> chosenList;

        if (elementType == ElementType.Fire)
        {
            chosenType = Status.StatusTypes.Burning;
            chosenList = turnManager.lavaTiles.Cast<Tile>().ToList();
        }
        else if (elementType == ElementType.Water)
        {
            chosenType = Status.StatusTypes.Wet;
            chosenList = turnManager.waterTiles.Cast<Tile>().ToList();
        }
        else
        {
            chosenType = Status.StatusTypes.Bound;
            chosenList = turnManager.grassTiles.Cast<Tile>().ToList();
        }

        foreach(Tile tile in chosenList)
        {
            if(tile.characterOnTile != null && tile.characterOnTile != this && !turnManager.characterList.Contains(tile.characterOnTile))
            {
                AttemptStatusApply(tile.characterOnTile, chosenType, 2);
            }
        }
    }

    // Check if the status is already applied
    public void AttemptStatusApply(Character target, Status.StatusTypes statusType, int effectTurns)
    {
        if(target.characterType == TurnEnums.CharacterType.Player)
        {
            UndoManager.Instance.StoreHero((Hero)target);
        }
        else
        {
            UndoManager.Instance.StoreEnemy((Enemy_Base)target, false);
        }

        Status oldStatus = Status.GrabIfStatusActive(target, statusType);
        if (oldStatus != null)
        {
            oldStatus.effectTurns += 1;
            UpdateStatus?.Invoke();
            return;
        }

        Status newStatus = new Status();
        newStatus.effectTurns = effectTurns;
        newStatus.statusType = statusType;
        target.AddStatus(newStatus);
    }

    // Check additional effects if having a Status and being attacked  
    private int AttackStatusEffect(ElementType type)
    {
        int potentialDamageAddOn = 0;

        if (type == ElementType.Base)
        {
            return 0;
        }

        foreach (Status status in statusList)
        {
            if (status.statusType == Status.StatusTypes.Burning)
            {
                if (type == ElementType.Fire)
                {
                    //status.damageAddOn += 1;
                    TakeDamage(1, ElementType.Base);
                }
                else if (type == ElementType.Water)
                {
                    statusToRemove.Add(status);
                    TakeDamage(2, ElementType.Base);
                }
                else
                {
                    status.effectTurns++;
                }
                break;
            }
            else if (status.statusType == Status.StatusTypes.Wet)
            {
                if (type == ElementType.Fire)
                {
                    //statusToRemove.Add(status);
                    status.effectTurns++;
                    TakeDamage(1, ElementType.Base);
                }
                else if (type == ElementType.Water)
                {
                    //status.effectTurns++;
                    TakeDamage(1, ElementType.Base);
                }
                else
                {
                    statusToRemove.Add(status);
                    TakeDamage(2, ElementType.Base);
                }
                break;
            }
            else if (status.statusType == Status.StatusTypes.Bound)
            {
                if (type == ElementType.Fire)
                {
                    statusToRemove.Add(status);
                    TakeDamage(2, ElementType.Base);
                }
                else if (type == ElementType.Water)
                {
                    status.effectTurns++;
                }
                else
                {
                    //potentialDamageAddOn += 3;
                    TakeDamage(1, ElementType.Base);
                }
                break;
            }
        }

        foreach (Status status in statusToRemove)
        {
            RemoveStatus(status);
        }
        statusToRemove.Clear();

        return potentialDamageAddOn;
    }
    #endregion


    #region Attack Prediction

    // Used on target to predict the damage taken
    public void PreviewDamage(Character attacker)
    {
        int damage = (int)attacker.attackDamage;
        damage += AddedElementalDamagePreview(attacker.elementType);
        damage += PredictElementalStatusDamage(attacker.elementType, damage);

        DamagePreview?.Invoke(damage);
    }

    // Returns the additional or reduced damage taken from Elemental Weakness and Strength
    private int AddedElementalDamagePreview(ElementType attackerType)
    {
        if (elementWeakAgainst == attackerType)
        {
            return +1;
        }
        else if (elementStrongAgainst == attackerType)
        {
            return -1;
        }
        return 0;
    }

    // Returns the additional damage taken from Status Effects
    private int AddedOnDamagePreview(ElementType enemyType)
    {
        int potentialDamageAddOn = 0;

        if (enemyType == ElementType.Base)
        {
            return 0;
        }

        foreach (Status status in statusList)
        {
            if (status.statusType == Status.StatusTypes.Wet)
            {
                if (enemyType == ElementType.Fire)
                {
                    potentialDamageAddOn++;
                }
            }
            if (status.statusType == Status.StatusTypes.Bound)
            {
                if (enemyType == ElementType.Grass)
                {
                    potentialDamageAddOn += 3;
                }
            }
        }

        return potentialDamageAddOn;
    }

    // Predict any additional modification dealt from Status Effects
    // Also Invoke StatusPreview
    private int PredictElementalStatusDamage(ElementType attackerType, int damage)
    {
        int addedDamage = 0;
        if (statusList.Count > 0)
        {
            foreach (Status status in statusList)
            {
                if (status.statusType == Status.StatusTypes.Burning)
                {
                    if (attackerType == ElementType.Water)
                    {
                        addedDamage += 2;
                        RemoveStatusPreview?.Invoke(status);
                    }
                    else if (attackerType == ElementType.Fire)
                    {
                        addedDamage += 1;
                    }
                    else if (attackerType == ElementType.Grass)
                    {
                        ChangeStatusPreview?.Invoke(status, (status.effectTurns + 1));
                    }
                    break;
                }
                else if (status.statusType == Status.StatusTypes.Wet)
                {
                    if (attackerType == ElementType.Grass)
                    {
                        addedDamage += 2;
                        RemoveStatusPreview?.Invoke(status);
                    }
                    else if (attackerType == ElementType.Water)
                    {
                        addedDamage += 1;
                    }
                    else if (attackerType == ElementType.Fire)
                    {
                        addedDamage += 1;
                        ChangeStatusPreview?.Invoke(status, (status.effectTurns + 1));
                    }
                    break;
                }
                else if (status.statusType == Status.StatusTypes.Bound)
                {
                    if (attackerType == ElementType.Fire)
                    {
                        addedDamage += 2;
                        RemoveStatusPreview?.Invoke(status);
                    }
                    else if (attackerType == ElementType.Grass)
                    {
                        addedDamage += 1;
                    }
                    else if (attackerType == ElementType.Water)
                    {
                        ChangeStatusPreview?.Invoke(status, (status.effectTurns + 1));
                    }
                    break;
                }
            }
        }
        return addedDamage;
    }

    // Used on attacker to predict the status on the targets
    public void PredictTargetsStatus(List<Character> targets, Tile origin)
    {
        Status.StatusTypes chosenType = Status.StatusTypes.None;

        // ------ Start Getting Targets------
        // Add targets in the attack area
        List<Character> targetsToCheck = new List<Character>(targets);

        // Fire guy can add status on Buff
        if (elementType == ElementType.Fire)
        {
            chosenType = Status.StatusTypes.Burning;

            List<Tile> tiles = Pathfinder.Instance.ReturnRange(origin);

            List<Character> charactersToHit = new List<Character>();

            foreach (Tile tile in tiles)
            {
                Character characterOnTile = tile.characterOnTile;
                if (characterOnTile != null && characterOnTile.characterType != characterType)
                {
                    charactersToHit.Add(tile.characterOnTile);
                }
            }

            // Add targets within range 2
            foreach (Character character in charactersToHit)
            {
                if (!targetsToCheck.Contains(character))
                {
                    targetsToCheck.Add(character);
                }
            }
        }
        else if (elementType == ElementType.Water)
        {
            chosenType = Status.StatusTypes.Wet;
        }
        else if (elementType == ElementType.Grass)
        {
            chosenType = Status.StatusTypes.Bound;
        }


        // Add targets on the same elemental tile
        foreach (Character character in GetTargetsOnElementalTile())
        {
            if (!targetsToCheck.Contains(character))
            {
                targetsToCheck.Add(character);
            }
        }

        // ------ Start Checking Status------
        Status addStatus = new Status();
        addStatus.statusType = chosenType;
        addStatus.effectTurns = 2;

        foreach (Character character in targetsToCheck)
        {
            character.PredictAddStatus(addStatus);
        }
    }

    // Returns a list of targets on the same Elemental Tile
    private List<Character> GetTargetsOnElementalTile()
    {
        List<Character> targets = new List<Character>();
        List<Tile> tileList;

        if (elementType == ElementType.Fire)
        {
            tileList = turnManager.lavaTiles.Cast<Tile>().ToList();
        }
        else if (elementType == ElementType.Water)
        {
            tileList = turnManager.waterTiles.Cast<Tile>().ToList();
        }
        else
        {
            tileList = turnManager.grassTiles.Cast<Tile>().ToList();
        }

        foreach (Tile tile in tileList)
        {
            if (tile.characterOnTile != null && tile.characterOnTile != this && !turnManager.characterList.Contains(tile.characterOnTile))
            {
                targets.Add(tile.characterOnTile);
            }
        }
        return targets;
    }

    // Check if the status is already existing
    private void PredictAddStatus(Status status)
    {
        if (statusList.Count > 0)
        {
            foreach (Status oldStatus in statusList)
            {
                if (oldStatus.statusType == status.statusType)
                {
                    ChangeStatusPreview?.Invoke(oldStatus, (oldStatus.effectTurns + 1));
                    return;
                }
            }
        }
        AddStatusPreview?.Invoke(status);
    }
    #endregion


    #region Actual Status add and remove

    public void AddStatus(Status status) // Consider make this private method
    {
        ShowEffect(status);
        statusList.Add(status);

        UpdateStatus?.Invoke();
    }

    public void ShowEffect(Status status)
    {
        GameObject vfx = null;
        switch (status.statusType)
        {
            case Status.StatusTypes.Burning:
                vfx = Instantiate(Config.Instance.characterUIConfig.burningVFX, transform.position, Quaternion.identity);
                break;

            case Status.StatusTypes.Bound:
                vfx = Instantiate(Config.Instance.characterUIConfig.boundVFX, transform.position, Quaternion.identity);
                break;

            case Status.StatusTypes.Blessing:
                break;

            case Status.StatusTypes.CannotMove:
                break;

            case Status.StatusTypes.CannotAttack:
                break;

            case Status.StatusTypes.Wet:
                vfx = Instantiate(Config.Instance.characterUIConfig.wetVFX, transform.position, Quaternion.identity);
                break;

            case Status.StatusTypes.Haste:
                break;

            case Status.StatusTypes.Shield:
                break;

            default:
                break;
        }

        if (vfx != null)
        {
            vfx.transform.SetParent(transform);
            StatusVFX statusVFX = vfx.GetComponent<StatusVFX>();
            statusVFX.Initialize(this, status.effectTurns);
        }
    }

    public void RemoveStatus(Status status)
    {
        statusList.Remove(status);

        UpdateStatus?.Invoke();

        if (status.statusType == Status.StatusTypes.Hurt)
        {
            isHurt = false;
        }
    }
    #endregion


    #region TakeDamage and Heal and Die

    public virtual void TakeDamage(float damage, ElementType type)
    {
        if(statusList.Count > 0)
        {
            damage += AttackStatusEffect(type);
        }

        /*foreach(Status status in statusList)
        {
            if(status.statusType == Status.StatusTypes.Bound)
            {
                damage += 2;
            }
        }*/

        currentHealth -= damage;

        if (isHurt) { currentHealth--; }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Died();
        }
        else
        {
            animator.SetTrigger("hit");
        }

        // Show damage text
        DamageText damageText = Instantiate(damagePrefab, transform.position, Quaternion.identity).GetComponent<DamageText>();
        damageText.ShowDamage(damage);

        UpdateHealthBar?.Invoke();
    }

    public virtual void Heal(float heal)
    {
        currentHealth += heal;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateHealthBar?.Invoke();
    }

    public virtual void Died()
    {
        FindObjectOfType<CameraController>().MoveToDeathPosition(transform, true);
        Time.timeScale = 0.5f;
        if (animator != null)
        {
            animator.SetTrigger("died");
        }
        DestroyTileVFX();
        Invoke("Destroy", 0.6f);
    }

    private void Destroy()
    {
        FindObjectOfType<CameraController>().MoveToDefault(true);
        Time.timeScale = 1f;
        turnManager.DestroyACharacter(this);
    }
    #endregion


    #region BreadthFirstMethods

    //Used for planting the character down onto a tile when the game starts
    public void FindTile()
    {
        if (characterTile != null)
        {
            FinalizeTileChoice(characterTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, tileLayer))
        {
            FinalizeTileChoice(hit.transform.GetComponent<Tile>());
            return;
        }
    }

    //Starts the process of Moving and Attacking with the Character
    public void MoveAndAttack(Tile[] path, Tile attackTargetTile, TurnManager turnManager, bool activeSkillUse, Vector3 tilePos)
    {
        if(path.Length > 0)
        {
            moving = true;
            animator.SetBool("walking", true);
            turnManager.mainCameraController.controlEnabled = false;
            DestroyTileVFX();
        }

        StartCoroutine(PerformMoveAndAttack(path, attackTargetTile, turnManager, activeSkillUse, tilePos));
    }

    //Moves the character if a path is provided and performs an attack if a Tile is provided
    private IEnumerator PerformMoveAndAttack(Tile[] path, Tile attackTargetTile, TurnManager turnManager, bool activeSkillUse, Vector3 tilePos)
    {
        //Moves the Character
        if(path.Length > 0)
        {
            int step = 1;
            int pathLength = path.Length;
            List<Tile> tilesInPath = new List<Tile>(path);

            float animationTime = 0f;
            const float distanceToNext = 0.05f;
            turnManager.mainCameraController.FollowTarget(transform, true);

            characterTile.tileOccupied = false;
            characterTile.characterOnTile = null;
            characterTile.OnTileExit(this);

            Tile currentTile = path[0];
            tilesInPath.Remove(currentTile);

            while(step < pathLength)
            {
                yield return null;

                foreach (Tile tile in tilesInPath)
                {
                    tile.ChangeTileColor(TileEnums.TileMaterial.path);
                }

                Vector3 nextTilePosition = path[step].transform.position;

                //Moves and roates towards the next point
                MoveAndRotate(currentTile.transform.position, nextTilePosition, animationTime / (moveSpeed / GameManager.Instance.GameSpeed));
                animationTime += Time.deltaTime;

                //Checks if we are close enough to move onto the next point
                if (Vector3.Distance(transform.position, nextTilePosition) > distanceToNext)
                {
                    continue;
                }
                movementThisTurn += (int)path[step].tileData.tileCost;

                //Moves onto the next point
                currentTile = path[step];
                currentTile.OnTileEnter(this);
                currentTile = WalkOntoTileEffect(currentTile);

                if(currentTile.tileHasObject && currentTile.objectOnTile.objectType == ObjectType.PoisonCloud)
                {
                    PoisonCloud poison = (PoisonCloud)currentTile.objectOnTile;
                    TakeDamage(poison.Damage, ElementType.Poison);
                }

                tilesInPath.Remove(path[step]);
                path[step].ChangeTileColor(TileEnums.TileMaterial.baseMaterial);

                step++;

                //Checks if we have arrived at the last tile, if not it triggers OnTileExit
                if (step < pathLength)
                {
                    currentTile.OnTileExit(this);
                }

                animationTime = 0f;
            }

            foreach (Tile tile in path)
            {
                if(tile == null)
                {
                    continue;
                }
                tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            }

            characterTile = null;
            transform.position += new Vector3(0, 0.2f, 0);

            yield return null;
            FindTile();

            animator.SetBool("walking", false);
        }

        //Makes the character attack
        if(attackTargetTile != null && characterType != TurnEnums.CharacterType.Enemy)
        {
            Hero thisHero = (Hero)this;
            AttackArea attackAreaPrefab;

            if (activeSkillUse)
            {
                attackAreaPrefab = Instantiate(activeSkillArea);
                if(attackAreaPrefab.freeRange)
                {
                    attackAreaPrefab.transform.position = tilePos;
                }
                else
                {
                    attackAreaPrefab.PositionAndRotateAroundCharacter(turnManager.pathfinder, characterTile, attackTargetTile);
                }
            }
            else
            {
                attackAreaPrefab = Instantiate(basicAttackArea);
                attackAreaPrefab.PositionAndRotateAroundCharacter(turnManager.pathfinder, characterTile, attackTargetTile);
            }

            yield return new WaitForSeconds(0.03f);
            attackAreaPrefab.DetectArea(true, true);
            attackAreaPrefab.ExecuteAddOnEffects();

            transform.LookAt(attackAreaPrefab.transform.position);

            List<Character> enemiesHit = new List<Character>();
            List<Character> heroesHit = new List<Character>();
            List<TileObject> objectsHit = attackAreaPrefab.ObjectsHit();

            if(attackAreaPrefab.hitsEnemies)
            {
                enemiesHit = attackAreaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy);
                if (activeSkillUse)
                {
                    ReleaseActiveSkill(enemiesHit);
                }
                else
                {
                    PerformBasicAttack(enemiesHit);
                }
            }
            if (attackAreaPrefab.hitsHeroes)
            {
                heroesHit = attackAreaPrefab.CharactersHit(TurnEnums.CharacterType.Player);
                if (activeSkillUse)
                {
                    ReleaseActiveSkill(heroesHit);
                }
                else
                {
                    PerformBasicAttack(heroesHit);
                }
            }

            if (attackAreaPrefab.hitsTileObjects)
            {
                PerformBasicAttackObjects(objectsHit);
            }

            GenerateHitMarkers(thisHero, enemiesHit, heroesHit, objectsHit);

            if (characterTile.tileData.tileType == elementType)
            {
                ApplyBuffCharacter();
            }

            yield return new WaitForSeconds(0.5f);
            attackAreaPrefab.DestroySelf();
        }

        if(turnManager.TurnType == TurnEnums.TurnState.PlayerTurn)
        {
            turnManager.mainCameraController.controlEnabled = true;
        }
        turnManager.mainCameraController.StopFollowingTarget();

        if(characterType == TurnEnums.CharacterType.Player)
        {
            turnManager.mainCameraController.MoveToDefault(true);
        }

        movementComplete.Invoke(this);
    }

    //Generates the HitMarker prefab over hit Enemies and Objects
    public void GenerateHitMarkers(Hero thisHero, List<Character> enemiesHit, List<Character> heroesHit, List<TileObject> objectsHit)
    {
        foreach (Character character in enemiesHit)
        {
            TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.hitMarker, character.transform.position, 1.5f, 0.5f);
        }
        foreach (TileObject tileObj in objectsHit)
        {
            TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.hitMarker, tileObj.transform.position, 2.5f, 0.5f);
        }
        foreach (Character character in heroesHit)
        {
            TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.hitMarker, character.transform.position, 1.5f, 0.5f);
        }
    }

    //Moves and Rotates the character
    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);

        float angle = Vector3.Angle(transform.forward, destination - transform.forward);
        if(Mathf.Abs(angle) <= 0.5f)
        {
            transform.LookAt(destination);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(destination - origin), Time.deltaTime * 5.0f);
        }
    }

    //Plants the character down onto the tile they are overtop of
    public void FinalizeTileChoice(Tile tile)
    {
        transform.position = tile.transform.position + new Vector3(0, 0.2f, 0);
        characterTile = tile;

        moving = false;

        tile.tileOccupied = true;
        tile.characterOnTile = this;
    }

    public void PushedBack(Vector3 direction, int distance)
    {
        Tile targetTile = characterTile;
        List<Tile> tiles = new List<Tile>();

        //temporarily get the pathfinder
        Pathfinder pathfinder = GameObject.Find("MapNavigators").GetComponentInChildren<Pathfinder>();

        while (distance > 0)
        {
            Tile newTile = pathfinder.GetTileInDirection(targetTile, direction);
            if (newTile == null || newTile.tileOccupied)
            {
                break;
            }
            targetTile = newTile;
            tiles.Add(newTile);
            distance--;
        }

        if (tiles != null && tiles.Count != 0)
        {
            Tile[] path = tiles.ToArray();
            TurnManager turnManager = FindObjectOfType<TurnManager>();
            MoveAndAttack(path, null, turnManager, false, Vector3.zero);
            // TODO: rotate after move, fix movement reduced next turn
            //StartCoroutine(RotateBack());
        }
    }

    public void DragTowards(Tile dragTile, int damage)
    {
        Pathfinder pathFinder = GameObject.Find("MapNavigators").GetComponentInChildren<Pathfinder>();

        Tile currentTile = characterTile;
        float curDistance = Vector3.Distance(currentTile.transform.position, dragTile.transform.position);

        while (currentTile != dragTile)
        {
            float inspectDistance = 100f;
            Tile chosenTile = null;

            foreach(Tile tile in pathFinder.FindAdjacentTiles(currentTile, true))
            {
                float newDistance = Vector3.Distance(tile.transform.position, dragTile.transform.position);
                if (newDistance < inspectDistance)
                {
                    chosenTile = tile;
                    inspectDistance = newDistance;
                }
            }

            if(chosenTile == null || chosenTile.tileOccupied || inspectDistance >= curDistance)
            {
                break;
            }

            currentTile = chosenTile;
            curDistance = Vector3.Distance(currentTile.transform.position, dragTile.transform.position);
        }

        characterTile.characterOnTile = null;
        characterTile.tileOccupied = false;
        characterTile = null;

        transform.position = currentTile.transform.position + new Vector3(0, 0.2f, 0);
        FindTile();

        TakeDamage(damage, ElementType.Base);

        Status status = Status.GrabIfStatusActive(this, Status.StatusTypes.Wet);
        if(status == null)
        {
            status = new Status();
            status.effectTurns = 1;
            status.statusType = Status.StatusTypes.Wet;
        }
        else
        {
            status.effectTurns += 1;
        }
    }

    IEnumerator RotateBack()
    {
        while (moving)
        {
            yield return null;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-transform.forward), Time.deltaTime * 5.0f);
    }

    protected virtual Tile WalkOntoTileEffect(Tile tile)
    {
        return tile;
    }

    public void ResetCharacter()
    {
        moving = false;
        statusList.Clear();
        hasMadeDecision = false;
        effectedByWeather = false;
    }

    public void SpawnBuffPreview(Vector3 location, float height)
    {
        if(characterType == TurnEnums.CharacterType.Player)
        {
            Hero thisHero = (Hero)this;
            GameObject buffPrefab = thisHero.heroSO.attributes.buffPrefab;
            if (buffPrefab != null)
            {
                Vector3 potentialLocation = location;
                potentialLocation.y += height;
                if(buffPreview != null && buffPreview.transform.localPosition == potentialLocation)
                {
                    return;
                }
                DestroyBuffPreview();
                buffPreview = TemporaryMarker.GenerateMarker(buffPrefab, location, height);
            }
        }
    }

    public void DestroyBuffPreview()
    {
        if(buffPreview != null)
        {
            Destroy(buffPreview);
        }
    }

    public void SpawnTileVFX(Vector3 position, bool isBuff)
    {
        if (tileVFX == null)
        {
            if (isBuff)
            {
                tileVFX = Instantiate(Config.Instance.GetBuffVFX(elementType, true), position, Quaternion.identity);
            }
            else
            {
                tileVFX = Instantiate(Config.Instance.GetDebuffVFX(true), position, Quaternion.identity);
            }
        }
    }

    public void DestroyTileVFX()
    {
        if (tileVFX != null)
        {
            Destroy(tileVFX);
        }
    }

    #endregion
}