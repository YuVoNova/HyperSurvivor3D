using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CollectibleData", menuName = "CollectibleData")]
public class CollectibleData : ScriptableObject
{
    public int HealAmount;

    public int XP_SmallAmount;
    public int XP_BigAmount;

    public int Coins_SmallAmount;
    public int Coins_BigAmount;

    public float FreezeAllDuration;

    public Dictionary<string, GameObject> Prefabs;
}
