using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that holds all enemy data. <br></br>
/// If you're wondering what should go in the EnemyType class, <br></br>
/// It's basically all the "data that shouldn't change" (i.e. name, base stats, etc.) 
/// </summary>
[CreateAssetMenu(fileName="EnemyType", menuName="EnemyType", order=1)] 
public class EnemyType : ScriptableObject {
    public string EnemyName;
    public Sprite Sprite;
    public int BaseHitPoints;
    public int BaseAttack;
    public int BaseDefense;
    public int BaseSpeed;
}
