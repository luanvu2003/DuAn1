using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [Header("UI Loading")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public RectTransform characterImage; // Drag Image của nhân vật vào đây
    public RectTransform sliderFillArea; // Fill Area của Slider

    private string targetSceneName;

    void Start()
    {
        targetSceneName = PlayerPrefs.GetString("NextScene", "");

        Debug.Log("Target scene: " + targetSceneName);

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Scene cần load không được thiết lập!");
            return;
        }

        LoadTargetScene();
    }

    public void LoadTargetScene()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        operation.allowSceneActivation = false;

        float loadingTime = 7f;
        float timer = 0f;

        while (timer < loadingTime)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / loadingTime);

            if (progressBar != null)
            {
                progressBar.value = progress;
                if (characterImage != null && sliderFillArea != null)
                {
                    float moveX = progress * sliderFillArea.rect.width;
                    Vector2 anchoredPos = characterImage.anchoredPosition;
                    anchoredPos.x = moveX;
                    characterImage.anchoredPosition = anchoredPos;
                }
                Debug.Log("Slider value = " + progress);
            }

            yield return null;
        }

        // Sau 7 giây mới kích hoạt scene
        operation.allowSceneActivation = true;
    }

}
