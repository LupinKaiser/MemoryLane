using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

// 1. Added TargetSelection to the state machine
public enum BattleState { Start, PlayerAction, PlayerMove, TargetSelection, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    // 2. Changed single fields into Lists to handle multiple enemies dynamically
    [SerializeField] List<BattleUnit> enemyUnits;

    [SerializeField] BattleHUD playerHud;
    [SerializeField] List<BattleHUD> enemyHuds;

    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentTarget; // 3. Track which enemy is currently being targeted

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.Start;

        // Setup player
        playerUnit.Setup();
        playerHud.SetData(playerUnit);

        // Setup all enemies in the list
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyUnits[i].Setup();
            enemyHuds[i].SetData(enemyUnits[i]);
            // Optional: activate UI game objects if they were hidden
            enemyUnits[i].gameObject.SetActive(true);
            enemyHuds[i].gameObject.SetActive(true);
        }

        dialogBox.SetMoveNames(playerUnit.Enemy.Moves);
        Debug.Log("Battle Starts");

        yield return dialogBox.TypeDialog("Wild enemies appeared.");
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        Debug.Log("Player turn");
        dialogBox.EnableActionSelector(true);
        dialogBox.EnableMoveSelector(false);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // 4. New Phase: Allow player to select which enemy to hit
    void StartTargetSelection()
    {
        state = BattleState.TargetSelection;
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);

        // Find the first enemy that is still alive to default the target selection
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (!enemyUnits[i].Enemy.IsFainted) // Assuming your Enemy class has an IsFainted property
            {
                currentTarget = i;
                break;
            }
        }

        dialogBox.UpdateDialogText($"Select Target: {enemyUnits[currentTarget].Enemy.Base.Name}");
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        var move = playerUnit.Enemy.Moves[currentMove];
        var targetEnemyUnit = enemyUnits[currentTarget];

        Debug.Log($"Player attacked enemy {currentTarget}");
        yield return dialogBox.TypeDialog($"The player used {move.Base.Name}.");
        yield return new WaitForSeconds(1f);

        bool isFainted = targetEnemyUnit.Enemy.TakeDamage(move, playerUnit.Enemy);
        enemyHuds[currentTarget].UpdateHP();
        playerHud.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{targetEnemyUnit.Enemy.Base.Name} was Defeated.");
            targetEnemyUnit.gameObject.SetActive(false); // Hide fainted enemy sprite
            enemyHuds[currentTarget].gameObject.SetActive(false); // Hide fainted enemy UI
            yield return new WaitForSeconds(1f);
        }

        // Check if ALL enemies are defeated
        bool allEnemiesDefeated = true;
        foreach (var unit in enemyUnits)
        {
            if (!unit.Enemy.IsFainted)
            {
                allEnemiesDefeated = false;
                break;
            }
        }

        if (allEnemiesDefeated)
        {
            yield return dialogBox.TypeDialog("All enemies defeated!");
            SavingSystem.i.Save("saveSlot1");
            OnBattleOver(true);
        }
        else
        {
            // Proceed to enemy team's collective turn
            StartCoroutine(EnemyTurn());
        }
    }

    // 5. Run sequence where every active enemy gets to attack one after another
    IEnumerator EnemyTurn()
    {
        state = BattleState.EnemyMove;

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            // Skip enemies that are already dead
            if (enemyUnits[i].Enemy.IsFainted) continue;

            Debug.Log($"Enemy {i} attacked");
            var move = enemyUnits[i].Enemy.GetRandomMove();
            yield return dialogBox.TypeDialog($"{enemyUnits[i].Enemy.Base.Name} used {move.Base.Name}.");
            yield return new WaitForSeconds(1f);

            bool isPlayerFainted = playerUnit.Enemy.TakeDamage(move, enemyUnits[i].Enemy);
            playerHud.UpdateHP();

            if (isPlayerFainted)
            {
                yield return dialogBox.TypeDialog("The player was defeated");
                yield return new WaitForSeconds(2f);
                SavingSystem.i.Load("saveSlot1");
                OnBattleOver(false);
                yield break; // Exit coroutine early because game is over
            }
        }

        // If player survived all enemy attacks, return to Player turn
        PlayerAction();
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.TargetSelection)
        {
            HandleTargetSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1) ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0) --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Enemy.Moves.Count - 1) ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0) --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Enemy.Moves.Count - 2) currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1) currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Enemy.Moves[currentMove]);

        //&& (currentMove == 0 || currentMove == 2)

        if (Input.GetKeyDown(KeyCode.Z))
        {
                StartTargetSelection();
        }
       
    }

    // 6. Handle input to flip between valid enemy targets
    void HandleTargetSelection()
    {
        int totalEnemies = enemyUnits.Count;

        


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            do
            {
                currentTarget = (currentTarget - 1 + totalEnemies) % totalEnemies;
            } while (enemyUnits[currentTarget].Enemy.IsFainted); // Skip dead enemies
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            do
            {
                currentTarget = (currentTarget + 1) % totalEnemies;
            } while (enemyUnits[currentTarget].Enemy.IsFainted); // Skip dead enemies
        }

        dialogBox.UpdateDialogText($"Select Target: {enemyUnits[currentTarget].Enemy.Base.Name}");

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X)) // Press 'X' to go back to Move Selector
        {
            PlayerMove();
        }
    }
}