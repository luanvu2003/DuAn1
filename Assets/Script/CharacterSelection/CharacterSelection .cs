using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public void SelectWarrior()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Warrior");
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }

    public void SelectMage()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Mage");
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }

    public void SelectArcher()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Archer");
        PlayerPrefs.SetString("NextScene", "main");
        SceneManager.LoadScene("Loading");
    }
}
