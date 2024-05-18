using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBoot : MonoBehaviour
{
    public GameObject MenuRoot;

    private void Awake()
    {
        gameObject.SetActive(true);
        MenuRoot.SetActive(true);
    }
}
