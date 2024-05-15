using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    #region Struct

    private struct MoveAttackCombo
    {
        public int value;
        public Tile movementTile;
        public Tile attackTile;
    }

    #endregion

    #region Variables

    [SerializeField] private TurnManager turnManager;

    private List<Tile> currentMovementFrontier;
    private List<Tile> currentAttackFrontier;
    private Tile previousTile;

    private Dictionary<Tile, Tile> movementDictionary = new Dictionary<Tile, Tile>();

    private Dictionary<int, MoveAttackCombo> moveAttackCombos = new Dictionary<int, MoveAttackCombo>();
    List<int> inThreatKeys = new List<int>();
    private int lowestValue = -100;
    private int uniqueIds = 0;

    public bool DecisionMakingFinished { get; private set; }

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
        foreach (Character enemy in turnManager.enemyList)
        {
            yield return new WaitForSeconds(0.1f);

            turnManager.pathfinder.type = TurnEnums.PathfinderTypes.Movement;
            turnManager.pathfinder.ResetPathFinder();
            turnManager.pathfinder.FindPaths(enemy);
            currentMovementFrontier = turnManager.pathfinder.frontier;

            foreach (Tile tile in currentMovementFrontier)
            {
                movementDictionary.Add(tile, tile.parentTile);
            }

            turnManager.pathfinder.type = TurnEnums.PathfinderTypes.BasicAttack;
            enemy.characterTile.tileOccupied = false;
            enemy.characterTile.characterOnTile = null;
            previousTile = enemy.characterTile;

            
            foreach (Tile tile in movementDictionary.Keys)
            {
                yield return new WaitForSeconds(0.01f);

                //Calculates the value of the combination movement wise
                int comboValue = 100;
                foreach (Character character in turnManager.characterList)
                {
                    int distance = (int)Vector3.Distance(enemy.transform.position, character.transform.position);
                    if (comboValue > distance)
                    {
                        comboValue = distance;
                    }
                }
                comboValue *= -1;

                yield return new WaitForSeconds(0.1f);

                //Calculates what tiles can be attacked from the current movement tile
                enemy.characterTile = tile;

                turnManager.pathfinder.ResetPathFinder();
                turnManager.pathfinder.FindPaths(enemy);
                currentAttackFrontier = turnManager.pathfinder.frontier;

                //Calculates the value for each attack in combination with the movement value
                foreach (Tile attackTile in currentAttackFrontier)
                {
                    //Calculates the Attack value of the combination
                    if (attackTile.tileOccupied && attackTile.characterOnTile.characterType == TurnEnums.CharacterType.Player)
                    {
                        comboValue += 3;
                    }

                    //If we haven't reached 5 combos yet just add the value
                    if (moveAttackCombos.Count < 5)
                    {
                        moveAttackCombos.Add(uniqueIds, GenerateMoveAttackCombo(comboValue, tile, attackTile));
                        uniqueIds++;

                    }
                    /*//Else calculate what combo to replace
                    else
                    {
                        //Checks what the lowest value in the dictionary is
                        foreach (KeyValuePair<KeyValuePair<Tile, Tile>, int> combo in movementAttackCombinations)
                        {
                            if (combo.Value < lowestValue)
                            {
                                lowestValue = combo.Value;
                            }
                        }

                        //Pulls the lowest values into a list
                        foreach (KeyValuePair<KeyValuePair<Tile, Tile>, int> combo in movementAttackCombinations)
                        {
                            if (combo.Value == lowestValue && combo.Value <= comboValue)
                            {
                                inThreatKeys.Add(combo.Key);
                            }
                        }

                        //Checks if we have any keys at threat
                        if (inThreatKeys.Count > 0)
                        {
                            //If the new combo is also at threat the conflict is settled with it included
                            if (comboValue == lowestValue)
                            {
                                SettleValueConflict(new KeyValuePair<Tile, Tile>(tile, attackTile), comboValue, true);
                            }
                            //If the new combo isn't at threat and there was only one new lowest value the lowest is discarded
                            else if (inThreatKeys.Count == 1)
                            {
                                movementAttackCombinations.Remove(inThreatKeys[0]);
                                movementAttackCombinations.Add(new KeyValuePair<Tile, Tile>(tile, attackTile), comboValue);
                            }
                            //If the combo isn't at threat but their are multiple lowest the conflict is settled
                            else
                            {
                                SettleValueConflict(new KeyValuePair<Tile, Tile>(tile, attackTile), comboValue, false);
                            }
                        }
                    }

                    inThreatKeys.Clear();
                    lowestValue = -100;*/
                }

                /*yield return new WaitForSeconds(0.1f);

                int totalValue = 0;
                int runningTotal = 0;
                int choice;
                KeyValuePair<Tile, Tile> finalChoice = new KeyValuePair<Tile, Tile>(null, null);

                //Calculates the total value of pairs in the dictionary
                foreach (KeyValuePair<KeyValuePair<Tile, Tile>, int> combo in movementAttackCombinations)
                {
                    totalValue += combo.Value;
                }

                choice = Random.Range(0, totalValue);

                //Checks what key will be selected
                foreach (KeyValuePair<KeyValuePair<Tile, Tile>, int> combo in movementAttackCombinations)
                {
                    runningTotal += combo.Value;
                    if (runningTotal >= choice)
                    {
                        finalChoice = combo.Key;
                        break;
                    }
                }

                //Moves the character and attacks based on the tiles included in the final selection
                if (finalChoice.Key != null)
                {
                    Move(finalChoice.Key, enemy);
                    AttackTile(finalChoice.Key, enemy);
                }*/

                //movementAttackCombinations.Clear();
            }
            movementDictionary.Clear();
        }

        DecisionMakingFinished = true;
    }

    //Determines the path and moves the enemy
    private void Move(Tile destination, Character enemy)
    {
        List<Tile> tiles = new List<Tile>();
        Tile current = destination;

        while (current != previousTile)
        {
            tiles.Add(current);

            if (movementDictionary.TryGetValue(current, out Tile parent))
            {
                current = current.parentTile;
            }
            else
            {
                break;
            }
        }

        tiles.Add(previousTile);
        tiles.Reverse();

        Tile[] path = tiles.ToArray();

        enemy.Move(path);
    }

    //Attacks the tile the enemy selected
    private void AttackTile(Tile tile, Character enemy)
    {
        Debug.Log("ENEMY IS ATTACKING TILE: " + tile.name);
    }

    //Settles conflicts while deciding what MovementAttack combinations to keep
    private void SettleValueConflict(KeyValuePair<Tile, Tile> newCombo, int newValue, bool newInThreat)
    {
        int choice;
        if (newInThreat)
        {
            choice = Random.Range(0, 4);
            if (choice > 1)
            {
                return;
            }
        }

        choice = Random.Range(0, inThreatKeys.Count);
        //movementAttackCombinations.Remove(inThreatKeys[choice]);
        //movementAttackCombinations.Add(newCombo, newValue);
    }

    private MoveAttackCombo GenerateMoveAttackCombo(int value, Tile moveTile, Tile attackTile)
    {
        MoveAttackCombo newCombo = new MoveAttackCombo();

        newCombo.value = value;
        newCombo.attackTile = attackTile;
        newCombo.movementTile = moveTile;

        return newCombo;
    }

    #endregion
}
