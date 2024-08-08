using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] private GameObject loadingAnimationParent;
    [SerializeField] private Animator[] loadingAnimators;
    private bool firstCheck = true;

    private void OnEnable()
    {
        loadingAnimationParent.SetActive(true);

        // Randomly select a loading animation
        int randomIndex = Random.Range(0, loadingAnimators.Length);
        for (int i = 0; i < loadingAnimators.Length; i++)
        {
            if (i == randomIndex)
            {
                loadingAnimators[i].gameObject.SetActive(true);
                if (!firstCheck)
                {
                    if (SceneLoader.Instance.IsLoadingBattle)
                    {
                        loadingAnimators[i].SetTrigger("Battle");
                    }
                }
                continue;
            }
            loadingAnimators[i].gameObject.SetActive(false);
        }
        firstCheck = false;
    }

    private void OnDisable()
    {
        loadingAnimationParent.SetActive(false);
    }
}
