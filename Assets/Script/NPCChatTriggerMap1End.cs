using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCChatTriggerMap1End : MonoBehaviour
{
    public GameObject chatBox;           // Khung chat (Panel hoặc Image)
    public Text chatText;                // Text hiển thị nội dung
    public float typeSpeed = 0.05f;      // Tốc độ gõ từng chữ
    public float delayBetweenLines = 1.5f; // Thời gian chờ giữa các câu

    private Coroutine typingCoroutine;

    // Danh sách câu thoại
    private string[] dialogueLines = new string[]
    {
        "Cổng đã được mở, hãy đi vào để đi đến ngôi làng bên cạnh.",  
        "Ở ngôi làng đó có một lối tắt dẫn đến lâu đài.",  
        "Nhưng ở đó cũng không được an toàn hãy cẩn thận!."
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
