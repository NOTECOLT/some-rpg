using System;
using UnityEngine;

[Serializable]
public enum BattleState {
    PLAYER_TURN,
    PLAYER_SELECT_ATTACK,
    ENEMY_TURN,
    ACTION_SEQUENCE // Performs all chosen actions in a turn
}
