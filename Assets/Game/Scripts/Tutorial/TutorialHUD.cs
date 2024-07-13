using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHUD : MonoBehaviour
{
    [Header("Turn Info")]
    [SerializeField] private GameObject playerTurnMessage;
    [SerializeField] private GameObject enemyTurnMessage;

    [Header("Hero Info")]
    [SerializeField] private GameObject heroListPanel;
    [SerializeField] private GameObject heroInfoPrefab;

    [Header("Enemy Info")]
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private GameObject enemyDetailPrefab;
    [SerializeField] private GameObject enemyHoverPanel;
    [SerializeField] private GameObject enemyHoverPrefab;

    [Header("Tile Info")]
    [SerializeField] private GameObject tileInfoPanel;
    private TileInfo tileInfo;

    [Header("Buttons")]
    [SerializeField] private Button pause;
    [SerializeField] private Button endTurn;

    private void Start()
    {
        DialogueTrigger dialogueTrigger = FindObjectOfType<DialogueTrigger>();
        dialogueTrigger.TriggerDialogue();
    }
}
