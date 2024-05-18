using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgenitorBrain : MonoBehaviour
{
    /*#region Variables

    [SerializeField] private TurnManager turnManager;

    List<Tile> currentFrontier = new List<Tile>();
    Dictionary<Tile, Tile> movementLibrary = new Dictionary<Tile, Tile>();

    //Dictionaries used to store information about the MoveAttack combinations
    Dictionary<int, Tile> chosenMovements = new Dictionary<int, Tile>();
    Dictionary<int, Tile> chosenAttacks = new Dictionary<int, Tile>();
    Dictionary<int, int> chosenValues = new Dictionary<int, int>();

    List<int> inThreatKeys = new List<int>();

    private int uniqueIds = 0;
    private int lowestValue = 100;

    //Enemies orginal Tile
    Tile originTile;

    public bool DecisionMakingFinished { get; private set; }

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

    //Calculates and Moves all the Enemies
    private IEnumerator EnemiesUpdate()
    {
        yield return new WaitForSeconds(0.01f);
        foreach(Character enemy in turnManager.enemyList)
        {
            yield return new WaitForSeconds(0.01f);
            ProduceMovementLibrary(enemy);

            //Stores the original tile of the Enemy for later
            originTile = enemy.characterTile;

            //Runs through the possible Attacks on all movement Tiles
            foreach(Tile moveTile in movementLibrary.Keys)
            {
                yield return new WaitForSeconds(0.01f);
                int valueOfMovement = CalculateMovementValue(enemy, moveTile);
                currentFrontier = DetermineAttackFrontier(enemy, moveTile);

                //Runs through the Attack tiles
                foreach(Tile attackTile in currentFrontier)
                {
                    int valueOfCombination = CalculateAttackValue(enemy, attackTile, valueOfMovement);

                    if(AlreadyAttemptingMove(moveTile))
                    {
                        continue;
                    }

                    //Stores the first five combinations
                    if (chosenAttacks.Count < 5)
                    {
                        AddCombination(moveTile, attackTile, valueOfCombination);
                    }
                    else
                    {
                        if (valueOfCombination >= lowestValue)
                        {
                            continue;
                        }

                        //Grabs all the keys for combinations at threat of being removed
                        foreach (KeyValuePair<int,int> combinationValue in chosenValues)
                        {
                            if(combinationValue.Value == lowestValue)
                            {
                                inThreatKeys.Add(combinationValue.Key);
                            }
                        }

                        if(inThreatKeys.Count == 0)
                        {
                            continue;
                        }

                        //Settles conflicts between combination values
                        if(valueOfCombination == lowestValue)
                        {
                            SettleValueConflict(moveTile, attackTile, valueOfCombination, true);
                        }
                        else if(inThreatKeys.Count == 1)
                        {
                            RemoveCombination(inThreatKeys[0]);
                            AddCombination(moveTile, attackTile, valueOfCombination);
                        }
                        else
                        {
                            SettleValueConflict(moveTile, attackTile, valueOfCombination, false);
                        }

                        inThreatKeys.Clear();
                    }
                }
            }

            yield return new WaitForSeconds(0.01f);

            //Clears out any non attackers if needed and picks the combination to execute
            ClearIfAnyAttacks();
            int combinationChoice = PickCombination();

            //Performs the AttackAction of the combination
            if(chosenAttacks.TryGetValue(combinationChoice, out Tile tileToAttack))
            {
                if(tileToAttack != null)
                {
                    Debug.Log("--ATTACKING TILE--: " + tileToAttack.name);
                }
            }

            //Performs the MovementAction
            if(chosenMovements.TryGetValue(combinationChoice, out Tile tileToMove))
            {
                MoveCharacter(enemy, tileToMove);
            }

            //Resets Variables
            chosenValues.Clear();
            chosenAttacks.Clear();
            chosenMovements.Clear();
            currentFrontier.Clear();
            lowestValue = 100;
        }

        //Resets Variables
        turnManager.pathfinder.ResetPathFinder();
        movementLibrary.Clear();
        DecisionMakingFinished = true;
    }*/

    /*
     * Stores the Movement Frontier of the Enemy into a Dictionary for later reference
     */
   /* private void ProduceMovementLibrary(Character enemy)
    {
        movementLibrary.Clear();

        //Resets the Pathfinder for calculating Enemy movement
        turnManager.pathfinder.type = TurnEnums.PathfinderTypes.EnemyMovement;
        turnManager.pathfinder.ResetPathFinder();

        //Uses the Pathfinder to determine where the Enemy can move
        turnManager.pathfinder.FindPaths(enemy);
        currentFrontier = turnManager.pathfinder.frontier;

        //Stores the Tiles and their Parent into a Movement Library for later use
        foreach (Tile tile in currentFrontier)
        {
            movementLibrary.Add(tile, tile.parentTile);
        }
    }*/

    /*
     * Calculates how valuable a tile is to Move to based on enviromental factors
     */
   /* private int CalculateMovementValue(Character enemy, Tile moveTile)
    {
        int valueOfMovement = -100;
        foreach (Character character in turnManager.characterList)
        {
            int distanceTile = (int)Vector3.Distance(moveTile.transform.position, character.transform.position);
            int distanceEnemy = (int)Vector3.Distance(enemy.transform.position, character.transform.position);
            int tileValue = distanceEnemy - distanceTile;

            if (valueOfMovement < tileValue)
            {
                valueOfMovement = tileValue;
            }
        }
        return valueOfMovement;
    }*/

    /*
     * Determines what tiles can be attacked based on a Movement Tile
     */
   /* private List<Tile> DetermineAttackFrontier(Character enemy, Tile moveTile)
    {
        turnManager.pathfinder.type = TurnEnums.PathfinderTypes.EnemyBasicAttack;

        enemy.characterTile = moveTile;

        turnManager.pathfinder.ResetPathFinder();
        turnManager.pathfinder.FindPaths(enemy);

        enemy.characterTile = originTile;

        return turnManager.pathfinder.frontier;
    }*/

    /*
     * Calculates how valuable a combination of moving and attacking is
     */
    /*private int CalculateAttackValue(Character enemy, Tile attackTile, int valueOfMovement)
    {
        //Checks the value of the Attack tile
        int valueOfCombination = valueOfMovement;
        if (attackTile.tileOccupied && attackTile.characterOnTile.characterType == TurnEnums.CharacterType.Player)
        {
            valueOfCombination += 3;
        }
        return valueOfCombination;
    }*/

    /*
     * Checks if a combination with the given Movement Tile is already in the combinations
     */
    /*private bool AlreadyAttemptingMove(Tile moveTile)
    {
        foreach (KeyValuePair<int, Tile> movementTiles in chosenMovements)
        {
            if (moveTile == movementTiles.Value)
            {
                return true;
            }
        }

        return false;
    }*/

    /*
     * If any combinations attack it removes any that don't
     */
    /*private void ClearIfAnyAttacks()
    {
        bool attackingComboFound = false;
        List<int> keysToRemove = new List<int>();

        foreach (KeyValuePair<int, Tile> combinationValue in chosenAttacks)
        {
            if (combinationValue.Value == null)
            {
                keysToRemove.Add(combinationValue.Key);
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
    }*/

    /*
     * Makes the final combination selection
     */
    /*private int PickCombination()
    {
        int totalValueOfCombinations = 0;
        int currentTotal = 0;
        int choice;

        //Totals up the values
        foreach (int combinationValue in chosenValues.Values)
        {
            totalValueOfCombinations += combinationValue;
        }

        //Selects a combination use based on weighted random
        choice = Random.Range(0, totalValueOfCombinations + 1);

        foreach (KeyValuePair<int, int> combinationValue in chosenValues)
        {
            currentTotal += combinationValue.Value;
            if (currentTotal >= choice)
            {
                choice = combinationValue.Key;
                break;
            }
        }

        return choice;
    }*/

    /*
     * Moves the character to the selected moveTile
     */
   /* private void MoveCharacter(Character enemy, Tile moveTile)
    {
        turnManager.pathfinder.type = TurnEnums.PathfinderTypes.EnemyMovement;
        turnManager.pathfinder.ResetPathFinder();
        turnManager.pathfinder.FindPaths(enemy);

        Tile[] path = PathMaker(moveTile, enemy.characterTile);

        enemy.Move(path);
    }*/

    /*
     * Creates the path for the Enemy to follow to its target 
     */
    /*private Tile[] PathMaker(Tile targetTile, Tile origin)
    {
        List<Tile> path = new List<Tile>();
        Tile current = targetTile;

        while (current != origin)
        {
            path.Add(current);
            if (movementLibrary.TryGetValue(current, out Tile parent))
            {
                if (parent == origin)
                {
                    break;
                }
                current = parent;
            }
            else
            {
                break;
            }
        }

        path.Add(origin);
        path.Reverse();
        return path.ToArray();
    }*/

    /*
     * Adds a combination into the dictionaries
     */
    /*private void AddCombination(Tile moveTile, Tile attackTile, int combinationValue)
    {
        chosenMovements.Add(uniqueIds, moveTile);
        chosenValues.Add(uniqueIds, combinationValue);

        if (attackTile.tileOccupied)
        {
            chosenAttacks.Add(uniqueIds, attackTile);
        }
        else
        {
            chosenAttacks.Add(uniqueIds, null);
        }

        uniqueIds++;
        CalculateLowestValue();
    }*/

    /*
     * Removes a combination from the dictionaries
     */
    /*private void RemoveCombination(int key)
    {
        chosenMovements.Remove(key);
        chosenAttacks.Remove(key);
        chosenValues.Remove(key);
    }*/

    /*
     * Confirms what the lowest current value in the combinations is
     */
    /*private void CalculateLowestValue()
    {
        foreach(int value in chosenValues.Values)
        {
            if(value < lowestValue)
            {
                lowestValue = value;
            }
        }
    }*/

    /*
     * Settles conflicts between values when adding new combinations
     */
    /*private void SettleValueConflict(Tile moveTile, Tile attackTile, int combinationValue, bool newAtThreat)
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

        choice = Random.Range(0, inThreatKeys.Count);

        RemoveCombination(inThreatKeys[choice]);

        AddCombination(moveTile, attackTile, combinationValue);
    }*/

    //#endregion
}
