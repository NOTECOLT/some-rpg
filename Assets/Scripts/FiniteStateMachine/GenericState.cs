using System;
using UnityEngine;

/// <summary>
/// Abstract class from which States inherit from
/// </summary>
public abstract class GenericState {

    /// <summary>
    /// Runs once when a state is entered
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Runs every frame in the Update() method
    /// </summary>
    public abstract void UpdateState();
}