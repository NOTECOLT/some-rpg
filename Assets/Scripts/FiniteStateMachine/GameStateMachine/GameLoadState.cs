using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is called on the first loading of the game.
/// </summary>
public class GameLoadState : GenericState<GameStateMachine.StateKey> {
    private GameStateMachine _context;
    private bool _isDoneLoading;
    private List<InventoryObjectData> _itemsList;
    public GameLoadState(GameStateMachine context, List<InventoryObjectData> itemList, GameStateMachine.StateKey key) : base(key) {
        _context = context;
        _itemsList = itemList;
    }

    public override void EnterState() {
        _isDoneLoading = false;
        
        InstantiateItemsDB();
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

    public void InstantiateItemsDB() {
        _context.Items = new Dictionary<string, InventoryObjectData>();

        foreach (InventoryObjectData i in _itemsList) {
            if (i.itemid == "") {
                Debug.LogWarning($"[Item Database] Object {i.name} does not have itemid. Weapon cannot be referenced in game.");
                continue;
            }
            
            _context.Items.Add(i.itemid, i);
        }
    }
}
