using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    #region Variables

    [SerializeField] private TurnManager turnManager;

    Dictionary<int, MoveAttackCombo> combinations = new Dictionary<int, MoveAttackCombo>();

    List<int> inThreatKeys = new List<int>();

    private int uniqueIds = 0;
    private int lowestValue = 100;

    private Vector3 nullVector = Vector3.back;

    public bool DecisionMakingFinished { get; private set; }

    #endregion

    #region UnityMethods

    void Start()
    {
        Debug.Assert(turnManager != null, "EnemyBrain doesn't have the TurnManager set");
        nullVector *= 20;
        DecisionMakingFinished = true;
    }

    #endregion

    #region CustomMethods

    public void CalculateEnemyTurns()
    {
        if(DecisionMakingFinished)
        {
            DecisionMakingFinished = false;
            uniqueIds = 0;
            StartCoroutine(EnemiesUpdate());
        }
    }

    private IEnumerator EnemiesUpdate()
    {
        foreach(Enemy_Base enemy_base in turnManager.enemyList)
        {
            yield return new WaitForSeconds(0.01f);
            AttackArea enemyAttackArea = AttackArea.SpawnAttackArea(enemy_base.basicAttack);

            //Calculates the tiles the character can move onto
            turnManager.pathfinder.ResetPathFinder();
            turnManager.pathfinder.FindPaths(enemy_base);
            foreach(Tile tile in turnManager.pathfinder.frontier)
            {
                int valueOfCombination = enemy_base.CalculateMovementValue(tile, enemy_base, turnManager);

                //Runs through all the Tiles adjacent to the current Movement tile
                foreach(Tile adjacentTile in turnManager.pathfinder.FindAdjacentTiles(tile, true))
                {
                    yield return new WaitForSeconds(0.01f);

                    //Positions and Rotates the AttackArea object
                    enemyAttackArea.transform.position = adjacentTile.transform.position;
                    Vector3 rotation = DetermineAttackAreaRotation(adjacentTile, tile);
                    enemyAttackArea.transform.eulerAngles = rotation;

                    //Calculates the value of Attacking in that direction
                    enemyAttackArea.DetectArea(false);
                    valueOfCombination += enemy_base.CalculteAttackValue(enemyAttackArea);

                    //If the attack won't hit any players the rotation value is set to the nullvector to mark it as non attacking
                    if(enemyAttackArea.CharactersHit(TurnEnums.CharacterType.Player).Count == 0)
                    {
                        rotation = nullVector;
                    }

                    if(combinations.Count < 5)
                    {
                        AddCombination(tile, adjacentTile, rotation, valueOfCombination);
                    }
                    else
                    {
                        //Only attempts to replace another combination if one of the previous ones is worse or equal in value
                        if(valueOfCombination >= lowestValue)
                        {
                            //Finds all the combinations of the lowest value
                            foreach(KeyValuePair<int,MoveAttackCombo> comboValues in combinations)
                            {
                                if(comboValues.Value.value == lowestValue)
                                {
                                    inThreatKeys.Add(comboValues.Key);
                                }
                            }

                            if(inThreatKeys.Count == 0)
                            {
                                continue;
                            }

                            //Determines how the combination dispute should be settled
                            if(valueOfCombination == lowestValue)
                            {
                                SettleValueConflict(tile, adjacentTile, rotation, valueOfCombination, true);
                            }
                            else if(inThreatKeys.Count == 1)
                            {
                                RemoveCombination(inThreatKeys[0]);
                                AddCombination(tile, adjacentTile, rotation, valueOfCombination);
                            }
                            else
                            {
                                SettleValueConflict(tile, adjacentTile, rotation, valueOfCombination, false);
                            }

                            inThreatKeys.Clear();
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.01f);

            //Picks the final choice to execute
            IfAttacksClearNon();
            int combinationChoice = PickCombination();

            //Executes the Movement portion than the Action portion on movement completion
            if(combinations.TryGetValue(combinationChoice, out MoveAttackCombo finalCombo))
            {
                Tile[] path = turnManager.pathfinder.PathBetween(finalCombo.movementTile, enemy_base.characterTile);
                enemy_base.Move(path);

                //Waits for the enemy to finish its movement
                while(enemy_base.moving)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                //Checks if it should perform an Attack action
                if (finalCombo.attackRotation != nullVector)
                {
                    enemyAttackArea.transform.position = finalCombo.attackingTile.transform.position;
                    enemyAttackArea.transform.eulerAngles = finalCombo.attackRotation;

                    enemy_base.ExecuteAttack(enemyAttackArea);
                }
            }
            else
            {
                Debug.Log($"!!-- {enemy_base.name} FAILED TO FIND FINAL COMBINATION KEY --!!");
            }

            //Resets Variables
            enemyAttackArea.DestroySelf();
            combinations.Clear();
            lowestValue = 100;
        }

        turnManager.pathfinder.ResetPathFinder();
        DecisionMakingFinished = true;
    }

    //Determines what rotation should be applied to the AttackArea
    private Vector3 DetermineAttackAreaRotation(Tile targetTile, Tile movementTile)
    {
        Transform movementTransform = movementTile.transform;
        Transform tileTransform = targetTile.transform;

        float rotation = movementTransform.eulerAngles.y;

        float angle = Vector3.Angle(movementTransform.forward, (tileTransform.position - movementTransform.position));

        if (Vector3.Distance(tileTransform.position, movementTransform.position + (movementTransform.right * 6)) <
            Vector3.Distance(tileTransform.position, movementTransform.position + (-movementTransform.right) * 6))
        {
            rotation += angle;
        }
        else
        {
            rotation -= angle;
        }

        return new Vector3(0, rotation, 0);
    }

    //Adds a combination into the combination dictionary
    private void AddCombination(Tile moveTile, Tile attackTile, Vector3 attackRotation, int combinationValue)
    {
        combinations.Add(uniqueIds, new MoveAttackCombo(combinationValue, moveTile, attackTile, attackRotation));

        uniqueIds++;
        CalculateLowestValue();
    }

    //Removes a combination from the combination dictionary
    private void RemoveCombination(int key)
    {
        combinations.Remove(key);
    }

    //Determines what the lowest value in the combination dictionary is
    private void CalculateLowestValue()
    {
        lowestValue = 100;
        foreach (MoveAttackCombo combo in combinations.Values)
        {
            if (combo.value < lowestValue)
            {
                lowestValue = combo.value;
            }
        }
    }

    //Decides what combinations to keep when attempting to add a new one
    private void SettleValueConflict(Tile moveTile, Tile attackTile, Vector3 attackRotation, int combinationValue, bool newAtThreat)
    {
        int choice = 0;
        if (newAtThreat)
        {
            choice = Random.Range(0, 5);
            if (choice > 1)
            {
                return;
            }
        }

        choice = Random.Range(0, inThreatKeys.Count);

        RemoveCombination(inThreatKeys[choice]);

        AddCombination(moveTile, attackTile, attackRotation, combinationValue);
    }

    //Decides what one of the final combinations will be executed
    private int PickCombination()
    {
        int totalValueOfCombinations = 0;
        int currentTotal = 0;
        int choice;

        //Totals up the values
        foreach (MoveAttackCombo combo in combinations.Values)
        {
            if(combo.value < 1)
            {
                combo.value = 1;
            }
            totalValueOfCombinations += combo.value;
        }

        choice = Random.Range(0, totalValueOfCombinations + 1);

        foreach (KeyValuePair<int, MoveAttackCombo> combo in combinations)
        {
            currentTotal += combo.Value.value;
            if (currentTotal >= choice)
            {
                choice = combo.Key;
                break;
            }
        }

        return choice;
    }

    //If there is a final combination that results in an Attack being performed all non attacking combinations are discarded
    private void IfAttacksClearNon()
    {
        bool attackingComboFound = false;
        List<int> keysToRemove = new List<int>();

        foreach(KeyValuePair<int, MoveAttackCombo> combo in combinations)
        {
            if(combo.Value.attackRotation == nullVector)
            {
                keysToRemove.Add(combo.Key);
            }
            else
            {
                attackingComboFound = true;
            }
        }

        if (attackingComboFound)
        {
            foreach (int key in keysToRemove)
            {
                RemoveCombination(key);
            }
        }
    }

    #endregion
}
