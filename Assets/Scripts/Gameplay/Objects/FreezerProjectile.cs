using UnityEngine;

public class FreezerProjectile : MonoBehaviour
{
    private float Duration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (!other.GetComponent<Enemy>().IsDead)
            {
                other.GetComponent<Enemy>().Freeze(Duration);
            }
        }
    }

    public void SetValues(float duration)
    {
        Duration = duration;
    }
}
