using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is only a container for the dialogue data
[System.Serializable]
public class Dialogue
{
    public int speakerID;
    [TextArea(3, 10)]
    public string sentence;
}
