using UnityEngine;

public class KnifeProjectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigidbody;

    private int Damage;
    private float Range;

    private Vector3 origin;
    private Vector3 velocity;

    private void Start()
    {
        origin = transform.position;
        velocity = transform.forward * 20f;
        Range = 100f;
    }

    private void FixedUpdate()
    {
        Rigidbody.velocity = velocity;

        if (Vector3.Distance(origin, transform.position) > Range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (!other.GetComponent<Enemy>().IsDead)
            {
                other.GetComponent<Enemy>().TakeDamage(Damage);
            }
        }
    }

    public void SetValues(float range, int damage)
    {
        Range = range;
        Damage = damage;
    }
}
