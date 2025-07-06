using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Text messageText;
    public LoginManager loginManager;

    public void OnRegisterButton()
    {
        loginManager.Register(usernameInput.text, passwordInput.text);
        messageText.text = "Đăng ký xong (xem Console để biết kết quả)";
    }

    public void OnLoginButton()
    {
        loginManager.Login(usernameInput.text, passwordInput.text);
        messageText.text = "Đăng nhập xong (xem Console để biết kết quả)";
    }
}
