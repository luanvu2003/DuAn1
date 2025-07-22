using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [Header("UI Loading")]
    public GameObject loadingPanel;
    public Slider progressBar;

    private string targetSceneName;

    void Start()
    {
        targetSceneName = PlayerPrefs.GetString("NextScene", "");

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

        float timer = 0f;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            timer += Time.deltaTime;

            // Delay ít nhất 7s để nhìn thấy loading
            if (operation.progress >= 0.9f && timer >= 7f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
