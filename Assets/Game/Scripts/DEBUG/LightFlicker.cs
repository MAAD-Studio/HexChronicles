using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light lightSource;

    [Header("Light Flicker Controls:")]
    [Min(0f)]
    [SerializeField] private float minimumIntensity = 5f;

    [Min(0f)]
    [SerializeField] private float maximumIntensity = 25f;

    [Range(0.01f, 5f)]
    [SerializeField] private float timeBetweenChange = 1f;
    private float currentTime = 0;

    void Start()
    {
        if(maximumIntensity < minimumIntensity)
        {
            maximumIntensity = minimumIntensity;
        }

        lightSource = GetComponent<Light>();
        Debug.Assert(lightSource != null, "LightFlicker could not find a Light component");
    }

    void Update()
    {
        if(currentTime < timeBetweenChange)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            float intensity = Random.Range(minimumIntensity, maximumIntensity);
            lightSource.intensity = intensity;
            currentTime = 0;
        }
    }
}
