using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuSelectionChar : MonoBehaviour
{
    public LoginManager loginManager;

    public void OnPlayButton()
    {
        string username = PlayerPrefs.GetString("LoggedInUsername", "");
        if (string.IsNullOrEmpty(username)) return;

        Account acc = loginManager.GetAccount(username);

        if (acc != null && string.IsNullOrEmpty(acc.characterClass))
        {
            // Nếu chưa chọn nhân vật → sang scene chọn nhân vật
            PlayerPrefs.SetString("NextScene", "CharacterSelection");
        }
        else
        {
            // Nếu đã có nhân vật → vào game
            PlayerPrefs.SetString("NextScene", "main"); // đổi "Map1" nếu tên scene khác
        }

        SceneManager.LoadScene("Loading");
    }
}
