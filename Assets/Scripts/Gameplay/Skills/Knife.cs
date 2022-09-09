using UnityEngine;

public class Knife : MonoBehaviour
{
    private int CurrentLevel;

    [SerializeField]
    private GameObject KnifeProjectile;
    [SerializeField]
    private float Cooldown;
    [SerializeField]
    private int Damage;
    
    private float Range;

    private float timer;
    private GameObject projectile;

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
                    ShootKnife();

                    timer = Cooldown;
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
        }
    }

    private void ShootKnife()
    {
        projectile = Instantiate(KnifeProjectile, transform.position, Quaternion.identity) as GameObject;
        projectile.transform.rotation = Quaternion.LookRotation(transform.forward);
        projectile.GetComponent<KnifeProjectile>().SetValues(Range, Damage);
    }

    public void ChangeLevel(int level)
    {
        CurrentLevel = level;

        if (CurrentLevel == 0)
        {

        }

        Range = Manager.Instance.PlayerData.Upgrades["Knife"].LevelValues[0].Values[level];
    }
}
