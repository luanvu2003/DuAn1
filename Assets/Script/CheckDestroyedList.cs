using System.Collections.Generic;
using UnityEngine;

public class CheckDestroyedList : MonoBehaviour
{
    [Header("Danh sách Enemy cần kiểm tra")]
    public List<GameObject> enemiesToCheck;

    [Header("Danh sách Item cần kiểm tra")]
    public List<GameObject> itemsToCheck;

    [Header("Khi tất cả đã bị destroy")]
    public List<GameObject> objectsToEnable;
    public List<GameObject> objectsToDisable;

    private bool hasSwitched = false;

    void Update()
    {
        if (hasSwitched) return;

        bool allEnemiesDestroyed = true;
        bool allItemsDestroyed = true;

        // Kiểm tra enemy
        foreach (GameObject enemy in enemiesToCheck)
        {
            if (enemy != null)
            {
                allEnemiesDestroyed = false;
                break;
            }
        }

        // Kiểm tra item
        foreach (GameObject item in itemsToCheck)
        {
            if (item != null)
            {
                allItemsDestroyed = false;
                break;
            }
        }

        // Nếu cả enemy và item đều đã bị destroy
        if (allEnemiesDestroyed && allItemsDestroyed)
        {
            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            hasSwitched = true;
        }
    }
}
