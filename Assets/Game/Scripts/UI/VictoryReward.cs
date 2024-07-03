using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class VictoryReward : MonoBehaviour
{
    [SerializeField] private GameObject upgradeCard;
    [SerializeField] private GameObject skillCard;
    [SerializeField] private Transform upgradePanel;

    public void SpawnCards()
    {
        ResetCards();

        // Spawn 3 upgrade cards
        /*for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(upgradeCard, transform);
            card.GetComponent<UpgradeCard>().InitializeUI();
        }*/

        // Spawn 3 skill cards
        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(skillCard, upgradePanel);

            // Select a random element skill
            ElementType elementType = (ElementType)Random.Range(0, 3);
            card.GetComponent<SkillRewardCard>().SetSkillDisplay(ActiveSkillCollection.Instance.GetRandomSkill(elementType));
        }
    }

    private void ResetCards()
    {
        foreach (Transform child in upgradePanel)
        {
            Destroy(child.gameObject);
        }
    }
}
