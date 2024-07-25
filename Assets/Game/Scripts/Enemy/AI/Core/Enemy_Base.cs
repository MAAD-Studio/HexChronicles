using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : Character, EnemyInterface
{
    #region Variables

    [Header("Enemy Specific:")]
    public EnemyAttributesSO enemySO;
    public EnemyType enemyType;
    public PreviewOrigin attackAreaPreview;

    protected bool mindControl = false;

    #endregion

    #region UnityMethods
    protected override void Start()
    {
        base.Start();

        moveDistance = enemySO.attributes.movementRange;
        attackDamage = enemySO.attributes.attackDamage;
        defensePercentage = enemySO.attributes.defensePercentage;
        elementType = enemySO.attributes.elementType;
        elementWeakAgainst = enemySO.attributes.elementWeakAgainst;
        elementStrongAgainst = enemySO.attributes.elementStrongAgainst;

        maxHealth = enemySO.attributes.health;
        currentHealth = maxHealth;

        basicAttackArea = enemySO.attackArea;

        attackAreaPreview = enemySO.attributes.attackAreaPreview;
        defaultScale = enemySO.attributes.defaultScale;
    }

    #endregion

    #region InterfaceMethods

    public virtual void PreCalculations(TurnManager turnManager)
    {
        mindControl = (Status.GrabIfStatusActive(this, Status.StatusTypes.MindControl) != null);
    }

    public virtual int CalculateMovementValue(Tile tile, Enemy_Base enemy, TurnManager turnManager, Character closestCharacter)
    {
        int distanceTile = (int)Vector3.Distance(tile.transform.position, closestCharacter.transform.position);
        int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, closestCharacter.transform.position);
        int tileValue = distanceEnemy - distanceTile;

        return tileValue * 2;
    }

    public virtual int CalculteAttackValue(AttackArea attackArea, TurnManager turnManager, Tile currentTile)
    {
        List<Character> charactersToCheck;
        if (!mindControl)
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Player);
        }
        else
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Enemy);
        }

        int valueOfAttack = 0;
        foreach (Character character in charactersToCheck)
        {
            valueOfAttack += 25;

            //Bias towards remaining on current tile
            if (currentTile == characterTile)
            {
                valueOfAttack += 30;
            }
        }

        return valueOfAttack;
    }

    public virtual void ExecuteAttack(AttackArea attackArea, TurnManager turnManager)
    {
        animator.SetTrigger("attack");
        List<Character> charactersToCheck;
        if (!mindControl)
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Player);
            PerformBasicAttack(charactersToCheck);
        }
        else
        {
            charactersToCheck = attackArea.CharactersHit(TurnEnums.CharacterType.Enemy);
            PerformBasicAttack(charactersToCheck);
        }

        foreach (Character character in charactersToCheck)
        {
            transform.LookAt(character.transform.position);

            // Spawn attack vfx
            GameObject vfx = Instantiate(attackVFX, transform.position, Quaternion.identity);
            vfx.transform.forward = transform.forward;
            Destroy(vfx, 3f);

            character.TakeDamage(attackDamage, elementType);
        }
    }

    public virtual bool FollowUpEffect(AttackArea attackArea, TurnManager turnManager)
    {
        return false;
    }

    #endregion

    #region CustomMethods

    public override void TakeDamage(float damage, ElementType type)
    {
        int hitResult = Random.Range(0, 101);

        if(elementWeakAgainst == type)// && hitResult <= defensePercentage)
        {
            base.TakeDamage(damage + 1, type);
        }
        else if(elementStrongAgainst == type)
        {
            base.TakeDamage(damage - 1, type);
        }
        else
        {
            base.TakeDamage(damage, type);
        }
    }

    public void DestroySelfEnemy(TurnManager turnManager)
    {
        turnManager.enemyList.Remove(this);
        characterTile.tileOccupied = false;
        characterTile.characterOnTile = null;
        Destroy(gameObject);
    }

    public virtual void ActionCleanup()
    {
        
    }

    public virtual Character LikelyTarget()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        Character closestCharacter = null;
        float distance = 1000f;
        foreach (Character character in turnManager.characterList)
        {
            float newDistance = Vector3.Distance(transform.position, character.transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                closestCharacter = character;
            }
        }
        return closestCharacter;
    }

    #endregion
}
