using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables

    [Header("Tile Information:")]
    [SerializeField] public TileSO tileData;
    [HideInInspector] public float cost = 1f;

    [Header("Tile Materials:")]
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material reachableMaterial;
    [SerializeField] private Material attackableMaterial;

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

        Debug.Assert(baseMaterial != null, $"{gameObject.name} doesn't have a BaseMaterial provided");

        Debug.Assert(highlightMaterial != null, $"{gameObject.name} doesn't have a HighlightMaterial provided");

        Debug.Assert(reachableMaterial != null, $"{gameObject.name} doesn't have a ReachableMaterial provided");

        Debug.Assert(attackableMaterial != null, $"{gameObject.name} doesn't have a AttackableMaterial provided");

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
                tileRenderer.material = baseMaterial;
                break;

            case TileEnums.TileMaterial.highlight:
                tileRenderer.material = highlightMaterial;
                break;

            case TileEnums.TileMaterial.frontier:
                tileRenderer.material = reachableMaterial;
                break;

            case TileEnums.TileMaterial.attackable:
                tileRenderer.material = attackableMaterial;
                break;
        }
    }

    //Called when a Character enters a tile
    public void OnTileEnter(Character character)
    {
        if(tileData.elementsStrongAgainst.Contains(character.elementType))
        {
            character.defensePercentage += 10;
        }
        else if(tileData.tileType == character.elementType)
        {
            character.attackDamage += 1;
        }
    }

    //Called when a Character stays on a tile
    public void OnTileStay(Character character)
    {
        if (tileData.elementsWeakAgainst.Contains(character.elementType))
        {
            character.movementThisTurn += 1;
        }
    }

    //Called when a Character is leaving a tile
    public void OnTileExit(Character character)
    {
        if (tileData.elementsStrongAgainst.Contains(character.elementType))
        {
            character.defensePercentage -= 10;
        }
        else if (tileData.tileType == character.elementType)
        {
            character.attackDamage -= 1;
        }
    }

    #endregion
}
