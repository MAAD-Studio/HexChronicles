using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowEffect : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float destroyAfterCollide = 0.5f;
    [SerializeField] private GameObject trail;

    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            speed = speed / 2;
            trail.SetActive(false);
            Destroy(gameObject, destroyAfterCollide);
        }
    }
}
