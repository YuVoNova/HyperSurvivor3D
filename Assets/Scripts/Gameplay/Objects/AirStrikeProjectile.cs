using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrikeProjectile : MonoBehaviour
{
    private int Damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            other.GetComponent<Enemy>().TakeDamage(Damage);
        }
    }

    public void SetValues(int damage)
    {
        Damage = damage;
    }
}
