using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAttackCombo
{
    public MoveAttackCombo(int comboVal, Tile moveTile, Tile attackTile, Vector3 rotation)
    {
        value = comboVal;
        movementTile = moveTile;
        attackingTile = attackTile;
        attackRotation = rotation;
    }

    public int value;
    public Tile movementTile;
    public Tile attackingTile;
    public Vector3 attackRotation;
}
