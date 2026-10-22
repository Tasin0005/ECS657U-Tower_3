using UnityEngine;
using UnityEngine.SceneManagement;

// Member 4 - Victory / Defeat overlays and Restart.
// Listens to Member 3's SkillPoints.OnVictory and SkillPoints.OnDefeat,
// so no changes are needed in their code.
public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    public GameObject victoryPanel;
    public GameObject defeatPanel;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }

    private void Start()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnVictory += ShowVictory;
            SkillPoints.Instance.OnDefeat += ShowDefeat;
        }
        else
        {
            Debug.LogError("GameOverUI: no SkillPoints in the scene");
        }
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnVictory -= ShowVictory;
            SkillPoints.Instance.OnDefeat -= ShowDefeat;
        }
    }

    public void ShowVictory() { EndGame(victoryPanel); }
    public void ShowDefeat() { EndGame(defeatPanel); }

    private void EndGame(GameObject panel)
    {
        if (IsGameOver)
            return;   // only ever show one ending

        IsGameOver = true;

        if (WaveManager.Instance != null)
            WaveManager.Instance.StopWaves();

        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;  // freezes enemies and wizards; UI buttons still work
    }

    // Hook to the Restart button on BOTH panels.
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
