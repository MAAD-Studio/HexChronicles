using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileObject : MonoBehaviour
{
    [Header("Setup Info:")]
    [SerializeField] protected LayerMask tileLayer;
    [SerializeField] protected TurnManager turnManager;
    [SerializeField] protected TileObjectSO tileObjectData;

    [HideInInspector] public static UnityEvent<TileObject> objectDestroyed = new UnityEvent<TileObject>();

    public virtual void TakeDamage(float attackDamage)
    {
        tileObjectData.health -= attackDamage;

        if(tileObjectData.health < 0)
        {
            objectDestroyed.Invoke(this);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("MY HEALTH IS: " + tileObjectData.health);
        }
    }
}
