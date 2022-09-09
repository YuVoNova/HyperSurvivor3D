using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject InitialPanel;
    [SerializeField]
    private TMP_Text LevelText;
    [SerializeField]
    private TMP_Text CoinText;

    [SerializeField]
    private GameObject PlaceholderModel;

    [SerializeField]
    private GameObject ClassSelectionPanel;

    [SerializeField]
    private GameObject UpgradeSelectionPanel;
    [SerializeField]
    private Transform[] UpgradeParents;

    [SerializeField]
    private GameObject WaveCompletedPanel;
    [SerializeField]
    private TMP_Text WaveCompletedKillText;
    [SerializeField]
    private TMP_Text WaveCompletedCoinText;

    [SerializeField]
    private GameObject FailedLevelPanel;
    [SerializeField]
    private TMP_Text FailedLevelKillText;
    [SerializeField]
    private TMP_Text FailedLevelCoinText;

    [SerializeField]
    private GameObject FinishedLevelPanel;
    [SerializeField]
    private TMP_Text FinishedLevelKillText;
    [SerializeField]
    private TMP_Text FinishedLevelCoinText;

    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private Slider XPBarSlider;
    [SerializeField]
    private TMP_Text CurrentWaveText;
    [SerializeField]
    private GameObject DamageTaken;
    [SerializeField]
    private TMP_Text HUDKillText;
    [SerializeField]
    private TMP_Text HUDCoinText;

    private void Awake()
    {
        LevelText.text = "Level  " + (Manager.Instance.PlayerData.CurrentLevel + 1);

        CoinText.text = "" + Manager.Instance.PlayerData.Coins;
    }

    public void PrepareClassSelection()
    {
        InitialPanel.SetActive(false);
        PlaceholderModel.SetActive(false);
        ClassSelectionPanel.SetActive(true);
    }

    public void StartLevel()
    {
        ClassSelectionPanel.SetActive(false);

        HUD.SetActive(true);
        XPBarSlider.value = 0;
        CurrentWaveText.text = "WAVE " + 1;
        HUDKillText.text = "" + 0;
        HUDCoinText.text = "" + 0;
    }

    public void UpdateXPBar(float value)
    {
        XPBarSlider.value = value;
    }

    public void UpdateHUD(int kill, int coin)
    {
        HUDKillText.text = "" + kill;
        HUDCoinText.text = "" + coin;
    }

    public void PlayerDamage()
    {
        DamageTaken.SetActive(false);
        DamageTaken.SetActive(true);
    }

    public void PrepareUpgradeSelection(string[] upgradeNames)
    {
        for (int i = 0; i < upgradeNames.Length; i++)
        {
            if (UpgradeParents[i].childCount > 0)
            {
                Destroy(UpgradeParents[i].GetChild(0).gameObject);
            }

            string name = upgradeNames[i];

            GameObject uiPrefab = Instantiate(Manager.Instance.PlayerData.Upgrades[name].UIPrefab, UpgradeParents[i]);
            uiPrefab.GetComponent<UpgradeUI>().Levels[Manager.Instance.PlayerData.Upgrades[name].CurrentLevel].SetActive(true);

            uiPrefab.GetComponent<Button>().onClick.AddListener(delegate { GameManager.Instance.UpgradeSelected(name); DisableUpgradeSelection(); GameManager.Instance.UnpauseGame(); });
        }

        UpgradeSelectionPanel.SetActive(true);
    }

    public void DisableUpgradeSelection()
    {
        UpgradeSelectionPanel.SetActive(false);
    }

    public void ActivateWaveCompleted(int kill, int coin)
    {
        WaveCompletedKillText.text = "" + kill;
        WaveCompletedCoinText.text = "" + coin;

        WaveCompletedPanel.SetActive(false);
        WaveCompletedPanel.SetActive(true);
    }

    public void UpdateWaveText(string waveText)
    {
        CurrentWaveText.text = waveText;
    }

    public void PrepareFinishedLevel(int kill, int coin)
    {
        FinishedLevelKillText.text = "" + kill;
        FinishedLevelCoinText.text = "" + coin;

        FinishedLevelPanel.SetActive(true);
    }

    public void OnPressNextButton()
    {
        Manager.Instance.FinishedLevel();
    }

    public void PrepareFailedLevel(int kill, int coin)
    {
        FailedLevelKillText.text = "" + kill;
        FailedLevelCoinText.text = "" + coin;

        FailedLevelPanel.SetActive(true);
    }

    public void OnPressTryAgainButton()
    {
        Manager.Instance.FailedLevel();
    }
}
