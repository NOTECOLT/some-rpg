using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class WeaponQTE : WeaponAttribute {
    // new public string Name { get; protected set; } = "Weapon QTE";
    // new public AttrType Type { get; protected set; } = AttrType.PASSIVE;

    /// <summary>
    /// Expressed in seconds. -1 is the default window length (defined in BasicAttack.cs)
    /// </summary>
    [SerializeField] protected float _windowTime = -1;
    [Min(1)] public int Hits = 1;

    public abstract float GetWindowTime();

    public abstract float GetLeadTime();
}

[Serializable]
public class PressQTE : WeaponQTE {
    // new public string Name { get; protected set; } = "Press QTE";
    private static float DEFAULT_PRESS_WINDOW = 0.2f;
    private static float DEFAULT_PRESS_LEAD = 0.3f;

    /// <summary>
    /// In a series of QTE hits, fail leeway dictates how many QTEs can be "failed" and still bonus damage
    /// </summary>
    [Min(0)] public int FailAllowance = 0;

    public override float GetWindowTime() {
        if (_windowTime == -1) {
            return DEFAULT_PRESS_WINDOW;
        } else {
            return _windowTime;
        }
    }

    public override float GetLeadTime() {
        return DEFAULT_PRESS_LEAD;
    }
}

[Serializable]
public class ReleaseQTE : WeaponQTE {
    private static float DEFAULT_RELEASE_WINDOW = 0.3f;
    private static float DEFAULT_RELEASE_LEAD = 0.3f;
    // new public string Name { get; protected set; } = "Release QTE";

    public override float GetWindowTime() {
        if (_windowTime == -1) {
            return DEFAULT_RELEASE_WINDOW;
        } else {
            return _windowTime;
        }
    }

    public override float GetLeadTime() {
        return DEFAULT_RELEASE_LEAD;
    }
}

[Serializable]
public class MashQTE : WeaponQTE {
    private static float DEFAULT_MASH_WINDOW = 1.0f;
    private static float DEFAULT_MASH_LEAD = 0.3f;
    [Min(0)] public int MashHitBonus = 0;

    public override float GetWindowTime() {
        if (_windowTime == -1) {
            return DEFAULT_MASH_WINDOW;
        } else {
            return _windowTime;
        }
    }

    public override float GetLeadTime() {
        return DEFAULT_MASH_LEAD;
    }
}
