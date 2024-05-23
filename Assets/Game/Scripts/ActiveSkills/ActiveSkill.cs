using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveSkill
{
    #region Variables

    public string skillName;
    public string description;
    public Sprite icon;
    public AttackArea shapeArea;
    public GameObject particleEffect;
    public AudioClip soundEffect;

    public int damage;
    public List<Character> targets;
    public bool isFriendly;

    public Status status = new Status();

    [HideInInspector] public List<Tile> affectedTiles = new List<Tile>();

    #endregion

    #region Enum

    public enum ReleaseTypes
    {
        Single,
        OnTileType,
        Area, 
        Shape,
        All
    }
    public ReleaseTypes releaseTypes;

    #endregion Enum

    #region Use Skill
    public void Release(List<Character> targets)
    {
        foreach (var target in targets)
        {
            if ((isFriendly && target.characterType == TurnEnums.CharacterType.Enemy) || !isFriendly)
            {
                target.TakeDamage(damage);

                if (status.statusType != Status.StatusTypes.None)
                {
                    target.AddStatus(status);
                }
            }
        }
    }

    public void GetTargets()
    {
        // release skill
        switch (releaseTypes)
        {
            case ReleaseTypes.Single:
                //targets = GetSelectedHero();
                break;
            case ReleaseTypes.OnTileType:
                //targets = GetTargetsOnTileType();
                break;
            case ReleaseTypes.Area:
                //targets = GetTargetsInRange();
                break;
            case ReleaseTypes.Shape:
                //targets = GetTargetsInShape();
                break;
            case ReleaseTypes.All:
                //targets = GetAllMapTargets();
                break;
        }
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
