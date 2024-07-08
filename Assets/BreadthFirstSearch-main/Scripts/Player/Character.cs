using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("Character Movement Info:")]
    [SerializeField] private float moveSpeed = 0.4f;
    [SerializeField] public int moveDistance = 2;
    [HideInInspector] public int movementThisTurn = 0;
    [HideInInspector] public bool canMove = true;

    [Header("Character Attack Info:")]
    [SerializeField] public int attackDistance = 2;
    [SerializeField] public TurnEnums.CharacterType characterType;
    [HideInInspector] public AttackArea basicAttackArea;
    [HideInInspector] public AttackArea activeSkillArea;
    [HideInInspector] public bool canAttack = true;

    [Header("Character Basic Attributes:")]
    [HideInInspector] public float currentHealth = 0f;
    [HideInInspector] public float maxHealth = 0f;
    [HideInInspector] public float attackDamage = 0;
    [HideInInspector] public float defensePercentage = 0;
    [SerializeField] protected Animator animator;
    [SerializeField] public HealthBar healthBar;
    [HideInInspector] public ElementType elementType;
    [HideInInspector] public ElementType elementWeakAgainst;
    [HideInInspector] public ElementType elementStrongAgainst;
    [HideInInspector] public Vector3 defaultScale;


    [Header("Tile LayerMask:")]
    [SerializeField] private LayerMask tileLayer;

    [HideInInspector] public bool moving = false;
    [HideInInspector] public Tile characterTile;

    [Header("Character Status:")]
    public List<Status> statusList = new List<Status>();
    private List<Status> statusToRemove = new List<Status>();
    [HideInInspector] public bool isHurt = false;

    [HideInInspector] public bool effectedByWeather = false;
    [HideInInspector] public bool hasMadeDecision = false;

    [HideInInspector] public static UnityEvent<Character> movementComplete = new UnityEvent<Character>();

    [HideInInspector] public UnityEvent DamagePreview = new UnityEvent();
    [HideInInspector] public UnityEvent UpdateHealthBar = new UnityEvent();
    [HideInInspector] public UnityEvent UpdateAttributes = new UnityEvent();
    [HideInInspector] public UnityEvent UpdateStatus = new UnityEvent();

    private GameObject buffPreview;

    #endregion

    #region UnityMethods

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Can not find Animatior Component on Character");

        FindTile();
    }

    void Update()
    {

    }

    #endregion

    #region AttackMethods

    public virtual void PerformBasicAttack(List<Character> targets)
    {
        animator.SetTrigger("attack");

        if (characterTile.tileData.tileType == elementType)
        {
            TurnManager turnManager = FindObjectOfType<TurnManager>();
            ApplyBuffCharacter(turnManager);
            ApplyStatusAttackArea(targets);
            ApplyStatusElementalTile(turnManager);
        }
    }

    public void ApplyBuffCharacter(TurnManager turnManager)
    {
        Hero thisHero = null;
        if (characterType == TurnEnums.CharacterType.Player)
        {
            thisHero = (Hero)this;
        }

        if (elementType == ElementType.Fire)
        {
            turnManager.pathfinder.PathTilesInRange(characterTile, 0, 2, true, false);
            List<Tile> tiles = new List<Tile>(turnManager.pathfinder.frontier);
            List<Character> charactersToHit = new List<Character>();

            foreach (Tile tile in tiles)
            { 
                if(tile.characterOnTile != null && tile.characterOnTile != this && !turnManager.characterList.Contains(tile.characterOnTile))
                {
                    charactersToHit.Add(tile.characterOnTile);
                }

                if(characterType == TurnEnums.CharacterType.Player)
                {
                    TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.fireMarker, tile.transform.position, 2f, 0.5f);
                }
            }
            ApplyStatusAttackArea(charactersToHit);
            MouseTip.Instance.ShowTip(transform.position, $"AOE Burn damage", false);
        }
        else if (elementType == ElementType.Water)
        {
            currentHealth = maxHealth;
            UpdateHealthBar?.Invoke();
            MouseTip.Instance.ShowTip(transform.position, $"Restore full health", false);

            if (characterType == TurnEnums.CharacterType.Player)
            {
                TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.healText, transform.position, 4f, 0.5f);
            }
        }
        else
        {
            Status newStatus = new Status();
            newStatus.effectTurns = 1;
            newStatus.statusType = Status.StatusTypes.Haste;
            AddStatus(newStatus);
            MouseTip.Instance.ShowTip(transform.position, $"Got a Haste", false);
        }
    }

    public void ApplyStatusAttackArea(List<Character> targets)
    {
        foreach (Character target in targets)
        {
            if (elementType == ElementType.Fire)
            {
                AttemptStatusApply(Status.StatusTypes.Burning, target, false);
            }
            else if (elementType == ElementType.Water)
            {
                AttemptStatusApply(Status.StatusTypes.Wet, target, true);
            }
            else
            {
                AttemptStatusApply(Status.StatusTypes.Bound, target, true);
            }
        }
    }

    public void ApplyStatusElementalTile(TurnManager turnManager)
    {
        Status.StatusTypes chosenType = Status.StatusTypes.None;
        List<Tile> chosenList = new List<Tile>();

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
                if (chosenType == Status.StatusTypes.Burning)
                {
                    AttemptStatusApply(chosenType, tile.characterOnTile, false);
                }
                else
                {
                    AttemptStatusApply(chosenType, tile.characterOnTile, true);
                }
            }
        }
    }

    public void AttemptStatusApply(Status.StatusTypes statusType, Character target, bool checkOld)
    {
        if(checkOld)
        {
            Status oldStatus = Status.GrabIfStatusActive(target, statusType);
            if (oldStatus != null)
            {
                oldStatus.effectTurns += 1;
                return;
            }
        }

        Status newStatus = new Status();
        newStatus.effectTurns = 2;
        newStatus.statusType = statusType;
        target.AddStatus(newStatus);
    }

    public virtual void ReleaseActiveSkill(List<Character> targets)
    {
        animator.SetTrigger("skill");
    }

    public virtual void PerformBasicAttackObjects(List<TileObject> targets) { }

    public void PreviewDamage(float damage)
    {
        healthBar.damagePreview = damage;
        DamagePreview?.Invoke();
    }


    public virtual void EnterNewTurn()
    {
        if (statusList.Count > 0)
        {
            ApplyStatus();
        }
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

    public void AddStatus(Status status)
    {
        statusList.Add(status);

        UpdateStatus.Invoke();
    }

    public void RemoveStatus(Status status)
    {
        statusList.Remove(status);

        UpdateStatus.Invoke();

        if (status.statusType == Status.StatusTypes.Hurt)
        {
            isHurt = false;
        }
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

    public virtual void TakeDamage(float damage, ElementType type)
    {
        if(statusList.Count > 0)
        {
            damage += AttackStatusEffect(type);
        }

        foreach(Status status in statusList)
        {
            if(status.statusType == Status.StatusTypes.Bound)
            {
                damage += 2;
            }
        }

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
        UpdateHealthBar?.Invoke();
    }

    private int AttackStatusEffect(ElementType type)
    {
        int potentialDamageAddOn = 0;

        if(type == ElementType.Base)
        {
            return 0;
        }

        foreach(Status status in statusList)
        {
            if(status.statusType == Status.StatusTypes.Burning)
            {
                if(type == ElementType.Fire)
                {
                    Debug.Log("CHARACTER BURNING DEALING FIRE DAMAGE");
                    status.damageAddOn += 1;
                }
                else if(type == ElementType.Water)
                {
                    Debug.Log("CHARACTER BURNING DEALING WATER DAMAGE");
                    statusToRemove.Add(status);
                }
                else
                {
                    Debug.Log("CHARACTER BURNING DEALING GRASS DAMAGE");
                    status.effectTurns++;
                }
                break;
            }
            else if(status.statusType == Status.StatusTypes.Wet)
            {
                if (type == ElementType.Fire)
                {
                    Debug.Log("CHARACTER WET DEALING FIRE DAMAGE");
                    TakeDamage(1, ElementType.Base);
                    statusToRemove.Add(status);
                }
                else if (type == ElementType.Water)
                {
                    Debug.Log("CHARACTER WET DEALING WATER DAMAGE");
                    MouseTip.Instance.ShowTip(transform.position, "CHARACTER WET DEALING WATER DAMAGE", false);
                    status.effectTurns++;
                }
                else
                {
                    Debug.Log("CHARACTER WET DEALING GRASS DAMAGE");
                    statusToRemove.Add(status);
                }
                break;
            }
            else if(status.statusType == Status.StatusTypes.Bound)
            {
                if (type == ElementType.Fire)
                {
                    Debug.Log("CHARACTER BOUND DEALING FIRE DAMAGE");
                    statusToRemove.Add(status);
                }
                else if (type == ElementType.Water)
                {
                    Debug.Log("CHARACTER BOUND DEALING WATER DAMAGE");
                    status.effectTurns++;
                }
                else
                {
                    Debug.Log("CHARACTER BOUND DEALING GRASS DAMAGE");
                    potentialDamageAddOn += 3;
                }
                break;
            }
        }

        foreach(Status status in statusToRemove)
        {
            RemoveStatus(status);
        }
        statusToRemove.Clear();

        return potentialDamageAddOn;
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
        if(animator != null)
        {
            animator.SetTrigger("died");
        }
  
        Invoke("Destroy", 0.6f);
    }

    private void Destroy()
    {
        TurnManager tm = FindObjectOfType<TurnManager>();
        tm.DestroyACharacter(this);
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
                MoveAndRotate(currentTile.transform.position, nextTilePosition, animationTime / moveSpeed);
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

            List<Character> enemiesHit = attackAreaPrefab.CharactersHit(TurnEnums.CharacterType.Enemy);
            List<TileObject> objectsHit = attackAreaPrefab.ObjectsHit();
            if (activeSkillUse)
            {
                ReleaseActiveSkill(enemiesHit);
            }
            else
            {
                PerformBasicAttack(enemiesHit);
            }
            PerformBasicAttackObjects(objectsHit);
            GenerateHitMarkers(thisHero, enemiesHit, objectsHit);

            yield return new WaitForSeconds(0.5f);
            attackAreaPrefab.DestroySelf();
        }

        turnManager.mainCameraController.controlEnabled = true;
        turnManager.mainCameraController.StopFollowingTarget();

        if(characterType == TurnEnums.CharacterType.Player)
        {
            turnManager.mainCameraController.MoveToDefault(true);
        }

        movementComplete.Invoke(this);
    }

    //Generates the HitMarker prefab over hit Enemies and Objects
    public void GenerateHitMarkers(Hero thisHero, List<Character> enemiesHit, List<TileObject> objectsHit)
    {
        foreach (Character character in enemiesHit)
        {
            TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.hitMarker, character.transform.position, 1.5f, 0.5f);
        }
        foreach (TileObject tileObj in objectsHit)
        {
            TemporaryMarker.GenerateMarker(thisHero.heroSO.attributes.hitMarker, tileObj.transform.position, 2.5f, 0.5f);
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
        transform.position = tile.transform.position + new Vector3(0, 0.1f, 0);
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

    #endregion
}