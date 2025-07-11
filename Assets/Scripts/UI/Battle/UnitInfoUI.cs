using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the EntityInfo UI Prefab class with information like HP
/// </summary>
public class UnitInfoUI : MonoBehaviour {
    [SerializeField] private Image _hitpointsBar;
    [SerializeField] private Image _manapointsBar;
    [SerializeField] private TMP_Text _entityName;
    [SerializeField] private TMP_Text _hitPoints;
    [SerializeField] private TMP_Text _manaPoints;

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
        SetBarValue(_hitpointsBar, _hitPoints, 0, member.CurrentStats.HitPoints, member.BaseStats.HitPoints);
        SetBarValue(_manapointsBar, _manaPoints, 0, member.CurrentStats.ManaPoints, member.BaseStats.ManaPoints);

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

        SetBarValue(_xpBar, null, 0, currentXP, maxXP);

        if (_weaponSprite != null)
            _weaponSprite.sprite = member.Weapon.Data.Sprite;
        if (_weaponName != null)
            _weaponName.text = member.Weapon.Data.WeaponName;
        if (_levelText != null)
            _levelText.text = $"Lv. {member.Weapon.CurrentStats.Level}";
    }


    #region Bar Animations

    public void SetHPBarValue(int oldHP, int newHP, int totalHP, float time) {
        SetBarValue(_hitpointsBar, _hitPoints, oldHP, newHP, totalHP, time);
    }

    public void SetMPBarValue(int oldMP, int newMP, int totalMP, float time) {
        SetBarValue(_manapointsBar, _manaPoints, oldMP, newMP, totalMP, time);
    }

    public void SetXPBarValue(int oldXP, int newXP, int totalXP, float time) {
        SetBarValue(_xpBar, null, oldXP, newXP, totalXP, time);
    }

    public void SetLevel(int level) {
        if (_levelText != null)
            _levelText.text = $"Lv. {level}";
    }

    /// <summary>
    /// Sets the hp/mp/xp bar to a certain percentage
    /// </summary>
    private void SetBarValue(Image barObj, TMP_Text textObj, int oldValue, int newValue, int totalValue, float time = 0) {
        if (barObj == null) return;

        if (time == 0) {
            barObj.fillAmount = Mathf.Clamp((float)newValue / totalValue, 0, 1);
            if (textObj != null)
                textObj.text = $"{newValue}/{totalValue}";
        } else
            StartCoroutine(AnimateBar(barObj, textObj, oldValue, newValue, totalValue, time));
    }

    private IEnumerator AnimateBar(Image barObj, TMP_Text textObj, int oldValue, int newValue, int totalValue, float animationTime) {
        float oldPercentage = barObj.fillAmount;
        float newPercentage = Mathf.Clamp((float)newValue / totalValue, 0, 1);
        float currentHP = oldValue;
        float currentTime = animationTime;

        while (currentTime > 0) {
            barObj.fillAmount -= (oldPercentage - newPercentage) / animationTime * Time.deltaTime;
            currentHP -= (oldValue - newValue) / animationTime * Time.deltaTime;
            if (textObj != null)
                textObj.text = $"{Mathf.CeilToInt(currentHP)}/{totalValue}";

            currentTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        if (textObj != null)
            textObj.text = $"{newValue}/{totalValue}";
        barObj.fillAmount = newPercentage;
    }

    #endregion

    #region View Functions

    public void ViewShowLimited() {
        _hitpointsBar.gameObject.SetActive(true);
        if (_manapointsBar != null) _manapointsBar.gameObject.SetActive(true);
        _entityName.gameObject.SetActive(true);

        _hitPoints.gameObject.SetActive(false);
        if (_manaPoints != null) _manaPoints.gameObject.SetActive(false);
        _weaponSprite.gameObject.SetActive(false);
        if (_weaponName != null) _weaponName.gameObject.SetActive(false);
    }

    public void ViewShowFull() {
        _hitpointsBar.gameObject.SetActive(true);
        if (_manapointsBar != null) _manapointsBar.gameObject.SetActive(true);
        _entityName.gameObject.SetActive(true);
        _hitPoints.gameObject.SetActive(true);
        if (_manaPoints != null) _manaPoints.gameObject.SetActive(true);
        _weaponSprite.gameObject.SetActive(true);
        if (_weaponName != null) _weaponName.gameObject.SetActive(true);
    }

    public void ViewClear() {
        _hitpointsBar.gameObject.SetActive(false);
        if (_manapointsBar != null) _manapointsBar.gameObject.SetActive(false);
        _entityName.gameObject.SetActive(false);
        _hitPoints.gameObject.SetActive(false);
        if (_manaPoints != null) _manaPoints.gameObject.SetActive(false);
        _weaponSprite.gameObject.SetActive(false);   
        if (_weaponName != null) _weaponName.gameObject.SetActive(false);   
    }

    #endregion
}
