using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : Singleton<Config>
{
    public CharacterUIConfig characterUIConfig;

    #region Weather
    public Sprite GetWeatherSprite(WeatherType weatherType)
    {
        if (weatherType == WeatherType.rain)
        {
            return characterUIConfig.rain;
        }
        else if (weatherType == WeatherType.sporeStorm)
        {
            return characterUIConfig.sporeStorm;
        }
        else if (weatherType == WeatherType.heatWave)
        {
            return characterUIConfig.heatWave;
        }
        return null;
    }
    #endregion


    #region Buff and debuff
    public GameObject GetBuffVFX(ElementType elementType, bool isBig)
    {
        if (elementType == ElementType.Fire)
        {
            if (isBig)
            { 
                return characterUIConfig.fireBuffVFX;
            }
            else
            {
                return characterUIConfig.shortFireBuffVFX;
            }
        }
        else if (elementType == ElementType.Water)
        {
            if (isBig)
            {
                return characterUIConfig.waterBuffVFX;
            }
            else
            {
                return characterUIConfig.shortWaterBuffVFX;
            }
        }
        else if (elementType == ElementType.Grass)
        {
            if (isBig)
            {
                return characterUIConfig.grassBuffVFX;
            }
            else
            {
                return characterUIConfig.shortGrassBuffVFX;
            }
        }
        return null;
    }

    public GameObject GetDebuffVFX(bool isBig)
    {
        if (isBig)
        {
            return characterUIConfig.debuffVFX;
        }
        else
        {
            return characterUIConfig.shortDebuffVFX;
        }
    }
    #endregion


    #region Stat and element
    public Sprite GetStatSprite(string statType)
    {
        if (statType == "health")
        {
            return characterUIConfig.health;
        }
        else if (statType == "attack")
        {
            return characterUIConfig.attack;
        }
        else if (statType == "movement")
        {
            return characterUIConfig.movement;
        }
        return null;
    }

    public Sprite GetElementSprite(ElementType element)
    {
        if (element == ElementType.Fire)
        {
            return characterUIConfig.fire;
        }
        else if (element == ElementType.Water)
        {
            return characterUIConfig.water;
        }
        else if (element == ElementType.Grass)
        {
            return characterUIConfig.grass;
        }
        else if (element == ElementType.Poison)
        {
            return characterUIConfig.poison;
        }
        return null;
    }
    #endregion


    #region Status
    public List<Sprite> GetStatusSprites(Character character)
    {
        if (character.statusList.Count != 0)
        {
            List<Sprite> statusList = new List<Sprite>();
            foreach (var status in character.statusList)
            {
                if (status.statusType == Status.StatusTypes.Burning)
                {
                    statusList.Add(characterUIConfig.burning);
                }
                else if (status.statusType == Status.StatusTypes.Wet)
                {
                    statusList.Add(characterUIConfig.wet);
                }
                else if (status.statusType == Status.StatusTypes.Haste)
                {
                    statusList.Add(characterUIConfig.haste);
                }
                else if (status.statusType == Status.StatusTypes.Bound)
                {
                    statusList.Add(characterUIConfig.bound);
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
            return characterUIConfig.burning;
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            return characterUIConfig.wet;
        }
        else if (status.statusType == Status.StatusTypes.Haste)
        {
            return characterUIConfig.haste;
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            return characterUIConfig.bound;
        }
        else if (status.statusType == Status.StatusTypes.Shield)
        {
            return characterUIConfig.shield;
        }
        return null;
    }

    public string GetStatusExplain(Status status)
    {
        if (status.statusType == Status.StatusTypes.Burning)
        {
            return characterUIConfig.burningDetail;
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            return characterUIConfig.wetDetail;
        }
        else if (status.statusType == Status.StatusTypes.Haste)
        {
            return characterUIConfig.hasteDetail;
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            return characterUIConfig.boundDetail;
        }
        else if (status.statusType == Status.StatusTypes.Shield)
        {
            return characterUIConfig.shieldDetail;
        }
        return null;
    }

    public GameObject GetStatusVFX(Status status)
    {
        if (status.statusType == Status.StatusTypes.Burning)
        {
            return characterUIConfig.burningVFX;
        }
        else if (status.statusType == Status.StatusTypes.Wet)
        {
            //return characterUIConfig.wetVFX;
        }
        else if (status.statusType == Status.StatusTypes.Haste)
        {
            //return characterUIConfig.hasteVFX;
        }
        else if (status.statusType == Status.StatusTypes.Bound)
        {
            return characterUIConfig.boundVFX;
        }
        else if (status.statusType == Status.StatusTypes.Shield)
        {
            //return characterUIConfig.shieldVFX;
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
    #endregion
}
