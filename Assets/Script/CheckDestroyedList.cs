using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckDestroyedList : MonoBehaviour
{
    [Header("Danh sách Enemy cần kiểm tra")]
    public List<GameObject> enemiesToCheck;

    [Header("Danh sách Item cần kiểm tra")]
    public List<GameObject> itemsToCheck;

    [Header("UI hiển thị tiến độ")]
    public TextMeshProUGUI enemyText;
    public TextMeshProUGUI itemText;

    [Header("Khi hoàn thành tất cả")]
    public GameObject notifyComplete; // Panel hoặc text thông báo hoàn thành

    [Header("Khi tất cả đã bị destroy")]
    public List<GameObject> objectsToEnable;
    public List<GameObject> objectsToDisable;

    private bool hasSwitched = false;

    private int totalEnemies;
    private int totalItems;

    void Start()
    {
        totalEnemies = enemiesToCheck.Count;
        totalItems = itemsToCheck.Count;

        UpdateUI();
        if (notifyComplete != null) notifyComplete.SetActive(false);
    }

    void Update()
    {
        if (hasSwitched) return;

        int remainingEnemies = 0;
        int remainingItems = 0;

        foreach (GameObject enemy in enemiesToCheck)
        {
            if (enemy != null) remainingEnemies++;
        }

        foreach (GameObject item in itemsToCheck)
        {
            if (item != null) remainingItems++;
        }

        // Cập nhật UI
        if (enemyText != null)
            enemyText.text = $"Tiêu diệt quái vật: {totalEnemies - remainingEnemies}/{totalEnemies}";

        if (itemText != null)
            itemText.text = $"Chìa khóa: {totalItems - remainingItems}/{totalItems}";

        // Kiểm tra hoàn thành
        if (remainingEnemies == 0 && remainingItems == 0)
        {
            foreach (GameObject obj in objectsToEnable)
                if (obj != null) obj.SetActive(true);

            foreach (GameObject obj in objectsToDisable)
                if (obj != null) obj.SetActive(false);

            if (notifyComplete != null) notifyComplete.SetActive(true);

            hasSwitched = true;
        }
    }

    private void UpdateUI()
    {
        if (enemyText != null)
            enemyText.text = $"Tiêu diệt quái vật: 0/{totalEnemies}";

        if (itemText != null)
            itemText.text = $"Chìa khóa: 0/{totalItems}";
    }
}
