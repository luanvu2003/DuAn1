using UnityEngine;

public class OpenMisson : MonoBehaviour
{
    public GameObject TextOpenPanel;
    public GameObject MissonPanel;

    private bool isPlayerInRange = false;
    private bool hasAcceptedMission = false;
    void Update()
    {
        if (isPlayerInRange && !hasAcceptedMission && Input.GetKeyDown(KeyCode.E))
        {
            MissonPanel.SetActive(true);
            PlayerController.IsUIOpen = true; // Đánh dấu UI đang mở
            Time.timeScale = 0f;
        }
    }

    public void AcceptMission()
    {
        hasAcceptedMission = true;
        TextOpenPanel.SetActive(false); 
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
