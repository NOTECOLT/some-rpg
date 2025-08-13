using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : MonoBehaviour {
    [SerializeField] private TMP_Text _itemText;
    [SerializeField] private Image _itemSprite;
    [SerializeField] private Image _backgroundSelect;

    private InventoryMenu _inventoryMenu;
    public int inventoryObjectIndex;

    private bool _isSelected = false;
    public void Initialize(InventoryObject invObj, int index, InventoryMenu invMenu) {
        if (invObj is Item) {
            Item i = (Item)invObj;
            inventoryObjectIndex = index;

            _itemText.text = i.Data.ItemName;
            _itemSprite.sprite = i.Data.Sprite;

            GetComponent<WeaponInfo>().ViewClear();
        } else if (invObj is WeaponItem) {
            WeaponItem w = (WeaponItem)invObj;
            inventoryObjectIndex = index;

            _itemText.text = w.Data.ItemName;
            _itemSprite.sprite = w.Data.Sprite;

            GetComponent<WeaponInfo>().Instantiate(w);
            GetComponent<WeaponInfo>().ViewAll();
        }

        _inventoryMenu = invMenu;
    }

    public void Deselect() {
        _isSelected = false;
        _backgroundSelect.enabled = _isSelected;
    }


    public void OnClick() {
        _isSelected = !_isSelected;
        _backgroundSelect.enabled = _isSelected;

        if (_isSelected) {
            _inventoryMenu.SetSelectedItem(this);
        } else {
            _inventoryMenu.ClearSelectedItem();
        }
    }
}
