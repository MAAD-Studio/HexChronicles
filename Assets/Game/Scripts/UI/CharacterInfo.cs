using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    public GameObject defaultState;
    public GameObject selectedState;
    public GameObject deadState;

    public List<TextMeshProUGUI> names;
    public List<Image> avatars;
    public List<Image> icons;

    public void SetDefaultState()
    {
        defaultState.SetActive(true);
        selectedState.SetActive(false);
        deadState.SetActive(false);
    }

    public void SetSelectedState()
    {
        defaultState.SetActive(false);
        selectedState.SetActive(true);
        deadState.SetActive(false);
    }

    public void SetDeadState()
    {
        defaultState.SetActive(false);
        selectedState.SetActive(false);
        deadState.SetActive(true);
    }
}
