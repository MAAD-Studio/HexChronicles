using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class KeywordDescription
{
    [TextArea(3,10)] public string description;
    public Keyword[] keywords;

    public string DisplayKeywordDescription()
    {
        string formattedText = description;
        if (keywords != null)
        {
            foreach (var keyword in keywords)
            {
                string replacement = keyword.GetFormattedKeyword();
                formattedText = formattedText.Replace(keyword.keyword, replacement);
            }
        }

        return formattedText;
    }
    
}
