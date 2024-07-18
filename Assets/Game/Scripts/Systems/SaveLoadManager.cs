using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private List<SaveData> saveList = new List<SaveData>();
    public List<SaveData> SaveList
    {
        get => saveList;
    }
    private string SaveFilePath => Path.Combine(Application.dataPath, "SavedData.json");

    private void Start()
    {
        saveList.Clear();
        LoadSavedData();
    }

    #region Serializables
    [Serializable]
    public struct SaveData
    {
        public int CurrentLevel;

        public List<ActiveSkillSO> playerSkills;
        public List<ActiveSkillSO> PlayerSkills
        {
            get => playerSkills; 
            set => playerSkills = value; 
        }

        public string saveTime;
        public DateTime SaveTime
        {
            get => DateTime.Parse(saveTime);
            set => saveTime = value.ToString();
        }
    }

    [Serializable]
    public struct SaveDataList
    {
        public List<SaveData> DataList;
    }
    #endregion

    
    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            CurrentLevel = GameManager.Instance.CurrentLevelIndex,
            PlayerSkills = ActiveSkillCollection.Instance.PlayerSkills(),
            SaveTime = DateTime.Now
        };
        saveList.Add(saveData);

        SaveDataList data = new SaveDataList { DataList = saveList };
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

    public void ApplySavedData(SaveData data)
    {
        GameManager.Instance.CurrentLevelIndex = data.CurrentLevel;
        ActiveSkillCollection.Instance.LoadPlayerSkills(data.PlayerSkills);
    }
}
