using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShowSkill : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup canvasGroup;
    public RectTransform panelTransform;
    public Image skillIcon;
    public TMP_Text skillNameText;

    [Header("Animation Settings")]
    public float inDuration = 0.5f;   // thời gian xuất hiện
    public float outDuration = 0.5f;  // thời gian biến mất
    public float displayTime = 2f;    // thời gian hiển thị

    [Header("Object to Activate After Animation")]
    public GameObject objectToActivate; // object sẽ bật khi xong animation

    private Vector2 hiddenPos;
    private Vector2 shownPos;

    void Start()
    {
        // Ví dụ: hiện khi vừa vào scene
        Show("Kỹ năng mới", skillIcon.sprite);
    }

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        hiddenPos = new Vector2(0, 0); // vị trí ngoài màn hình
        shownPos = Vector2.zero; // vị trí giữa

        panelTransform.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0;
    }

    public void Show(string skillName, Sprite icon)
    {
        skillNameText.text = skillName;
        skillIcon.sprite = icon;

        gameObject.SetActive(true); // bật UI trước khi show
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // In Animation
        float t = 0;
        while (t < inDuration)
        {
            t += Time.deltaTime;
            float lerp = t / inDuration;
            panelTransform.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, Mathf.SmoothStep(0, 1, lerp));
            canvasGroup.alpha = lerp;
            yield return null;
        }

        // Giữ UI trên màn hình
        yield return new WaitForSeconds(displayTime);

        // Out Animation
        t = 0;
        while (t < outDuration)
        {
            t += Time.deltaTime;
            float lerp = t / outDuration;
            panelTransform.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, Mathf.SmoothStep(0, 1, lerp));
            canvasGroup.alpha = 1 - lerp;
            yield return null;
        }

        // Sau khi animation kết thúc
        gameObject.SetActive(false); // tắt UI
        if (objectToActivate != null)
            objectToActivate.SetActive(true); // bật object khác
    }
}
