using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI diem; // TextMeshPro để hiển thị điểm
    public GameObject gameOverPanel;

    public int targetScore = 5; // Điểm cần để chuyển scene (có thể chỉnh trong Inspector)
    public string nextSceneName = "map2"; // Tên scene sẽ chuyển tới

    private int score = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        score = 0;
        UpdateScoreText();
        gameOverPanel.SetActive(false);
    }

    public void AddScore()
    {
        if (!isGameOver)
        {
            score++;
            UpdateScoreText();

            if (score >= targetScore)
            {
                LoadNextScene();
            }
        }
    }

    void UpdateScoreText()
    {
        diem.text = score.ToString();
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}
