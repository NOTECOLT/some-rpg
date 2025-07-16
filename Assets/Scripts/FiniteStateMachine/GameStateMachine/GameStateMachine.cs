using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : FiniteStateMachine<GameStateMachine.StateKey> {
    // TODO: ADD TRANSITION INTO GAME BATTLE STATE
    // TODO: ADD PROPER GAME RELOAD
    public enum StateKey {
        LOAD_GAME_STATE,
        OVERWORLD_STATE,
        BATTLE_STATE
    }
    public static GameStateMachine Instance;
    private void Awake()  { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { 
            Destroy(this.gameObject); 
        } else { 
            Instance = this; 
        } 

        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private List<Weapon> _weaponsList;

    /// <summary>
    /// Holds references to every item in the game so that external scripts and savedata can use it
    /// ? Not my favorite solution but i'll keep it like this for now 
    /// </summary>
    public Dictionary<string, Weapon> Weapons = null;

    /// <summary>
    /// Holds the list of encounters when a player runs into a wild enemy. <br></br>
    /// Data held is here is stored upon encounter in the GameOverworldState, <br></br>
    /// Then upon scene transition, data is transferred to Encounters property in SceneLoader object. (to be transferred to BattleStateMachine)
    /// </summary>
    public List<EnemyType> encounters = new List<EnemyType>();

    protected override void Start() {
        if (SceneLoader.Instance is null) {
            Debug.LogError("Scene does not have SceneLoader Component! Battle failed to load.");
            return;
        }

        States = new Dictionary<StateKey, GenericState<StateKey>>(){
            {StateKey.LOAD_GAME_STATE, new GameLoadState(this, _weaponsList, StateKey.LOAD_GAME_STATE)},
            {StateKey.OVERWORLD_STATE, new GameOverworldState(this, StateKey.OVERWORLD_STATE)},
            {StateKey.BATTLE_STATE, new GameBattleState(this, StateKey.BATTLE_STATE)}
        };

        currentState = States[StateKey.LOAD_GAME_STATE];
        base.Start();
    }
    
    /// <summary>
    /// Allows other objects to be able to reference the current state context.
    /// </summary>
    /// <typeparam name="T">Current state the FSM is expected to be in</typeparam>
    /// <returns>Throws an exception if the state is incorrectly taken.</returns>
    public T GetCurrentStateContext<T>() where T : GenericState<StateKey> {
        if (currentState is not T) {
            throw new Exception($"[GameStateMachine] Warning! Current State {currentState} is not of type {typeof(T)}, and is of type {currentState.GetType()} This may cause errors.");
        }
        return (T)currentState;
    }

    /// <summary>
    /// IEnumerator that waits for the current state to change before running an action 
    /// </summary>
    /// <param name="func">Runs once the current state switches.</param>
    /// <param name="maxWaitTime">Max time to wait before bailing to prevent infinite loops</param>
    /// <typeparam name="T">Current State to wait for</typeparam>
    /// <returns></returns>
    public IEnumerator WaitUntilState<T>(Action func, float maxWaitTime = 5.0f) where T : GenericState<StateKey> {
        float timeElapsed = 0;
        while (currentState is not T) {
            yield return new WaitForSeconds(Time.deltaTime);
            timeElapsed += Time.deltaTime;

            // Failsafe condition to ensure that waiting does not create infinite loop.
            if (timeElapsed > maxWaitTime) {
                Debug.LogWarning($"[Game State Machine] Warning! WaitUntilState<{typeof(T)}> took longer than maxWaitTime of {maxWaitTime} seconds. Breaking out of loop.");
                break;
            }
        }

        func();
        yield return null;
    }
}
