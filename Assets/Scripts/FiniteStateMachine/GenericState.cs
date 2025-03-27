using System;
using UnityEngine;

/// <summary>
/// Abstract class from which States inherit from
/// </summary>
public abstract class GenericState<StateKey> where StateKey : Enum {
    public StateKey Key { get; protected set; }
    public GenericState(StateKey key) {
        this.Key = key;
    }
    
    /// <summary>
    /// Runs once when a state is entered
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Runs every frame in the Update() method
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Runs once when a state is exited
    /// </summary>
    public abstract void ExitState();

    /// <summary>
    /// Returns the next State to be transitioned into.
    /// Returns the State's own key if the state should not be transitioning.
    /// </summary>
    /// <returns></returns>
    public abstract StateKey GetNextState();
}