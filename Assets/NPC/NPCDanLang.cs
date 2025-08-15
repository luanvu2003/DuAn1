using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPCDanLang : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public string[] dialogueParts;
    public float typeSpeed = 0.05f;

    private int currentPart = 0;
    private bool isTyping = false;
    private string currentSentence;
    private bool hasAcceptedMission = false;

    [Header("UI Panels")]
    public GameObject NPCTalk;
    public GameObject ShowComfirmMisson;
    public GameObject ShowMisson;

    private RectTransform missionRect;
    public float slideDuration = 0.5f;
    public float slideDistanceRight = 800f;
    public float slideDistanceLeft = -800f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public GameObject btnShow;
    private bool isPlayerNear = false;
    void Start()
    {
        if (ShowMisson != null)
            missionRect = ShowMisson.GetComponent<RectTransform>();

        Button btnShow = ShowMisson.transform.Find("BtnShow")?.GetComponent<Button>();
        Button btnHide = ShowMisson.transform.Find("BtnHide")?.GetComponent<Button>();

        if (btnShow != null) btnShow.onClick.AddListener(OnClickShow);
        if (btnHide != null) btnHide.onClick.AddListener(OnClickHide);
    }

    void Update()
    {
        if (hasAcceptedMission) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) // chỉ bắt đầu khi bấm E gần NPC
        {
            NPCTalk.SetActive(true);
            currentPart = 0;
            StartCoroutine(TypeText(dialogueParts[currentPart]));
        }

        if (NPCTalk.activeSelf && Input.GetKeyDown(KeyCode.Space))
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
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        isTyping = false;
    }

    public void ComfirmMisson()
    {
        hasAcceptedMission = true;
        Time.timeScale = 1f;
        ShowComfirmMisson.SetActive(false);
        ShowMisson.SetActive(true);

        FindObjectOfType<OpenMisson>()?.AcceptMission();

        missionRect.anchoredPosition = new Vector2(slideDistanceRight, missionRect.anchoredPosition.y);
        StartCoroutine(SlideMission(slideDistanceRight, 0));
    }

    // 🔹 Hàm gán cho Button Hide
    public void OnClickHide()
    {
        StartCoroutine(SlideMission(0, slideDistanceRight, () =>
        {
            ShowMisson.SetActive(false);
            btnShow.gameObject.SetActive(true);
        }));
    }

    // 🔹 Hàm gán cho Button Show
    public void OnClickShow()
    {
        btnShow.gameObject.SetActive(false);

        ShowMisson.SetActive(true);
        missionRect.anchoredPosition = new Vector2(slideDistanceRight, missionRect.anchoredPosition.y);
        StartCoroutine(SlideMission(slideDistanceRight, 0));
    }

    IEnumerator SlideMission(float startX, float endX, System.Action onComplete = null)
    {
        Vector2 startPos = new Vector2(startX, missionRect.anchoredPosition.y);
        Vector2 endPos = new Vector2(endX, missionRect.anchoredPosition.y);

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);
            missionRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        missionRect.anchoredPosition = endPos;
        onComplete?.Invoke();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}
