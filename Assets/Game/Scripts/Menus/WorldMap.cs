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

    protected override void Start()
    {
        base.Start();

        CloseCharacterCollection();

        for (int i = 0; i < levels.Length; i++)
        {
            int index = i;
            //levelButtons[i].onClick.AddListener(() => OnLoadLevel(levels[index]));
            levelButtons[i].onClick.AddListener(() => ShowPreGame()); // testing
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnReturnToMainMenu();
        }
    }

    // Only For Testing
    public void ShowPreGame()
    {
        MenuManager.Instance.ShowMenu(preGameClassifier);
    }

    public void OnLoadLevel(SceneReference Level)
    {
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        SceneLoader.Instance.LoadScene(Level);
        SceneLoader.Instance.UnloadScene(worldMap);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.HideMenu(menuClassifier);
        MenuManager.Instance.HideMenu(preGameClassifier);
        MenuManager.Instance.ShowMenu(hudClassifier);
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

    public void OpenCharacterCollection()
    {
        characterCollection.SetActive(true);
    }

    public void CloseCharacterCollection()
    {
        characterCollection.SetActive(false);
    }

    //test
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

    //test
    /*public void UnloadWorldMap()
    {
        SceneLoader.Instance.UnloadScene(worldMap);
    }*/

    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        LoadWorldMap();
        //SceneLoader.Instance.LoadScene(worldMap); // Not working
    }
}
