using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    // Goals,
    //      Active scene is the scene for runtime objects
    //      Need to load from Startup but not track scenes (easy to add a boot loader)
    //      Track scenes loaded after (enables streaming)
    //      When quit you should be able to purge all go in active and unload all other scenes.
    //      This should support Streaming or multi scene loading (both the same)

    public Scene activeScene;
    
    public System.Action<List<string>> OnSceneLoadedEvent;
    public System.Action OnScenesUnLoadedEvent;

    public float delayTime = 1.0f;

    private List<string> loadedScenes = new List<string>();

    // When loading just add a flag for persistence. If true don't add to the loadedScenes
    // Only remove the scenes when you unload

    public void SetActiveScene(Scene _scene)
    {
        activeScene = _scene;
        SceneManager.SetActiveScene(_scene);
    }

    public void LoadScene(string scene, bool showLoadingScreen = true, bool cacheScene = true)
    {
        StartCoroutine(loadScene(scene, showLoadingScreen, true, cacheScene));
    }

    public void LoadScenes(List<string> scenes, bool showLoadingScreen = true, bool cacheScene = true)
    {
        StartCoroutine(loadScenes(scenes, showLoadingScreen, cacheScene));
    }

    IEnumerator loadScenes(List<string> scenes, bool showLoadingScreen, bool cacheScene)
    {
        if (showLoadingScreen)
        {
            MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        }

        foreach (string scene in scenes)
        {
            yield return StartCoroutine(loadScene(scene, false, false, cacheScene));
        }

        if (showLoadingScreen)
        {
            MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        }

        if (OnSceneLoadedEvent != null)
        {
            OnSceneLoadedEvent(loadedScenes);
        }
    }

    IEnumerator loadScene(string scene, bool showLoadingScreen, bool raiseEvent, bool cacheScene)
    {
        if (SceneManager.GetSceneByPath(scene).isLoaded == false)
        {
            if (showLoadingScreen)
            {
                MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
            }

            yield return new WaitForSeconds(delayTime);

            AsyncOperation sync;

            Application.backgroundLoadingPriority = ThreadPriority.Low;

            sync = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            while (sync.isDone == false) { yield return null; }

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            yield return new WaitForSeconds(delayTime);

            if (showLoadingScreen)
            {
                MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
            }
        }

        if (cacheScene)
        {
            loadedScenes.Add(scene);
        }
        if (raiseEvent)
        {
            if (OnSceneLoadedEvent != null)
            {
               OnSceneLoadedEvent(loadedScenes);
            }
        }
    }

    // 4 Methods:
    //	- Unload single scene
    //	- Unload list of scenes
    //		- Support to unload multiple (Coroutine)
    //	- Actual Unload of scenes.

    public void UnLoadAllLoadedScenes()
    {
        StartCoroutine(unLoadAllLoadedScenes());
    }

    IEnumerator unLoadAllLoadedScenes()
    {
        for(int i = 0; i < loadedScenes.Count; i++)
        {
            string sceneName = loadedScenes[i];
            loadedScenes.RemoveAt(0);
            yield return StartCoroutine(unloadScene(sceneName));
        }

        if (OnScenesUnLoadedEvent != null)
        {
            OnScenesUnLoadedEvent();
        }
    }

    public void UnloadScene(string scene)
    {
        StartCoroutine(unloadScene(scene));
    }

    public void UnloadScenes(List<string> scenes)
    {
        StartCoroutine(unloadScenes(scenes));
    }

    IEnumerator unloadScenes(List<string> scenes)
    {
        foreach (string scene in scenes)
        {
            yield return StartCoroutine(unloadScene(scene));
        }
    }

    IEnumerator unloadScene(string scene)
    {
        AsyncOperation sync = null;

        try
        {
            sync = SceneManager.UnloadSceneAsync(scene);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        if (sync != null)
        {
            while (sync.isDone == false) { yield return null; }
        }

        sync = Resources.UnloadUnusedAssets();
        while (sync.isDone == false) { yield return null; }

        loadedScenes.Remove(scene);
    }
}
