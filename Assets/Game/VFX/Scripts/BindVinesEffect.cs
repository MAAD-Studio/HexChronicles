using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindVinesEffect : StatusVFX
{
    private Material mat;
    private float grow = 0.0f;
    [SerializeField] private float speed = 0.5f;

    void Start()
    {
        mat = GetComponentInChildren<Renderer>().material;
        grow = mat.GetFloat("_Grow");
        grow = 0.0f;

        StartCoroutine(GrowVines());
    }

    private IEnumerator GrowVines()
    {
        while (grow < 1.0f)
        {
            grow += Time.deltaTime * speed;
            mat.SetFloat("_Grow", grow);
            yield return null;
        }
    }

    protected override void Remove()
    {
        StartCoroutine(RemoveVines());
    }

    private IEnumerator RemoveVines()
    {
        while (grow > 0.0f)
        {
            grow -= Time.deltaTime * speed * 2; // Remove faster than grow
            mat.SetFloat("_Grow", grow);
            yield return null;
        }
        base.Remove();
    }
}
