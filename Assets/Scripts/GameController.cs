using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public enum GameState { FreeRoam, Dialog, Battle }


public class GameControler : MonoBehaviour
{
    [SerializeField] PlayerControler playerControler;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    public IGameState currentState;

    //GameState state;

    private void Start()
    {
        this.setGameState(new FreeWalkS());
        playerControler.OnEncounter += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        DialogManager.Instance.OnShowDialog += () =>
        {
            this.setGameState(new DialogS());
        };
        DialogManager.Instance.OnHideDialog += () =>
        {
            if (currentState.getState() == "Dialog")
                this.setGameState(new FreeWalkS());
        };
    }

    void StartBattle()
    {
        this.setGameState(new BattleS());
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        battleSystem.StartBattle();
    }

    void EndBattle(bool won)
    {
        this.setGameState(new FreeWalkS());
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(currentState.getState() == "FreeWalk")
        {
            playerControler.HandleUpdate();

        }
        else if(currentState.getState() == "Dialog") 
        {
            DialogManager.Instance.HandleUpdate();

        }
        else if(currentState.getState() == "Battle")
        {
            battleSystem.HandleUpdate();
        }
    }

    public void setGameState(IGameState state)
    {
        this.currentState = state;
    }
}
