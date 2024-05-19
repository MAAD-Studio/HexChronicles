using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Store a list of heroes, used to save and load hero data
/// </summary>
[System.Serializable]
public class HeroList
{
    public List<BasicAttributes> heroes;

    // Constructor to initialize the list
    public HeroList(List<BasicAttributes> Heroes)
    {
        this.heroes = Heroes;
    }
}
