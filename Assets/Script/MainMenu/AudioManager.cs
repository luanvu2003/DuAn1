using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource musicSource;

    [Tooltip("Danh sách scene giữ nhạc không bị destroy")]
    public string[] allowedScenes;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource.volume = savedVolume;
    }

    void Update()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!IsAllowedScene(currentScene))
        {
            Destroy(gameObject);
        }
    }

    private bool IsAllowedScene(string sceneName)
    {
        foreach (string scene in allowedScenes)
        {
            if (scene == sceneName)
                return true;
        }
        return false;
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
