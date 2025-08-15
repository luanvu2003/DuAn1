using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class NPCDanLang : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;

    [Header("Cấu hình thoại")]
    [TextArea(2, 5)]
    public string[] dialogueParts;
    public float typeSpeed = 0.05f;

    private int currentPart = 0;
    private bool isTyping = false;
    private string currentSentence;

    [Header("Ẩn UI")]
    public GameObject NPCTalk;

    [Header("Hiện nhận nhiệm vụ")]
    public GameObject ShowComfirmMisson;

    [Header("Nhiệm Vụ")]
    public GameObject ShowMisson;
    private RectTransform missionRect;
    public float slideDuration = 0.5f;
    public float slideDistance = 800f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Nút điều khiển")]
    public GameObject buttonShow; // nút để trượt vào
    public GameObject buttonHide; // nút để trượt ra
    void Start()
    {
        if (dialogueParts.Length > 0)
        {
            StartCoroutine(TypeText(dialogueParts[currentPart]));
        }

        if (ShowMisson != null)
            missionRect = ShowMisson.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentSentence;
                isTyping = false;
            }
            else
            {
                currentPart++;
                if (currentPart < dialogueParts.Length)
                {
                    StartCoroutine(TypeText(dialogueParts[currentPart]));
                }
                else
                {
                    dialogueText.text = "";
                    NPCTalk.SetActive(false);
                    ShowComfirmMisson.SetActive(true);
                }
            }
        }
    }

    IEnumerator TypeText(string sentence)
    {
        isTyping = true;
        currentSentence = sentence;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    public void ComfirmMisson()
    {
        ShowComfirmMisson.SetActive(false);
        ShowMisson.SetActive(true);
        buttonHide.SetActive(true);
        buttonShow.SetActive(false);
        StartCoroutine(SlideMission(slideDistance, 0)); // từ phải vào giữa
    }

    public void HideMissionRight()
    {
        StartCoroutine(SlideMission(0, slideDistance, () =>
        {
            buttonHide.SetActive(false);
            buttonShow.SetActive(true);
        }));
    }

    public void ShowMissionFromRight()
    {
        ShowMisson.SetActive(true);
        StartCoroutine(SlideMission(slideDistance, 0, () =>
        {
            buttonHide.SetActive(true);
            buttonShow.SetActive(false);
        }));
    }

    IEnumerator SlideMission(float startX, float endX, System.Action onComplete = null)
    {
        Vector2 startPos = new Vector2(startX, missionRect.anchoredPosition.y);
        Vector2 endPos = new Vector2(endX, missionRect.anchoredPosition.y);

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);
            missionRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        missionRect.anchoredPosition = endPos;

        if (endX != 0)
        {
            // Nếu trượt ra ngoài thì ẩn bảng
            ShowMisson.SetActive(false);
        }

        onComplete?.Invoke();
    }
}
