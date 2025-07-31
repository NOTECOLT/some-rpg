using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : MonoBehaviour {
    [SerializeField] private TMP_Text _itemText;
    [SerializeField] private Image _itemSprite;
    public void Initialize(InventoryObject invObj) {
        if (invObj is Item) {
            Item i = (Item)invObj;
            _itemText.text = i.Data.ItemName;
            _itemSprite.sprite = i.Data.Sprite;
        } else if (invObj is WeaponItem) {
            WeaponItem w = (WeaponItem)invObj;
            _itemText.text = w.Data.ItemName;
            _itemSprite.sprite = w.Data.Sprite;
        }
    }
}
