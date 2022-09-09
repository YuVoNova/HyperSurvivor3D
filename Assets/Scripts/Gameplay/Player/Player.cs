using UnityEngine;

public class Player : MonoBehaviour
{
    // Objects & Components

    private Rigidbody rigidbody;

    private PlayerController controller;

    [HideInInspector]
    public Classes ActiveClass;

    [HideInInspector]
    public BaseClass BaseClass;

    public GunslingerClass GunslingerClass;
    public RiflemanClass RiflemanClass;
    public RocketeerClass RocketeerClass;

    [SerializeField]
    private PlayerUI PlayerUI;
    [SerializeField]
    private Magnet Magnet;

    [SerializeField]
    private Material PlayerMaterial;
    [SerializeField]
    private Material InvincibilityMaterial;

    [SerializeField]
    private GameObject Damage_VFX;
    [SerializeField]
    private GameObject Heal_VFX;
    [SerializeField]
    private GameObject XP_VFX;
    [SerializeField]
    private GameObject Coin_VFX;


    // Skills

    [SerializeField]
    private Lightning Lightning;
    [SerializeField]
    private Shield Shield;
    [SerializeField]
    private Knife Knife;
    [SerializeField]
    private Freezer Freezer;
    [SerializeField]
    private AirStrike AirStrike;


    // Values

    #region Values From Upgrades

    private int MaxHealth;

    private float MagnetRange;

    private int HealthPerSecond;

    #endregion

    private float health;

    [SerializeField]
    private int EnemyDamageValue;
    [SerializeField]
    private float InvincibilityDuration;

    [SerializeField]
    private float HealCooldown;

    private float invincibilityTimer;
    private bool isInvincible;

    private float healTimer;

    private bool isMagnetAllActivated;


    // Unity Functions

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        controller = GetComponent<PlayerController>();

        BaseClass = GunslingerClass;

        PlayerUI.gameObject.SetActive(false);

        GunslingerClass.gameObject.SetActive(false);
        RiflemanClass.gameObject.SetActive(false);
        RocketeerClass.gameObject.SetActive(false);
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (invincibilityTimer > 0f)
            {
                invincibilityTimer -= Time.deltaTime;

                if (invincibilityTimer <= 0f)
                {
                    BaseClass.Renderer.material = PlayerMaterial;
                    GetComponent<Collider>().isTrigger = false;
                    isInvincible = false;
                }
            }

            if (healTimer > 0f)
            {
                healTimer -= Time.deltaTime;

                if (healTimer <= 0f)
                {
                    Heal(HealthPerSecond, false);
                    healTimer = HealCooldown;
                }
            }
        }
    }

    private void FixedUpdate()
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (collision.gameObject.layer == 7) // Enemy
            {
                if (!isInvincible && !collision.gameObject.GetComponent<Enemy>().IsFrozen)
                {
                    TakeDamage(EnemyDamageValue);
                }
            }
        }
    }


    // Functions

    public void SetClass(Classes chosenClass)
    {
        ActiveClass = chosenClass;
    }

    public void StartLevel()
    {
        switch (ActiveClass)
        {
            case Classes.Gunslinger:
                BaseClass = GunslingerClass;
                break;
            case Classes.Rifleman:
                BaseClass = RiflemanClass;
                break;
            case Classes.Rocketeer:
                BaseClass = RocketeerClass;
                break;
            default:
                break;
        }
        BaseClass.gameObject.SetActive(true);

        PlayerUI.gameObject.SetActive(true);

        invincibilityTimer = 0f;
        isInvincible = false;

        healTimer = HealCooldown;

        health = MaxHealth;

        Magnet.SetRadius(MagnetRange);

        isMagnetAllActivated = false;

        BaseClass.StartLevel();
    }

    private void UpdateHealthBar()
    {
        PlayerUI.HealthBar.value = health / (float)MaxHealth * 100f;
    }

    public void Heal(int amount, bool isCollectible)
    {
        health = Mathf.Clamp(health + amount, 0f, MaxHealth);

        UpdateHealthBar();

        if (isCollectible)
        {
            Heal_VFX.SetActive(false);
            Heal_VFX.SetActive(true);
        }
    }

    private void TakeDamage(int damage)
    {
        if (!Shield.UseShield())
        {
            health = Mathf.Clamp(health - damage, 0, MaxHealth);

            GameManager.Instance.PlayerDamage();
        }

        UpdateHealthBar();

        Damage_VFX.SetActive(false);
        Damage_VFX.SetActive(true);

        if (health <= 0)
        {
            Death();
        }
        else
        {
            invincibilityTimer = InvincibilityDuration;
            BaseClass.Renderer.material = InvincibilityMaterial;
            GetComponent<Collider>().isTrigger = true;
            isInvincible = true;
        }
    }

    private void Death()
    {
        BaseClass.Animator.SetBool("isWalk", false);

        GameManager.Instance.FailedGame();
    }


    // Upgrade Functions

    public void UpgradeSkill(string upgradedSkillName, int level)
    {
        switch (upgradedSkillName)
        {
            case "AirStrike":

                AirStrike.ChangeLevel(level);

                break;

            case "Freezer":

                Freezer.ChangeLevel(level);

                break;

            case "Knife":

                Knife.ChangeLevel(level);

                break;

            case "Lightning":

                Lightning.ChangeLevel(level);

                break;

            case "Shield":

                Shield.ChangeLevel(level);

                break;

            default:

                break;
        }
    }

    public void UpgradeHealthRegen(float value)
    {
        HealthPerSecond = Mathf.FloorToInt(value);
    }

    public void UpgradeMagnet(float value)
    {
        MagnetRange = value;

        Magnet.SetRadius(MagnetRange);
    }

    public void UpgradeMaxHealth(int value)
    {
        MaxHealth = value;

        health = MaxHealth;
        UpdateHealthBar();
    }

    public void UpgradeMovementSpeed(float value)
    {
        controller.UpgradeMovementSpeed(value);
    }


    // Collectible Functions

    public void XPEarned()
    {
        XP_VFX.SetActive(false);
        XP_VFX.SetActive(true);
    }

    public void CoinEarned()
    {
        Coin_VFX.SetActive(false);
        Coin_VFX.SetActive(true);
    }

    public void PowerupMagnetAll()
    {
        if (!isMagnetAllActivated)
        {
            isMagnetAllActivated = true;

            Magnet.SetRadius(200f);

            Invoke("DisableMagnetAll", 0.5f);
        }
    }

    private void DisableMagnetAll()
    {
        Magnet.SetRadius(MagnetRange);

        isMagnetAllActivated = false;
    }
}

public enum Classes
{
    Gunslinger,
    Rifleman,
    Rocketeer
}
