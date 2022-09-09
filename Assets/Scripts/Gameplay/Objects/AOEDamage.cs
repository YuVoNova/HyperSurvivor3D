using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    private int Damage;

    public void SetValues(int damage, float radius)
    {
        Damage = damage;
        transform.localScale = new Vector3(radius, 0.1f, radius);
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
}
