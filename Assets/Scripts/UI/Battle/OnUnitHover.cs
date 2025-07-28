using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUnitHover : MonoBehaviour {
    private UnitInfoBattleUI _entityInfoUI;
    
    void Start() {
        _entityInfoUI = GetComponent<UnitInfoBattleUI>();
        _entityInfoUI.ViewShowLimited();
    }

    void Update() {
        
    }

    void OnMouseOver() {
        _entityInfoUI.ViewShowFull();
    }

    void OnMouseExit() {
        _entityInfoUI.ViewShowLimited();
    }
}
