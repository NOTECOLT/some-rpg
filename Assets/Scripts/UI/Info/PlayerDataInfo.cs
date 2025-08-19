using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Used for displaying non party member-specific data. Like experience
public class PlayerDataInfo : MonoBehaviour {
    [SerializeField] private AnimatedBar _xpAnimatedBar;
    [SerializeField] private TMP_Text _levelText;

    void OnEnable() {
        Initialize();
        PlayerDataManager.Instance.Data.OnXPChange += SetXPBarValue;
    }

    // void OnDisable() {
    //     PlayerDataManager.Instance.Data.OnXPChange -= SetXPBarValue;
    // }

    void OnDestroy() {
        PlayerDataManager.Instance.Data.OnXPChange -= SetXPBarValue;
    }

    public void Initialize() {
        PlayerData data = PlayerDataManager.Instance.Data;
        SetXPBarValue(data.ExperiencePool, data.ExperiencePool, data.ExperienceMax, 0);
    }

    public void SetXPBarValue(int oldXP, int newXP, int totalXP, float time) {
        _xpAnimatedBar?.SetBarValue(oldXP, newXP, totalXP, time);
    }
}
