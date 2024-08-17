using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatedScreen : Menu
{
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    protected override void Start()
    {
        base.Start();
        replayButton.onClick.AddListener(OnReplay);
        continueButton.onClick.AddListener(OnReturnToMap);
    }

    private void OnReplay()
    {
        MenuManager.Instance.HideMenu(menuClassifier);
        GameManager.Instance.LoadCurrentLevel();

        /*SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
        GameManager.Instance.CleanActiveScene();*/
    }

    /*private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        SceneLoader.Instance.LoadScene(GameManager.Instance.gameLevels[GameManager.Instance.CurrentLevelIndex]);
    }*/

    /*private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.ShowMenu(MenuManager.Instance.HUDMenuClassifier);
    }*/

    public void OnReturnToMap()
    {
        AudioManager.Instance.PlayMusicFadeIn("MainTheme", 2);
        MenuManager.Instance.GetMenu<WorldMap>(MenuManager.Instance.WorldMapClassifier)?.OnReturnToMap();
        MenuManager.Instance.HideMenu(menuClassifier);

        GameManager.Instance.CleanActiveScene();
    }
}
