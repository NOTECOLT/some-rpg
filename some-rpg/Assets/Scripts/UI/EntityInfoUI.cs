using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the EntityInfo UI Prefab class with information like HP
/// </summary>
public class EntityInfoUI : MonoBehaviour {
    [SerializeField] private Image _hitpointsBar;

    /// <summary>
    /// Sets the hp bar to a certain percentage without any animation
    /// </summary>
    public void SetHPBar(float hpPercentage) {
        _hitpointsBar.fillAmount = hpPercentage;
    }

    public void SetHPBar(float hpPercentage, float time) {
        StartCoroutine(AnimateHPBar(hpPercentage, time));
    }

    private IEnumerator AnimateHPBar(float newPercentage, float animationTime) {
        float oldPercentage = _hitpointsBar.fillAmount;
        float speed = (oldPercentage - newPercentage) / animationTime * Time.deltaTime;

        while (Math.Round(_hitpointsBar.fillAmount, 2) != Math.Round(newPercentage, 2)) {
            _hitpointsBar.fillAmount -= speed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
