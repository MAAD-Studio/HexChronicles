using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoData_Character
{
    #region Variables

    public float currentHealth;
    public bool isHurt;
    public bool effectedByWeather;
    public int movementThisTurn;
    public List<Status> statusList = new List<Status>();
    public Vector3 position;
    public Quaternion rotation;

    #endregion
}
