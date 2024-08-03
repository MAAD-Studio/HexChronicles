using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVFX : MonoBehaviour
{
    protected Character target;
    protected System.Action<Character> onHitCallback;

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

    protected IEnumerator HitCallback()
    {
        yield return new WaitForSeconds(callbackWaitTime);

        onHitCallback?.Invoke(target);
        Destroy(gameObject, destroyAfterCollide);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target.gameObject)
        {
            onHitCallback?.Invoke(target);
            Destroy(gameObject, destroyAfterCollide);
        }
    }
}
