using UnityEngine;
using UnityEngine.UI;

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
    }

    public void CloseOptions()
    {
        optionPanel.SetActive(false);
    }
}
