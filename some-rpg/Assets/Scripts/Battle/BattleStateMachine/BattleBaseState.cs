using System;
using UnityEngine;

/// <summary>
/// Abstract class from which BattleStates inherit from
/// </summary>
public abstract class BattleBaseState {
    /// <summary>
    /// Runs once when a state is entered
    /// </summary>
    public abstract void EnterState(BattleStateMachine battle);

    /// <summary>
    /// Runs every frame in the Update() method
    /// </summary>
    public abstract void UpdateState(BattleStateMachine battle);

    /// <summary>
    /// Runs once a state is exited from. Runs before the EnterState() of the next state
    /// </summary>
    public abstract void ExitState(BattleStateMachine battle);
}
