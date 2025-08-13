using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to InventoryPartyMemberInfo object. Used to equip items in the inventory menu
/// </summary>
public class InventoryPartyMember : MonoBehaviour {
    private bool _isSelected = false;
    [SerializeField] private Image _backgroundSelect;
    [SerializeField] private InventoryMenu _inventoryMenu;

    private void OnEnable() {
        Deselect();
    }

    public void Select() {
        _isSelected = true;
        _backgroundSelect.enabled = _isSelected;    
    }

    public void Deselect() {
        _isSelected = false;
        _backgroundSelect.enabled = _isSelected;
    }

    public void OnClick() {
        if (_inventoryMenu.IsSelectedItemWeapon() == true) {
            _inventoryMenu.EquipMember(GetComponent<UnitInfoMenuUI>());
        } else if (_inventoryMenu.IsSelectedItemWeapon() == null) {
            _isSelected = !_isSelected;
            _backgroundSelect.enabled = _isSelected;
        }

        if (_isSelected) {
            _inventoryMenu.SetSelectedMember(this);
        } else {
            _inventoryMenu.ClearSelectedMember();
        }
    }
}
