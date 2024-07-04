using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Spawner
{
    #region Variables

    [Range(0, 4)]
    [SerializeField] private int tileRange = 3;
    private float actualRange;

    [SerializeField] private int damage = 5;

    [SerializeField] private AttackArea attackAreaPrefab;
    private AttackArea spawnedAttackArea;

    private List<Character> possibleCharacterChoices = new List<Character>();
    List<Tile> tilesToColor = new List<Tile>();

    #endregion

    #region UnityMethods

    public override void Start()
    {
        base.Start();

        Debug.Assert(attackAreaPrefab != null, $"{name} doesn't have an AttackArea prefab provided.");

        actualRange = tileRange * 1.75f;
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
            foreach (Character character in spawnedAttackArea.CharactersHit(TurnEnums.CharacterType.Player))
            {
                character.TakeDamage(damage, ElementType.Base);
            }

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

    #endregion
}
