using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;

    public int targetScore = 20;


    private int score = 0;
    private bool isGameOver = false;

    public float fadeDuration = 0.3f; // thời gian fade in/out
    [Header("Start Image")]
    public CanvasGroup startImageCanvasGroup; // CanvasGroup của ảnh intro
    public float startImageDisplayTime = 3f; // Tổng thời gian hiển thị (fade in + giữ + fade out)
    public float startImageFadeDuration = 0.5f; // thời gian fade in/out
    [Header("Luật chơi")]
    public GameObject infoPanel; // Panel sẽ bật lên sau intro
    public Button closeInfoButton; // Nút đóng panel
    public TextMeshProUGUI infoText; // Text để chạy chữ
    [TextArea] public string message = "Người chơi phải đi tìm chìa khóa và tìm cửa ra ở bên trong mê cung, nếu người chơi thoát khỏi mê cung thành công thì sẽ được thưởng 100 coin, nếu người chơi bỏ cuộc thì sẽ bị trừ 50% coin."; // Nội dung
    public float typeSpeed = 0.05f; // tốc độ chạy chữ
    [Header("Give Up Button")]
    public GameObject GiveUp;
    private static bool hasShownIntro = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        score = 0;
        UpdateScoreText();
        gameOverPanel.SetActive(false);

        if (!hasShownIntro)
        {
            if (startImageCanvasGroup != null)
                StartCoroutine(ShowStartImage());
        }
        else
        {
            Time.timeScale = 1f;
            GiveUp.SetActive(true);
        }
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

    public void PlayerHitCot(GameObject player)
    {
        if (isGameOver) return;
        StartCoroutine(GameOverWithAnimation(player));
    }

    private IEnumerator GameOverWithAnimation(GameObject player)
    {
        isGameOver = true;

        // Lấy Animator & Rigidbody2D
        Animator anim = player.GetComponent<Animator>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        // Play animation Die
        if (anim != null)
            anim.SetTrigger("Die");

        // Cho player rơi xuống
        if (rb != null)
        {
            rb.gravityScale = 2f;       // tăng trọng lực
            rb.velocity = new Vector2(0, -5f); // rơi nhanh xuống
        }

        // Chờ 3 giây
        yield return new WaitForSeconds(1f);

        // Hiện panel Game Over
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        hasShownIntro = true;
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

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        cg.alpha = start;
        cg.interactable = end > 0.5f;
        cg.blocksRaycasts = end > 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        cg.alpha = end;
        cg.interactable = end > 0.5f;
        cg.blocksRaycasts = end > 0.5f;
    }
    private IEnumerator ShowStartImage()
    {
        Time.timeScale = 0f;
        startImageCanvasGroup.gameObject.SetActive(true);

        // Fade in
        yield return StartCoroutine(FadeCanvasGroup(startImageCanvasGroup, 0, 1, startImageFadeDuration));

        // Giữ nguyên trong một khoảng thời gian
        yield return new WaitForSecondsRealtime(startImageDisplayTime - (startImageFadeDuration * 2));

        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(startImageCanvasGroup, 1, 0, startImageFadeDuration));

        startImageCanvasGroup.gameObject.SetActive(false);

        // ✅ Hiện panel sau intro
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);

            // Chạy hiệu ứng chữ
            if (infoText != null)
            {
                StartCoroutine(TypeText(message));
            }

            // Gán nút đóng panel
            if (closeInfoButton != null)
            {
                closeInfoButton.onClick.RemoveAllListeners();
                closeInfoButton.onClick.AddListener(() =>
                {
                    infoPanel.SetActive(false);
                    GiveUp.SetActive(true);
                    Time.timeScale = 1f;
                    hasShownIntro = true; // Đánh dấu đã xem
                });
            }
        }
    }


    private IEnumerator TypeText(string textToType)
    {
        infoText.text = "";
        foreach (char c in textToType)
        {
            infoText.text += c;
            yield return new WaitForSecondsRealtime(typeSpeed); // Dùng Realtime vì đang Time.timeScale = 0
        }
    }


    public void OnGiveUpButton()
    {
        Time.timeScale = 1f;
        int hp = PlayerPrefs.GetInt("player_hp", 200);
        int score = PlayerPrefs.GetInt("player_score", 0);
        int coin = PlayerPrefs.GetInt("player_coin", 0);
        int lostCoin = coin / 2;
        coin -= lostCoin;
        PlayerPrefs.SetInt("player_hp", hp);
        PlayerPrefs.SetInt("player_score", score);
        PlayerPrefs.SetInt("player_coin", coin);
        PlayerPrefs.Save();
        PlayerPrefs.SetString("NextScene", "map2");
        SceneManager.LoadScene("Loading");

    }
    public void OnGiveUpAgainButton()
    {
        Time.timeScale = 1f;
        int hp = PlayerPrefs.GetInt("player_hp", 200);
        int score = PlayerPrefs.GetInt("player_score", 0);
        int coin = PlayerPrefs.GetInt("player_coin", 0);
        int lostCoin = coin / 2;
        coin -= lostCoin;
        PlayerPrefs.SetInt("player_hp", hp);
        PlayerPrefs.SetInt("player_score", score);
        PlayerPrefs.SetInt("player_coin", coin);
        PlayerPrefs.Save();
        PlayerPrefs.SetString("NextScene", "map2");
        SceneManager.LoadScene("Loading");
    }
}
