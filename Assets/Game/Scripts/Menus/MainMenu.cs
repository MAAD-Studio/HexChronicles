using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    public SceneReference Tutorial;
    public SceneReference Map;
    public MenuClassifier tutorialHUDClassifier;
    public MenuClassifier mapClassifier;

    public void OnStartTutorial()
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        SceneLoader.Instance.LoadScene(Tutorial);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.ShowMenu(tutorialHUDClassifier);
    }

    public void OnContinueGame()
    {
        SceneLoader.Instance.LoadScene(Map);

        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.ShowMenu(mapClassifier);
    }

    public void OnReturnToMainMenu()
    {
        Time.timeScale = 1.0f;

        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.HUDMenuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.TutorialHUDClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
    }
    
    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        MenuManager.Instance.ShowMenu(MenuManager.Instance.MainMenuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
