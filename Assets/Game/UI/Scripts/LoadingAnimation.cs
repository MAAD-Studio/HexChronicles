using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] private GameObject loadingAnimationParent;
    [SerializeField] private GameObject[] loadingAnimations;

    private void OnEnable()
    {
        loadingAnimationParent.SetActive(true);

        // Randomly select a loading animation
        int randomIndex = Random.Range(0, loadingAnimations.Length);
        for (int i = 0; i < loadingAnimations.Length; i++)
        {
            loadingAnimations[i].SetActive(i == randomIndex);
        }
    }

    private void OnDisable()
    {
        loadingAnimationParent.SetActive(false);
    }
}
