using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoginManager : MonoBehaviour
{
    private string filePath;
    private AccountList accountList = new AccountList();

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "accounts.json");
        LoadAccounts();
        Debug.Log("File lưu tại: " + filePath);
    }

    public void Register(string username, string password)
    {
        if (accountList.accounts.Exists(acc => acc.username == username))
        {
            Debug.Log("❌ Tài khoản đã tồn tại.");
            return;
        }

        Account newAccount = new Account { username = username, password = password };
        accountList.accounts.Add(newAccount);
        SaveAccounts();
        Debug.Log("✅ Đăng ký thành công!");
    }

    public bool Login(string username, string password)
    {
        Account acc = accountList.accounts.Find(a => a.username == username && a.password == password);
        return acc != null;
    }
    public bool IsUsernameTaken(string username)
    {
        return accountList.accounts.Exists(acc => acc.username == username);
    }

    void SaveAccounts()
    {
        string json = JsonUtility.ToJson(accountList, true);
        File.WriteAllText(filePath, json);
    }

    void LoadAccounts()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            accountList = JsonUtility.FromJson<AccountList>(json);
        }
    }
    public Account GetAccount(string username)
    {
        return accountList.accounts.Find(acc => acc.username == username);
    }

    public void SetCharacterForAccount(string username, string characterClass)
    {
        var acc = GetAccount(username);
        if (acc != null && string.IsNullOrEmpty(acc.characterClass))
        {
            acc.characterClass = characterClass;
            SaveAccounts();
            Debug.Log($"✅ Nhân vật {characterClass} đã lưu cho tài khoản {username}");
        }
    }

}
