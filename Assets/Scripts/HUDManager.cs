using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Member 4 - the main HUD: wave number, status, Start Wave button, How to Play.
// Points are read from Member 3's SkillPoints (pointsText is optional,
// since PointsDisplay already shows them).
public class HUDManager : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text waveText;
    public TMP_Text statusText;
    public TMP_Text pointsText;       // optional

    [Header("Buttons / panels")]
    public Button startWaveButton;
    public GameObject howToPlayPanel;

    private void Start()
    {
        if (startWaveButton != null)
            startWaveButton.onClick.AddListener(OnStartWave);
    }

    private void OnStartWave()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.StartNextWave();
    }

    private void Update()
    {
        if (pointsText != null && SkillPoints.Instance != null)
            pointsText.text = "Skill Points: " + SkillPoints.Instance.CurrentPoints;

        WaveManager wm = WaveManager.Instance;
        if (wm == null)
            return;

        bool gameOver = GameOverUI.Instance != null && GameOverUI.Instance.IsGameOver;

        if (waveText != null)
            waveText.text = wm.CurrentWave == 0 ? "Wave: -" : "Wave: " + wm.CurrentWave;

        if (startWaveButton != null)
            startWaveButton.interactable = !wm.WaveInProgress && !gameOver;

        if (statusText != null)
            statusText.text = wm.WaveInProgress
                ? "Wave in progress..."
                : "Place wizards, then press Start Wave";
    }

    // Hook to a "How to Play" / "?" button.
    public void ToggleHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(!howToPlayPanel.activeSelf);
    }
}
