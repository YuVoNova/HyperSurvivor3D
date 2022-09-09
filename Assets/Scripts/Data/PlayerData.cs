using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int Coins;

    public int CurrentLevel;

    public Dictionary<string, Upgrade> Upgrades;

    public PlayerData()
    {
        Coins = 0;
        CurrentLevel = 0;

        Upgrades = new Dictionary<string, Upgrade>();
    }

    public PlayerData(int Coins, int CurrentLevel)
    {
        this.Coins = Coins;
        this.CurrentLevel = CurrentLevel;

        Upgrades = new Dictionary<string, Upgrade>();
    }

    public void ResetUpgradeLevels()
    {
        if (Upgrades.Count > 0)
        {
            foreach (KeyValuePair<string, Upgrade> pair in Upgrades)
            {
                pair.Value.CurrentLevel = 0;
            }
        }
    }
}
