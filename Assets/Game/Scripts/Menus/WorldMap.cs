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
    [SerializeField] private SceneReference[] levels;
    [SerializeField] private Button[] levelButtons;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < levels.Length; i++)
        {
            int index = i;
            levelButtons[i].onClick.AddListener(() => OnLoadLevel(levels[index]));
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnReturnToMainMenu();
        }
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

    //test
    public void LoadWorldMap()
    {
        SceneLoader.Instance.LoadScene(worldMap);
    }

    //test
    public void UnloadWorldMap()
    {
        SceneLoader.Instance.UnloadScene(worldMap);
    }

    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        SceneLoader.Instance.LoadScene(worldMap); // Not working

        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.ShowMenu(menuClassifier);
    }
}
