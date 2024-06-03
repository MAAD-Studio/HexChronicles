using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables

    [Header("Tile Information:")]
    [SerializeField] public TileSO tileData;
    [HideInInspector] public float cost = 1f;

    [HideInInspector] public float weatherCost = 1f;
    [HideInInspector] public bool underWeatherAffect = false;

    [Header("Connected Tile (Can be NULL):")]
    public Tile connectedTile;

    [HideInInspector] public Tile parentTile;
    [HideInInspector] public Character characterOnTile;
    [HideInInspector] public TileObject objectOnTile;

    [HideInInspector] public bool tileOccupied = false;
    [HideInInspector] public bool tileHasObject = false;
    [HideInInspector] public bool inFrontier = false;

    [HideInInspector] public bool Reachable { get { return !tileOccupied && !tileHasObject && inFrontier; } }

    private Renderer tileRenderer;

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(tileData != null, $"{gameObject.name} doesn't have a TileSO provided");

        Debug.Assert(tileData.baseMaterial != null, $"{tileData.name} SO doesn't have a base material included");
        Debug.Assert(tileData.highlightMaterial != null, $"{tileData.name} SO doesn't have a highlight material included");
        Debug.Assert(tileData.reachableMaterial != null, $"{tileData.name} SO doesn't have a reachable material included");
        Debug.Assert(tileData.attackableMaterial != null, $"{tileData.name} SO doesn't have a attackable material included");
        Debug.Assert(tileData.pathMaterial != null, $"{tileData.name} SO doesn't have a path material included");
        Debug.Assert(tileData.selectedCharMaterial != null, $"{tileData.name} SO doesn't have a selectedChar material included");

        tileRenderer = GetComponent<Renderer>();
    }

    void Update()
    {

    }

    #endregion

    #region CustomMethods

    //Changes the material applied to the Tile
    public void ChangeTileColor(TileEnums.TileMaterial tileMat)
    {
        switch (tileMat)
        {
            case TileEnums.TileMaterial.baseMaterial:
                tileRenderer.material = tileData.baseMaterial;
                break;

            case TileEnums.TileMaterial.highlight:
                tileRenderer.material = tileData.highlightMaterial;
                break;

            case TileEnums.TileMaterial.frontier:
                tileRenderer.material = tileData.reachableMaterial;
                break;

            case TileEnums.TileMaterial.attackable:
                tileRenderer.material = tileData.attackableMaterial;
                break;

            case TileEnums.TileMaterial.path:
                tileRenderer.material = tileData.pathMaterial;
                break;

            case TileEnums.TileMaterial.selectedChar:
                tileRenderer.material = tileData.selectedCharMaterial;
                break;
        }
    }

    //Called when a Character enters a tile
    public void OnTileEnter(Character character)
    {
        if (tileData.elementsStrongAgainst.Contains(character.elementType))
        {
            character.defensePercentage += 10;
            if (character.characterType == TurnEnums.CharacterType.Player)
            {
                MouseTip.Instance.ShowTip(character.transform.position, $"{character}'s defense +10%", false);
            }
        }
        else if(tileData.tileType == character.elementType)
        {
            character.attackDamage += 1;
            if (character.characterType == TurnEnums.CharacterType.Player)
            {
                MouseTip.Instance.ShowTip(character.transform.position, $"{character}'s attack damage +1", false);
            }
        }
    }

    //Called when a Character stays on a tile
    public void OnTileStay(Character character)
    {
        if (tileData.elementsWeakAgainst.Contains(character.elementType))
        {
            character.movementThisTurn += 1;
            if (character.characterType == TurnEnums.CharacterType.Player)
            {
                MouseTip.Instance.ShowTip(character.transform.position, $"{character}'s move range +1", false);
            }
        }
    }

    //Called when a Character is leaving a tile
    public void OnTileExit(Character character)
    {
        if (tileData.elementsStrongAgainst.Contains(character.elementType))
        {
            character.defensePercentage -= 10;
            if (character.characterType == TurnEnums.CharacterType.Player)
            {
                MouseTip.Instance.ShowTip(character.transform.position, $"{character}'s defense -10%", false);
            }
        }
        else if (tileData.tileType == character.elementType)
        {
            character.attackDamage -= 1;
            if (character.characterType == TurnEnums.CharacterType.Player)
            {
                MouseTip.Instance.ShowTip(character.transform.position, $"{character}'s attack damage -1", false);
            }
        }
    }

    #endregion
}
