using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEffect : AttackVFX
{
    [Header("Arrow Effect:")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject trail;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Enemy"))
        {
            speed = speed / 2;
            trail.SetActive(false);
        }
    }
}
