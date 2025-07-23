using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is called on the first loading of the game.
/// </summary>
public class GameLoadState : GenericState<GameStateMachine.StateKey> {
    private GameStateMachine _context;
    private bool _isDoneLoading;
    private List<WeaponData> _weaponsList;
    public GameLoadState(GameStateMachine context, List<WeaponData> weaponsList, GameStateMachine.StateKey key) : base(key) {
        _context = context;
        _weaponsList = weaponsList;
    }

    public override void EnterState() {
        _isDoneLoading = false;
        
        InstantiateWeaponsDB();
        PlayerDataManager.Instance.LoadPlayerData();

        _isDoneLoading = true;
    }

    public override GameStateMachine.StateKey GetNextState() {
        if (_isDoneLoading) {
            return GameStateMachine.StateKey.OVERWORLD_STATE;
        } else {
            return Key;
        }
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    public void InstantiateWeaponsDB() {
        _context.Weapons = new Dictionary<string, WeaponData>();

        foreach (WeaponData w in _weaponsList) {
            if (w.weaponid == "") {
                Debug.LogWarning($"[Item Database] Object {w.name} does not have weaponid. Weapon cannot be referenced in game.");
                continue;
            }
            
            _context.Weapons.Add(w.weaponid, w);
        }
    }
}
