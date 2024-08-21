using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    public SceneReference MainMenuScene;
    public SceneReference Map;
    public MenuClassifier tutorialHUDClassifier;
    public MenuClassifier mapClassifier;

    private SelectTutorial selectTutorial;
    [SerializeField] private CreditsScreen credits;
    [SerializeField] private OptionsMenuUI options;

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);

        selectTutorial = GetComponent<SelectTutorial>();
        selectTutorial.HidePanel();
        credits.Initialize();
        options.Initialize();
    }

    public void OnSelectTutorial()
    {
        selectTutorial.ShowPanel();
    }

    public void OnSelectCredits()
    {
        credits.ShowCredits();
    }

    public void OnSelectOptions()
    {
        options.ShowOptionsMenu();
    }

    public void OnContinueGame()
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        MenuManager.Instance.HideMenu(menuClassifier);
        SceneLoader.Instance.LoadNormalScene(Map);
        SceneLoader.Instance.UnloadScene(MainMenuScene);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.ShowMenu(mapClassifier);
    }

    public void OnReturnToMainMenu()
    {
        Time.timeScale = 1.0f;

        SceneLoader.Instance.IsLoadingBattle = false;
        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.HUDMenuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.TutorialHUDClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();

    }
    
    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;
        SceneLoader.Instance.OnSceneLoadedEvent += MainMenuLoaded;

        SceneLoader.Instance.LoadNormalScene(MainMenuScene);
    }

    private void MainMenuLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= MainMenuLoaded;

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
