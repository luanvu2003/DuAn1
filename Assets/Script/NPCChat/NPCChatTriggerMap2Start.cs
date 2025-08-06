using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCChatTriggerMap2Start : MonoBehaviour
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
        "Đây là ngôi làng có một đường hầm để đi đến lâu đâì gần nhất.",
        "Tôi thấy bạn có vẻ đang bị lũ quái vật đánh trọng thương.",
        "Ở phía sau tôi có một cái thang bạn có thể leo lên đó.",    
        "Gặp một dân làng ở đó, người đó có bán băng cứu thương bạn có thể mua nó.",    
        "Sau khi được vết thương đã ổn định thì nhiệm vụ tiếp theo của bạn là giết hết các quái vật ở đây.",   
        "Vua của lũ quái đó có ma thuật và đang giấu cổng đến lâu đài.",   
        "Sau khi bạn đánh bại tất cả lũ quái ở làng này, tôi sẽ giúp bạn canh giữ cánh cổng đó.",  
        "Ở sau lưng bạn có một cái cầu thang dẫn đến đường hầm, sau khi đánh bại lũ quái này thì cánh cổng sẽ hiện ra!",
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
