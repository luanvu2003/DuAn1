using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject warriorPrefab;
    public GameObject magePrefab;
    public GameObject archerPrefab;

    void Start()
    {
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Warrior");

        GameObject prefabToSpawn = null;
        switch (selectedCharacter)
        {
            case "Warrior":
                prefabToSpawn = warriorPrefab;
                break;
            case "Mage":
                prefabToSpawn = magePrefab;
                break;
            case "Archer":
                prefabToSpawn = archerPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity); // hoặc vị trí spawn tùy ý
        }
    }
}
