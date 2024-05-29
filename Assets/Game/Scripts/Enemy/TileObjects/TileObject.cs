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
        int hitNum = Random.Range(0, 101);
        if (hitNum > tileObjectData.defense)
        {
            tileObjectData.health -= attackDamage;
        }
        else
        {
            TemporaryMarker.GenerateMarker(tileObjectData.missText, gameObject.transform.position, 4f, 0.5f);
            Debug.Log("HIT MISSED");
        }

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
