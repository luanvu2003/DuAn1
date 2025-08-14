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
        PlayerController.shouldResetUI = true;
        ItemUseManager.ResetBuffs();
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }
    public void PlayAgain()
    {
        PlayerController.shouldResetUI = true;
        ItemUseManager.ResetBuffs();
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }
    public void MainMenu()
    {
        PlayerPrefs.SetString("NextScene", "MainMenu"); 
        SceneManager.LoadScene("Loading");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
