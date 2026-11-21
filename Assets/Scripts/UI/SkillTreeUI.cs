using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Controls Skill Tree overlay: opens/pauses between waves, displays SP, refreshes nodes
public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI Instance { get; private set; }

    [Header("Panel & Controls")]
    public GameObject skillTreePanel;
    public Button closeButton;
    public TMP_Text pointsText;
    public SkillTooltipUI tooltip;

    public bool IsOpen { get; private set; }

    private SkillNodeUI[] nodes;

    private void Awake()
    {
        Instance = this;
        if (skillTreePanel != null)
        {
            nodes = skillTreePanel.GetComponentsInChildren<SkillNodeUI>(true);
            skillTreePanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnPointsChanged += RefreshUI;
            SkillPoints.Instance.OnSkillUnlocked += HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked += HandleSkillChanged;
            SkillPoints.Instance.OnVictory += ForceCloseForGameOver;
            SkillPoints.Instance.OnDefeat += ForceCloseForGameOver;
        }
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnPointsChanged -= RefreshUI;
            SkillPoints.Instance.OnSkillUnlocked -= HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked -= HandleSkillChanged;
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

        if (WaveManager.Instance != null && WaveManager.Instance.WaveInProgress)
            return;

        if (SettingsMenu.Instance != null && SettingsMenu.Instance.IsOpen)
            return;

        IsOpen = true;
        if (skillTreePanel != null)
            skillTreePanel.SetActive(true);

        if (tooltip != null)
            tooltip.Hide();

        Time.timeScale = 0f;
        RefreshUI();
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        IsOpen = false;
        if (tooltip != null)
            tooltip.Hide();

        if (skillTreePanel != null)
            skillTreePanel.SetActive(false);

        bool stayPaused = (GameOverUI.Instance != null && GameOverUI.Instance.IsGameOver)
            || (SettingsMenu.Instance != null && SettingsMenu.Instance.IsOpen);

        if (!stayPaused)
            Time.timeScale = 1f;
    }

    private void ForceCloseForGameOver()
    {
        if (!IsOpen)
            return;

        IsOpen = false;
        if (tooltip != null)
            tooltip.Hide();

        if (skillTreePanel != null)
            skillTreePanel.SetActive(false);
    }

    private void HandleSkillChanged(SkillId _)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (pointsText != null && SkillPoints.Instance != null)
            pointsText.text = "SP: " + SkillPoints.Instance.CurrentPoints;

        if (nodes == null)
            return;

        for (int i = 0; i < nodes.Length; i++)
            nodes[i].RefreshVisual();
    }

    public void ShowTooltip(SkillNodeUI node)
    {
        if (tooltip != null)
            tooltip.Show(node);
    }

    public void HideTooltip()
    {
        if (tooltip != null)
            tooltip.Hide();
    }
}