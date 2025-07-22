using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject warriorPrefab;
    public GameObject magePrefab;
    public GameObject archerPrefab;
    public LoginManager loginManager;

    public Transform spawnPoint;

    void Start()
    {
        string username = PlayerPrefs.GetString("LoggedInUsername", "");
        string selectedCharacter = "";

        if (!string.IsNullOrEmpty(username))
        {
            Account acc = loginManager.GetAccount(username);
            selectedCharacter = acc.characterClass;
        }

        GameObject prefab = selectedCharacter switch
        {
            "Mage" => magePrefab,
            "Archer" => archerPrefab,
            _ => warriorPrefab
        };

        if (prefab != null)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
