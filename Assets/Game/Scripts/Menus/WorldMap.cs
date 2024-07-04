using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WorldMap : Menu
{
    public SceneReference worldMap;
    public MenuClassifier hudClassifier;
    public MenuClassifier preGameClassifier;
    [SerializeField] private GameObject characterCollection;
    [SerializeField] public SceneReference[] levels;
    [SerializeField] private Button[] levelButtons;
    private int levelIndex;

    protected override void Start()
    {
        base.Start();

        CloseCharacterCollection();

        for (int i = 0; i < levels.Length; i++)
        {
            int index = i;
            levelButtons[i].onClick.AddListener(() => ShowPreGame(index));
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnReturnToMainMenu();
        }
    }

    public void ShowPreGame(int index)
    {
        levelIndex = index; // Set level index
        MenuManager.Instance.ShowMenu(preGameClassifier);
        MenuManager.Instance.HideMenu(menuClassifier);

        PreGameScreen preGameScreen = MenuManager.Instance.GetMenu<PreGameScreen>(preGameClassifier);
        preGameScreen.UpdatePreGameScreen(levelIndex);
    }

    public void LoadLevel()
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        MenuManager.Instance.HideMenu(menuClassifier);
        //MenuManager.Instance.ShowMenu(hudClassifier);

        SceneLoader.Instance.LoadScene(levels[levelIndex]);
        SceneLoader.Instance.UnloadScene(worldMap);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.ShowMenu(hudClassifier); // Show HUD after scene is loaded
    }

    public void OnReturnToMainMenu()
    {
        MenuManager.Instance.GetMenu<MainMenu>(MenuManager.Instance.MainMenuClassifier)?.OnReturnToMainMenu();
        MenuManager.Instance.HideMenu(menuClassifier);
    }

    public void OnReturnToMap()
    {
        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.HUDMenuClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
    }

    #region Load WorldMap Scene
    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        LoadWorldMap();
    }

    public void LoadWorldMap()
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnMapLoaded;

        SceneLoader.Instance.LoadScene(worldMap);
    }

    private void OnMapLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnMapLoaded;

        MenuManager.Instance.ShowMenu(menuClassifier);
        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
    }
    #endregion

    #region CharacterCollection
    public void OpenCharacterCollection()
    {
        characterCollection.SetActive(true);
    }

    public void CloseCharacterCollection()
    {
        characterCollection.SetActive(false);
    }
    #endregion
}
