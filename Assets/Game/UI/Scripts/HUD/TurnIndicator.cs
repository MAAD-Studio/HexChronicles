using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnIndicator : MonoBehaviour
{
    [SerializeField] private GameObject turnInfoPrefab;
    private List<TurnInfo> turnInfos = new List<TurnInfo>();
    private int currentTurn = 1;

    public void Initialize()
    {
        int limitTurns = GameManager.Instance.levelDetails[GameManager.Instance.CurrentLevelIndex].limitTurns;
        for (int i = 1; i <= limitTurns; i++)
        {
            GameObject gameObject = Instantiate(turnInfoPrefab, transform);
            TurnInfo turnInfo = gameObject.GetComponent<TurnInfo>();
            turnInfo.Initialize(i, limitTurns);
            turnInfos.Add(turnInfo);
        }
    }

    public void SetWeatherTurn(WeatherType weatherType, int turnsLast)
    {
        for (int i = currentTurn; i < currentTurn + turnsLast + 1; i++)
        {
            turnInfos[i].SetWeatherTurn(weatherType);
        }
    }
    
    public void SetCurrentTurn(int turn)
    {
        currentTurn = turn;

        turnInfos[turn - 1].CurrentTurn();

        if (turn > 1)
        {
            turnInfos[turn - 2].EndTurn();
        }
    }

    public void ResetTurn()
    {
        turnInfos.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
