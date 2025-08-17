using UnityEngine;
using TMPro;
[ExecuteAlways]
public class CoinFollowText : MonoBehaviour
{
    public TextMeshProUGUI coinText;   // gán Text vào đây
    public RectTransform coinIcon;     // gán Image vào đây
    public float spacing = 2f;         // khoảng cách giữa số và icon

    void Update()
    {
        // Lấy chiều rộng thật của text
        float textWidth = coinText.preferredWidth;

        // Đặt icon sát ngay sau số
        coinIcon.anchoredPosition = new Vector2(textWidth + spacing, coinIcon.anchoredPosition.y);
    }
}
