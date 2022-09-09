using UnityEngine;

public class BaseClass : MonoBehaviour
{
    // Objects & Components

    public Animator Animator;

    public SkinnedMeshRenderer Renderer;


    // Target Finder

    [SerializeField]
    protected Range EnemyRange;
    [SerializeField]
    protected float SetTargetCooldown;

    private float setTargetTimer;
    private float minDistance;

    [HideInInspector]
    public Transform ClosestTargetTransform;


    // Shooter
    
    [SerializeField]
    protected int AmmoCapacity;
    [SerializeField]
    protected GameObject ProjectilePrefab;
    [SerializeField]
    protected float ProjectileSpeed;
    [SerializeField]
    protected Transform[] ProjectileSpawnPoints;

    private bool hasReloaded;
    private int projectileSpawnIndex;

    private float reloadTimer;
    private float fireTimer;

    [HideInInspector]
    public float CurrentAmmo;

    #region Values From Upgrades

    protected int Damage;

    protected float RangeRadius;

    [SerializeField]
    protected float RateOfFire;
    [SerializeField]
    protected float ReloadTime;

    #endregion


    // Unity Functions

    protected void Awake()
    {

    }

    protected void Start()
    {
        EnemyRange.ChangeRadius(RangeRadius);
    }

    protected void Update()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (setTargetTimer > 0.0f)
            {
                setTargetTimer -= Time.deltaTime;
            }
            else
            {
                FindClosestTarget();

                setTargetTimer = SetTargetCooldown;
            }

            if (reloadTimer <= 0.0f)
            {
                if (!hasReloaded)
                {
                    CurrentAmmo = AmmoCapacity;

                    hasReloaded = true;
                }

                if (fireTimer <= 0.0f)
                {
                    FireProjectile();
                    CurrentAmmo--;

                    if (CurrentAmmo <= 0)
                    {
                        reloadTimer = ReloadTime;
                    }

                    fireTimer = RateOfFire;
                }
                else
                {
                    fireTimer -= Time.deltaTime;
                }
            }
            else
            {
                reloadTimer -= Time.deltaTime;

                // TO DO -> Reload bar animation.

                if (reloadTimer <= 0.0f)
                {
                    hasReloaded = false;
                }
            }
        }
    }


    // Functions

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(ProjectilePrefab, ProjectileSpawnPoints[projectileSpawnIndex].position, Quaternion.identity) as GameObject;
        projectile.transform.rotation = Quaternion.LookRotation(transform.parent.forward);
        projectile.GetComponent<Projectile>().SetValues(ProjectileSpeed * projectile.transform.forward, RangeRadius, Damage);

        if (ProjectileSpawnPoints.Length > 1)
        {
            projectileSpawnIndex++;

            if (projectileSpawnIndex == ProjectileSpawnPoints.Length)
            {
                projectileSpawnIndex = 0;
            }
        }
    }

    private void FindClosestTarget()
    {
        if (EnemyRange.TargetTransforms.Count < 1)
        {
            if (ClosestTargetTransform != null)
            {
                ClosestTargetTransform = null;
            }
        }
        else
        {
            minDistance = 100.0f;

            for (int i = 0; i < EnemyRange.TargetTransforms.Count; i++)
            {
                if (Vector3.Distance(GameManager.Instance.PlayerPosition, EnemyRange.TargetTransforms[i].position) < minDistance)
                {
                    ClosestTargetTransform = EnemyRange.TargetTransforms[i];
                    minDistance = Vector3.Distance(GameManager.Instance.PlayerPosition, EnemyRange.TargetTransforms[i].position);
                }
            }
        }
    }

    public void StartLevel()
    {
        setTargetTimer = SetTargetCooldown;

        CurrentAmmo = AmmoCapacity;

        hasReloaded = true;
        projectileSpawnIndex = 0;

        fireTimer = RateOfFire;
        reloadTimer = 0.0f;
    }

    public virtual void UpgradeDamage(int value)
    {
        Damage = value;
    }

    public virtual void UpgradeRange(float value)
    {
        RangeRadius = value;

        EnemyRange.ChangeRadius(RangeRadius);
    }

    public virtual void UpgradeRateOfFire(float value)
    {

    }
}
