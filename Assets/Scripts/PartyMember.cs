using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PartyMember : ICloneable, ISerializable {
    /// <summary>
    /// These stats do not change in battle. BaseStats only change in between battles, through level up or permanent status changes
    /// </summary>
    public EntityStats BaseStats = new EntityStats();

    /// <summary>
    /// These stats may change in battle through status effects
    /// </summary>
    public EntityStats CurrentStats = new EntityStats();
    public string Name;
    public WeaponItem Weapon;

    #region ISerializable
    public IDeserializable Serialize() {
        SerializedPartyMember ret = new SerializedPartyMember() {
            BaseStats = (EntityStats)BaseStats.Clone(),
            CurrentStats = (EntityStats)CurrentStats.Clone(),
            Name = Name
        };

        ret.Weapon = (SerializedWeaponItem)Weapon.Serialize();
        return ret;
    }

    #endregion

    public object Clone() {
        return new PartyMember() {
            BaseStats = (EntityStats)BaseStats.Clone(),
            CurrentStats = (EntityStats)CurrentStats.Clone(),
            Weapon = (WeaponItem)Weapon.Clone(),
            Name = Name
        };
    }
}

[Serializable]
public class SerializedPartyMember : IDeserializable {
    // BaseStats only change in between battles, through level up or permanent status changes
    public EntityStats BaseStats = new EntityStats();

    // CurrentStats may change through status effects in battle
    public EntityStats CurrentStats = new EntityStats();
    public string Name;

    /// <summary>
    /// Weapon Keys must be stored instead of scriptable object instances.
    /// </summary>
    public SerializedWeaponItem Weapon;

    #region IDeserializable

    public ISerializable Deserialize() {
        PartyMember ret = new PartyMember() {
            BaseStats = (EntityStats)BaseStats.Clone(),
            CurrentStats = (EntityStats)CurrentStats.Clone(),
            Name = Name
        };

        ret.Weapon = (WeaponItem)Weapon.Deserialize();

        return ret;
    }

    #endregion

    // public object Clone() {
    //     return new SerializedPartyMember() {
    //         BaseStats = (EntityStats)BaseStats.Clone(),
    //         CurrentStats = (EntityStats)CurrentStats.Clone(),
    //         Weapon = (SerializedWeaponItem)Weapon.Clone(),
    //         Name = Name
    //     };
    // }
}