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

    public TileEnums.TileMaterial PreviousMaterial
    {
        get;
        private set;
    }

    public TileEnums.TileMaterial CurrentMaterial
    {
        get;
        private set;
    }

    [HideInInspector] public bool Reachable { get { return !tileOccupied && !tileHasObject && inFrontier; } }

    private Renderer tileRenderer;
    private Renderer topRenderer;
    private GameObject tileTop;

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

        tileTop = transform.GetChild(1).gameObject;
        topRenderer = tileTop.GetComponent<Renderer>();

        tileRenderer = transform.GetChild(0).GetComponent<Renderer>();
    }

    #endregion

    #region CustomMethods

    //Changes the material applied to the Tile
    public void ChangeTileColor(TileEnums.TileMaterial tileMat)
    {
        if(CurrentMaterial != TileEnums.TileMaterial.highlight)
        {
            PreviousMaterial = CurrentMaterial;
        }
 
        switch (tileMat)
        {
            case TileEnums.TileMaterial.baseMaterial:
                tileTop.SetActive(false);
                topRenderer.material = tileData.baseMaterial;
                tileRenderer.material = tileData.baseMaterial;
                CurrentMaterial = TileEnums.TileMaterial.baseMaterial;
                break;

            case TileEnums.TileMaterial.highlight:
                if (topRenderer.material == tileData.reachableMaterial)
                {
                    tileTop.SetActive(true);
                }
                tileRenderer.material = tileData.highlightMaterial;
                CurrentMaterial = TileEnums.TileMaterial.highlight;
                break;

            case TileEnums.TileMaterial.frontier:
                tileTop.SetActive(true);
                topRenderer.material = tileData.reachableMaterial;
                tileRenderer.material = tileData.baseMaterial;
                CurrentMaterial = TileEnums.TileMaterial.frontier;
                break;

            case TileEnums.TileMaterial.attackable:
                tileTop.SetActive(false);
                tileRenderer.material = tileData.attackableMaterial;
                CurrentMaterial = TileEnums.TileMaterial.attackable;
                break;

            case TileEnums.TileMaterial.path:
                tileTop.SetActive(false);
                tileRenderer.material = tileData.pathMaterial;
                CurrentMaterial = TileEnums.TileMaterial.path;
                break;

            case TileEnums.TileMaterial.selectedChar:
                tileTop.SetActive(false);
                tileRenderer.material = tileData.selectedCharMaterial;
                CurrentMaterial = TileEnums.TileMaterial.selectedChar;
                break;

            case TileEnums.TileMaterial.towerAttack:
                if (topRenderer.material == tileData.reachableMaterial)
                {
                    tileTop.SetActive(true);
                }
                tileRenderer.material = tileData.towerAttackMaterial;
                CurrentMaterial = TileEnums.TileMaterial.towerAttack;
                break;

            case TileEnums.TileMaterial.weather:
                if(topRenderer.material == tileData.reachableMaterial)
                {
                    tileTop.SetActive(true);
                }
                tileRenderer.material = tileData.weatherMaterial;
                CurrentMaterial = TileEnums.TileMaterial.weather;
                break;
        }
    }

    //Called when a Character enters a tile
    public virtual void OnTileEnter(Character character)
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
    public virtual void OnTileStay(Character character)
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
    public virtual void OnTileExit(Character character)
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

    public void TransferTileData(Tile tile)
    {
        tile.name = name;
        tile.transform.parent = transform.parent;
        tile.tileOccupied = tileOccupied;
        tile.characterOnTile = characterOnTile;
        tile.cost = cost;
        tile.tileHasObject = tileHasObject;
        tile.objectOnTile = objectOnTile;
        tile.underWeatherAffect = underWeatherAffect;
        tile.weatherCost = weatherCost;
        tile.inFrontier = inFrontier;
        tile.parentTile = parentTile;
    }

    #endregion
}
