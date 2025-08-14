using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject TextOpenPanel;
    public GameObject ShopPanel;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale = 0f; // Dừng game khi mở shop
            ShopPanel.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TextOpenPanel.SetActive(true);
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TextOpenPanel.SetActive(false);
            isPlayerInRange = false;
        }
    }
}
