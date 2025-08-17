using UnityEngine;
using TMPro;

public class ItemUseManager : MonoBehaviour
{
    public PlayerController player;
    public TMP_Text[] itemAmountsHUD; // HUD hiển thị xN cho từng item

    private void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        // Load số lượng item từ PlayerPrefs
        for (int i = 0; i < itemAmountsHUD.Length; i++)
        {
            int savedAmount = PlayerPrefs.GetInt("item_amount_" + i, 0);
            itemAmountsHUD[i].text = "x" + savedAmount;
        }

        // Load buff từ PlayerPrefs
        player.minDamage = PlayerPrefs.GetFloat("buff_minDamage", player.minDamage);
        player.maxDamage = PlayerPrefs.GetFloat("buff_maxDamage", player.maxDamage);
        player.moveSpeed = PlayerPrefs.GetFloat("buff_moveSpeed", player.moveSpeed);
        player.jumpForce = PlayerPrefs.GetFloat("buff_jumpForce", player.jumpForce);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0); // Hồi máu
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1); // Buff damage
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2); // Buff tốc chạy
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3); // Buff jump
    }

    void UseItem(int index)
    {
        string hudText = itemAmountsHUD[index].text.Replace("x", "");
        int amount = 0;
        int.TryParse(hudText, out amount);

        if (amount <= 0)
        {
            Debug.Log($"Item {index + 1} không còn để dùng!");
            return;
        }

        amount--;
        itemAmountsHUD[index].text = "x" + amount;
        PlayerPrefs.SetInt("item_amount_" + index, amount);

        switch (index)
        {
            case 0: // Hồi máu
                player.Heal(50);
                break;

            case 1: // Buff damage vĩnh viễn
                player.minDamage += 3f;
                player.maxDamage += 3f;
                PlayerPrefs.SetFloat("buff_minDamage", player.minDamage);
                PlayerPrefs.SetFloat("buff_maxDamage", player.maxDamage);
                break;

            case 2: // Buff tốc chạy vĩnh viễn
                player.moveSpeed += 1f;
                PlayerPrefs.SetFloat("buff_moveSpeed", player.moveSpeed);
                break;

            case 3: // Buff jump vĩnh viễn
                player.jumpForce += 1f;
                PlayerPrefs.SetFloat("buff_jumpForce", player.jumpForce);
                break;
        }

        PlayerPrefs.Save();
    }
    public static void ResetBuffs()
    {
        PlayerPrefs.DeleteKey("buff_minDamage");
        PlayerPrefs.DeleteKey("buff_maxDamage");
        PlayerPrefs.DeleteKey("buff_moveSpeed");
        PlayerPrefs.DeleteKey("buff_jumpForce");

        // Nếu muốn reset cả số lượng item
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.DeleteKey("item_amount_" + i);
        }

        PlayerPrefs.Save();
    }

}
