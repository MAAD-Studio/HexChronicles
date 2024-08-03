using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;
    public Vector3 minScale = Vector3.one * 0.1f; 
    public Vector3 maxScale = Vector3.one;
    public float factor = 0.02f;

    private void Start()
    {
         mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;

        // Scale based on distance from camera
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        float scale = distance * factor;
        transform.localScale = Vector3.Lerp(minScale, maxScale, scale);
    }
}
