using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float spawnRate = 2f;
    public float minY = -1f;
    public float maxY = 1f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnPipe), 0f, spawnRate);
    }

    void SpawnPipe()
    {
        float yPos = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, yPos, 0);
        Instantiate(pipePrefab, spawnPos, Quaternion.identity);
    }
}
