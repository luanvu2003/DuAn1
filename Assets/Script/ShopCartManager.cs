using UnityEngine;
using TMPro;

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
    }

    [Header("Danh sách Item trong Shop")]
    public ShopItem[] items = new ShopItem[4]; // 4 item

    [Header("UI khác")]
    public TMP_Text totalPriceText;        // TextMeshPro tổng tiền
    public GameObject notifySuccess;       // Notify mua thành công
    public GameObject notifyFail;          // Notify không đủ tiền

    private int totalPrice = 0;

    void Start()
    {
        // Khởi tạo dữ liệu item
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemNameText != null)
                items[i].itemNameText.text = items[i].itemName;

            if (items[i].priceText != null)
                items[i].priceText.text = $"{items[i].price} <sprite name=\"coin\">";

            items[i].quantity = 0;
            if (items[i].quantityText != null)
                items[i].quantityText.text = "0";
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
            totalPriceText.text = $"Tổng tiền: {totalPrice} <sprite name=\"coin\">";
    }

    // Nút "Mua"
    public void Buy()
    {
        int playerCoin = PlayerPrefs.GetInt("player_coin", 0);

        if (playerCoin >= totalPrice && totalPrice > 0)
        {
            // Trừ coin
            playerCoin -= totalPrice;
            PlayerPrefs.SetInt("player_coin", playerCoin);
            PlayerPrefs.Save();

            // Reset số lượng đã chọn
            for (int i = 0; i < items.Length; i++)
            {
                items[i].quantity = 0;
                UpdateItemUI(i);
            }

            UpdateTotalPrice();

            if (notifySuccess != null) notifySuccess.SetActive(true);
            if (notifyFail != null) notifyFail.SetActive(false);
        }
        else
        {
            if (notifyFail != null) notifyFail.SetActive(true);
            if (notifySuccess != null) notifySuccess.SetActive(false);
        }
    }
}
