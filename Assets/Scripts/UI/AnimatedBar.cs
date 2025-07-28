using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimatedBar : MonoBehaviour {
    [SerializeField] private Image _progressBar;
    [SerializeField] private TMP_Text _progressLabel;

    /// <summary>
    /// Formatting string for how the label will display the percentage.
    /// "O" = Old Value
    /// "N" = New Value
    /// "T" = Total Value
    /// </summary>
    public string labelFormat = "C/T";

    public void ShowAll() {
        _progressBar?.gameObject.SetActive(true);
        _progressLabel?.gameObject.SetActive(true);
    }

    public void ShowBarOnly() {
        _progressBar?.gameObject.SetActive(true);
        _progressLabel?.gameObject.SetActive(false);

    }

    public void ShowNone() {
        _progressBar?.gameObject.SetActive(false);
        _progressLabel?.gameObject.SetActive(false);

    }

    private string ReturnStringWithFormat(string currentValue, string totalValue) {
        string ret = labelFormat;

        string ReplaceCharFormatter(string symbol, string replacement) {
            int i = ret.IndexOf(symbol);
            if (i != -1) ret = ret.Insert(i, replacement).Replace(symbol, "");
            return ret;
        }
        ret = ReplaceCharFormatter("C", currentValue);
        ret = ReplaceCharFormatter("T", totalValue);

        return ret;
    }

    /// <summary>
    /// Sets the hp/mp/xp bar to a certain percentage
    /// </summary>
    public void SetBarValue(int oldValue, int newValue, int totalValue, float time = 0) {
        if (_progressBar == null) return;

        if (time == 0) {
            _progressBar.fillAmount = Mathf.Clamp((float)newValue / totalValue, 0, 1);
            if (_progressLabel != null)
                _progressLabel.text = ReturnStringWithFormat(newValue.ToString(), totalValue.ToString());
        } else
            StartCoroutine(AnimateBar(oldValue, newValue, totalValue, time));
    }

    public IEnumerator AnimateBar(int oldValue, int newValue, int totalValue, float animationTime) {
        float oldPercentage = _progressBar.fillAmount;
        float newPercentage = Mathf.Clamp((float)newValue / totalValue, 0, 1);
        float currentValue = oldValue;
        float currentTime = animationTime;

        while (currentTime > 0) {
            _progressBar.fillAmount -= (oldPercentage - newPercentage) / animationTime * Time.deltaTime;
            currentValue -= (oldValue - newValue) / animationTime * Time.deltaTime;
            if (_progressLabel != null)
                _progressLabel.text = ReturnStringWithFormat(Mathf.CeilToInt(currentValue).ToString(), totalValue.ToString());

            currentTime -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        if (_progressLabel != null)
            _progressLabel.text = $"{newValue}/{totalValue}";
        _progressBar.fillAmount = newPercentage;
    }
}
