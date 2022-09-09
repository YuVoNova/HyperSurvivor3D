using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 target;

    private void Awake()
    {
        offset = transform.position;
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        target = GameManager.Instance.PlayerPosition + offset;
        transform.position = Vector3.Lerp(transform.position, target, 5.0f * Time.deltaTime);
    }
}
