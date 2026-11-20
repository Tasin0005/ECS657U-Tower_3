using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Event driven HUD displays: skill points, wave number, status text, and Start Wave button.
public class HUDManager : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text waveText;
    public TMP_Text statusText;
    public TMP_Text pointsText;

    [Header("Buttons")]
    public Button startWaveButton;

    private bool isGameOver;

    private void Start()
    {
        if (startWaveButton != null)
            startWaveButton.onClick.AddListener(OnStartWave);

        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnPointsChanged += RefreshPoints;
            SkillPoints.Instance.OnVictory += HandleGameOver;
            SkillPoints.Instance.OnDefeat += HandleGameOver;
        }

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveStateChanged += RefreshWaveUI;

        isGameOver = GameOverUI.Instance != null && GameOverUI.Instance.IsGameOver;
        RefreshPoints();
        RefreshWaveUI();
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnPointsChanged -= RefreshPoints;
            SkillPoints.Instance.OnVictory -= HandleGameOver;
            SkillPoints.Instance.OnDefeat -= HandleGameOver;
        }

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveStateChanged -= RefreshWaveUI;
    }

    private void OnStartWave()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.StartNextWave();
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        RefreshWaveUI();
    }

    private void RefreshPoints()
    {
        if (pointsText != null && SkillPoints.Instance != null)
            pointsText.text = "Points: " + SkillPoints.Instance.CurrentPoints;
    }

    private void RefreshWaveUI()
    {
        WaveManager wm = WaveManager.Instance;
        if (wm == null)
            return;

        if (waveText != null)
            waveText.text = wm.CurrentWave == 0 ? "Wave: -" : "Wave: " + wm.CurrentWave;

        if (startWaveButton != null)
            startWaveButton.interactable = !wm.WaveInProgress && !isGameOver;

        if (statusText != null)
            statusText.text = wm.WaveInProgress
                ? "Wave in progress..."
                : "Place wizards, then press Start Wave";
    }
}