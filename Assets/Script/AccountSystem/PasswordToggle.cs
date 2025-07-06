using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public InputField passwordInput;
    public Sprite showIcon;  // icon mắt mở
    public Sprite hideIcon;  // icon mắt đóng
    public Image toggleImage;

    private bool isHidden = true;

    public void TogglePasswordVisibility()
    {
        isHidden = !isHidden;

        if (isHidden)
        {
            passwordInput.contentType = InputField.ContentType.Password;
            toggleImage.sprite = hideIcon;
        }
        else
        {
            passwordInput.contentType = InputField.ContentType.Standard;
            toggleImage.sprite = showIcon;
        }

        // Force InputField to refresh display
        passwordInput.ForceLabelUpdate();
    }
}
