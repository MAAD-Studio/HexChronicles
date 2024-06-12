using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public event EventHandler OnDamagePreview;
    public event EventHandler OnUpdateHealthBar;
    public event EventHandler OnUpdateAttributes;
    public event EventHandler OnUpdateStatus;

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
    }
    public virtual void ReleaseActiveSkill(List<Character> targets)
    {
        animator.SetTrigger("skill");
    }

    public virtual void PerformBasicAttackObjects(List<TileObject> targets) { }

    public void PreviewDamage(float damage)
    {
        healthBar.damagePreview = damage;
        OnDamagePreview?.Invoke(this, EventArgs.Empty);
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
        OnUpdateStatus.Invoke(this, EventArgs.Empty);
    }

    public void RemoveStatus(Status status)
    {
        statusList.Remove(status);
        OnUpdateStatus.Invoke(this, EventArgs.Empty);

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
    }

    public virtual void InvokeUpdateHealthBar()
    {
        OnUpdateHealthBar?.Invoke(this, EventArgs.Empty);
    }

    public virtual void InvokeUpdateAttributes()
    {
        OnUpdateAttributes?.Invoke(this, EventArgs.Empty);
    }

    public virtual void TakeDamage(float damage, ElementType type)
    {
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
        InvokeUpdateHealthBar();
    }

    public virtual void Heal(float heal)
    {
        currentHealth += heal;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        InvokeUpdateHealthBar();
    }

    public virtual void Died()
    {
        animator.SetTrigger("died");
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
    public void MoveAndAttack(Tile[] path, Tile attackTargetTile, TurnManager turnManager, bool activeSkillUse)
    {
        if(path.Length > 0)
        {
            moving = true;
            animator.SetBool("walking", true);
            turnManager.mainCameraController.controlEnabled = false;
        }

        StartCoroutine(PerformMoveAndAttack(path, attackTargetTile, turnManager, activeSkillUse));
    }

    //Moves the character if a path is provided and performs an attack if a Tile is provided
    private IEnumerator PerformMoveAndAttack(Tile[] path, Tile attackTargetTile, TurnManager turnManager, bool activeSkillUse)
    {
        //Moves the Character
        if(path.Length > 0)
        {
            int step = 1;
            int pathLength = Mathf.Clamp(path.Length, 0, moveDistance + 1);
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
                else
                {
                    currentTile.OnTileStay(this);
                }

                animationTime = 0f;
            }

            foreach (Tile tile in path)
            {
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
            }
            else
            {
                attackAreaPrefab = Instantiate(basicAttackArea);
            }
            attackAreaPrefab.PositionAndRotateAroundCharacter(turnManager.pathfinder, characterTile, attackTargetTile);
            yield return new WaitForSeconds(0.03f);
            attackAreaPrefab.DetectArea(true, true);

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
        turnManager.mainCameraController.MoveToDefault(true);

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
        transform.position = tile.transform.position;
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
            MoveAndAttack(path, null, turnManager, false);
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

    #endregion
}