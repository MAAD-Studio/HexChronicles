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

    [Header("Character Attack Info:")]
    [SerializeField] public int attackDistance = 2;
    [SerializeField] public TurnEnums.CharacterType characterType;
    [SerializeField] public List<Status> statusList = new List<Status>();
    [HideInInspector] public AttackArea basicAttackArea;
    [HideInInspector] public AttackArea activeSkillArea;

    [Header("Character Basic Attributes:")]
    [HideInInspector] public float currentHealth = 0f;
    [HideInInspector] public float maxHealth = 0f;
    [HideInInspector] public float attackDamage = 0;
    [HideInInspector] public float defensePercentage = 0;
    public ElementType elementType;

    [Header("Tile LayerMask:")]
    [SerializeField] private LayerMask tileLayer;

    [HideInInspector] public bool moving = false;
    [HideInInspector] public Tile characterTile;
    private Tile previousTile;

    #endregion

    /*#region Events
    public event EventHandler OnDamage;
    public event EventHandler OnHeal;
    public event EventHandler OnDeath;
    #endregion*/

    #region UnityMethods

    protected virtual void Start()
    {
        FindTile();
    }

    void Update()
    {

    }

    #endregion

    #region AttackMethods
    public virtual void PerformBasicAttack(List<Character> targets) { }
    public virtual void ReleaseActiveSkill(List<Character> targets) { }

    public void AddStatus(Status status)
    {
        statusList.Add(status);
    }

    public void RemoveStatus(Status status)
    {
        statusList.Remove(status);
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

        if (currentHealth < 0)
        {
            currentHealth = 0;
            Died();
            //OnDeath?.Invoke(this, EventArgs.Empty);
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
        // TODO
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

        characterTile.OnTileExit(this);
        Tile currentTile = path[0];

        //Debug.Log("CURRENT TILE TARGET: " + currentTile.name);

        float animationTime = 0f;
        const float distanceToNext = 0.05f;

        //While we still have points in the path to cover
        while (step < pathLength)
        {
            yield return null;

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

            step++;

            //Checks if we have arrived at the last tile, if not it triggers OnTileExit
            if(step < pathLength)
            {
                previousTile.OnTileExit(this);
            }

            animationTime = 0f;
        }

        //Plants the character down onto the newest tile
        FinalizeTileChoice(path[pathLength - 1]);
    }

    //Starts the process of moving the character to a new location
    public void Move(Tile[] _path)
    {
        moving = true;
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

    #endregion
}