using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrediction : MonoBehaviour
{
    [SerializeField] private GameObject predictionItemPrefab;

    public struct AttackPredictionData
    {
        public int damage;
        public int heal;
        public Status addStatus;
        public Status removeStatus;
    }

    public void SetData(AttackPredictionData data)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (data.damage > 0)
        {
            AttackPredictionItem item = Instantiate(predictionItemPrefab, transform).GetComponent<AttackPredictionItem>();
            item.SetData(Config.Instance.GetStatSprite("health"), "Health", "-" + data.damage.ToString());
        }
    }
}
