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
    [SerializeField] private Button[] levelButtonsInOrder;
    private SceneReference[] levels;
    private int levelIndex;
    private int unlockedLevelIndex;

    protected override void Start()
    {
        base.Start();

        CloseCharacterCollection();

        // Get the levels and subscribe the buttons
        int levelCount = GameManager.Instance.levelDetails.Length;
        levels = new SceneReference[levelCount];
        Debug.Assert(levelButtonsInOrder.Length == levelCount, "Level buttons count does not match the level count");

        for (int i = 0; i < levelCount; i++)
        {
            int index = i; // Prevent referencing the last value of i
            levels[i] = GameManager.Instance.levelDetails[i].levelScene;
            levelButtonsInOrder[i].onClick.AddListener(() => ShowPreGame(index));
        }

        RefreshLevelButtons();
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
        MenuManager.Instance.HideMenu(MenuManager.Instance.TutorialHUDClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
    }

    public void RefreshLevelButtons()
    {
        unlockedLevelIndex = GameManager.Instance.CurrentLevelIndex;
        for (int i = 0; i < levelButtonsInOrder.Length; i++)
        {
            if (i > unlockedLevelIndex)
            {
                levelButtonsInOrder[i].interactable = false;
            }
            else
            {
                levelButtonsInOrder[i].interactable = true;
            }
        }
    }

    public void UnlockAll()
    {
        for (int i = 0; i < levelButtonsInOrder.Length; i++)
        {
            levelButtonsInOrder[i].interactable = true;
        }
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

        // Enable the current level button
        unlockedLevelIndex = GameManager.Instance.CurrentLevelIndex;
        levelButtonsInOrder[unlockedLevelIndex].interactable = true;
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
