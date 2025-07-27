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
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }
    public void PlayAgain()
    {
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }
    public void MainMenu()
    {
        PlayerPrefs.SetString("NextScene", "MainMenu"); 
        SceneManager.LoadScene("Loading");
    }
    // public void Exit()
    // {

    // }
}
