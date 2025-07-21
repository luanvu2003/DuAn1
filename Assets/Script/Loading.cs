using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [Header("Loading")]
    public string targetSceneName = "";

    [Header("Loading")]
    public GameObject loadingPanel;
    public Slider progressBar;

    public void LoadTargetScene()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        // Bật panel loading
        if (loadingPanel != null) loadingPanel.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            // Khi gần hoàn tất (90%), cho phép load qua scene mới
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f); // thời gian đợi ảo
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Login")
            targetSceneName = "MainMenu";
        else if (currentScene == "MainMenu")
            targetSceneName = "main";

        LoadTargetScene();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
