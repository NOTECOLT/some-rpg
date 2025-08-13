using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfo : MonoBehaviour {
    [SerializeField] private AnimatedBar _xpAnimatedBar;
    [SerializeField] private Image _weaponSprite;
    [SerializeField] private TMP_Text _weaponName;
    [SerializeField] private TMP_Text _levelText;

    public void Instantiate(BattleUnit unit) {
        Instantiate(unit.MemberData.Weapon);

        unit.OnXPChange += SetXPBarValue;
        unit.OnLevelChange += SetLevel;
    }

    public void Instantiate(WeaponItem weapon) {
        // TODO: Fix this lol so shit
        int currentXP = weapon.CurrentStats.LevelXP;
        int maxXP;
        try {
            maxXP = weapon.Data.Levels[weapon.CurrentStats.Level].Experience;
        } catch (ArgumentOutOfRangeException) {
            maxXP = weapon.CurrentStats.LevelXP;
        }

        if (maxXP == 0 && currentXP == 0) {
            maxXP = 1;
            currentXP = 1;
        }

        _xpAnimatedBar?.SetBarValue(0, currentXP, maxXP);

        // -------------------

        if (_weaponSprite != null)
            _weaponSprite.sprite = weapon.Data.Sprite;
        if (_weaponName != null)
            _weaponName.text = weapon.Data.ItemName;
        if (_levelText != null)
            _levelText.text = $"Lv. {weapon.CurrentStats.Level}";
    }

    public void SetLevel(int level) {
        if (_levelText != null)
            _levelText.text = $"Lv. {level}";
    }

    public void SetXPBarValue(int oldXP, int newXP, int totalXP, float time) {
        _xpAnimatedBar?.SetBarValue(oldXP, newXP, totalXP, time);
    }

    #region View Functions
    public void ViewSprite() {
        _weaponSprite?.gameObject.SetActive(true);
    }

    public void ViewAll() {
        _weaponSprite?.gameObject.SetActive(true);
        _levelText?.gameObject.SetActive(true);
        _xpAnimatedBar?.ShowAll();
    }

    public void ViewClear() {
        _weaponSprite?.gameObject.SetActive(false);
        _levelText?.gameObject.SetActive(false);
        _xpAnimatedBar?.ShowNone();
    }

    #endregion
}
