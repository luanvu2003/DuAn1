using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections; // cần cho IEnumerator

public class ItemCollector : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI itemText;
    public GameObject warningPanel;
    public CanvasGroup warningCanvasGroup; // CanvasGroup của warningPanel
    public Button closeButton;
    public Button closeButtonX;

    [Header("Item Settings")]
    public int collectedItems = 0;
    public int totalItems = 1;

    public float fadeDuration = 0.3f; // thời gian fade in/out

    private void Start()
    {
        UpdateItemText();

        if (warningPanel != null)
            warningPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseWarning);
        if (closeButtonX != null)
            closeButtonX.onClick.AddListener(CloseWarning);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nhặt item
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            collectedItems++;
            UpdateItemText();
        }

        // Chạm cổng
        if (collision.CompareTag("Cong"))
        {
            if (collectedItems >= totalItems)
            {
                PlayerPrefs.SetString("NextScene", "map3");
                SceneManager.LoadScene("Loading");
            }
            else
            {
                if (warningPanel != null)
                    ShowWarning();
            }
        }
    }

    private void UpdateItemText()
    {
        if (itemText != null)
            itemText.text = collectedItems + "/" + totalItems;
    }

    private void ShowWarning()
    {
        warningPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 0, 1, fadeDuration));
        Time.timeScale = 0f;
    }

    private void CloseWarning()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return StartCoroutine(FadeCanvasGroup(warningCanvasGroup, 1, 0, fadeDuration));
        warningPanel.SetActive(false);
        Time.timeScale = 1f;
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
}
