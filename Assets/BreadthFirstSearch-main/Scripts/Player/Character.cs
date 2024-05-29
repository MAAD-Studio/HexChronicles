using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    #region Variables

    [Header("Character Movement Info:")]
    [SerializeField] private float moveSpeed = 0.4f;
    [SerializeField] public int moveDistance = 2;
    [HideInInspector] public int movementThisTurn = 0;
    [HideInInspector] public bool hasMoved = false;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool needsToPath = true;

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
    public ElementType elementType;
    [SerializeField] protected Animator animator;
    [SerializeField] private HealthBar healthBar;

    [Header("Tile LayerMask:")]
    [SerializeField] private LayerMask tileLayer;

    [HideInInspector] public bool moving = false;
    [HideInInspector] public Tile characterTile;
    private Tile previousTile;

    [Header("Character Status:")]
    public List<Status> statusList = new List<Status>();
    [HideInInspector] public bool isHurt = false;

    [HideInInspector] public bool effectedByWeather = false;

    #endregion

    /*#region Events
    public event EventHandler OnDamage;
    public event EventHandler OnHeal;
    public event EventHandler OnDeath;
    #endregion*/

    #region UnityMethods

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Can not find Animatior Component on Character");

        if (healthBar == null) { healthBar = GetComponentInChildren<HealthBar>(); }
        healthBar.character = this;

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

    public virtual void EnterNewTurn()
    {
        if (statusList.Count > 0)
        {
            ApplyStatus();
        }
    }

    public virtual void EndTurn()
    {
        movementThisTurn = 0;
        canMove = true;
        canAttack = true;
        hasMoved = false;
    }

    public void AddStatus(Status status)
    {
        statusList.Add(status);
    }

    public void RemoveStatus(Status status)
    {
        statusList.Remove(status);

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
                RemoveStatus(status);
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (isHurt) { currentHealth--; }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Died();
            //OnDeath?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            animator.SetTrigger("hit");
        }
        //OnDamage?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Heal(float heal)
    {
        currentHealth += heal;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        //OnHeal?.Invoke(this, EventArgs.Empty);
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

    //Carries the character through a path
    IEnumerator MoveThroughPath(Tile[] path)
    {
        int step = 1;
        int pathLength = Mathf.Clamp(path.Length, 0, moveDistance + 1);

        List<Tile> tilesInPath = new List<Tile>();
        foreach(Tile tile in path)
        {
            tilesInPath.Add(tile);
        }

        characterTile.OnTileExit(this);
        Tile currentTile = path[0];
        tilesInPath.Remove(currentTile);

        float animationTime = 0f;
        const float distanceToNext = 0.05f;

        //While we still have points in the path to cover
        while (step < pathLength)
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
            previousTile = currentTile;
            currentTile = path[step];
            currentTile.OnTileEnter(this);
            tilesInPath.Remove(path[step]);
            path[step].ChangeTileColor(TileEnums.TileMaterial.baseMaterial);

            step++;

            //Checks if we have arrived at the last tile, if not it triggers OnTileExit
            if(step < pathLength)
            {
                previousTile.OnTileExit(this);
            }

            animationTime = 0f;
        }

        foreach(Tile tile in path)
        {
            tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
        }

        //Plants the character down onto the newest tile
        FinalizeTileChoice(path[pathLength - 1]);

        animator.SetBool("walking", false);
    }

    //Starts the process of moving the character to a new location
    public void Move(Tile[] _path)
    {
        moving = true;
        animator.SetBool("walking", true);

        characterTile.tileOccupied = false;
        StartCoroutine(MoveThroughPath(_path));
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
            Move(path);
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

    #endregion
}