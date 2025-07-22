using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GoToRegister()
    {
        SceneManager.LoadScene("Register");
    }
    public void GoToLogin()
    {
        SceneManager.LoadScene("Login");
    }
    public void Play()
    {
        PlayerPrefs.SetString("NextScene", "main"); // lưu tên scene cần load
        SceneManager.LoadScene("Loading");          // chuyển sang scene Loading
    }
    // public void Exit()
    // {
    
    // }
}
