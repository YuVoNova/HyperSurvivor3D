using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton

    public static GameManager Instance;


    // Objects & Components

    [SerializeField]
    private Player player;

    public GameUIManager GameUIManager;

    [SerializeField]
    private Camera UICamera;

    [SerializeField]
    private GameObject PropsObject;
    [SerializeField]
    private Renderer ArenaRenderer;

    private GameObject[] allCurrentEnemies;

    public GameObject FloatingTextPrefab;


    // Player Stats

    private int CurrentPlayerLevel;
    private float CurrentXP;
    private int CoinsCollected;

    private List<string> availableUpgrades;
    private List<string> maxedUpgrades;

    private int TotalEnemyKilled;
    private int TotalCoinsCollected;


    // Values

    [HideInInspector]
    public Vector3 PlayerPosition;

    [HideInInspector]
    public bool IsGameOn;
    [HideInInspector]
    public bool IsPaused;

    private float Luck;
    private float XPMultiplier;

    [SerializeField]
    private float MinSpawnDistance;
    [SerializeField]
    private float MaxSpawnDistance;
    [SerializeField]
    private float MaxDistanceFromCenter;


    // Level Generation

    [SerializeField]
    private int EnemyCap;

    private LevelData levelData;

    private int currentPool;
    private int currentWave;
    private int enemyKilled;

    private float frequentialSpawnTimer;
    private int frequentialSpawnAmount;

    private float chunkSpawnTimer;
    private int chunkSpawnAmount;

    [SerializeField]
    private float BetweenWavesCountdown;

    private float betweenWavesTimer;


    // Calculation

    private List<float> Degrees;

    private List<float> calculatedDegrees;
    private int degreeIndex;
    private float distance;
    private Vector3 point;


    // Random Generation

    private int randomValue;
    private string collectibleName;
    private GameObject collectibleObject;

    [SerializeField]
    private GameObject enemyMinionPrefab;

    // Collectibles

    [SerializeField]
    private GameObject NukeExplosionPrefab;

    private GameObject nukeExplosionObject;


    // Unity Functions

    private void Awake()
    {
        Instance = this;

        IsPaused = false;
        IsGameOn = false;

        currentWave = 0;

        Degrees = new List<float>();
        for (int i = 0; i < 360; i += 3)
        {
            Degrees.Add(i);
        }
        calculatedDegrees = new List<float>();

        PlayerPosition = Vector3.zero;

        availableUpgrades = new List<string>();
        maxedUpgrades = new List<string>();

        PropsObject.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 359f), 0f);
        ArenaRenderer.material = Manager.Instance.ArenaColorData.Materials[Mathf.FloorToInt(Random.Range(0f, Manager.Instance.ArenaColorData.Materials.Length))];

        Manager.Instance.Save();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        PlayerPosition = player.transform.position;

        if (IsGameOn)
        {
            if (betweenWavesTimer <= 0f)
            {
                if (frequentialSpawnTimer <= 0f)
                {
                    if (currentPool < EnemyCap)
                    {
                        SpawnChunk(frequentialSpawnAmount);
                    }

                    frequentialSpawnTimer = levelData.Waves[currentWave].FrequentialSpawnTimer;
                }
                else
                {
                    frequentialSpawnTimer -= Time.deltaTime;
                }

                if (chunkSpawnTimer <= 0f)
                {
                    if (currentPool < EnemyCap)
                    {
                        SpawnChunk(chunkSpawnAmount);
                        Debug.Log("Chunk " + chunkSpawnAmount);

                        chunkSpawnAmount += levelData.Waves[currentWave].ChunkSpawnAddition;
                    }

                    chunkSpawnTimer = levelData.Waves[currentWave].ChunkSpawnTimer;
                }
                else
                {
                    chunkSpawnTimer -= Time.deltaTime;
                }
            }
            else
            {
                betweenWavesTimer -= Time.deltaTime;

                if (betweenWavesTimer <= 0f)
                {
                    GameUIManager.UpdateWaveText((currentWave < levelData.Waves.Length - 1) ? "WAVE " + (currentWave + 1) : "FINAL WAVE");
                }
            }
        }
    }

    private void FixedUpdate()
    {

    }


    // General Functions

    public void StartLevel(string chosenClass)
    {
        CurrentPlayerLevel = -1;
        CurrentXP = 0;
        CoinsCollected = 0;

        TotalEnemyKilled = 0;
        TotalCoinsCollected = 0;

        levelData = Manager.Instance.CurrentLevel();

        betweenWavesTimer = 0f;

        StartWave();

        switch (chosenClass)
        {
            case "Gunslinger":
                player.SetClass(Classes.Gunslinger);
                break;
            case "Rifleman":
                player.SetClass(Classes.Rifleman);
                break;
            case "Rocketeer":
                player.SetClass(Classes.Rocketeer);
                break;
            default:
                break;
        }

        player.StartLevel();

        SetDefaultUpgrades(chosenClass);

        GameUIManager.StartLevel();

        IsGameOn = true;
    }

    private void FinishedLevel()
    {
        Manager.Instance.PlayerData.Coins += CoinsCollected;

        Invoke("DelayedFinishedLevel", 1.25f);

        IsGameOn = false;
    }

    private void DelayedFinishedLevel()
    {

        GameUIManager.PrepareFinishedLevel(TotalEnemyKilled, TotalCoinsCollected);
    }

    public void PauseGame()
    {
        IsPaused = true;

        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        IsPaused = false;

        Time.timeScale = 1;
    }

    private void StartWave()
    {
        currentPool = 0;
        enemyKilled = 0;

        frequentialSpawnTimer = 0f;
        chunkSpawnTimer = levelData.Waves[currentWave].ChunkSpawnTimer;

        frequentialSpawnAmount = levelData.Waves[currentWave].FrequentialSpawnCount;
        chunkSpawnAmount = levelData.Waves[currentWave].ChunkSpawnAddition;
    }

    private void FinishedWave()
    {
        if (currentWave != levelData.Waves.Length - 1)
        {
            GameUIManager.ActivateWaveCompleted(enemyKilled, CoinsCollected);

            betweenWavesTimer = BetweenWavesCountdown;

            currentWave++;

            StartWave();
        }
        else
        {
            FinishedLevel();
        }

        Manager.Instance.PlayerData.Coins += CoinsCollected;
        CoinsCollected = 0;

        Manager.Instance.Save();
    }

    public void FailedGame()
    {
        Manager.Instance.PlayerData.Coins += CoinsCollected;

        allCurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allCurrentEnemies)
        {
            enemy.GetComponent<Enemy>().Failed();
        }

        UpgradeGained("AirStrike", true);
        UpgradeGained("Freezer", true);
        UpgradeGained("Knife", true);
        UpgradeGained("Lightning", true);
        UpgradeGained("Shield", true);

        GameUIManager.PrepareFailedLevel(TotalEnemyKilled, TotalCoinsCollected);

        IsGameOn = false;
    }


    // Spawn Functions

    private void SpawnChunk(int amount)
    {
        int totalSpawned = currentPool + enemyKilled;

        if (totalSpawned == levelData.Waves[currentWave].PoolCount)
        {
            amount = 0;
        }
        else
        {
            if (levelData.Waves[currentWave].PoolCount - (totalSpawned + amount) < 0)
            {
                int newAmount = levelData.Waves[currentWave].PoolCount - totalSpawned;

                if (EnemyCap - (currentPool + amount) < newAmount)
                {
                    amount = EnemyCap - currentPool;
                }
                else
                {
                    amount = newAmount;
                }
            }
            else
            {
                if (EnemyCap - (currentPool + amount) < 0)
                {
                    amount = EnemyCap - currentPool;
                }
            }
        }

        if (amount > 0)
        {
            currentPool += amount;

            float distanceFromCenter = Vector3.Distance(Vector3.zero, player.transform.position);
            Vector3 spawnPosition;

            if (distanceFromCenter > MaxDistanceFromCenter)
            {
                Ray ray;
                RaycastHit hit;
                Vector3 direction;
                Vector3 position = PlayerPosition;

                position.y = 5.0f;
                calculatedDegrees = new List<float>();

                for (int i = 0; i < Degrees.Count; i++)
                {
                    direction = AngleToDirection(Degrees[i]);

                    ray = new Ray(position, direction);
                    if (Physics.Raycast(ray, out hit, MaxSpawnDistance))
                    {
                        if (hit.transform.tag != "Wall")
                        {
                            calculatedDegrees.Add(Degrees[i]);
                        }
                    }
                    else
                    {
                        calculatedDegrees.Add(Degrees[i]);
                    }
                }
            }
            else
            {
                calculatedDegrees = Degrees;
            }

            for (int i = 0; i < amount; i++)
            {
                spawnPosition = RandomPointInCircle(player.transform.position, MinSpawnDistance, MaxSpawnDistance, calculatedDegrees);
                spawnPosition.x += Random.Range(-100f, 100f) / 100f;
                spawnPosition.z += Random.Range(-100f, 100f) / 100f;
                Instantiate(enemyMinionPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    private Vector3 RandomPointInCircle(Vector3 origin, float bottomRange, float topRange, List<float> degreeList)
    {
        degreeIndex = Mathf.FloorToInt(Random.Range(0.0f, degreeList.Count));
        distance = Random.Range(bottomRange, topRange);

        point = AngleToDirection(degreeList[degreeIndex]) * distance + origin;

        return point;
    }

    private Vector3 AngleToDirection(float degree)
    {
        return new Vector3(Mathf.Sin(Mathf.Deg2Rad * degree), 0.0f, Mathf.Cos(Mathf.Deg2Rad * degree));
    }


    // Gameplay Functions

    public void SetDefaultUpgrades(string chosenClass)
    {
        Manager.Instance.PlayerData.ResetUpgradeLevels();

        UpgradeGained("AirStrike", true);
        UpgradeGained("Freezer", true);
        UpgradeGained("Knife", true);
        UpgradeGained("Lightning", true);
        UpgradeGained("Shield", true);

        UpgradeGained("GunslingerDamage", true);
        UpgradeGained("GunslingerRange", true);
        UpgradeGained("GunslingerRateOfFire", true);
        UpgradeGained("RiflemanDamage", true);
        UpgradeGained("RiflemanRange", true);
        UpgradeGained("RiflemanRateOfFire", true);
        UpgradeGained("RocketeerDamage", true);
        UpgradeGained("RocketeerRange", true);
        UpgradeGained("RocketeerReloadSpeed", true);

        UpgradeGained("HealthRegen", true);
        UpgradeGained("Luck", true);
        UpgradeGained("Magnet", true);
        UpgradeGained("MaxHealth", true);
        UpgradeGained("MovementSpeed", true);
        UpgradeGained("XPGain", true);

        availableUpgrades.Add("AirStrike");
        availableUpgrades.Add("Freezer");
        availableUpgrades.Add("Knife");
        availableUpgrades.Add("Lightning");
        availableUpgrades.Add("Shield");

        availableUpgrades.Add(chosenClass + "Damage");
        availableUpgrades.Add(chosenClass + "Range");
        if (chosenClass == "Rocketeer")
        {
            availableUpgrades.Add("RocketeerReloadSpeed");
        }
        else
        {
            availableUpgrades.Add(chosenClass + "RateOfFire");
        }

        availableUpgrades.Add("HealthRegen");
        availableUpgrades.Add("Luck");
        availableUpgrades.Add("Magnet");
        availableUpgrades.Add("MaxHealth");
        availableUpgrades.Add("MovementSpeed");
        availableUpgrades.Add("XPGain");
    }

    private void FreezeAllEnemies()
    {
        allCurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        float duration = Manager.Instance.CollectibleData.FreezeAllDuration;

        foreach (GameObject enemy in allCurrentEnemies)
        {
            if (!enemy.GetComponent<Enemy>().IsDead)
            {
                enemy.GetComponent<Enemy>().Freeze(duration);
            }
        }

        allCurrentEnemies = null;
    }

    private void KillAllEnemies()
    {
        allCurrentEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allCurrentEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(1000);

            nukeExplosionObject = Instantiate(NukeExplosionPrefab, enemy.transform.position, Quaternion.identity);
            Destroy(nukeExplosionObject, 1.25f);
        }

        allCurrentEnemies = null;
    }

    public void CollectibleEarned(CollectibleTypes type)
    {
        switch (type)
        {
            case CollectibleTypes.Health:

                player.Heal(Manager.Instance.CollectibleData.HealAmount, true);

                break;

            case CollectibleTypes.XP_Small:

                XPEarned(20f);

                player.XPEarned();

                break;

            case CollectibleTypes.XP_Big:

                XPEarned(50f);

                player.XPEarned();

                break;

            case CollectibleTypes.Coin_Small:

                CoinEarned(10);

                player.CoinEarned();

                break;

            case CollectibleTypes.Coin_Big:

                CoinEarned(50);

                player.CoinEarned();

                break;

            case CollectibleTypes.Powerup_MagnetAll:

                player.PowerupMagnetAll();

                break;

            case CollectibleTypes.Powerup_FreezeAll:

                FreezeAllEnemies();

                break;

            case CollectibleTypes.Powerup_Nuke:

                KillAllEnemies();

                break;

            default:

                break;
        }
    }

    private void XPEarned(float amount)
    {
        CurrentXP += amount * XPMultiplier;

        if (CurrentXP >= Manager.Instance.XPData.RequiredAmount[Mathf.FloorToInt(Mathf.Clamp(CurrentPlayerLevel + 1, 0, 41))])
        {
            CurrentPlayerLevel++;
            CurrentXP -= Mathf.Clamp(Manager.Instance.XPData.RequiredAmount[Mathf.FloorToInt(Mathf.Clamp(CurrentPlayerLevel, 0, 41))], 0, float.MaxValue);

            PlayerLevelUp();
        }

        GameUIManager.UpdateXPBar((CurrentXP / Manager.Instance.XPData.RequiredAmount[Mathf.FloorToInt(Mathf.Clamp(CurrentPlayerLevel + 1, 0, 41))]) * 100f);
    }

    private void CoinEarned(int amount)
    {
        CoinsCollected += amount;
        TotalCoinsCollected += amount;

        GameUIManager.UpdateHUD(TotalEnemyKilled, TotalCoinsCollected);
    }

    private void PlayerLevelUp()
    {
        string[] upgradeNames;

        if (availableUpgrades.Count < 3)
        {
            if (availableUpgrades.Count == 2)
            {
                upgradeNames = new string[2];
                upgradeNames[0] = availableUpgrades[0];
                upgradeNames[1] = availableUpgrades[1];

                PauseGame();
                GameUIManager.PrepareUpgradeSelection(upgradeNames);
            }
            else if (availableUpgrades.Count == 1)
            {
                upgradeNames = new string[1];
                upgradeNames[0] = availableUpgrades[0];

                PauseGame();
                GameUIManager.PrepareUpgradeSelection(upgradeNames);
            }
            else if (availableUpgrades.Count == 0)
            {
                player.Heal(200, true);
            }
        }
        else
        {
            List<int> indexList = new List<int>();
            for (int i = 0; i < availableUpgrades.Count; i++)
            {
                indexList.Add(i);
            }
            upgradeNames = new string[3];
            int random, randomIndex;

            for (int i = 0; i < 3; i++)
            {
                randomIndex = Mathf.FloorToInt(Random.Range(0, indexList.Count));
                random = indexList[randomIndex];
                indexList.RemoveAt(randomIndex);

                upgradeNames[i] = availableUpgrades[random];
            }

            PauseGame();
            GameUIManager.PrepareUpgradeSelection(upgradeNames);
        }
    }

    public void UpgradeSelected(string upgradeName)
    {
        if (Manager.Instance.PlayerData.Upgrades[upgradeName].CurrentLevel == 2)
        {
            if (availableUpgrades.Contains(upgradeName))
            {
                availableUpgrades.Remove(upgradeName);
                maxedUpgrades.Add(upgradeName);
            }
        }

        UpgradeGained(upgradeName, false);

        if (IsPaused)
        {
            UnpauseGame();
        }
    }

    public void UpgradeGained(string upgradeName, bool isDefault)
    {
        if (!isDefault)
        {
            Manager.Instance.PlayerData.Upgrades[upgradeName].IncreaseLevel();
        }

        int level = Manager.Instance.PlayerData.Upgrades[upgradeName].CurrentLevel;

        if (Manager.Instance.PlayerData.Upgrades[upgradeName].UpgradeType == UpgradeType.Active)
        {
            player.UpgradeSkill(upgradeName, level);
        }
        else
        {
            float value = Manager.Instance.PlayerData.Upgrades[upgradeName].LevelValues[0].Values[level];

            if (Manager.Instance.PlayerData.Upgrades[upgradeName].ClassType == ClassType.General)
            {
                switch (upgradeName)
                {
                    case "HealthRegen":

                        player.UpgradeHealthRegen(value);

                        break;

                    case "Luck":

                        Luck = value;

                        break;

                    case "Magnet":

                        player.UpgradeMagnet(value);

                        break;

                    case "MaxHealth":

                        player.UpgradeMaxHealth(Mathf.FloorToInt(value));

                        break;

                    case "MovementSpeed":

                        player.UpgradeMovementSpeed(value);

                        break;

                    case "XPGain":

                        XPMultiplier = value;

                        break;

                    default:

                        break;
                }
            }
            else
            {
                switch (upgradeName)
                {
                    case "GunslingerDamage":

                        player.GunslingerClass.UpgradeDamage(Mathf.FloorToInt(value));

                        break;

                    case "GunslingerRange":

                        player.GunslingerClass.UpgradeRange(value);

                        break;

                    case "GunslingerRateOfFire":

                        player.GunslingerClass.UpgradeRateOfFire(value);

                        break;

                    case "RiflemanDamage":

                        player.RiflemanClass.UpgradeDamage(Mathf.FloorToInt(value));

                        break;

                    case "RiflemanRange":

                        player.RiflemanClass.UpgradeRange(value);

                        break;

                    case "RiflemanRateOfFire":

                        player.RiflemanClass.UpgradeRateOfFire(value);

                        break;

                    case "RocketeerDamage":

                        player.RocketeerClass.UpgradeDamage(Mathf.FloorToInt(value));

                        break;

                    case "RocketeerRange":

                        player.RocketeerClass.UpgradeRange(value);

                        break;

                    case "RocketeerReloadSpeed":

                        player.RocketeerClass.UpgradeRateOfFire(value);

                        break;

                    default:

                        break;
                }
            }
        }
    }

    public void EnemyKilled(Vector3 position)
    {
        currentPool--;
        enemyKilled++;

        TotalEnemyKilled++;
        GameUIManager.UpdateHUD(TotalEnemyKilled, TotalCoinsCollected);

        randomValue = Mathf.FloorToInt(Random.Range(0, 100));

        if (randomValue <= Luck)
        {
            randomValue = Mathf.FloorToInt(Random.Range(0, 100));

            if (randomValue <= 69)
            {
                collectibleName = "XP_Small";
            }
            else if (randomValue >= 70 && randomValue <= 80)
            {
                collectibleName = "XP_Big";
            }
            else if (randomValue >= 81 && randomValue <= 84)
            {
                collectibleName = "Coin_Small";
            }
            else if (randomValue >= 85 && randomValue <= 86)
            {
                collectibleName = "Coin_Big";
            }
            else if (randomValue >= 87 && randomValue <= 94)
            {
                collectibleName = "Health";
            }
            else if (randomValue >= 95 && randomValue <= 96)
            {
                collectibleName = "Powerup_MagnetAll";
            }
            else if (randomValue >= 97 && randomValue <= 98)
            {
                collectibleName = "Powerup_FreezeAll";
            }
            else if (randomValue == 99)
            {
                collectibleName = "Powerup_Nuke";
            }

            position.y = 0f;
            collectibleObject = Instantiate(Manager.Instance.CollectibleData.Prefabs[collectibleName], position, Quaternion.identity);
        }

        if (currentPool == 0 && enemyKilled == levelData.Waves[currentWave].PoolCount)
        {
            FinishedWave();
        }
    }

    public void PlayerDamage()
    {
        GameUIManager.PlayerDamage();
    }
}
