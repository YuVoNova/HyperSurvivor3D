using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider HealthBar;

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
