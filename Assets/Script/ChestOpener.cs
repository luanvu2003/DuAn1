using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChestOpener : MonoBehaviour
{
    [Header("Danh sách rương")]
    public List<GameObject> chests; // Drag các rương vào đây

    [Header("Danh sách item rơi ra (tương ứng)")]
    public List<GameObject> dropItems; // Drag item (key) tương ứng từng rương

    [Header("UI hiển thị khi gần rương")]
    public GameObject interactCanvas; // Canvas hoặc Text "E để mở"
    public TextMeshProUGUI interactText;

    [Header("Cài đặt thời gian")]
    public float openDelay = 1.5f; // Thời gian animation mở
    public float destroyDelay = 0.5f; // Thời gian chờ trước khi rương bị xóa

    private GameObject currentChest;
    private int currentChestIndex = -1;
    private bool isNearChest = false;
    private bool isOpening = false;

    void Start()
    {
        if (interactCanvas != null) interactCanvas.SetActive(false);
    }

    void Update()
    {
        if (isNearChest && !isOpening && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(OpenChestRoutine());
        }
    }

    private IEnumerator OpenChestRoutine()
    {
        isOpening = true;

        // Chạy animation mở
        Animator anim = currentChest.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Open");
        }

        // Chờ animation chạy
        yield return new WaitForSeconds(openDelay);

        // Spawn item rơi ra
        if (currentChestIndex >= 0 && currentChestIndex < dropItems.Count)
        {
            if (dropItems[currentChestIndex] != null)
            {
                Instantiate(dropItems[currentChestIndex],
                    currentChest.transform.position + Vector3.up * 1f,
                    Quaternion.identity);
            }
        }

        // Chờ thêm rồi destroy rương
        yield return new WaitForSeconds(destroyDelay);
        Destroy(currentChest);

        interactCanvas.SetActive(false);
        isNearChest = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu player va chạm rương
        for (int i = 0; i < chests.Count; i++)
        {
            if (other.gameObject == chests[i])
            {
                currentChest = chests[i];
                currentChestIndex = i;
                isNearChest = true;

                if (interactCanvas != null)
                {
                    interactCanvas.SetActive(true);
                    if (interactText != null)
                        interactText.text = "Nhấn E để mở rương";
                }
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentChest)
        {
            isNearChest = false;
            if (interactCanvas != null)
                interactCanvas.SetActive(false);
        }
    }
}
