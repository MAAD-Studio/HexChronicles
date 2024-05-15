using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Startup : MonoBehaviour
{
    public SceneReference UIScene;
    public SceneReference ActiveScene;

    void Start()
    {
        Scene uiScene = SceneManager.GetSceneByPath(UIScene);
        if (uiScene.isLoaded == false)
        {
            StartCoroutine(BootUISequence());
            return;
        }

        Scene activeScene = SceneManager.GetSceneByPath(ActiveScene);
        if (activeScene.isLoaded == false)
        {
            StartCoroutine(BootActiveSequence());
            return;
        }

        StartCoroutine(IgnoreBootSequence());
    }

    IEnumerator IgnoreBootSequence()
    {
        yield return new WaitForSeconds(1);
        SceneLoadedCallback(null);
    }

    IEnumerator BootUISequence()
    {
        yield return new WaitForSeconds(0.5f);
        SceneLoader.Instance.LoadScene(UIScene, false, false);
        StartCoroutine(BootActiveSequence());
    }

    IEnumerator BootActiveSequence()
    {
        yield return new WaitForSeconds(0.5f);
        SceneLoader.Instance.OnSceneLoadedEvent += SceneLoadedCallback;
        SceneLoader.Instance.LoadScene(ActiveScene, false, false);
    }

    void SceneLoadedCallback(List<string> scenesLoaded)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= SceneLoadedCallback;
        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);

        // Set the Active Scene for dynamic game objects
        Scene activeScene = SceneManager.GetSceneByPath(ActiveScene);
        SceneLoader.Instance.SetActiveScene(activeScene);

#if UNITY_EDITOR
        if (SceneManager.sceneCount >= 4)
        {
            MenuManager.Instance.HideMenu(MenuManager.Instance.MainMenuClassifier);
            MenuManager.Instance.ShowMenu(MenuManager.Instance.HUDMenuClassifier);

            for (int i = 2; i < SceneManager.sceneCount; i++)
            {
                Scene _scene = SceneManager.GetSceneAt(i);
                Debug.Log(_scene.name);
            }
        }
        else
        {
            MenuManager.Instance.ShowMenu(MenuManager.Instance.MainMenuClassifier);
        }
#else
        MenuManager.Instance.ShowMenu(MainMenuClassifier);
#endif
    }
}
