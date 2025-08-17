using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TwoPersonDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public bool isPlayer; // true = Player, false = Enemy
        [TextArea(2, 5)]
        public string text;
    }

    [Header("Player UI")]
    public GameObject playerPanel;
    public Image playerAvatar;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerDialogueText;

    [Header("Enemy UI")]
    public GameObject enemyPanel;
    public Image enemyAvatar;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyDialogueText;

    [Header("Cấu hình thoại")]
    public DialogueLine[] lines;
    public float typeSpeed = 0.05f;

    private int currentIndex = 0;
    private bool isTyping = false;
    private string currentSentence;

    private bool dialogueStarted = false;

    void Start()
    {
        // Ẩn UI khi chưa bắt đầu
        playerPanel.SetActive(false);
        enemyPanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogueStarted) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // Hiển thị toàn bộ câu nếu đang gõ
                StopAllCoroutines();
                if (lines[currentIndex].isPlayer)
                    playerDialogueText.text = currentSentence;
                else
                    enemyDialogueText.text = currentSentence;

                isTyping = false;
            }
            else
            {
                // Sang câu tiếp theo
                currentIndex++;
                if (currentIndex < lines.Length)
                {
                    ShowLine(currentIndex);
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    public void StartDialogue()
    {
        Time.timeScale = 0f; // Dừng game
        dialogueStarted = true;
        currentIndex = 0;
        ShowLine(currentIndex);
        PlayerController.IsUIOpen = true; // Đánh dấu UI đang mở
    }

    void ShowLine(int index)
    {
        DialogueLine line = lines[index];

        if (line.isPlayer)
        {
            playerPanel.SetActive(true);
            enemyPanel.SetActive(false);
            StartCoroutine(TypeText(line.text, true));
        }
        else
        {
            enemyPanel.SetActive(true);
            playerPanel.SetActive(false);
            StartCoroutine(TypeText(line.text, false));
        }
    }

    IEnumerator TypeText(string sentence, bool isPlayerTalking)
    {
        isTyping = true;
        currentSentence = sentence;

        if (isPlayerTalking) playerDialogueText.text = "";
        else enemyDialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (isPlayerTalking)
                playerDialogueText.text += letter;
            else
                enemyDialogueText.text += letter;

            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueStarted = false;
        Time.timeScale = 1f; // Tiếp tục game
        playerPanel.SetActive(false);
        enemyPanel.SetActive(false);
        PlayerController.IsUIOpen = false; // Đánh dấu UI đã đóng
    }
}
