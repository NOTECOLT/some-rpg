using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the EntityInfo UI Prefab class with information like HP
/// </summary>
public class UnitInfoMenuUI : MonoBehaviour {
    [SerializeField] private TMP_Text _entityName;


    [SerializeField] private AnimatedBar _hpAnimatedBar;
    [SerializeField] private AnimatedBar _mpAnimatedBar;
    [SerializeField] private AnimatedBar _xpAnimatedBar;

    [SerializeField] private Image _weaponSprite;
    [SerializeField] private TMP_Text _weaponName;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Image _xpBar;

    public void Instantiate(BattleUnit unit) {
        Instantiate(unit.MemberData);

        unit.OnHPChange += SetHPBarValue;
        unit.OnMPChange += SetMPBarValue;
        unit.OnXPChange += SetXPBarValue;
        unit.OnLevelChange += SetLevel;
    }

    public void Instantiate(PartyMember member) {
        _entityName.text = member.Name;
        _hpAnimatedBar?.SetBarValue(0, member.CurrentStats.HitPoints, member.BaseStats.HitPoints);
        _mpAnimatedBar?.SetBarValue(0, member.CurrentStats.ManaPoints, member.BaseStats.ManaPoints);

        // TODO: Fix this lol so shit

        int currentXP = member.Weapon.CurrentStats.LevelXP;
        int maxXP;
        try {
            maxXP = member.Weapon.Data.Levels[member.Weapon.CurrentStats.Level].Experience;
        } catch (ArgumentOutOfRangeException) {
            maxXP = member.Weapon.CurrentStats.LevelXP;
        }

        if (maxXP == 0 && currentXP == 0) {
            maxXP = 1;
            currentXP = 1;
        }

        _xpAnimatedBar?.SetBarValue(0, currentXP, maxXP);

        if (_weaponSprite != null)
            _weaponSprite.sprite = member.Weapon.Data.Sprite;
        if (_weaponName != null)
            _weaponName.text = member.Weapon.Data.ItemName;
        if (_levelText != null)
            _levelText.text = $"Lv. {member.Weapon.CurrentStats.Level}";
    }


    #region Bar Animations

    public void SetHPBarValue(int oldHP, int newHP, int totalHP, float time) {
        _hpAnimatedBar?.SetBarValue(oldHP, newHP, totalHP, time);
    }

    public void SetMPBarValue(int oldMP, int newMP, int totalMP, float time) {
        _mpAnimatedBar?.SetBarValue(oldMP, newMP, totalMP, time);
    }

    public void SetXPBarValue(int oldXP, int newXP, int totalXP, float time) {
        _xpAnimatedBar?.SetBarValue(oldXP, newXP, totalXP, time);
    }

    public void SetLevel(int level) {
        if (_levelText != null)
            _levelText.text = $"Lv. {level}";
    }

    #endregion
}
