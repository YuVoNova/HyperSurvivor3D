using UnityEngine;

public class Lightning : MonoBehaviour
{
    private int CurrentLevel;

    private float LightningAOERadius;
    private int LightningCount;

    [SerializeField]
    private Range EnemyRange;
    [SerializeField]
    private int Damage;
    [SerializeField]
    private float Cooldown;
    [SerializeField]
    private GameObject LightningObject;

    private float timer;

    private GameObject lightningObj;

    private void Awake()
    {
        timer = 0f;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (CurrentLevel > 0)
            {
                if (timer <= 0f)
                {
                    if (EnemyRange.TargetTransforms.Count >= LightningCount)
                    {
                        SpawnLightning();

                        timer = Cooldown;
                    }
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
        }
    }

    private void SpawnLightning()
    {
        int random;
        for (int i = 0; i < LightningCount; i++)
        {
            random = Random.Range(0, EnemyRange.TargetTransforms.Count - 1);

            lightningObj = Instantiate(LightningObject, EnemyRange.TargetTransforms[random].position, Quaternion.identity);
            lightningObj.GetComponent<LightningObject>().AOEDamage.SetValues(Damage, LightningAOERadius);

            Destroy(lightningObj.GetComponent<LightningObject>().AOEDamage, 0.2f);
            Destroy(lightningObj, 1f);
        }

    }

    public void ChangeLevel(int level)
    {
        CurrentLevel = level;

        
        if (CurrentLevel == 0)
        {

        }

        LightningAOERadius = Manager.Instance.PlayerData.Upgrades["Lightning"].LevelValues[0].Values[level];
        LightningCount = Mathf.FloorToInt(Manager.Instance.PlayerData.Upgrades["Lightning"].LevelValues[1].Values[level]);
    }
}
