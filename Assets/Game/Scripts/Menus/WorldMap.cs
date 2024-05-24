using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
        SceneLoader.Instance.LoadScene(Level);
        SceneLoader.Instance.UnloadScene(worldMap);

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
        //SceneLoader.Instance.LoadScene(worldMap);
    }

    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;

        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.ShowMenu(menuClassifier);
    }
}
