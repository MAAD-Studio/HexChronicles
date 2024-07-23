using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveSkill
{
    #region Variables

    private SkillEffect skillEffect;
    private int skillEffectValue;

    private Status status = new Status();
    //public ReleaseTypes releaseTypes; // Not used yet

    private List<Character> targets;
    [HideInInspector] public Character thisCharacter;

    [HideInInspector] public List<Tile> affectedTiles = new List<Tile>();

    #endregion

    public void Initialize(ActiveSkillSO newSkill)
    {
        skillEffect = newSkill.skillEffect;
        skillEffectValue = newSkill.skillEffectValue;
        status = newSkill.status;
    }

    #region Enum

    [Flags]
    public enum SkillEffect
    {
        Damage = 1 << 0,     
        Heal = 1 << 1,     
        AddStatus = 1 << 2, 
        ReduceCD = 1 << 3,
        Push = 1 << 4,
        ChangeTileType = 1 << 5,
        Shield = 1 <<6,
    }

    public enum ReleaseTypes
    {
        Single,
        OnTileType,
        Area, 
        Shape,
        All
    }

    #endregion Enum

    #region Use Skill
    public void Release(List<Character> targets)
    {
        if ((skillEffect & SkillEffect.Damage) != 0) // Has Damage effect
        {
            foreach (var target in targets)
            {
                target.TakeDamage(skillEffectValue, thisCharacter.elementType);
            }
        }

        if ((skillEffect & SkillEffect.Heal) != 0) 
        {
            foreach (var target in targets)
            {
                Debug.Log("Target Healed: " + target.name);
                target.Heal(skillEffectValue);
            }
        }

        if ((skillEffect & SkillEffect.AddStatus) != 0 && status.statusType != Status.StatusTypes.None)
        {
            foreach (var target in targets)
            {
                target.AddStatus(status);
            }
        }

        if ((skillEffect & SkillEffect.ReduceCD) != 0)
        {
            foreach (var target in targets)
            {
                Hero heroTarget = (Hero)target;
                heroTarget.CurrentSkillCD -= skillEffectValue;
            }
        }

        if ((skillEffect & SkillEffect.Push) != 0)
        {
            foreach (var target in targets)
            {
                int distance = skillEffectValue;
                Vector3 direction = target.transform.position - thisCharacter.transform.position;
                target.PushedBack(direction, distance);
            }
        }

        if ((skillEffect & SkillEffect.Shield) != 0)
        {
            if(Status.GrabIfStatusActive(thisCharacter, Status.StatusTypes.Shield) == null)
            {
                Status shieldStatus = new Status();
                shieldStatus.effectTurns = skillEffectValue;
                shieldStatus.statusType = Status.StatusTypes.Shield;
                thisCharacter.AddStatus(shieldStatus);
            }
        }

        if ((skillEffect & SkillEffect.ChangeTileType) != 0)
        {
            // TODO
        }
    }

    public void GetTargets()
    {
        /*switch (releaseTypes)
        {
            case ReleaseTypes.Shape:
                targets = shapeArea.CharactersHit(TurnEnums.CharacterType.Enemy);
                Debug.Log("Targets: " + targets.Count);
                break;

            case ReleaseTypes.OnTileType:
                //targets = GetTargetsOnTileType();
                break;

            case ReleaseTypes.Area:
                //targets = GetTargetsInRange();
                break;
        }*/
    }

    public void GetTargetsOnTileType(TileEnums.TileType tileType)
    {
        /*foreach (Character character in field)
        {
            if (character.tileType == tileType)
            {
                if ((isFriendly && character.isFriendly) || (!isFriendly && !character.isFriendly))
                {
                    targets.Add(character);
                }
            }
        }*/

    }
    #endregion Use Skill

    //Resets the Tiles affected by the SkillTileReporters. Important for when the ActiveSkill gets Destroyed
    public void ResetTiles()
    {
        foreach(Tile tile in affectedTiles)
        {
            tile.ChangeTileColor(TileEnums.TileMaterial.baseMaterial);
            tile.inFrontier = false;
        }
    }

    /*//PlayerTurn needs this method to Instantiate ActiveSkills for it since it isn't a MonoBehavior
    public static GameObject SpawnSelf(ActiveSkill activeSkill)
    {
        return Instantiate(activeSkill.gameObject, new Vector3(100, 100, 100), Quaternion.identity);
    }

    //PlayerTurn needs this method to Destroy ActiveSkills for it since it isn't a MonoBehavior
    public void DestroySelf()
    {
        ResetTiles();
        Destroy(this.gameObject);
    }*/
}
