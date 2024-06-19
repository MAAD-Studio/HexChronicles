using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

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
    protected int characterTimeOnTile = 0;
    [HideInInspector] public TileObject objectOnTile;

    [HideInInspector] public bool tileOccupied = false;
    [HideInInspector] public bool tileHasObject = false;
    [HideInInspector] public bool inFrontier = false;

    [HideInInspector] public bool Reachable { get { return !tileOccupied && !tileHasObject && inFrontier; } }

    private Renderer tileRenderer;

    private GameObject tileTop;
    private Renderer topRenderer;
    private List<TileEnums.TileTops> activeTileTops = new List<TileEnums.TileTops>();
    //[Header("Priority Level for Tile Tops:")]
    //public List<TileEnums.TileTops> priorityLevelTops = new List<TileEnums.TileTops>();

    private GameObject tileEffect;
    private Renderer effectRenderer;
    private List<TileEnums.TileEffects> activeTileEffects = new List<TileEnums.TileEffects>();
    //[Header("Priority Level for Tile Effects:")]
    //public List<TileEnums.TileEffects> priorityLevelEffects = new List<TileEnums.TileEffects>();

    private GameObject tileWeather;
    private Renderer weatherRenderer;

    public static UnityEvent<Tile, Tile> tileReplaced = new UnityEvent<Tile, Tile>();

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

        tileEffect = transform.GetChild(2).gameObject;
        effectRenderer = tileEffect.GetComponent<Renderer>();

        tileWeather = transform.GetChild(3).gameObject;
        weatherRenderer = tileWeather.GetComponent<Renderer>();

        tileRenderer = transform.GetChild(0).GetComponent<Renderer>();
    }

    #endregion

    #region TileEffectMethods

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

            case TileEnums.TileMaterial.path:
                tileRenderer.material = tileData.pathMaterial;
                break;

            case TileEnums.TileMaterial.selectedChar:
                tileRenderer.material = tileData.selectedCharMaterial;
                break;
        }
    }

    public void ChangeTileTop(TileEnums.TileTops tileTopType, bool enable)
    {
        if(enable)
        {
            if(!activeTileTops.Contains(tileTopType))
            {
                activeTileTops.Add(tileTopType);
            }

            switch (tileTopType)
            {
                case TileEnums.TileTops.frontier:
                    tileTop.SetActive(true);
                    topRenderer.material = tileData.reachableMaterial;
                    break;
            }
        }
        else
        {
            if(activeTileTops.Contains(tileTopType))
            {
                activeTileTops.Remove(tileTopType);
            }

            if(activeTileTops.Count > 0)
            {
                ChangeTileTop(activeTileTops[0], true);
            }
            else
            {
                tileTop.SetActive(false);
            }
        }
    }

    /*private void TopPriorityChange()
    {
        foreach (TileEnums.TileTops priTileTop in priorityLevelTops)
        {
            foreach (TileEnums.TileTops top in activeTileTops)
            {
                if (priTileTop == top)
                {
                    ChangeTileTop(top, true);
                    return;
                }
            }
        }

        ChangeTileTop(activeTileTops[0], true);
    }*/

    public void ChangeTileEffect(TileEnums.TileEffects tileEffectType, bool enable)
    {
        if(enable)
        {
            if(!activeTileEffects.Contains(tileEffectType))
            {
                activeTileEffects.Add(tileEffectType);
            }

            switch (tileEffectType)
            {
                case TileEnums.TileEffects.towerAttack:
                    tileEffect.SetActive(true);
                    effectRenderer.material = tileData.towerAttackMaterial;
                    break;

                case TileEnums.TileEffects.attackable:
                    tileEffect.SetActive(true);
                    effectRenderer.material = tileData.attackableMaterial;
                    break;
            }
        }
        else
        {
            if(activeTileEffects.Contains(tileEffectType))
            {
                activeTileEffects.Remove(tileEffectType);
            }

            if (activeTileEffects.Count > 0)
            {
                ChangeTileEffect(activeTileEffects[0], true);
            }
            else
            {
                tileEffect.SetActive(false);
            }
        }
    }

    /*private void EffectPriorityChange()
    {
        foreach (TileEnums.TileEffects priTileEffect in priorityLevelEffects)
        {
            foreach (TileEnums.TileEffects effect in activeTileEffects)
            {
                if (priTileEffect == effect)
                {
                    ChangeTileEffect(effect, true);
                    return;
                }
            }
        }

        ChangeTileEffect(activeTileEffects[0], true);
    }*/

    public void ChangeTileWeather(TileEnums.TileWeather tileWeatherType)
    {
        switch(tileWeatherType)
        {
            case TileEnums.TileWeather.disabled:
                tileWeather.SetActive(false);
                break;

            case TileEnums.TileWeather.rain:
                tileWeather.SetActive(true);
                weatherRenderer.material = tileData.weatherMaterial;
                break;
        }
    }

    #endregion

    #region TileMovementMethods

    //Called when a Character enters a tile
    public virtual void OnTileEnter(Character character)
    {

    }

    //Called when a Character stays on a tile
    public virtual void OnTileStay(Character character)
    {
        characterTimeOnTile += 1;
    }

    //Called when a Character is leaving a tile
    public virtual void OnTileExit(Character character)
    {
        characterTimeOnTile = 0;
    }

    #endregion

    #region TileReplacement

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

        /*if(tile.underWeatherAffect)
        {
            tile.ChangeTileWeather(TileEnums.TileWeather.rain);
        }*/
    }

    public void ReplaceTileWithNew(Tile newTile)
    {
        TransferTileData(newTile);
        tileReplaced.Invoke(this, newTile);

        Destroy(gameObject);
    }

    #endregion
}
