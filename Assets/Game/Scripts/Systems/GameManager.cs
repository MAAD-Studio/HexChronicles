using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public LevelSO[] levelDetails;

    private SceneReference currentLevel;

    [HideInInspector] private float gameSpeed = 1f;

    public float GameSpeed
    {
        get { return gameSpeed; }
    }

    public SceneReference CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }
    public int CurrentLevelIndex { get; set; }

    private List<SaveData> saveList = new List<SaveData>();
    private SaveData saveData;
    public List<SaveData> SaveList
    {
        get => saveList;
    }
    private string SaveFilePath => Path.Combine(Application.dataPath, "SavedData.json");


    #region Unity Methods
    private void Start()
    {
        CurrentLevelIndex = 0;
        LoadSavedData();
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


    #region Game Save and Load

    [Serializable]
    public struct SaveData
    {
        private int currentLevel;
        private string saveTime;
        public int CurrentLevel
        {
            get => currentLevel;
            set => currentLevel = value;
        }
        public DateTime SaveTime
        {
            get => DateTime.Parse(saveTime);
            set => saveTime = value.ToString();
        }
    }

    [Serializable]
    private struct SaveDataList
    {
        public List<SaveData> DataList;
    }

    public void SaveGame()
    {
        saveData = new SaveData
        {
            CurrentLevel = CurrentLevelIndex,
            SaveTime = DateTime.Now
        };
        saveList.Add(saveData);

        var data = new SaveDataList { DataList = saveList };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SaveFilePath, json);
    }

    public void LoadSavedData()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            saveList = JsonUtility.FromJson<SaveDataList>(json).DataList;
        }
    }
    #endregion
}
