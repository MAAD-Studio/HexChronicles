using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private SaveLoadManager saveLoadManager;
    public SaveLoadManager SaveLoadManager => saveLoadManager;

    [Header("Level Info")]
    public LevelSO[] levelDetails;

    private SceneReference currentLevel;
    public SceneReference CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }
    public int CurrentLevelIndex { get; set; }

    [Header("Settings")]
    private bool isFast = false;
    public bool IsFast
    {
        get { return isFast; }
    }
    private float gameSpeed = 1f;
    public float GameSpeed
    {
        get { return gameSpeed; }
    }

    private bool isTutorial = false;
    public bool IsTutorial
    {
        get { return isTutorial; }
        set { isTutorial = value; }
    }

    #region Unity Methods
    private void Start()
    {
        CurrentLevelIndex = 0;
        saveLoadManager = GetComponent<SaveLoadManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            saveLoadManager.SaveGame();
        }
    }
    #endregion

    #region CustomMethods

    public void IncreaseGameSpeed()
    {
        gameSpeed = 2f;
        isFast = true;
    }

    public void DecreaseGameSpeed()
    {
        gameSpeed = 1f;
        isFast = false;
    }
    #endregion


    #region Level Loading
    public void LoadCurrentLevel()
    {
        SceneLoader.Instance.IsLoadingBattle = true;
        MenuManager.Instance.ShowMenu(MenuManager.Instance.LoadingScreenClassifier);

        SceneLoader.Instance.OnScenesUnLoadedEvent += AllScenesUnloaded;
        SceneLoader.Instance.UnLoadAllLoadedScenes();
        CleanActiveScene();
    }

    private void AllScenesUnloaded()
    {
        SceneLoader.Instance.OnScenesUnLoadedEvent -= AllScenesUnloaded;
        SceneLoader.Instance.OnSceneLoadedEvent += OnSceneLoaded;

        // Load CurrentLevel
        SceneReference currentScene = levelDetails[CurrentLevelIndex].levelScene;
        SceneLoader.Instance.LoadBattleScene(currentScene);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.ShowMenu(MenuManager.Instance.HUDMenuClassifier);
        AudioManager.Instance.PlayMusicFadeIn("BattleMusic", 2);
    }

    public void CleanActiveScene()
    {
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Destroy(go);
        }
    }
    #endregion
}
