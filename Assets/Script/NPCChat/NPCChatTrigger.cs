using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCChatTrigger : MonoBehaviour
{
    public GameObject chatBox;           // Khung chat (Panel hoặc Image)
    public Text chatText;                // Text hiển thị nội dung
    public float typeSpeed = 0.05f;      // Tốc độ gõ từng chữ
    public float delayBetweenLines = 1.5f; // Thời gian chờ giữa các câu

    private Coroutine typingCoroutine;

    // Danh sách câu thoại
    private string[] dialogueLines = new string[]
    {
        "Xin chào, hiệp sĩ!",
        "Ta đã chờ đợi người từ rất lâu rồi...",
        "Ngọn lửa hi vọng đang dần lụi tàn, và chỉ có người mới có thể thắp sáng lại nó.",
        "Thế giới này đang rơi vào hỗn loạn bởi bóng tối lan tràn.",
        "Nhưng trước khi chiến đấu, người cần phải hiểu: sức mạnh thật sự đến từ lòng dũng cảm và lòng nhân ái.",
        "Giờ thì...",
        "Hãy đi về hướng bên phải để bắt đầu hành trình."
    };

    private void Start()
    {
        if (chatBox != null)
            chatBox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            chatBox.SetActive(true);
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(PlayDialogue());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            chatBox.SetActive(false);
            chatText.text = "";
        }
    }

    IEnumerator PlayDialogue()
    {
        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(delayBetweenLines);
        }
    }

    IEnumerator TypeText(string line)
    {
        chatText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            chatText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
