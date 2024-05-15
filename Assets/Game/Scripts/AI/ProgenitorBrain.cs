using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgenitorBrain : MonoBehaviour
{
    #region Variables

    [SerializeField] private TurnManager turnManager;
    Tile originTile;

    List<Tile> currentFrontier = new List<Tile>();
    Dictionary<Tile, Tile> movementLibrary = new Dictionary<Tile, Tile>();

    //Triple Entente Solution
    Dictionary<int, Tile> chosenMovements = new Dictionary<int, Tile>();
    Dictionary<int, Tile> chosenAttacks = new Dictionary<int, Tile>();
    Dictionary<int, int> chosenValues = new Dictionary<int, int>();

    List<int> inThreatKeys = new List<int>();

    public bool DecisionMakingFinished { get; private set; }
    private int uniqueIds = 0;
    private int lowestValue = 100;

    #endregion

    #region UnityMethods

    private void Start()
    {
        Debug.Assert(turnManager != null, "EnemyBrain doesn't have the TurnManager set");
    }

    #endregion

    #region CustomMethods

    public void CalculateEnemyTurns()
    {
        DecisionMakingFinished = false;
        uniqueIds = 0;
        StartCoroutine(EnemiesUpdate());
    }

    private IEnumerator EnemiesUpdate()
    {
        yield return new WaitForSeconds(0.01f);

        //Runs through each of the Enemies contained in the TurnManager
        foreach(Character enemy in turnManager.enemyList)
        {
            yield return new WaitForSeconds(0.01f);

            //Resets the Pathfinder for calculating Enemy movement
            turnManager.pathfinder.type = TurnEnums.PathfinderTypes.Movement;
            turnManager.pathfinder.ResetPathFinder();

            //Uses the Pathfinder to determine where the Enemy can move
            turnManager.pathfinder.FindPaths(enemy);
            currentFrontier = turnManager.pathfinder.frontier;

            //Stores the Tiles and their Parent into a Movement Library for later use
            foreach(Tile tile in currentFrontier)
            {
                movementLibrary.Add(tile, tile.parentTile);
            }

            //Takes the Enemy off the tile they are on and holds onto it for later use
            originTile = enemy.characterTile;
            originTile.tileOccupied = false;
            originTile.characterOnTile = null;

            //Runs through the possible Attacks on all movement Tiles
            turnManager.pathfinder.type = TurnEnums.PathfinderTypes.EnemyBasicAttack;
            foreach(Tile moveTile in movementLibrary.Keys)
            {
                yield return new WaitForSeconds(0.01f);

                //Calculates the value of a Movement Tile based on its distance to any Enemy
                int valueOfMovement = 100;
                foreach(Character character in turnManager.characterList)
                {
                    int distance = (int)Vector3.Distance(enemy.transform.position, character.transform.position);
                    if(valueOfMovement > distance)
                    {
                        valueOfMovement = distance;
                    }
                }
                valueOfMovement *= -1;

                //Calculates what tiles the enemy can attack from the current Movement Tile
                enemy.characterTile = moveTile;

                turnManager.pathfinder.ResetPathFinder();
                turnManager.pathfinder.FindPaths(enemy);

                currentFrontier = turnManager.pathfinder.frontier;

                //Runs through the Attack tiles
                foreach(Tile attackTile in currentFrontier)
                {
                    //Checks the value of the Attack tile
                    int valueOfCombination = valueOfMovement;
                    if(attackTile.tileOccupied && attackTile.characterOnTile.characterType == TurnEnums.CharacterType.Player)
                    {
                        valueOfCombination += 3;
                    }

                    //Checks if we are storing any new combinations for later
                    if(chosenAttacks.Count < 5)
                    {
                        if(attackTile.tileOccupied)
                        {
                            AddCombination(moveTile, attackTile, valueOfCombination);
                        }
                        else
                        {
                            AddCombination(moveTile, null, valueOfCombination);
                        }
                    }
                    else
                    {
                        if(attackTile.tileOccupied && valueOfCombination >= lowestValue)
                        {
                            //Grabs all the keys for combinations at threat of being removed
                            foreach(KeyValuePair<int,int> combinationValue in chosenValues)
                            {
                                if(combinationValue.Value == lowestValue)
                                {
                                    inThreatKeys.Add(combinationValue.Value);
                                }
                            }

                            //Settles any conflicts between Values being stored and the new one
                            if(valueOfCombination == lowestValue)
                            {
                                SettleValueConflict(moveTile, attackTile, valueOfCombination, true);
                            }
                            else if(inThreatKeys.Count == 1)
                            {
                                RemoveCombination(inThreatKeys[0]);
                                AddCombination(moveTile, attackTile, inThreatKeys[0]);
                            }
                            else
                            {
                                SettleValueConflict(moveTile, attackTile, valueOfCombination, false);
                            }
                        }

                        inThreatKeys.Clear();
                    }
                }
            }

            yield return new WaitForSeconds(0.01f);

            int totalValueOfCombinations = 0;
            int currentTotal = 0;
            int choice = -100;

            //Totals up the values
            foreach(int combinationValue in chosenValues.Values)
            {
                totalValueOfCombinations += combinationValue;
            }

            //Selects a combination use based on weighted random
            choice = Random.Range(0, totalValueOfCombinations + 1);

            foreach(KeyValuePair<int, int> combinationValue in chosenValues)
            {
                currentTotal += combinationValue.Value;
                if(currentTotal >= choice)
                {
                    choice = combinationValue.Key;
                    break;
                }
            }

            //Restablishes the Enemy on their Tile
            enemy.characterTile = originTile;
            originTile.tileOccupied = true;
            originTile.characterOnTile = enemy;

            //Checks if any valid combinations were picked
            if (choice != -100)
            {
                //Confirms an AttackTile exists, only executes an Attack if not null
                if(chosenAttacks.TryGetValue(choice, out Tile attackTile))
                {
                    if(attackTile != null)
                    {
                        Debug.Log("--ATTACKING TILE--: " + attackTile.name);
                    }
                    else
                    {
                        Debug.Log("**DIDN'T FIND A TILE WITH A PLAYER ON IT TO ATTACK**");
                    }
                }
            }

            //Resets Variables
            chosenValues.Clear();
            chosenAttacks.Clear();
            chosenMovements.Clear();
            lowestValue = 100;
        }

        //Resets Variables
        turnManager.pathfinder.ResetPathFinder();
        movementLibrary.Clear();
        DecisionMakingFinished = true;
    }

    //Adds a combination into the dictionaries
    private void AddCombination(Tile moveTile, Tile attackTile, int combinationValue)
    {
        chosenMovements.Add(uniqueIds, moveTile);
        chosenAttacks.Add(uniqueIds, attackTile);
        chosenValues.Add(uniqueIds, combinationValue);
        uniqueIds++;

        CalculateLowestValue();
    }

    //Removes a combination from the dictionaries
    private void RemoveCombination(int key)
    {
        chosenMovements.Remove(key);
        chosenAttacks.Remove(key);
        chosenValues.Remove(key);
    }

    //Confirms what the lowest current value in the combinations is
    private void CalculateLowestValue()
    {
        foreach(int value in chosenValues.Values)
        {
            if(value < lowestValue)
            {
                lowestValue = value;
            }
        }
    }

    //Settles conflicts between values when adding new combinations
    private void SettleValueConflict(Tile moveTile, Tile attackTile, int combinationValue, bool newAtThreat)
    {
        int choice = 0;
        if(newAtThreat)
        {
            choice = Random.Range(0, 5);
            if(choice > 1)
            {
                return;
            }
        }

        choice = Random.Range(0, inThreatKeys.Count + 1);
        RemoveCombination(choice);
        AddCombination(moveTile, attackTile, combinationValue);
    }

    #endregion
}
