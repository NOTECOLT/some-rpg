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
    [SerializeField] private TMP_Text _entityName;
    [SerializeField] private TMP_Text _hitPoints;

    public void Instantiate(BattleUnit unit) {
        SetEntityName(unit.Name);
        SetHPBar(unit.CurrentStats.HitPoints, unit.BaseStats.HitPoints);
    }

    public void SetEntityName(string name) {
        _entityName.text = name;
    }

    /// <summary>
    /// Sets the hp bar to a certain percentage without any animation
    /// </summary>
    public void SetHPBar(int newHP, int totalHP) {
        newHP = (newHP < 0) ? 0 : newHP;
        _hitpointsBar.fillAmount = (float)newHP / totalHP;
        _hitPoints.text = $"{newHP}/{totalHP}";
    }

    public void SetHPBar(int oldHP, int newHP, int totalHP, float time) {
        newHP = (newHP < 0) ? 0 : newHP;
        StartCoroutine(AnimateHPBar(oldHP, newHP, totalHP, time));
    }

    private IEnumerator AnimateHPBar(int oldHP, int newHP, int totalHP, float animationTime) {
        float oldPercentage = _hitpointsBar.fillAmount;
        float newPercentage = (float)newHP / totalHP;
        float currentHP = oldHP;
        float currentTime = animationTime;

        while (currentTime > 0) {
            _hitpointsBar.fillAmount -= (oldPercentage - newPercentage) / animationTime * Time.deltaTime;
            currentHP -= (oldHP - newHP) / animationTime * Time.deltaTime;
            _hitPoints.text = $"{Mathf.CeilToInt(currentHP)}/{totalHP}";

            currentTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _hitPoints.text = $"{newHP}/{totalHP}";
        _hitpointsBar.fillAmount = newPercentage;
    }
}
