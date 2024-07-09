using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEffect : MonoBehaviour
{
    [SerializeField] private float destroyAfterCollide = 0.5f;

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject, destroyAfterCollide);
    }
}
