using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleState : GenericState<GameStateMachine.StateKey> {
    private GameStateMachine _context;
    private BattleStateMachine _battleContext;

    private bool _isBattleDone;
    public GameBattleState(GameStateMachine context, GameStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _isBattleDone = false;
        SceneLoader.Instance.LoadEncounter(_context.encounters); 
        _context.encounters = new List<EnemyType>();
    }

    public override GameStateMachine.StateKey GetNextState() {
        if (_isBattleDone) {
            return GameStateMachine.StateKey.OVERWORLD_STATE;
        } else {
            return Key;
        }
    }

    public override void UpdateState() { }
    public override void ExitState() { 
        SceneLoader.Instance.LoadOverworld();
    }

    /// <summary>
    /// Handles all end battle logic. Mainly experience & levelup
    /// </summary>
    public void EndBattle() {
        for (int i = 0; i < PlayerDataManager.Instance.Data.PartyStats.Count; i++) {
            // Transfer all data from battle unit to player data
            PlayerData.MemberStats member = PlayerDataManager.Instance.Data.PartyStats[i];
            member.CurrentStats = (EntityStats)_battleContext.playerBattleUnits[i].CurrentStats.Clone();

            member.Weapon.AddExperience(5);
        }
        _isBattleDone = true;
    }

    public void SetBattleContext(BattleStateMachine battleContext) {
        _battleContext = battleContext;
    }
}
