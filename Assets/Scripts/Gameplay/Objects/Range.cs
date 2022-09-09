using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    [SerializeField]
    private float radius;

    [SerializeField]
    private int targetLayer;

    //[HideInInspector]
    public List<Transform> TargetTransforms;

    private void Awake()
    {
        TargetTransforms = new List<Transform>();

        SetRadius();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            if (!TargetTransforms.Contains(other.transform))
            {
                TargetTransforms.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            if (TargetTransforms.Contains(other.transform))
            {
                TargetTransforms.Remove(other.transform);
            }
        }
    }

    public void RemoveMe(Transform removeTransform)
    {
        if (TargetTransforms.Contains(removeTransform))
        {
            TargetTransforms.Remove(removeTransform);
        }
    }

    private void SetRadius()
    {
        transform.localScale = new Vector3(radius * 2, transform.localScale.y, radius * 2);
    }

    public void ChangeRadius(float r)
    {
        radius = r;

        SetRadius();
    }
}
