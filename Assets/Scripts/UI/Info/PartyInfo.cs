using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyInfo : MonoBehaviour {
    [SerializeField] private UnitInfoMenuUI[] _playerInfo;

    void OnEnable() {
        PlayerData playerData = PlayerDataManager.Instance.Data;
        for (int i = 0; i < PlayerDataManager.Instance.Data.PartyStats.Count; i++) {
            _playerInfo[i].Instantiate(playerData.PartyStats[i]);
            _playerInfo[i].gameObject.GetComponent<WeaponInfo>().Instantiate(playerData.PartyStats[i].Weapon);
        }
    }
}
