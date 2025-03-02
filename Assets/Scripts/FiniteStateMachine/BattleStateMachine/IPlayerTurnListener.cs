using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTurnListener {
    /// <summary>
    /// Triggers when an enemy is clicked on during enemy target selection
    /// </summary>
    /// <param name="targetEnemy">Passed Target Id of the clicked enemy</param>
    void OnEnemyClicked(Enemy targetEnemy);

    /// <summary>
    /// Triggers when a player chooses an action.
    /// </summary>
    /// <param name="action">The Selected action (i.e. ATTACK, HEAL, etc.)</param> <summary>
    void OnPlayerSetAction(ActionType action);
}
