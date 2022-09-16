using Tabtale.TTPlugins;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    // Singleton

    private static Manager instance;
    public static Manager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("Manager").GetComponent<Manager>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }


    // Player

    [HideInInspector]
    public PlayerData PlayerData;


    // Data

    public CollectibleData CollectibleData;

    public XPData XPData;

    public ArenaColorData ArenaColorData;


    // Levels

    public List<LevelData> Levels;


    // Data

    private JsonData jsonData;
    private string dataPath;


    // Unity Functions

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        Save();
    }


    // Functions

    private void Initialize()
    {
        InitializeSDK();

        InitializePlayerData();

        InitializeUpgrades();

        InitializeCollectibleData();

        DebugManager.instance.enableRuntimeUI = false;
    }

    private void InitializeSDK()
    {
        TTPCore.Setup();
    }

    private void InitializeUpgrades()
    {
        PlayerData.Upgrades.Add("AirStrike", Resources.Load("Scriptables/Upgrades/Active/AirStrike") as Upgrade);
        PlayerData.Upgrades.Add("Freezer", Resources.Load("Scriptables/Upgrades/Active/Freezer") as Upgrade);
        PlayerData.Upgrades.Add("Knife", Resources.Load("Scriptables/Upgrades/Active/Knife") as Upgrade);
        PlayerData.Upgrades.Add("Lightning", Resources.Load("Scriptables/Upgrades/Active/Lightning") as Upgrade);
        PlayerData.Upgrades.Add("Shield", Resources.Load("Scriptables/Upgrades/Active/Shield") as Upgrade);

        PlayerData.Upgrades.Add("GunslingerDamage", Resources.Load("Scriptables/Upgrades/Class/GunslingerDamage") as Upgrade);
        PlayerData.Upgrades.Add("GunslingerRange", Resources.Load("Scriptables/Upgrades/Class/GunslingerRange") as Upgrade);
        PlayerData.Upgrades.Add("GunslingerRateOfFire", Resources.Load("Scriptables/Upgrades/Class/GunslingerRateOfFire") as Upgrade);

        PlayerData.Upgrades.Add("RiflemanDamage", Resources.Load("Scriptables/Upgrades/Class/RiflemanDamage") as Upgrade);
        PlayerData.Upgrades.Add("RiflemanRange", Resources.Load("Scriptables/Upgrades/Class/RiflemanRange") as Upgrade);
        PlayerData.Upgrades.Add("RiflemanRateOfFire", Resources.Load("Scriptables/Upgrades/Class/RiflemanRateOfFire") as Upgrade);

        PlayerData.Upgrades.Add("RocketeerDamage", Resources.Load("Scriptables/Upgrades/Class/RocketeerDamage") as Upgrade);
        PlayerData.Upgrades.Add("RocketeerRange", Resources.Load("Scriptables/Upgrades/Class/RocketeerRange") as Upgrade);
        PlayerData.Upgrades.Add("RocketeerReloadSpeed", Resources.Load("Scriptables/Upgrades/Class/RocketeerReloadSpeed") as Upgrade);

        PlayerData.Upgrades.Add("HealthRegen", Resources.Load("Scriptables/Upgrades/Passive/HealthRegen") as Upgrade);
        PlayerData.Upgrades.Add("Luck", Resources.Load("Scriptables/Upgrades/Passive/Luck") as Upgrade);
        PlayerData.Upgrades.Add("Magnet", Resources.Load("Scriptables/Upgrades/Passive/Magnet") as Upgrade);
        PlayerData.Upgrades.Add("MaxHealth", Resources.Load("Scriptables/Upgrades/Passive/MaxHealth") as Upgrade);
        PlayerData.Upgrades.Add("MovementSpeed", Resources.Load("Scriptables/Upgrades/Passive/MovementSpeed") as Upgrade);
        PlayerData.Upgrades.Add("XPGain", Resources.Load("Scriptables/Upgrades/Passive/XPGain") as Upgrade);

        PlayerData.ResetUpgradeLevels();
    }

    private void InitializeCollectibleData()
    {
        CollectibleData.Prefabs = new Dictionary<string, GameObject>();
        CollectibleData.Prefabs.Add("Health", Resources.Load("Prefabs/Collectibles/Collectible_Health") as GameObject);
        CollectibleData.Prefabs.Add("XP_Small", Resources.Load("Prefabs/Collectibles/Collectible_XP_Small") as GameObject);
        CollectibleData.Prefabs.Add("XP_Big", Resources.Load("Prefabs/Collectibles/Collectible_XP_Big") as GameObject);
        CollectibleData.Prefabs.Add("Coin_Small", Resources.Load("Prefabs/Collectibles/Collectible_Coin_Small") as GameObject);
        CollectibleData.Prefabs.Add("Coin_Big", Resources.Load("Prefabs/Collectibles/Collectible_Coin_Big") as GameObject);
        CollectibleData.Prefabs.Add("Powerup_MagnetAll", Resources.Load("Prefabs/Collectibles/Collectible_Powerup_MagnetAll") as GameObject);
        CollectibleData.Prefabs.Add("Powerup_FreezeAll", Resources.Load("Prefabs/Collectibles/Collectible_Powerup_FreezeAll") as GameObject);
        CollectibleData.Prefabs.Add("Powerup_Nuke", Resources.Load("Prefabs/Collectibles/Collectible_Powerup_Nuke") as GameObject);
    }

    public void Save()
    {
        SerializeData();
    }

    public LevelData CurrentLevel()
    {
        return Levels[Mathf.FloorToInt(Mathf.Clamp(PlayerData.CurrentLevel, 0, Levels.Count - 1))];
    }

    public void FinishedLevel()
    {
        PlayerData.CurrentLevel++;

        Save();

        SceneManager.LoadScene("Game");
    }

    public void FailedLevel()
    {
        Save();

        SceneManager.LoadScene("Game");
    }

    #region Data Handling

    private void InitializePlayerData()
    {
        PlayerData = new PlayerData();
        jsonData = new JsonData();

        dataPath = Path.Combine(Application.persistentDataPath, "HyperSurvivorSave.json");

        if (File.Exists(dataPath))
        {
            DeserializeData();
        }
        else
        {
            PlayerData.Coins = 0;
            PlayerData.CurrentLevel = 0;

            File.Create(dataPath).Close();

            SerializeData();
        }
    }

    // Saves progress data.
    private void SerializeData()
    {
        jsonData.Coins = PlayerData.Coins;
        jsonData.CurrentLevel = PlayerData.CurrentLevel;

        string jsonDataString = JsonUtility.ToJson(jsonData, true);

        File.WriteAllText(dataPath, jsonDataString);
    }

    // Loads progress data.
    private void DeserializeData()
    {
        string jsonDataString = File.ReadAllText(dataPath);

        jsonData = JsonUtility.FromJson<JsonData>(jsonDataString);

        PlayerData.Coins = jsonData.Coins;
        PlayerData.CurrentLevel = jsonData.CurrentLevel;
    }

    #endregion
}

public class JsonData
{
    public int Coins;
    public int CurrentLevel;

    public JsonData()
    {
        Coins = 0;
        CurrentLevel = 0;
    }
}
