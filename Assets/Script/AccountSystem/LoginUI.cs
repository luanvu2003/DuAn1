using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
public class LoginUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;

    public Text usernameErrorText;
    public Text passwordErrorText;
    public Text confirmPasswordErrorText;
    public Text generalMessageText;

    public LoginManager loginManager;

    public void OnRegisterButton()
    {
        // Xoá lỗi cũ
        usernameErrorText.text = "";
        passwordErrorText.text = "";
        confirmPasswordErrorText.text = "";
        generalMessageText.text = "";

        string username = usernameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        bool valid = true;

        // Kiểm tra username trùng
        if (loginManager.IsUsernameTaken(username))
        {
            usernameErrorText.text = "Tên tài khoản đã có người dùng";
            valid = false;
        }

        // Kiểm tra mật khẩu hợp lệ
        if (!IsPasswordValid(password))
        {
            passwordErrorText.text = "Mật khẩu cần ít nhất 1 chữ hoa và 1 số";
            valid = false;
        }

        // Kiểm tra khớp mật khẩu
        if (password != confirmPassword)
        {
            confirmPasswordErrorText.text = "Mật khẩu không khớp";
            valid = false;
        }

        if (!valid)
        {
            return;
        }

        // Đăng ký nếu hợp lệ
        loginManager.Register(username, password);
        generalMessageText.text = "✅ Đăng ký thành công!";
        SceneManager.LoadScene("Login");
    }

    public void OnLoginButton()
    {
        generalMessageText.text = "";
        string username = usernameInput.text;
        string password = passwordInput.text;

        bool success = loginManager.Login(username, password);

        if (success)
        {
            generalMessageText.text = "✅ Đăng nhập thành công!";
            PlayerPrefs.SetString("NextScene", "MainMenu");
            SceneManager.LoadScene("Loading");
        }
        else
            generalMessageText.text = "❌ Sai tài khoản hoặc mật khẩu.";
    }

    private bool IsPasswordValid(string password)
    {
        return Regex.IsMatch(password, @"[A-Z]") && Regex.IsMatch(password, @"\d");
    }
}
