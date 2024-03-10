using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    static int templevel;
    static int tempxp;
    static int tempmaxhp;
    static int tempdamage;
    static bool tempslain;

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        if (templevel != 0)
        {
            playerUnit.Creature.Level = templevel;
            if (tempslain == false)
            {
                playerUnit.Creature.HP = tempmaxhp - tempdamage;
            }
            else
            {
                playerUnit.Creature.HP = tempmaxhp;
            }
        }
        if (tempxp != 0 )
        {
            playerUnit.Creature.XP = tempxp;
        }
        playerHud.SetData(playerUnit.Creature);
        enemyHud.SetData(enemyUnit.Creature);
        Debug.Log(playerUnit.Creature.Level);
        dialogBox.SetMoveNames(playerUnit.Creature.Moves);

        yield return dialogBox.TypeDialog($"A {enemyUnit.Creature.Base.Name} appeared!");

        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        Debug.Log(playerUnit.Creature.Level);
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerUnit.Creature.Moves[currentMove];
        move.Energy--;
        yield return dialogBox.TypeDialog($"You performed {move.Base.Name}");

        bool isSlain = enemyUnit.Creature.TakeDamage(move, playerUnit.Creature);
        yield return enemyHud.UpdateHP();
        if (isSlain)
        {
            tempdamage = playerUnit.Creature.MaxHp - playerUnit.Creature.HP;
            yield return dialogBox.TypeDialog($"You have defeated {enemyUnit.Creature.Base.Name}");
            yield return new WaitForSeconds(2f);

            int xpYield = enemyUnit.Creature.Base.XpYield;
            int enemyLevel = enemyUnit.Creature.Level;
            int xpGain = (xpYield * enemyLevel) / 5;
            playerUnit.Creature.XP += xpGain;
            yield return dialogBox.TypeDialog($"You earned {xpGain} experience");
            yield return playerHud.UpdateXp();

            while(playerUnit.Creature.CheckForLevelUp())
            {
                playerHud.SetLevel();
                Debug.Log(playerUnit.Creature.Level);
                yield return dialogBox.TypeDialog("You leveled up");
                yield return playerHud.UpdateXp(true);
            }
            templevel = playerUnit.Creature.Level;
            tempxp = playerUnit.Creature.XP;
            tempmaxhp = playerUnit.Creature.MaxHp;
            tempslain = false;
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Creature.GetMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Creature.Base.Name} performed {move.Base.Name}");

        bool isSlain = playerUnit.Creature.TakeDamage(move, enemyUnit.Creature);
        yield return playerHud.UpdateHP();

        if (isSlain)
        {
            yield return dialogBox.TypeDialog($"You have been defeated by {enemyUnit.Creature.Base.Name}, wait for respawn");
            tempslain = true;
            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            ActionSelection();
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentAction == 0)
            {
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                StartCoroutine(Escape());
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentMove < playerUnit.Creature.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentMove > 0)
                --currentMove;
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Creature.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var move = playerUnit.Creature.Moves[currentMove];
            if (move.Energy == 0) return;
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
    }

    IEnumerator Escape()
    {
        state = BattleState.Busy;
        dialogBox.EnableActionSelector(false);
        yield return dialogBox.TypeDialog($"You ran away");
        yield return new WaitForSeconds(2f);
        OnBattleOver(true);
    }
}