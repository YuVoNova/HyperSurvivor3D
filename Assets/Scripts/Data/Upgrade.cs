using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public string Name;
    public GameObject UIPrefab;
    public LevelEntry[] LevelValues;
    public UpgradeType UpgradeType;
    public ClassType ClassType;

    [HideInInspector]
    public int CurrentLevel;

    [System.Serializable]
    public class LevelEntry
    {
        public string ValueIdentifier;
        public float[] Values;
    }

    public Upgrade()
    {
        CurrentLevel = 0;
    }

    public void IncreaseLevel()
    {
        CurrentLevel = Mathf.FloorToInt(Mathf.Clamp(CurrentLevel + 1, 0, 3));
    }
}

public enum UpgradeType
{
    Passive,
    Active
}

public enum ClassType
{
    General,
    Gunslinger,
    Rifleman,
    Rocketeer
}
