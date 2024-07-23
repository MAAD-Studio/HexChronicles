using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static TurnEnums;

public class TileObject : MonoBehaviour
{
    [Header("Setup Info:")]
    [SerializeField] protected LayerMask tileLayer;
    protected TurnManager turnManager;
    [SerializeField] public TileObjectSO tileObjectData;
    [HideInInspector] public float currentHealth = 0f;
    [SerializeField] public TileObjectHealthBar healthBar;

    [HideInInspector] public static UnityEvent<TileObject> objectDestroyed = new UnityEvent<TileObject>();
    [HideInInspector] public static UnityEvent<TileObject> objectCreated = new UnityEvent<TileObject>();

    [HideInInspector] public UnityEvent<int> DamagePreview;
    [HideInInspector] public UnityEvent DonePreview;
    [HideInInspector] public UnityEvent UpdateHealthBar;

    public ObjectType objectType;

    protected Tile attachedTile;
    public Tile AttachedTile
    {
        get { return attachedTile; }
    }

    public virtual void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();

        currentHealth =  tileObjectData.health;
        if (healthBar == null) 
        { 
            healthBar = GetComponentInChildren<TileObjectHealthBar>();
            healthBar.tileObject = this;
        }

        if (attachedTile != null)
        {
            FinalizeTileChoice(attachedTile);
            return;
        }
        else if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, tileLayer))
        {
            FinalizeTileChoice(hit.transform.GetComponent<Tile>());
            return;
        }
        else
        {
            Debug.Assert(attachedTile != null, $"{name} couldn't find a tile under it to attach onto");
        }
    }

    public virtual void TakeDamage(float attackDamage)
    {
        if(objectType == ObjectType.ElementalWall)
        {
            return;
        }

        currentHealth -= attackDamage;

        UpdateHealthBar?.Invoke();

        if (currentHealth <= 0)
        {
            objectDestroyed.Invoke(this);

            attachedTile.objectOnTile = null;
            attachedTile.tileHasObject = false;

            Destroy(gameObject);
        }
    }

    public void PreviewDamage(float damage)
    {
        if (objectType == ObjectType.ElementalWall)
        {
            return;
        }

        DamagePreview?.Invoke((int)damage);
    }

    public virtual void Undo(UndoData_TileObjCustomInfo data)
    {

    }

    public virtual bool CheckDestruction()
    {
        return false;
    }

    #region breadthFirstMethods

    //Used for attaching the spawner onto the tile under it
    public void FinalizeTileChoice(Tile tile)
    {
        transform.position = tile.transform.position;
        attachedTile = tile;

        tile.tileHasObject = true;
        tile.objectOnTile = this;
    }

    public void FindTile()
    {
        if (attachedTile != null)
        {
            FinalizeTileChoice(attachedTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, tileLayer))
        {
            FinalizeTileChoice(hit.transform.GetComponent<Tile>());
            return;
        }
    }

    #endregion
}
