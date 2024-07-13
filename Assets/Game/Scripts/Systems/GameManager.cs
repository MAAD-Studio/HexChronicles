using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private List<SaveData> saveList = new List<SaveData>();
    private SaveData saveData;
    private SceneReference currentLevel;
    public SceneReference CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }

    private string SaveFilePath => Path.Combine(Application.dataPath, "SavedData.json");

    #region Unity Methods
    private void Start()
    {

    }
    #endregion


    #region Game Save and Load

    [Serializable]
    public struct SaveData
    {
        [SerializeField] private string saveTime;
        public DateTime SaveTime
        {
            get => DateTime.Parse(saveTime);
            set => saveTime = value.ToString();
        }
    }

    public void SaveGame()
    {
        saveData = new SaveData
        {
            SaveTime = DateTime.Now
        };

        saveList.Add(saveData);

        string json = JsonUtility.ToJson(saveList);
        File.WriteAllText(SaveFilePath, json);
    }

    public void LoadSavedData()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            saveList = JsonUtility.FromJson<List<SaveData>>(json);
        }
    }
    #endregion
}
