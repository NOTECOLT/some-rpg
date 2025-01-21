using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equippable items that will affect the type of attack the player will perform.
/// </summary>
[CreateAssetMenu(fileName="Weapon", menuName="Weapon", order=1)]  
public class Weapon : ScriptableObject {
    public WeaponType Type;
    public string WeaponName;
    public Sprite Sprite;
}