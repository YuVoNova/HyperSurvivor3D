using UnityEngine;

public class Shield : MonoBehaviour
{
    private int CurrentLevel;
    private int ActiveShields;

    [SerializeField]
    private GameObject[] Shields;
    [SerializeField]
    private GameObject VFX;
    [SerializeField]
    private float Cooldown;
    [SerializeField]
    private Transform FloatingTextParent;

    private float timer;

    private void Awake()
    {
        CurrentLevel = 0;
        ActiveShields = 0;

        VFX.SetActive(false);
        Shields[0].SetActive(false);
        Shields[1].SetActive(false);
        Shields[2].SetActive(false);

        timer = Cooldown;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOn)
        {
            if (CurrentLevel > 0)
            {
                if (ActiveShields < CurrentLevel)
                {
                    if (timer <= 0f)
                    {
                        ActiveShields++;

                        for (int i = 0; i < ActiveShields; i++)
                        {
                            if (!Shields[i].activeSelf)
                            {
                                Shields[i].SetActive(true);
                            }
                        }

                        timer = Cooldown;
                    }
                    else
                    {
                        timer -= Time.deltaTime;
                    }
                }
            }
        }
    }

    private void ShowBlockedText()
    {
        GameObject textObject = Instantiate(GameManager.Instance.FloatingTextPrefab, FloatingTextParent);
        textObject.GetComponent<FloatingText>().text.text = "BLOCKED!";
    }

    public bool UseShield()
    {
        if (ActiveShields > 0)
        {
            ActiveShields--;
            Shields[ActiveShields].SetActive(false);

            ShowBlockedText();

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ChangeLevel(int level)
    {
        CurrentLevel = level;

        if (CurrentLevel == 0)
        {
            VFX.SetActive(false);
            Shields[0].SetActive(false);
            Shields[1].SetActive(false);
            Shields[2].SetActive(false);
        }
        else if (CurrentLevel > 0)
        {
            VFX.SetActive(true);
        }
    }
}
