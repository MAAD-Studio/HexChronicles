using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : ElementalWall
{
    #region Variables

    [Header("PoisonCloud: ")]
    [SerializeField] private int damage = 1;
    public int Damage
    {
        get { return damage; }
        private set { damage = value; }
    }

    #endregion
}
