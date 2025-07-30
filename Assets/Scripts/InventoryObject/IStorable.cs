using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base interface for all inventory items
/// </summary>
public interface IStorable {
    string GetName();
    Sprite GetSprite();
    string GetID();
}
