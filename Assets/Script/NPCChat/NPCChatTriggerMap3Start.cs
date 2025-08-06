using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCChatTriggerMap3Start : MonoBehaviour
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
        "Đây là bên trong lâu đài có những lũ quái vật mạnh nhất và có một pháp sư canh giữ.",
        "Tên pháp sư đó rất nguy hiểm, bạn hãy cẩn thận!",
        "Bạn phải đánh bại tên pháp sư đó để mở cổng vào phòng của tên Vua đó!",    
        "Sau khi đánh bại hắn thì cánh cổng sẽ hiện ra!",    
        "Con đường đi trong lâu đài cũng có rất nhiều bẫy, hãy cẩn thận!",   
        "Chúc bạn may mắn!"
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
