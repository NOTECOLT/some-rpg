using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the EntityInfo UI Prefab class with information like HP
/// </summary>
public class EntityInfoUI : MonoBehaviour {
    [SerializeField] private Image _hitpointsBar;
    [SerializeField] private Image _manapointsBar;
    [SerializeField] private TMP_Text _entityName;
    [SerializeField] private TMP_Text _hitPoints;
    [SerializeField] private TMP_Text _manaPoints;
    [SerializeField] private Image _weaponSprite;

    public void Instantiate(BattleUnit unit) {
        _entityName.text = unit.Name;
        SetHPBar(unit.CurrentStats.HitPoints, unit.BaseStats.HitPoints);
        if (_manapointsBar != null)
            SetMPBar(unit.CurrentStats.ManaPoints, unit.BaseStats.ManaPoints);
        if (_weaponSprite != null)
            _weaponSprite.sprite = unit.Weapon.Sprite;
    }

    public void Instantiate(string name, EntityStats currentStats, EntityStats baseStats, Weapon weapon) {
        _entityName.text = name;
        SetHPBar(currentStats.HitPoints, baseStats.HitPoints);
        if (_manapointsBar != null)
            SetMPBar(currentStats.ManaPoints, baseStats.ManaPoints);
        if (_weaponSprite != null)
            _weaponSprite.sprite = weapon.Sprite;
    }

    /// <summary>
    /// Sets the hp bar to a certain percentage without any animation
    /// </summary>
    public void SetHPBar(int newHP, int totalHP) {
        _hitpointsBar.fillAmount = (float)newHP / totalHP;
        if (_hitPoints != null)
            _hitPoints.text = $"{newHP}/{totalHP}";
    }

    public void SetMPBar(int newMP, int totalMP) {
        _manapointsBar.fillAmount = (float)newMP / totalMP;
        if (_manaPoints != null)
            _manaPoints.text = $"{newMP}/{totalMP}";
    }

    #region Bar Animations

    public void SetHPBar(int oldHP, int newHP, int totalHP, float time) {
        StartCoroutine(AnimateHPBar(oldHP, newHP, totalHP, time));
    }

    public void SetMPBar(int oldMP, int newMP, int totalMP, float time) {
        StartCoroutine(AnimateMPBar(oldMP, newMP, totalMP, time));
    }

    private IEnumerator AnimateHPBar(int oldHP, int newHP, int totalHP, float animationTime) {
        float oldPercentage = _hitpointsBar.fillAmount;
        float newPercentage = (float)newHP / totalHP;
        float currentHP = oldHP;
        float currentTime = animationTime;

        while (currentTime > 0) {
            _hitpointsBar.fillAmount -= (oldPercentage - newPercentage) / animationTime * Time.deltaTime;
            currentHP -= (oldHP - newHP) / animationTime * Time.deltaTime;
            if (_hitPoints != null)
                _hitPoints.text = $"{Mathf.CeilToInt(currentHP)}/{totalHP}";

            currentTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        if (_hitPoints != null)
            _hitPoints.text = $"{newHP}/{totalHP}";
        _hitpointsBar.fillAmount = newPercentage;
    }

    private IEnumerator AnimateMPBar(int oldMP, int newMP, int totalMP, float animationTime) {
        float oldPercentage = _manapointsBar.fillAmount;
        float newPercentage = (float)newMP / totalMP;
        float currentMP = oldMP;
        float currentTime = animationTime;

        while (currentTime > 0) {
            _manapointsBar.fillAmount -= (oldPercentage - newPercentage) / animationTime * Time.deltaTime;
            currentMP -= (oldMP - newMP) / animationTime * Time.deltaTime;
            if (_manaPoints != null)
                _manaPoints.text = $"{Mathf.CeilToInt(currentMP)}/{totalMP}";

            currentTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        if (_manaPoints != null)
            _manaPoints.text = $"{newMP}/{totalMP}";
        _manapointsBar.fillAmount = newPercentage;
    }

    #endregion
}
