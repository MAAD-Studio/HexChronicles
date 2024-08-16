using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVFX : MonoBehaviour
{
    protected Character target;
    protected System.Action<Character> onHitCallback;

    protected TileObject objectTarget;
    protected System.Action<TileObject> onHitObjectCallback;

    [Header("Use Collision:")]
    [SerializeField] protected bool needCollision = true;
    [SerializeField] protected float destroyAfterCollide = 0.5f;

    [Header("Don't Use Collision:")]
    [Tooltip("Only used when needCollision is false")]
    [SerializeField] protected float callbackWaitTime = 0.2f;

    public void Initialize(Character target, System.Action<Character> onHitCallback)
    {
        this.target = target;
        this.onHitCallback = onHitCallback;

        if (!needCollision)
        {
            StartCoroutine(HitCallback());
        }
    }

    public void InitializeObjectTarget(TileObject objectTarget, System.Action<TileObject> onHitObjectCallback)
    {
        this.objectTarget = objectTarget;
        this.onHitObjectCallback = onHitObjectCallback;

        if (!needCollision)
        {
            StartCoroutine(HitObjectCallback());
        }
    }

    protected IEnumerator HitCallback()
    {
        yield return new WaitForSeconds(callbackWaitTime);

        onHitCallback?.Invoke(target);
        Destroy(gameObject, destroyAfterCollide);
    }

    protected IEnumerator HitObjectCallback()
    {
        yield return new WaitForSeconds(callbackWaitTime);

        onHitObjectCallback?.Invoke(objectTarget);
        Destroy(gameObject, destroyAfterCollide);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (target != null && other.gameObject == target.gameObject)
        {
            onHitCallback?.Invoke(target);
            Destroy(gameObject, destroyAfterCollide);
        }
        else if (objectTarget != null && other.gameObject == objectTarget.gameObject)
        {
            onHitObjectCallback?.Invoke(objectTarget);
            Destroy(gameObject, destroyAfterCollide);
        }
    }
}
