using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUnitHover : MonoBehaviour {
    private UnitInfoBattleUI _entityInfoUI;
    private WeaponInfo _weaponInfo;

    void Start() {
        _entityInfoUI = GetComponent<UnitInfoBattleUI>();
        _weaponInfo = GetComponent<WeaponInfo>();

        _entityInfoUI.ViewShowLimited();
        _weaponInfo.ViewClear();   
    }

    void Update() {
        
    }

    void OnMouseOver() {
        _entityInfoUI.ViewShowFull();
        _weaponInfo.ViewSprite();
    }

    void OnMouseExit() {
        _entityInfoUI.ViewShowLimited();
        _weaponInfo.ViewClear();
    }
}
