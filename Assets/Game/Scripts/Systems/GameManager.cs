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
    [HideInInspector] private float gameSpeed = 1f;

    public float GameSpeed
    {
        get { return gameSpeed; }
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
    }

    public void DecreaseGameSpeed()
    {
        gameSpeed = 1f;
    }
    #endregion


    #region Level Loading
    public void LoadCurrentLevel()
    {
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
        SceneLoader.Instance.LoadScene(currentScene);
    }

    private void OnSceneLoaded(List<string> list)
    {
        SceneLoader.Instance.OnSceneLoadedEvent -= OnSceneLoaded;

        MenuManager.Instance.HideMenu(MenuManager.Instance.LoadingScreenClassifier);
        MenuManager.Instance.ShowMenu(MenuManager.Instance.HUDMenuClassifier);
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
