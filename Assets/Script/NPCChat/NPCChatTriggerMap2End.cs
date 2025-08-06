using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCChatTriggerMap2End : MonoBehaviour
{
    public GameObject chatBox;           // Khung chat (Panel hoặc Image)
    public Text chatText;                // Text hiển thị nội dung
    public float typeSpeed = 0.05f;      // Tốc độ gõ từng chữ
    public float delayBetweenLines = 1.5f; // Thời gian chờ giữa các câu

    private Coroutine typingCoroutine;

    // Danh sách câu thoại
    private string[] dialogueLines = new string[]
    {
        "Cổng đã được mở, hãy đi vào để đi đến lâu đài.",  
        "Ở trong lâu đài có rất nhiều quái vật mạnh, bạn hãy cẩn thận!",  
        "Ở đó có một pháp sư trấn giữ cánh cửa để vào phòng Vua của chúng.",
        "Sau lưng tôi là cánh cổng, hãy tiến vào đó!",
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
