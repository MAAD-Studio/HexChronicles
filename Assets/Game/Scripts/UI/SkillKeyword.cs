using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class SkillKeyword
{
    public string keyword;
    public Color color;
    public bool bold;
    public bool italic;

    public string GetFormattedKeyword()
    {
        string formattedKeyword = keyword;

        if (bold) formattedKeyword = $"<b>{formattedKeyword}</b>";
        if (italic) formattedKeyword = $"<i>{formattedKeyword}</i>";
        formattedKeyword = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{formattedKeyword}</color>";

        return formattedKeyword;
    }
}
