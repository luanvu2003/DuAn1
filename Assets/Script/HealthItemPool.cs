using UnityEngine;
using System.Collections.Generic;

public class HealthItemPool : MonoBehaviour
{
    public GameObject healthItemPrefab;
    public int poolSize = 10;

    private List<GameObject> pool = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject item = Instantiate(healthItemPrefab);
            item.SetActive(false);
            pool.Add(item);
        }
    }

    public GameObject GetAvailableItem(Vector3 spawnPosition)
    {
        foreach (GameObject item in pool)
        {
            if (!item.activeInHierarchy)
            {
                item.transform.position = spawnPosition;
                item.SetActive(true);
                return item;
            }
        }

        // Nếu không có item nào rảnh, có thể tạo thêm hoặc return null
        Debug.LogWarning("Pool full, no health item available!");
        return null;
    }
}
