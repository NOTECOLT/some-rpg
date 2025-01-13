using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for overworld objects or NPCs that can be interacted with by pressing spacebar
/// </summary>
public interface IInteractable {
    /// <summary>
    /// Runs when the player presses spacebar while facing the object
    /// </summary>
    void OnInteract();
}
