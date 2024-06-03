using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileObject : MonoBehaviour
{
    [Header("Setup Info:")]
    [SerializeField] protected LayerMask tileLayer;
    [SerializeField] protected TurnManager turnManager;
    [SerializeField] public TileObjectSO tileObjectData;
    [HideInInspector] public float currentHealth = 0f;
    [SerializeField] public TileObjectHealthBar healthBar;

    [HideInInspector] public static UnityEvent<TileObject> objectDestroyed = new UnityEvent<TileObject>();
    
    public event System.EventHandler OnDamagePreview;
    public event System.EventHandler OnUpdateHealthBar;

    public virtual void Start()
    {
        currentHealth =  tileObjectData.health;
        if (healthBar == null) 
        { 
            healthBar = GetComponentInChildren<TileObjectHealthBar>();
            healthBar.tileObject = this;
        }
    }

    public virtual void TakeDamage(float attackDamage)
    {
        int hitNum = Random.Range(0, 101);
        if (hitNum > tileObjectData.defense)
        {
            Debug.Log("TOOK DAMAGE");
            currentHealth -= attackDamage;
        }
        else
        {
            TemporaryMarker.GenerateMarker(tileObjectData.missText, gameObject.transform.position, 4f, 0.5f);
        }

        OnUpdateHealthBar?.Invoke(this, System.EventArgs.Empty);

        if (currentHealth <= 0)
        {
            objectDestroyed.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void PreviewDamage(float damage)
    {
        healthBar.damagePreview = damage;
        OnDamagePreview?.Invoke(this, System.EventArgs.Empty);
    }
}
