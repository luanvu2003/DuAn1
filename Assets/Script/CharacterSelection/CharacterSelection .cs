using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public LoginManager loginManager;

    public void SelectCharacter(string characterClass)
    {
        string username = PlayerPrefs.GetString("LoggedInUsername", "");
        if (!string.IsNullOrEmpty(username))
        {
            loginManager.SetCharacterForAccount(username, characterClass);
            PlayerPrefs.SetString("NextScene", "MainMenu");
            SceneManager.LoadScene("Loading");
        }
    }
}
