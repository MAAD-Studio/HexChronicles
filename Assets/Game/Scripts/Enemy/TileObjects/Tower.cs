using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Spawner
{
    #region Variables

    [Range(0, 4)]
    [SerializeField] private int tileRange = 3;
    private float actualRange;

    public int TileRange
    {
        get { return tileRange; }
    }

    [SerializeField] private int damage = 5;

    [SerializeField] private AttackArea attackAreaPrefab;
    private AttackArea spawnedAttackArea;

    private List<Character> possibleCharacterChoices = new List<Character>();
    List<Tile> tilesToColor = new List<Tile>();

    [SerializeField] private Projectile projectile;
    [SerializeField] private float archHeight;
    [SerializeField] private float projectileSpeed;

    #endregion

    #region UnityMethods

    public override void Start()
    {
        base.Start();

        Debug.Assert(attackAreaPrefab != null, $"{name} doesn't have an AttackArea prefab provided.");

        actualRange = tileRange * 2f;
    }

    #endregion

    #region CustomMethods

    public bool CanAttack()
    {
        if(spawnedAttackArea != null )
        {
            return true;
        }
        else
        {
            possibleCharacterChoices.Clear();
            foreach (Character character in turnManager.characterList)
            {
                if (Vector3.Distance(character.transform.position, transform.position) <= actualRange)
                {
                    possibleCharacterChoices.Add(character);
                }
            }

            if(possibleCharacterChoices.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void AttemptAttack()
    {
        List<Character> charactersToRemove = new List<Character>();
        foreach(Character character in possibleCharacterChoices)
        {
            if(character ==  null)
            {
                charactersToRemove.Add(character);
            }
        }

        foreach(Character character in charactersToRemove)
        {
            possibleCharacterChoices.Remove(character);
        }

        if(spawnedAttackArea == null)
        {
            if(possibleCharacterChoices.Count > 0)
            {
                int choice = Random.Range(0, possibleCharacterChoices.Count);
                Vector3 spawnChoice = possibleCharacterChoices[choice].transform.position;
                spawnChoice.y = 0;

                spawnedAttackArea = Instantiate(attackAreaPrefab, spawnChoice, Quaternion.identity);
                turnManager.mainCameraController.MoveToTargetPosition(spawnedAttackArea.transform.position, true);

                Tile originTile = possibleCharacterChoices[choice].characterTile;
                tilesToColor = new List<Tile>(turnManager.pathfinder.FindAdjacentTiles(originTile, true))
                {
                    originTile
                };

                foreach (Tile tile in tilesToColor)
                {
                    tile.ChangeTileEffect(TileEnums.TileEffects.towerAttack, true);
                }
            }
        }
        else
        {
            turnManager.mainCameraController.MoveToTargetPosition(spawnedAttackArea.transform.position, true);

            spawnedAttackArea.DetectArea(false, false);
            foreach(Tile tile in tilesToColor)
            {
                Projectile newProjectile = Instantiate(projectile, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                if(tile.tileOccupied)
                {
                    newProjectile.Launch(tile.transform.position, archHeight, projectileSpeed, tile.characterOnTile, damage);
                }
                else
                {
                    newProjectile.Launch(tile.transform.position, archHeight, projectileSpeed);
                }
            }

            AudioManager.Instance.PlaySound(tileObjectData.SFX);

            foreach (Tile tile in tilesToColor)
            {
                tile.ChangeTileEffect(TileEnums.TileEffects.towerAttack, false);
            }

            spawnedAttackArea.DestroySelf();
            spawnedAttackArea = null;
        }
    }

    public override void TakeDamage(float attackDamage)
    {
        currentHealth -= attackDamage;

        UpdateHealthBar?.Invoke();

        // Show damage text
        DamageText damageText = Instantiate(damagePrefab, transform.position, Quaternion.identity).GetComponent<DamageText>();
        damageText.ShowDamage(attackDamage);

        if (currentHealth <= 0)
        {
            if(spawnedAttackArea != null)
            {
                foreach (Tile tile in tilesToColor)
                {
                    tile.ChangeTileEffect(TileEnums.TileEffects.towerAttack, false);
                }
                spawnedAttackArea.DestroySelf();
            }

            objectDestroyed.Invoke(this);

            attachedTile.objectOnTile = null;
            attachedTile.tileHasObject = false;

            Destroy(gameObject);
        }
    }

    public UndoData_Tower CustomUndoData()
    {
        UndoData_Tower data = new UndoData_Tower();
        if (spawnedAttackArea != null)
        {
            data.attacking = true;
            data.attackAreaPosition = spawnedAttackArea.transform.position;
        }
        else
        {
            data.attacking = false;
        }

        return data;
    }

    public override void Undo(UndoData_TileObjCustomInfo data)
    {
        if(spawnedAttackArea == null)
        {
            UndoData_Tower towerData = (UndoData_Tower)data;

            spawnedAttackArea = Instantiate(attackAreaPrefab, towerData.attackAreaPosition, Quaternion.identity);

            foreach (Tile tile in tilesToColor)
            {
                tile.ChangeTileEffect(TileEnums.TileEffects.towerAttack, true);
            }
        }
    }

    #endregion
}
