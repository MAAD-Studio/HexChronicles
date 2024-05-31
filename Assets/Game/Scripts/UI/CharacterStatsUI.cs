using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatsUI : StatsUI
{
    [Header("Hero")]
    public Image attackShape;
    public TextMeshProUGUI attackInfo;

    public Image skillShape;
    public TextMeshProUGUI skillInfo;
    public TextMeshProUGUI skillCD;

    [Header("Buttons")]
    public Button moveBtn;
    public Button attackBtn;
    public Button skillBtn;

}
