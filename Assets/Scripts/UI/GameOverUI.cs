using UnityEngine;
using UnityEngine.SceneManagement;

// Victory / Defeat overlays and scene restart
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
            return;

        IsGameOver = true;

        if (WaveManager.Instance != null)
            WaveManager.Instance.StopWaves();

        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;
    }

    // Hooked to the Reset button and Restart buttons on both panels
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}