using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected float Health;

    [SerializeField]
    protected float MaxHealth;
    [SerializeField]
    protected float MinMovementSpeed;
    [SerializeField]
    protected float MaxMovementSpeed;
    [SerializeField]
    protected float Damage;

    protected float MovementSpeed;
    protected float FreezeDuration;

    [SerializeField]
    protected Animator Animator;
    [SerializeField]
    protected Rigidbody Rigidbody;
    [SerializeField]
    protected Collider Collider;
    [SerializeField]
    protected Transform FloatingTextParent;
    [SerializeField]
    protected SkinnedMeshRenderer Renderer;
    [SerializeField]
    protected Material NormalMaterial;
    [SerializeField]
    protected Material FrozenMaterial;
    [SerializeField]
    protected Material DeathMaterial;

    [SerializeField]
    private List<Range> ranges;

    private Vector3 target;
    private Vector3 direction;
    private float freezeTimer;

    public bool IsFrozen;
    public bool IsDead;

    protected virtual void Awake()
    {
        Health = MaxHealth;

        MovementSpeed = Random.Range(MinMovementSpeed, MaxMovementSpeed);

        ranges = new List<Range>();

        freezeTimer = 0f;
        IsFrozen = false;

        IsDead = false;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (IsFrozen)
        {
            if (freezeTimer > 0f)
            {
                freezeTimer -= Time.deltaTime;
            }
            else
            {
                Defreeze();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOn && !IsFrozen && !IsDead)
        {
            target = GameManager.Instance.PlayerPosition;
            direction = Vector3.Normalize(target - transform.position);
            transform.rotation = Quaternion.LookRotation(direction);
            Rigidbody.velocity = direction * MovementSpeed * Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (!ranges.Contains(other.GetComponent<Range>()))
            {
                ranges.Add(other.GetComponent<Range>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (ranges.Contains(other.GetComponent<Range>()))
            {
                ranges.Remove(other.GetComponent<Range>());
            }
        }
    }

    public virtual void Freeze(float duration)
    {
        freezeTimer = duration;

        if (!IsFrozen)
        {
            IsFrozen = true;

            Renderer.material = FrozenMaterial;

            Rigidbody.isKinematic = true;
        }
    }

    protected virtual void Defreeze()
    {
        IsFrozen = false;
        freezeTimer = 0f;

        Renderer.material = NormalMaterial;

        Rigidbody.isKinematic = false;
    }

    private void ShowFloatingText(string text)
    {
        GameObject textObject = Instantiate(GameManager.Instance.FloatingTextPrefab, FloatingTextParent);
        textObject.GetComponent<FloatingText>().text.text = text;
    }

    public virtual void TakeDamage(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);

        ShowFloatingText(damage + "");

        if (Health <= 0 && !IsDead)
        {
            Death();
        }
    }

    public virtual void Death()
    {
        IsDead = true;

        if (IsFrozen)
        {
            Defreeze();
        }

        Rigidbody.isKinematic = true;
        Collider.enabled = false;

        for (int i = 0; i < ranges.Count; i++)
        {
            ranges[i].RemoveMe(transform);
        }

        Renderer.material = DeathMaterial;

        GameManager.Instance.EnemyKilled(transform.position);
    }

    public virtual void Failed()
    {

    }
}
