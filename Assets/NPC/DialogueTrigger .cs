using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public TwoPersonDialogue dialogueManager; // Kéo script TwoPersonDialogue vào đây

    private bool hasTriggered = false; // Ngăn không cho kích hoạt nhiều lần

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            hasTriggered = true;
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue();
                Destroy(gameObject);
            }
        }
    }
}
