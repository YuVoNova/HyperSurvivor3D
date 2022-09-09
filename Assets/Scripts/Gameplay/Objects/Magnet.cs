using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private int targetLayer = 8;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            other.GetComponent<Collectible>().Magnetize();
        }
    }

    public void SetRadius(float radius)
    {
        transform.localScale = new Vector3(radius, 0.1f, radius);
    }
}
