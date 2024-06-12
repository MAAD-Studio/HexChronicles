using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class WeatherTurn : MonoBehaviour, StateInterface
{
    #region Variables

    private TurnManager turnManager;
    [SerializeField] protected WeatherManager weatherManager;

    private bool updateCalled = false;
    private bool updateDone = false;

    #endregion

    #region UnityMethods

    private void Start()
    {
        turnManager = GetComponent<TurnManager>();
        Debug.Assert(weatherManager != null, $"WeatherTurn wasn't provided the WeatherManager");
    }

    #endregion

    #region InterfaceMethods

    public void EnterState()
    {
        
    }

    public void UpdateState()
    {
        if (!updateCalled)
        {
            updateCalled = true;
            StartCoroutine(WeatherUpdate());
        }
        else if (updateDone)
        {
            turnManager.SwitchState(TurnEnums.TurnState.PlayerTurn);
        }
    }

    public void ExitState()
    {
        updateCalled = false;
        updateDone = false;
    }

    #endregion

    #region CustomMethods

    public IEnumerator WeatherUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        weatherManager.UpdateWeather();
        yield return new WaitForSeconds(0.5f);
        updateDone = true;
    }

    #endregion
}
