using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryReward : MonoBehaviour
{
    [SerializeField] private GameObject upgradeCard;
    [SerializeField] private GameObject skillCard;
    [SerializeField] private Transform upgradePanel;
    private List<ActiveSkillSO> rewardSkills = new List<ActiveSkillSO>();

    public void SpawnCards()
    {
        ResetCards();

        // Spawn 3 upgrade cards
        /*for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(upgradeCard, transform);
            card.GetComponent<UpgradeCard>().InitializeUI();
        }*/

        // Spawn 3 skill cards ---- TEMPORARY
        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(skillCard, upgradePanel);

            // Spawn a random element skill card
            ElementType elementType = (ElementType)Random.Range(0, 3);
            SkillRewardCard rewardCard = card.GetComponent<SkillRewardCard>();

            ActiveSkillSO skill = ActiveSkillCollection.Instance.GetRandomSkillReward(elementType);

            if (skill == null)
            {
                Debug.LogError($"No more {elementType} skills to reward");
                Destroy(card);
                return;
            }

            // If the skill is already in the reward, get another skill
            /*while (rewardSkills.Contains(skill))
            {
                skill = ActiveSkillCollection.Instance.GetRandomSkillReward(elementType);
            }*/

            rewardSkills.Add(skill);
            rewardCard.SetSkillDisplay(skill);
        }

        rewardSkills.Clear();
    }

    private void ResetCards()
    {
        foreach (Transform child in upgradePanel)
        {
            Destroy(child.gameObject);
        }
    }
}
