using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : Singleton<UndoManager>
{
    #region Variables

    [Header("Hero Prefabs")]
    [SerializeField] private GameObject fireHeroPrefab;
    [SerializeField] private GameObject waterHeroPrefab;
    [SerializeField] private GameObject grassHeroPrefab;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject gangstaPrefab;
    [SerializeField] private GameObject tntPrefab;
    [SerializeField] private GameObject soloJellyPrefab;
    [SerializeField] private GameObject masterJellyPrefab;
    [SerializeField] private GameObject kingJellyPrefab;
    [SerializeField] private GameObject drainerPrefab;
    [SerializeField] private GameObject makerPrefab;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject baseTilePrefab;
    [SerializeField] private GameObject fireTilePrefab;
    [SerializeField] private GameObject waterTilePrefab;
    [SerializeField] private GameObject grassTilePrefab;
    [SerializeField] private GameObject deathTilePrefab;

    [Header("Object Prefabs")]
    [SerializeField] private GameObject towerPrefab;

    private List<UndoData_Hero> heroList = new List<UndoData_Hero>();
    private List<UndoData_Enemies> enemyList = new List<UndoData_Enemies>();
    private List<UndoData_TileObject> tileObjectsList = new List<UndoData_TileObject>();
    private List<UndoData_Tile> tileList = new List<UndoData_Tile>();

    public bool DataStored
    {
        get;
        private set;
    }

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(fireHeroPrefab != null, "UndoManager hasn't been provided a Fire Hero Prefab");
        Debug.Assert(waterHeroPrefab != null, "UndoManager hasn't been provided a Water Hero Prefab");
        Debug.Assert(grassHeroPrefab != null, "UndoManager hasn't been provided a Grass Hero Prefab");

        Debug.Assert(gangstaPrefab != null, "UndoManager hasn't been provided a Gangsta Prefab");
        Debug.Assert(tntPrefab != null, "UndoManager hasn't been provided a TNT Prefab");
        Debug.Assert(soloJellyPrefab != null, "UndoManager hasn't been provided a SoloJelly Prefab");
        Debug.Assert(masterJellyPrefab != null, "UndoManager hasn't been provided a MasterJelly Prefab");
        Debug.Assert(kingJellyPrefab != null, "UndoManager hasn't been provided a KingJelly Prefab");
        Debug.Assert(drainerPrefab != null, "UndoManager hasn't been provided a Drainer Prefab");
        Debug.Assert(makerPrefab != null, "UndoManager hasn't been provided a Maker Prefab");

        Debug.Assert(baseTilePrefab != null, "UndoManager hasn't been provided a BaseTile Prefab");
        Debug.Assert(fireTilePrefab != null, "UndoManager hasn't been provided a FireTile Prefab");
        Debug.Assert(waterTilePrefab != null, "UndoManager hasn't been provided a WaterTile Prefab");
        Debug.Assert(grassTilePrefab != null, "UndoManager hasn't been provided a GrassTile Prefab");
        Debug.Assert(deathTilePrefab != null, "UndoManager hasn't been provided a DeathTile Prefab");

        Debug.Assert(towerPrefab != null, "UndoManager hasn't been provided a Tower Prefab");
    }

    void Update()
    {
        
    }

    #endregion

    #region CustomMethods

    public void StoreHero(Hero hero)
    {
        if(heroList.Find(x => x.heroInvolved == hero) != null)
        {
            return;
        }

        DataStored = true;

        UndoData_Hero heroData = new UndoData_Hero();
        StoreCharacter(heroData, hero);

        heroData.heroInvolved = hero;
        heroData.heroType = hero.elementType;

        heroData.hasMadeDecision = hero.hasMadeDecision;
        heroData.upgradeList = new List<BasicUpgrade>(hero.upgradeList);

        heroList.Add(heroData);
    }

    public void StoreEnemy(Enemy_Base enemy)
    {
        if(enemyList.Find(x => x.enemyInvolved == enemy) != null)
        {
            return;
        }

        DataStored = true;

        UndoData_Enemies enemyData = new UndoData_Enemies();
        StoreCharacter(enemyData, enemy);

        enemyData.enemyInvolved = enemy;
        enemyData.enemyType = enemy.enemyType;

        enemyList.Add(enemyData);
    }

    private void StoreCharacter(UndoData_Character data, Character character)
    {
        data.isHurt = character.isHurt;
        data.movementThisTurn = character.movementThisTurn;
        data.currentHealth = character.currentHealth;
        data.effectedByWeather = character.effectedByWeather;

        data.statusList = new List<Status>(character.statusList);

        data.position = character.transform.position;
        data.position.y += 1f;
        data.rotation = character.transform.rotation;
    }

    public void StoreTileObject(TileObject tileObj, bool destroy)
    {
        DataStored = true;

        UndoData_TileObject tileObjData = new UndoData_TileObject();
        
        tileObjData.destroy = true;
      
        tileObjData.involvedObject = tileObj;

        tileObjData.position = tileObj.transform.position;
        tileObjData.position.y += 1f;

        tileObjData.type = tileObj.objectType;
        tileObjData.currentHealth = tileObj.currentHealth;

        tileObjectsList.Add(tileObjData);
    }

    public void StoreTile(Tile newTile, ElementType oldTilesType)
    {
        DataStored = true;

        UndoData_Tile tileData = new UndoData_Tile();

        tileData.invovledTile = newTile;
        tileData.elementType = oldTilesType;
        tileData.tilePosition = newTile.transform.position;

        tileList.Add(tileData);
    }

    public void ClearData()
    {
        heroList.Clear();
        enemyList.Clear();
        tileObjectsList.Clear();
        tileList.Clear();

        DataStored = false;
    }

    public void RestoreState()
    {
        DataStored = false;

        TurnManager turnManager = FindObjectOfType<TurnManager>();
        Debug.Assert(turnManager != null, "UndoManager couldn't locate a TurnManager for it to restore data into");

        RestoreTileData();
        RestoreHeroData(turnManager);
        RestoreEnemyData(turnManager);
        RestoreTileObjectData(turnManager);

        ClearData();
    }

    private void RestoreHeroData(TurnManager turnManager)
    {
        foreach (UndoData_Hero data in heroList)
        {
            Hero currentHero;
            if (turnManager.characterList.Contains(data.heroInvolved))
            {
                currentHero = data.heroInvolved;
                currentHero.characterTile.characterOnTile = null;
                currentHero.characterTile.tileOccupied = false;
                currentHero.characterTile = null;
            }
            else
            {
                currentHero = GenerateHero(data.heroType);
                turnManager.characterList.Add(currentHero);
            }

            currentHero.hasMadeDecision = data.hasMadeDecision;
            currentHero.upgradeList = new List<BasicUpgrade>(data.upgradeList);

            EventBus.Instance.Publish(new UpdateCharacterDecision { 
               character = currentHero, hasMadeDecision = currentHero.hasMadeDecision });

            StartCoroutine(RestoreCharacterData(data, currentHero));
        }
    }

    private void RestoreEnemyData(TurnManager turnManager)
    {
        foreach(UndoData_Enemies data in enemyList)
        {
            Enemy_Base currentEnemy;
            if(turnManager.enemyList.Contains(data.enemyInvolved))
            {
                currentEnemy = data.enemyInvolved;
                currentEnemy.characterTile.characterOnTile = null;
                currentEnemy.characterTile.tileOccupied = false;
                currentEnemy.characterTile = null;
            }
            else
            {
                currentEnemy = GenerateEnemy(data.enemyType);
                turnManager.enemyList.Add(currentEnemy);
            }

            StartCoroutine(RestoreCharacterData(data, currentEnemy));
        }
    }

    private IEnumerator RestoreCharacterData(UndoData_Character data, Character character)
    {
        yield return null;

        character.isHurt = data.isHurt;
        character.movementThisTurn = data.movementThisTurn;
        character.currentHealth = data.currentHealth;
        character.effectedByWeather = data.effectedByWeather;

        character.statusList = new List<Status>(data.statusList);

        EventBus.Instance.Publish(new OnRestoreCharacterData { character = character });
        character.UpdateHealthBar?.Invoke();
        character.UpdateAttributes?.Invoke();
        character.UpdateStatus?.Invoke();

        character.transform.position = data.position;
        character.transform.rotation = data.rotation;

        character.FindTile();
    }

    private void RestoreTileObjectData(TurnManager turnManager)
    {
        foreach(UndoData_TileObject data in tileObjectsList)
        {
            TileObject currentObject;
            if(data.involvedObject != null)
            {
                currentObject = data.involvedObject;
                if(data.destroy)
                {
                    turnManager.temporaryTileObjects.Remove(currentObject);
                    currentObject.AttachedTile.tileHasObject = false;
                    currentObject.AttachedTile.objectOnTile = null;
                    Destroy(currentObject.gameObject);
                    continue;
                }
            }
            else
            {
                currentObject = GenerateTileObject(data.type);
            }

            currentObject.currentHealth = data.currentHealth;
            currentObject.UpdateHealthBar?.Invoke();

            currentObject.transform.position = data.position;
            currentObject.FindTile();

            TileObject.objectCreated.Invoke(currentObject);
        }
    }

    private void RestoreTileData()
    {
        foreach(UndoData_Tile data in tileList)
        {
            Tile currentTile = GenerateTile(data.elementType);
            data.invovledTile.ReplaceTileWithNew(currentTile);
            currentTile.transform.position = data.tilePosition;
        }
    }

    //Instantiates a Hero of the given elemental type
    private Hero GenerateHero(ElementType type)
    {
        Hero generatedHero;

        if(type == ElementType.Fire)
        {
            generatedHero = Instantiate(fireHeroPrefab).GetComponent<Hero>();
            Debug.Assert(generatedHero != null, "The Fire Hero Prefab in UndoManager is not a Hero");
        }
        else if(type == ElementType.Water)
        {
            generatedHero = Instantiate(waterHeroPrefab).GetComponent<Hero>();
            Debug.Assert(generatedHero != null, "The Water Hero Prefab in UndoManager is not a Hero");
        }
        else
        {
            generatedHero = Instantiate(grassHeroPrefab).GetComponent<Hero>();
            Debug.Assert(generatedHero != null, "The Grass Hero Prefab in UndoManager is not a Hero");
        }

        return generatedHero;
    }

    private Enemy_Base GenerateEnemy(EnemyType type)
    {
        Enemy_Base generatedEnemy;

        switch(type)
        {
            case EnemyType.Gangsta:
                generatedEnemy = Instantiate(gangstaPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The Gansta Prefab in UndoManager is not a Enemy_Base");
                break;

            case EnemyType.TNT:
                generatedEnemy = Instantiate(tntPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The TNT Prefab in UndoManager is not a Enemy_Base");
                break;

            case EnemyType.SoloJelly:
                generatedEnemy = Instantiate(soloJellyPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The SoloJelly Prefab in UndoManager is not a Enemy_Base");
                break;

            case EnemyType.MasterJelly:
                generatedEnemy = Instantiate(masterJellyPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The MasterJelly Prefab in UndoManager is not a Enemy_Base");
                break;

            case EnemyType.KingJelly:
                generatedEnemy = Instantiate(kingJellyPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The KingJelly Prefab in UndoManager is not a Enemy_Base");
                break;

            case EnemyType.Drainer:
                generatedEnemy = Instantiate(drainerPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The Drainer Prefab in UndoManager is not a Enemy_Base");
                break;

            case EnemyType.Maker:
                generatedEnemy = Instantiate(makerPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The Maker Prefab in UndoManager is not a Enemy_Base");
                break;

            default:
                generatedEnemy = Instantiate(gangstaPrefab).GetComponent<Enemy_Base>();
                Debug.Assert(generatedEnemy != null, "The Gansta Prefab in UndoManager is not a Enemy_Base");
                break;
        }

        return generatedEnemy;
    }

    private TileObject GenerateTileObject(ObjectType type)
    {
        TileObject generatedObject;

        switch(type)
        {
            case ObjectType.Tower:
                generatedObject = Instantiate(towerPrefab).GetComponent<TileObject>();
                Debug.Assert(generatedObject != null, "The Tower Prefab in UndoManager is not a Tile Object");
                break;

            default:
                generatedObject = Instantiate(towerPrefab).GetComponent<TileObject>();
                Debug.Assert(generatedObject != null, "The Tower Prefab in UndoManager is not a Tile Object");
                break;
        }

        return generatedObject;
    }

    private Tile GenerateTile(ElementType type)
    {
        Tile generatedTile;

        switch(type)
        {
            case ElementType.Fire:
                generatedTile = Instantiate(fireTilePrefab).GetComponent<Tile>();
                Debug.Assert(generatedTile != null, "The FireTile Prefab in UndoManager is not a Tile");
                break;

            case ElementType.Water:
                generatedTile = Instantiate(waterTilePrefab).GetComponent<Tile>();
                Debug.Assert(generatedTile != null, "The WaterTile Prefab in UndoManager is not a Tile");
                break;

            case ElementType.Grass:
                generatedTile = Instantiate(grassTilePrefab).GetComponent<Tile>();
                Debug.Assert(generatedTile != null, "The GrassTile Prefab in UndoManager is not a Tile");
                break;

            case ElementType.death:
                generatedTile = Instantiate(deathTilePrefab).GetComponent<Tile>();
                Debug.Assert(generatedTile != null, "The DeathTile Prefab in UndoManager is not a Tile");
                break;

            default:
                generatedTile = Instantiate(baseTilePrefab).GetComponent<Tile>();
                Debug.Assert(generatedTile != null, "The BaseTile Prefab in UndoManager is not a Tile");
                break;
        }

        return generatedTile;
    }

    #endregion
}
