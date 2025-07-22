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
        SceneManager.LoadScene("main");
    }
    // public void Exit()
    // {
    
    // }
}
