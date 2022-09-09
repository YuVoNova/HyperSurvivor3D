using UnityEngine;

public class Freezer : MonoBehaviour
{
    private int CurrentLevel;

    [SerializeField]
    private GameObject FreezerProjectile;

    [SerializeField]
    private float Cooldown;

    private float Duration;

    private float timer;
    private GameObject projectile;
    private Vector3 angle;

    private void Awake()
    {
        CurrentLevel = 0;

        timer = 0f;
        angle = Vector3.zero;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (CurrentLevel > 0)
            {
                if (timer <= 0f)
                {
                    ShootFreeze();

                    timer = Cooldown;
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
        }
    }

    private void ShootFreeze()
    {
        angle.y = (angle.y + 45f) % 360;

        projectile = Instantiate(FreezerProjectile, transform.position, Quaternion.identity) as GameObject;
        projectile.transform.rotation = Quaternion.LookRotation(transform.forward);
        projectile.transform.eulerAngles = angle;
        projectile.GetComponent<FreezerProjectile>().SetValues(Duration);
        Destroy(projectile, 0.35f);
    }

    public void ChangeLevel(int level)
    {
        CurrentLevel = level;

        if (CurrentLevel == 0)
        {

        }

        Duration = Manager.Instance.PlayerData.Upgrades["Freezer"].LevelValues[0].Values[level];
    }
}
