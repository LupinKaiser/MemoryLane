using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHud;
    [SerializeField] BattleHUD enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        
        enemyUnit.Setup();
        Debug.Log(enemyUnit.Enemy.HP);
        enemyHud.SetData(enemyUnit);
        playerUnit.Setup();
        playerHud.SetData(playerUnit);


        dialogBox.SetMoveNames(playerUnit.Enemy.Moves);

        yield return dialogBox.TypeDialog($"A {enemyUnit.Enemy.Base.Name} appeared.");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
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
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                PlayerMove();
            }
           
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Enemy.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Enemy.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Enemy.Moves[currentMove]);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Enemy.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Enemy.Base.Name} used {move.Base.Name}");

        yield return new WaitForSeconds(1f);

        bool isFainted = enemyUnit.Enemy.TakeDamage(move, playerUnit.Enemy);
        enemyHud.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} Fainted");
            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Enemy.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Enemy.Base.Name} used {move.Base.Name}");

        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Enemy.TakeDamage(move, playerUnit.Enemy);
        playerHud.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Enemy.Base.Name} Fainted");

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            PlayerAction();
        }
    }

    
}

