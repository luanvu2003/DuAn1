using System.Collections.Generic;
using UnityEngine;

public class CheckDestroyedList : MonoBehaviour
{
    public List<GameObject> listToCheck;
    public List<GameObject> objectsToEnable;
    public List<GameObject> objectsToDisable;

    private bool hasSwitched = false;

    void Update()
    {
        if (hasSwitched) return;

        bool allDestroyed = true;

        foreach (GameObject obj in listToCheck)
        {
            if (obj != null)
            {
                allDestroyed = false;
                break;
            }
        }

        if (allDestroyed)
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
