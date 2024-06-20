using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CharacterUIConfig", menuName = "ScriptableObjects/CharacterUIConfig")]
public class CharacterUIConfig : ScriptableObject
{
    [Header("Element Sprites")]
    [SerializeField] private Sprite fire;
    [SerializeField] private Sprite water;
    [SerializeField] private Sprite grass;
    [SerializeField] private Sprite poison;
    
    [Header("Status Sprites")]
    [SerializeField] private Sprite burning;
    [SerializeField] private Sprite wet;
    [SerializeField] private Sprite haste;
    [SerializeField] private Sprite bound;

    [Header("Status Description")]
    [TextArea(3, 10)] public string burningDetail;
    [TextArea(3, 10)] public string wetDetail;
    [TextArea(3, 10)] public string hasteDetail;
    [TextArea(3, 10)] public string boundDetail;


    public Sprite GetElementSprite(ElementType element)
    {
        if (element == ElementType.Fire)
        {
            return fire;
        }
        else if (element == ElementType.Water)
        {
            return water;
        }
        else if (element == ElementType.Grass)
        {
            return grass;
        }
        else if (element == ElementType.Poison)
        {
            return poison;
        }
        return null;
    }

    public List<Sprite> GetStatusSprites(Character character)
    {
        if (character.statusList.Count != 0)
        {
            List<Sprite> statusList = new List<Sprite>();
            foreach (var status in character.statusList)
            {
                if (status.statusType == Status.StatusTypes.Burning)
                {
                    statusList.Add(burning);
                }
                else if (status.statusType == Status.StatusTypes.Wet)
                {
                    statusList.Add(wet);
                }
                else if (status.statusType == Status.StatusTypes.Haste)
                {
                    statusList.Add(haste);
                }
                else if (status.statusType == Status.StatusTypes.Bound)
                {
                    statusList.Add(bound);
                }
            }
            return statusList;
        }

        return null;
    }

    public Sprite GetStatusSprite(Status status)
    {
        if (status.statusType == Status.StatusTypes.Burning)
        {
            return burning;
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            return wet;
        }
        else if (status.statusType == Status.StatusTypes.Haste)
        {
            return haste;
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            return bound;
        }
        return null;
    }

    public string GetStatusExplain(Status status)
    {
        if (status.statusType == Status.StatusTypes.Burning)
        {
            return burningDetail;
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            return wetDetail;
        }
        else if (status.statusType == Status.StatusTypes.Haste)
        {
            return hasteDetail;
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            return boundDetail;
        }
        return null;
    }

    public string GetStatusTypes(Character character)
    {
        if (character.statusList.Count != 0)
        {
            string statusList = "";
            foreach (var status in character.statusList)
            {
                statusList += status.statusType.ToString() + ", ";
            }
            return statusList;
        }

        return "";
    }
}
