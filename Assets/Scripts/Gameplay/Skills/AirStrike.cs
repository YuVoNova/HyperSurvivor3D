using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrike : MonoBehaviour
{
    private int CurrentLevel;

    private float ReloadTime;
    private int Damage;

    [SerializeField]
    private GameObject AirStrikeArea;
    [SerializeField]
    private Transform ReferencePoint;
    [SerializeField]
    private float RotationAmount;
    [SerializeField]
    private float RateOfFire;

    [SerializeField]
    private GameObject AirStrikeProjectile;

    private bool isFiring;

    private Vector3 originAngle;
    private Vector3 targetAngle;

    private float reloadTimer;
    private float fireTimer;

    private float degree;
    private float distance;
    private Vector3 point;

    private GameObject projectile;

    private void Awake()
    {
        originAngle = Vector3.zero;
        targetAngle = Vector3.zero;

        reloadTimer = 0f;
        fireTimer = 0f;

        AirStrikeArea.SetActive(false);

        isFiring = false;
    }

    private void Update()
    {
        if (CurrentLevel > 0)
        {
            if (GameManager.Instance.IsGameOn)
            {
                if (isFiring)
                {
                    AirStrikeArea.transform.localRotation = Quaternion.RotateTowards(Quaternion.Euler(AirStrikeArea.transform.localEulerAngles), Quaternion.Euler(targetAngle), 25f * Time.deltaTime);

                    if (fireTimer <= 0f)
                    {
                        DropProjectile();

                        fireTimer = RateOfFire;
                    }
                    else
                    {
                        fireTimer -= Time.deltaTime;
                    }

                    if (Vector3.Distance(AirStrikeArea.transform.localEulerAngles, targetAngle) < 0.2f)
                    {
                        AirStrikeArea.SetActive(false);
                        reloadTimer = ReloadTime;
                        isFiring = false;
                    }
                }
                else
                {
                    if (reloadTimer <= 0f)
                    {
                        SetArea();

                        AirStrikeArea.SetActive(true);
                        isFiring = true;
                    }
                    else
                    {
                        reloadTimer -= Time.deltaTime;
                    }
                }
            }
            else
            {
                if (AirStrikeArea.activeSelf)
                {
                    AirStrikeArea.SetActive(false);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (AirStrikeArea.activeSelf)
            {
                AirStrikeArea.transform.position = GameManager.Instance.PlayerPosition;
            }
        }
    }

    private void SetArea()
    {
        originAngle.y = Random.Range(0f, 360f - RotationAmount);
        AirStrikeArea.transform.localRotation = Quaternion.Euler(originAngle);

        targetAngle.y = originAngle.y + RotationAmount;
    }

    private void DropProjectile()
    {
        degree = Random.Range(0f, 360f);
        distance = Random.Range(0f, 2.5f);

        point = new Vector3(Mathf.Sin(Mathf.Deg2Rad * degree), 0.25f, Mathf.Cos(Mathf.Deg2Rad * degree)) * distance + ReferencePoint.position;

        projectile = Instantiate(AirStrikeProjectile, point, Quaternion.identity) as GameObject;
        projectile.GetComponent<AirStrikeProjectile>().SetValues(Damage);
        Destroy(projectile, 0.35f);
    }

    public void ChangeLevel(int level)
    {
        CurrentLevel = level;

        if (CurrentLevel == 0)
        {
            AirStrikeArea.SetActive(false);
            isFiring = false;
        }

        ReloadTime = Manager.Instance.PlayerData.Upgrades["AirStrike"].LevelValues[0].Values[level];
        Damage = Mathf.FloorToInt(Manager.Instance.PlayerData.Upgrades["AirStrike"].LevelValues[1].Values[level]);
    }
}
