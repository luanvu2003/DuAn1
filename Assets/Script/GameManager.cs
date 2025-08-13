using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    public int targetScore = 20;
    

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
        Time.timeScale = 1f; // Đảm bảo luôn chạy khi vào scene
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
        scoreText.text = score.ToString();
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause khi thua
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        Time.timeScale = 1f;
        int hp = PlayerPrefs.GetInt("player_hp", 200);
        int score = PlayerPrefs.GetInt("player_score", 0);
        int coin = PlayerPrefs.GetInt("player_coin", 0);
        coin += 100;
        PlayerPrefs.SetInt("player_hp", hp);
        PlayerPrefs.SetInt("player_score", score);
        PlayerPrefs.SetInt("player_coin", coin);
        PlayerPrefs.Save();
        PlayerPrefs.SetString("NextScene", "map2");
        SceneManager.LoadScene("Loading");
    }
}
