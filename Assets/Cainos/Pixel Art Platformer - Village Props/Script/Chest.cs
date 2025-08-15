using System.Collections;
using UnityEngine;
using Cainos.LucidEditor;
using TMPro;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class Chest : MonoBehaviour
    {
        [FoldoutGroup("Reference")]
        public Animator animator;

        [Header("UI hiển thị khi gần rương")]
        public GameObject interactCanvas;
        public TextMeshProUGUI interactText;

        [Header("Item rơi ra (SetActive thay vì spawn prefab)")]
        public GameObject dropItem; // Object có sẵn trong scene (disable mặc định)

        [Header("Thời gian")]
        public float openDelay = 1.5f;
        public float destroyDelay = 0.5f;

        private bool isNear = false;
        private bool isOpened = false;
        private bool hasDropped = false;
        [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                animator.SetBool("IsOpened", isOpened);
            }
        }

        [FoldoutGroup("Runtime"), Button("Open"), HorizontalGroup("Runtime/Button")]
        public void Open()
        {
            IsOpened = true;
        }

        [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
        public void Close()
        {
            IsOpened = false;
        }

        private void Start()
        {
            if (interactCanvas != null) interactCanvas.SetActive(false);
            if (dropItem != null) dropItem.SetActive(false); // Ẩn item lúc đầu
        }

        private void Update()
        {
            if (isNear && !isOpened && Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(OpenChestRoutine());
            }
        }

        private IEnumerator OpenChestRoutine()
        {
            isOpened = true;
            IsOpened = true;
            hasDropped = true; // ✅ Đặt ở đây để không drop 2 lần

            if (interactCanvas != null) interactCanvas.SetActive(false);

            yield return new WaitForSeconds(openDelay);

            if (dropItem != null)
            {
                dropItem.SetActive(true);
                dropItem.transform.SetParent(null);
            }

            yield return new WaitForSeconds(destroyDelay);
            Destroy(gameObject);
        }


        private static Chest activeChest;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !isOpened)
            {
                // Tắt canvas của rương đang active
                if (activeChest != null && activeChest != this && activeChest.interactCanvas != null)
                    activeChest.interactCanvas.SetActive(false);

                activeChest = this;

                isNear = true;
                if (interactCanvas != null)
                {
                    interactCanvas.SetActive(true);
                    if (interactText != null)
                        interactText.text = "Nhấn E để mở rương";
                }
            }
        }



        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isNear = false;
                if (interactCanvas != null)
                    interactCanvas.SetActive(false);
            }
        }
    }
}
