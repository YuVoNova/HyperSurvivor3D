using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField]
    private CollectibleTypes CollectibleType;

    private bool isMagnetized;

    private void Awake()
    {
        isMagnetized = false;
    }

    private void FixedUpdate()
    {
        if (isMagnetized && GameManager.Instance.IsGameOn)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.PlayerPosition, 25f * Time.deltaTime);

            if (Vector3.Distance(transform.position, GameManager.Instance.PlayerPosition) < 0.2f)
            {
                GameManager.Instance.CollectibleEarned(CollectibleType);

                Destroy(gameObject);
            }
        }
    }

    public void Magnetize()
    {
        isMagnetized = true;
    }
}

public enum CollectibleTypes
{
    Health,
    XP_Small,
    XP_Big,
    Coin_Small,
    Coin_Big,
    Powerup_MagnetAll,
    Powerup_FreezeAll,
    Powerup_Nuke
}
