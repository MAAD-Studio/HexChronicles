using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using System;

/// <summary>
/// Save and load hero data to and from a JSON file
/// </summary>
public class HeroDataManager : Singleton<HeroDataManager> 
{
    //private string fileLocation = Application.persistentDataPath + "/SavedHeroData.json"; // this should be used for player's hero data
    private string fileLocation = "Assets/Game/Scripts/Hero/HeroData/HeroData.json";
    public List<HeroAttributesSO> heroSOs = new List<HeroAttributesSO>();
    [HideInInspector] public List<BasicAttributes> heroes = new List<BasicAttributes>();

    private void Start()
    {
        heroes.Clear();
        AddHeroSO();
    }

    public void AddHeroSO()
    {
        foreach (HeroAttributesSO heroSO in heroSOs)
        {
            heroes.Add(heroSO.attributes);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            WriteJSON();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadJSON();
        }
    }

    public void WriteJSON()
    {
        HeroList heroList = new HeroList(heroes);
        string jsonString = JsonUtility.ToJson(heroList);
        Debug.Log(jsonString);

        /*HeroSOList heroList = new HeroSOList(heroes);
        string jsonString = "{\"heroSOs\":[";
        foreach (HeroAttributes hero in heroList.heroSOs)
        {
            string heroData = JsonUtility.ToJson(hero);
            jsonString += heroData + ",";
        }
        jsonString = jsonString.TrimEnd(',') + "]}";
        Debug.Log(jsonString);*/

        File.WriteAllText(fileLocation, jsonString);
        Debug.Log("File saved to: " + fileLocation);
    }

    public void LoadJSON()
    {
        if (File.Exists(fileLocation))
        {
            // Read JSON data from file
            string jsonData = File.ReadAllText(fileLocation);

            // Deserialize JSON data
            HeroList dataContainer = JsonUtility.FromJson<HeroList>(jsonData);
            Debug.Log("Heroes loaded: " + dataContainer.heroes.Count);
            Debug.Log(jsonData);

            if (heroes.Count != dataContainer.heroes.Count)
            {
                Debug.LogError("Number of items in the existing list does not match the deserialized list: " + heroes.Count);
                return;
            }

            for (int i = 0; i < heroes.Count; i++)
            {
                BasicAttributes hero = heroes[i]; // existing SO
                BasicAttributes heroData = dataContainer.heroes[i]; // deserialized SO

                // Assign data from deserialized JSON to corresponding SO properties
                hero.name = heroData.name;
                hero.description = heroData.description;
                hero.avatar = heroData.avatar;
                hero.health = heroData.health;
                hero.movementRange = heroData.movementRange;
                hero.attackDamage = heroData.attackDamage;
                hero.attackRange = heroData.attackRange;
                hero.defensePercentage = heroData.defensePercentage;

                hero.elementType = heroData.elementType;

                Debug.Log("Loaded Hero: " + hero.name);
            }
        }
        else
        {
            Debug.LogError("JSON file not found: " + fileLocation);
        }
    }
}
