using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
public class OptionMenu : MonoBehaviour
{
    public GameObject optionPanel;
    public Slider volumeSlider;

    void Start()
    {
        optionPanel.SetActive(false);

        // Gán giá trị slider từ âm lượng đã lưu
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.GetVolume();
        }

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetVolume(value);
            }
        });
    }

    public void OpenOptions()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseOptions()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
