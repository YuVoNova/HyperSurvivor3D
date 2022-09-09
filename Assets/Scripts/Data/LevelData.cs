using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    public int ID;

    public Wave[] Waves;

    [System.Serializable]
    public class Wave
    {
        public int PoolCount;
        public int BossCount;

        public float FrequentialSpawnTimer;
        public int FrequentialSpawnCount;

        public float ChunkSpawnTimer;
        public int ChunkSpawnAddition;
    }
}
