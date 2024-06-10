using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "CharacterUIConfig", menuName = "ScriptableObjects/CharacterUIConfig")]
public class CharacterUIConfig : ScriptableObject
{
    public Sprite[] elementSprites;

    public void Initialize(Sprite[] sprites)
    {
        elementSprites = sprites;
    }

    public Sprite GetElementSprite(ElementType element)
    {
        if (element == ElementType.Fire)
        {
            return elementSprites[0];
        }
        else if (element == ElementType.Water)
        {
            return elementSprites[1];
        }
        else if (element == ElementType.Grass)
        {
            return elementSprites[2];
        }
        else
        {
            return null;
        }
    }

    public string GetStatusTypes(Character character)
    {
        if (character.statusList.Count != 0)
        {
            string statusList = "Status: ";
            foreach (var status in character.statusList)
            {
                statusList += status.statusType.ToString() + ", ";
            }
            return statusList;
        }

        return "";
    }
}
