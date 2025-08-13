using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour {
    [SerializeField] private Transform _invListParent;
    [SerializeField] private GameObject _invListPrefab;

    [SerializeField] private GameObject _weaponButtonOptions;
    private InventoryListItem _selectedItem = null;
    private InventoryPartyMember _selectedMember = null;


    void OnEnable() {
        ClearSelectedItem();
        // isEquipping = false;

        foreach (Transform child in _invListParent) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < PlayerDataManager.Instance.Data.Inventory.Count; i++) {
            AddInventoryListItem(i);
        }

        _weaponButtonOptions.SetActive(false);
    }

    public void SetSelectedItem(InventoryListItem listItem) {
        if (_selectedItem != null) _selectedItem.Deselect();
        _selectedItem = listItem;
        List<InventoryObject> inv = PlayerDataManager.Instance.Data.Inventory;
    
        if (_selectedMember != null) {
            _selectedMember.Deselect();
            ClearSelectedMember();
        }

        if (inv[_selectedItem.inventoryObjectIndex] is WeaponItem)
            _weaponButtonOptions.SetActive(true);
    }

    public void ClearSelectedItem() {
        _selectedItem = null;
        _weaponButtonOptions.SetActive(false);
    }

    public void SetSelectedMember(InventoryPartyMember invPartyMember) {
        if (_selectedMember != null) _selectedMember.Deselect();
        _selectedMember = invPartyMember;

        if (_selectedItem != null) {
            _selectedItem.Deselect();
            ClearSelectedItem();
        }

        _weaponButtonOptions.SetActive(true);
    }

    public void ClearSelectedMember() {
        _selectedMember = null;
        _weaponButtonOptions.SetActive(false);
    }

    public void EquipMember(UnitInfoMenuUI unitInfo) {
        List<InventoryObject> inv = PlayerDataManager.Instance.Data.Inventory;
        if (inv[_selectedItem.inventoryObjectIndex] is not WeaponItem) return;

        WeaponItem oldWeapon = unitInfo.member.Weapon;

        // Set new weapon to member's equipped
        unitInfo.member.Weapon = (WeaponItem)inv[_selectedItem.inventoryObjectIndex];
        unitInfo.gameObject.GetComponent<WeaponInfo>().Instantiate(unitInfo.member.Weapon);

        // Remove new weapon from inventory
        inv.RemoveAt(_selectedItem.inventoryObjectIndex);
        RemoveInventoryListItem(_selectedItem.inventoryObjectIndex);

        // Add old weapon back to inventory
        inv.Add(oldWeapon);
        AddInventoryListItem(inv.Count - 1);

        ClearSelectedItem();
    }

    private void AddInventoryListItem(int inventoryIndex) {
        InventoryObject invObj = PlayerDataManager.Instance.Data.Inventory[inventoryIndex];
        GameObject listItem = GameObject.Instantiate(_invListPrefab, _invListParent);
        listItem.GetComponent<InventoryListItem>().Initialize(invObj, inventoryIndex, this);
    }

    private void RemoveInventoryListItem(int inventoryIndex) {
        foreach (Transform invListItem in _invListParent) {
            if (invListItem.gameObject.GetComponent<InventoryListItem>().inventoryObjectIndex == inventoryIndex) {
                Destroy(invListItem.gameObject);
                return;
            }
        }

        Debug.LogWarning($"[Inventory Menu] Cannot remove inventory list item for item at index {inventoryIndex} because it does not exist!");
    }

    public bool? IsSelectedItemWeapon() {
        if (_selectedItem == null) return null;

        List<InventoryObject> inv = PlayerDataManager.Instance.Data.Inventory;
        if (inv[_selectedItem.inventoryObjectIndex] is WeaponItem) return true;

        return false;
    }


    #region Weapon Options
    public void AddExperience() {
        List<InventoryObject> inv = PlayerDataManager.Instance.Data.Inventory;

        if (inv[_selectedItem.inventoryObjectIndex] is not WeaponItem) return;
    }

    // public void Equip() {
    //     List<InventoryObject> inv = PlayerDataManager.Instance.Data.Inventory;

    //     if (inv[_selectedItem.inventoryObjectIndex] is not WeaponItem) return;

    //     // isEquipping = !isEquipping;

    //     // if (isEquipping) {

    //     // } else {

    //     // }
    // }
    
    #endregion
}
