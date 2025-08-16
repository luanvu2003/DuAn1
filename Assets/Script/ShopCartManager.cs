using UnityEngine;
using TMPro;
[ExecuteAlways]
public class ShopCartManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;            // Tên item
        public int price;                  // Giá mỗi item
        [HideInInspector] public int quantity; // Số lượng muốn mua

        [Header("UI trong Shop")]
        public TMP_Text itemNameText;      // TextMeshPro tên item
        public TMP_Text priceText;         // TextMeshPro giá (kèm icon coin)
        public TMP_Text quantityText;      // TextMeshPro số lượng chọn mua
        [Header("UI HUD ngoài gameplay")]
        public TMP_Text amountHUD;
    }

    [Header("Danh sách Item trong Shop")]
    public ShopItem[] items = new ShopItem[4]; // 4 item

    [Header("UI khác")]
    public TMP_Text totalPriceText;        // TextMeshPro tổng tiền
    public GameObject notifySuccess;       // Notify mua thành công
    public GameObject notifyFail;          // Notify không đủ tiền
    public GameObject ShopPanel;
    private int totalPrice = 0;
    public PlayerController player;
    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemNameText != null)
                items[i].itemNameText.text = items[i].itemName;

            if (items[i].priceText != null)
                items[i].priceText.text = $"{items[i].price}";

            items[i].quantity = 0;
            if (items[i].quantityText != null)
                items[i].quantityText.text = "0";

            if (items[i].amountHUD != null)
            {
                int savedAmount = PlayerPrefs.GetInt("item_amount_" + i, 0);
                items[i].amountHUD.text = "x" + savedAmount;
            }
        }

        UpdateTotalPrice();

        if (notifySuccess != null) notifySuccess.SetActive(false);
        if (notifyFail != null) notifyFail.SetActive(false);
    }

    // Nút "+"
    public void AddItem(int index)
    {
        items[index].quantity++;
        UpdateItemUI(index);
        UpdateTotalPrice();
    }

    // Nút "-"
    public void RemoveItem(int index)
    {
        if (items[index].quantity > 0)
        {
            items[index].quantity--;
            UpdateItemUI(index);
            UpdateTotalPrice();
        }
    }

    // Cập nhật số lượng hiển thị
    private void UpdateItemUI(int index)
    {
        if (items[index].quantityText != null)
            items[index].quantityText.text = items[index].quantity.ToString();
    }

    // Cập nhật tổng tiền tất cả item
    private void UpdateTotalPrice()
    {
        totalPrice = 0;
        for (int i = 0; i < items.Length; i++)
        {
            totalPrice += items[i].price * items[i].quantity;
        }
        if (totalPriceText != null)
            totalPriceText.text = $"Tổng: {totalPrice}";
    }

    // Nút "Mua"
    public void Buy()
    {
        UpdateTotalPrice();

        int playerCoin = player.GetCoin(); // lấy trực tiếp từ PlayerController

        if (playerCoin >= totalPrice && totalPrice > 0)
        {
            player.AddCoin(-totalPrice); // trừ coin và update HUD, PlayerPrefs

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].quantity > 0)
                {
                    string hudText = items[i].amountHUD.text.Replace("x", "");
                    int currentHUD = 0;
                    int.TryParse(hudText, out currentHUD);

                    currentHUD += items[i].quantity;
                    items[i].amountHUD.text = "x" + currentHUD;

                    PlayerPrefs.SetInt("item_amount_" + i, currentHUD);

                    items[i].quantity = 0;
                    UpdateItemUI(i);
                }
            }

            PlayerPrefs.Save();
            UpdateTotalPrice();

            notifySuccess?.SetActive(true);
            notifyFail?.SetActive(false);
        }
        else
        {
            notifyFail?.SetActive(true);
            notifySuccess?.SetActive(false);
        }
        PlayerController.IsUIOpen = false; // Đánh dấu UI đã đóng
    }

    public void CloseBuy()
    {
        // Reset số lượng mua của từng item
        for (int i = 0; i < items.Length; i++)
        {
            items[i].quantity = 0;
            UpdateItemUI(i);
        }

        // Reset tổng tiền
        totalPrice = 0;
        if (totalPriceText != null)
            totalPriceText.text = $"Tổng: {totalPrice}";

        Time.timeScale = 1f;
        ShopPanel.SetActive(false);
        notifySuccess.SetActive(false);
        notifyFail.SetActive(false);
        PlayerController.IsUIOpen = false; // Đánh dấu UI đã đóng
    }

}
