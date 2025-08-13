using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the EntityInfo UI Prefab class with information like HP
/// </summary>
public class UnitInfoBattleUI : MonoBehaviour {
    [SerializeField] private TMP_Text _entityName;


    [SerializeField] private AnimatedBar _hpAnimatedBar;
    [SerializeField] private AnimatedBar _mpAnimatedBar;

    [SerializeField] private WeaponInfo _weaponInfo;

    public void Instantiate(BattleUnit unit) {
        Instantiate(unit.MemberData);

        unit.OnHPChange += SetHPBarValue;
        unit.OnMPChange += SetMPBarValue;
    }

    public void Instantiate(PartyMember member) {
        _entityName.text = member.Name;
        _hpAnimatedBar?.SetBarValue(0, member.CurrentStats.HitPoints, member.BaseStats.HitPoints);
        _mpAnimatedBar?.SetBarValue(0, member.CurrentStats.ManaPoints, member.BaseStats.ManaPoints);
    }


    #region Bar Animations

    public void SetHPBarValue(int oldHP, int newHP, int totalHP, float time) {
        _hpAnimatedBar?.SetBarValue(oldHP, newHP, totalHP, time);
    }

    public void SetMPBarValue(int oldMP, int newMP, int totalMP, float time) {
        _mpAnimatedBar?.SetBarValue(oldMP, newMP, totalMP, time);
    }

    #endregion

    #region View Functions

    public void ViewShowLimited() {
        _hpAnimatedBar?.ShowBarOnly();
        _mpAnimatedBar?.ShowBarOnly();
        _entityName.gameObject.SetActive(true);
    }

    public void ViewShowFull() {
        _hpAnimatedBar?.ShowAll();
        _mpAnimatedBar?.ShowAll();
        _entityName.gameObject.SetActive(true);
    }

    public void ViewClear() {
        _hpAnimatedBar?.ShowNone();
        _mpAnimatedBar?.ShowNone();
        _entityName.gameObject.SetActive(false);
    }

    #endregion
}
