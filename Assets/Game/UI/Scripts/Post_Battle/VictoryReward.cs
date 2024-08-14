using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryReward : MonoBehaviour
{
    [SerializeField] private GameObject upgradeCard;
    [SerializeField] private GameObject skillCard;
    [SerializeField] private Transform upgradePanel;
    private List<ActiveSkillSO> rewardSkills = new List<ActiveSkillSO>();
    private List<SkillRewardCard> rewardCards = new List<SkillRewardCard>();
    [SerializeField] private ContinueConfirmButton continueConfirm;

    private ActiveSkillSO selectedSkill;
    public ActiveSkillSO SelectedSkill => selectedSkill;

    public void SpawnCards()
    {
        continueConfirm.Reset();
        ResetCards();

        // Spawn 3 upgrade cards
        /*for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(upgradeCard, transform);
            card.GetComponent<UpgradeCard>().InitializeUI();
        }*/

        // Spawn 3 random skill cards
        for (int i = 0; i < 3; i++)
        {
            // Select a random element skill
            ElementType elementType = (ElementType)Random.Range(0, 3);
            ActiveSkillSO skill = ActiveSkillCollection.Instance.GetRandomSkillReward(elementType);

            if (skill == null)
            {
                Debug.Log($"No more {elementType} skills to reward");

                // Change to the next elementType (between 0, 1, 2)
                elementType = (ElementType)(((int)elementType + 1) % 3);
                skill = ActiveSkillCollection.Instance.GetRandomSkillReward(elementType);

                if (skill == null)
                {
                    Debug.Log($"No more {elementType} skills to reward");

                    // Change to the next elementType
                    elementType = (ElementType)(((int)elementType + 1) % 3);
                    skill = ActiveSkillCollection.Instance.GetRandomSkillReward(elementType);

                    if (skill == null)
                    {
                        Debug.LogError($"No more skills left!!!");
                        return;
                    }
                }
            }

            // If the skill is already in the reward, get another skill
            /*while (rewardSkills.Contains(skill) && ActiveSkillCollection.Instance.RemainSkillsCount > 2)
            {
                skill = ActiveSkillCollection.Instance.GetRandomSkillReward(elementType);
                Debug.Log("Duplicated skill, getting another one");
            }*/

            // Spawn card
            GameObject card = Instantiate(skillCard, upgradePanel);
            SkillRewardCard rewardCard = card.GetComponent<SkillRewardCard>();
            rewardCard.SetSkillDisplay(skill, this);

            rewardSkills.Add(skill);
            rewardCards.Add(rewardCard);
        }

        rewardSkills.Clear();
    }

    public void OnSkillSelected(ActiveSkillSO skill, SkillRewardCard rewardCard)
    {
        selectedSkill = skill;

        foreach (SkillRewardCard card in rewardCards)
        {
            if (card != rewardCard)
            {
                card.OnUnselected();
            }
        }

        if (continueConfirm.IsConfirmPanelActive)
        {
            continueConfirm.HideConfirmPanel();
        }
    }

    private void ResetCards()
    {
        rewardCards.Clear();
        selectedSkill = null;

        foreach (Transform child in upgradePanel)
        {
            Destroy(child.gameObject);
        }
    }
}
