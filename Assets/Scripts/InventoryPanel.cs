using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour {
    [SerializeField] private Transform _invListParent;
    [SerializeField] private GameObject _invListPrefab;

    void OnEnable() {
        foreach (Transform child in _invListParent) {
            Destroy(child.gameObject);
        }

        foreach (InventoryObject invObj in PlayerDataManager.Instance.Data.Inventory) {
            GameObject listItem = GameObject.Instantiate(_invListPrefab, _invListParent);
            listItem.GetComponent<InventoryListItem>().Initialize(invObj);
        }
    }
}
