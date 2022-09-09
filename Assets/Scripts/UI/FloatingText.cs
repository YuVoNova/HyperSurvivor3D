using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private float destroyTime;

    public TMP_Text text;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
