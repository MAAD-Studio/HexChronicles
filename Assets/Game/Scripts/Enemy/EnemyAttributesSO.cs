using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Attributes/Enemy Attributes")]
public class EnemyAttributesSO : ScriptableObject
{
    public BasicAttributes attributes = new BasicAttributes();
    public AttackArea attackArea;

}
