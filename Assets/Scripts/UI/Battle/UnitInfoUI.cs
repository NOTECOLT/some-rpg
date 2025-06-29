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
        _entityName.text = unit.Name;
        SetBarValue(_hitpointsBar, _hitPoints, unit.CurrentStats.HitPoints, unit.BaseStats.HitPoints);
        SetBarValue(_manapointsBar, _manaPoints, unit.CurrentStats.ManaPoints, unit.BaseStats.ManaPoints);

        if (_weaponSprite != null)
            _weaponSprite.sprite = unit.WeaponItem.Data.Sprite;
        if (_weaponName != null)
            _weaponName.text = unit.WeaponItem.Data.WeaponName;
    }

    public void Instantiate(string name, EntityStats currentStats, EntityStats baseStats, WeaponItem weaponItem) {
        _entityName.text = name;
        SetBarValue(_hitpointsBar, _hitPoints, currentStats.HitPoints, baseStats.HitPoints);
        SetBarValue(_manapointsBar, _manaPoints, currentStats.ManaPoints, baseStats.ManaPoints);

        // TODO: Fix this lol so shit

        int currentXP = weaponItem.CurrentStats.LevelXP;
        int maxXP;
        try {
            maxXP = weaponItem.Data.Levels[weaponItem.CurrentStats.Level].Experience;
        } catch (ArgumentOutOfRangeException) {
            maxXP = weaponItem.CurrentStats.LevelXP;
        }
        
        if (maxXP == 0 && currentXP == 0) {
            maxXP = 1;
            currentXP = 1;
        }

        SetBarValue(_xpBar, null, currentXP, maxXP);

        if (_weaponSprite != null)
            _weaponSprite.sprite = weaponItem.Data.Sprite;
        if (_weaponName != null)
            _weaponName.text = weaponItem.Data.WeaponName;
        if (_levelText != null)
            _levelText.text = $"Lv. {weaponItem.CurrentStats.Level}";
    }


    #region Bar Animations

    public void SetHPBarValue(int oldHP, int newHP, int totalHP, float time) {
        SetBarValue(_hitpointsBar, _hitPoints, oldHP, newHP, totalHP, time);
    }

    public void SetMPBarValue(int oldMP, int newMP, int totalMP, float time) {
        SetBarValue(_manapointsBar, _manaPoints, oldMP, newMP, totalMP, time);
    }

    /// <summary>
    /// Sets the hp/mp/xp bar to a certain percentage without any animation
    /// </summary>
    private void SetBarValue(Image barObj, TMP_Text textObj, int newValue, int totalValue) {
        if (barObj == null) return;

        barObj.fillAmount = Mathf.Clamp((float)newValue / totalValue, 0, 1);
        if (textObj != null)
            textObj.text = $"{newValue}/{totalValue}";
    }

    /// <summary>
    /// Sets the hp/mp/xp bar to a certain percentage WITH animation
    /// </summary>
    private void SetBarValue(Image barObj, TMP_Text textObj, int oldValue, int newValue, int totalValue, float time) {
        if (barObj == null) return;
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
