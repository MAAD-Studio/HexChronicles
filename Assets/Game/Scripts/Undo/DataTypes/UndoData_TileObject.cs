using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoData_TileObject
{
    #region Variables

    public TileObject involvedObject;
    public Vector3 position;
    public ObjectType type;
    public float currentHealth;
    public bool destroy;

    #endregion
}
