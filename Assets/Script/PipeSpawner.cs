using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float spawnRate = 3f;
    public float minY = -1f;
    public float maxY = 1f;

    void Start()
    {
        InvokeRepeating("SpawnPipe", 1f, spawnRate);
    }

    void SpawnPipe()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(5f, randomY, 0f);
        Instantiate(pipePrefab, spawnPos, Quaternion.identity);
    }
}
