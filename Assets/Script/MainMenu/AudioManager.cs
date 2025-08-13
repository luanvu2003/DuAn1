using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource musicSource;

    void Awake()
    {
        // Đảm bảo chỉ tồn tại 1 AudioManager duy nhất
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Lấy volume đã lưu
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource.volume = savedVolume;
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return musicSource.volume;
    }
}
