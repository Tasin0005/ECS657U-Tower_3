using UnityEngine;
using UnityEngine.UI;

// Controls the top left Settings button and pause menu
public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance { get; private set; }

    [Header("Panel & Buttons")]
    public GameObject settingsPanel;
    public Button openButton;
    public Button closeButton;
    public Button resetButton;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Start()
    {
        if (openButton != null)
            openButton.onClick.AddListener(Open);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);

        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnVictory += ForceCloseForGameOver;
            SkillPoints.Instance.OnDefeat += ForceCloseForGameOver;
        }
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnVictory -= ForceCloseForGameOver;
            SkillPoints.Instance.OnDefeat -= ForceCloseForGameOver;
        }
    }

    public void Open()
    {
        if (IsOpen)
            return;

        if (GameOverUI.Instance != null && GameOverUI.Instance.IsGameOver)
            return;

        if (SkillTreeUI.Instance != null && SkillTreeUI.Instance.IsOpen)
            return;

        IsOpen = true;
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        IsOpen = false;
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        bool stayPaused = (GameOverUI.Instance != null && GameOverUI.Instance.IsGameOver)
            || (SkillTreeUI.Instance != null && SkillTreeUI.Instance.IsOpen);

        if (!stayPaused)
            Time.timeScale = 1f;
    }

    private void OnResetClicked()
    {
        IsOpen = false;
        if (GameOverUI.Instance != null)
            GameOverUI.Instance.Restart();
    }

    private void ForceCloseForGameOver()
    {
        if (!IsOpen)
            return;

        IsOpen = false;
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}