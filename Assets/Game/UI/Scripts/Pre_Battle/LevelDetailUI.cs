using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelDetailUI : MonoBehaviour
{
    [Header("Level Basic")]
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private Image levelImage;
    [SerializeField] private Image scaledImage;
    [SerializeField] private TextMeshProUGUI levelDescription;

    [Header("Enemies")]
    [SerializeField] private GameObject enemiesPanel;
    private List<Image> enemies = new List<Image>();

    [Header("Level Objective")]
    [SerializeField] private TextMeshProUGUI primaryObjective;
    [SerializeField, Tooltip("additional info on hover")] 
    private TextMeshProUGUI primaryObjectiveDescription;

    [Header("Level Rewards")]
    [SerializeField] private GameObject rewardPanel;
    //private List<Image> activeSkills;
    //private List<Image> upgrades;

    public void InitializeInfo(LevelSO level)
    {
        levelName.text = level.levelName;
        levelImage.sprite = level.levelImage;
        scaledImage.sprite = level.levelImage;
        levelDescription.text = level.levelDescription;
        primaryObjective.text = level.primaryObjective;
        primaryObjectiveDescription.text = level.primaryObjectiveDescription;
        CreateEnemyImages(level);

        // TODO: Implement rewards

    }

    private void CreateEnemyImages(LevelSO level)
    {
        foreach (Transform child in enemiesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        enemies.Clear();

        for (int i = 0; i < level.enemies.Count; i++)
        {
            // Create new enemy image
            Image enemyImage = new GameObject("Enemy" + i).AddComponent<Image>();
            enemyImage.transform.SetParent(enemiesPanel.transform);
            enemyImage.rectTransform.localScale = Vector3.one;
            enemyImage.rectTransform.sizeDelta = new Vector2(64, 64);
            enemies.Add(enemyImage);
            enemies[i].sprite = level.enemies[i].attributes.avatar;
        }
    }
}
