using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndState : GenericState<BattleStateMachine.StateKey> {
    private BattleStateMachine _context;

    public BattleEndState(BattleStateMachine context, BattleStateMachine.StateKey key) : base(key) {
        _context = context;
    }

    public override void EnterState() {
        _context.StartCoroutine(DoEndBattleSequence());        
    }

    public override void ExitState() {
    }

    private IEnumerator DoEndBattleSequence() {
        _context.endBattleScreen.SetActive(true);

        for (int i = 0; i < _context.playerBattleUnits.Count; i++) {
            _context.endBattlePlayerInfoPanels[i].Instantiate(_context.playerBattleUnits[i]);
            _context.endBattlePlayerInfoPanels[i].gameObject.GetComponent<WeaponInfo>().Instantiate(_context.playerBattleUnits[i]);
        }

        // Add experience for remaining party members if all enemies are dead
        if (_context.enemyObjectList.Count == 0)
            foreach (BattleUnit member in _context.playerBattleUnits) {
                if (member.MemberData.CurrentStats.HitPoints <= 0) continue;

                yield return _context.StartCoroutine(member.AddExperience(10));
            }

        yield return new WaitForSeconds(2.0f);

        _context.gameContext.SavePartyData();

        foreach (BattleUnit member in _context.playerBattleUnits) {
            member.RemoveAllListeners();
        }
    }

    public override BattleStateMachine.StateKey GetNextState() {
        return Key;
    }

    public override void UpdateState() { }
}
