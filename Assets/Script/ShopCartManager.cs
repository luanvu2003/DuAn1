using UnityEngine;
using TMPro;

public class ShopCartManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string itemName;            // Tên item (để dễ nhớ trong Inspector)
        public int price;                  // Giá mỗi item
        [HideInInspector] public int quantity;  // Số lượng muốn mua trong shop
        public TMP_Text quantityText;      // Text số lượng chọn trong shop
        public TMP_Text priceText;         // Text giá hiển thị trong shop

        [Header("HUD khi sở hữu")]
        public GameObject hudObject;       // HUD icon item khi sở hữu
        public TMP_Text hudQuantityText;   // Text số lượng trên HUD
        [HideInInspector] public int ownedQuantity; // Số lượng đang sở hữu
    }

    [Header("Danh sách Item trong Shop")]
    public ShopItem[] items = new ShopItem[4];

    [Header("UI khác")]
    public TMP_Text totalPriceText;        // Text tổng tiền
    public GameObject notifySuccess;       // Notify mua thành công
    public GameObject notifyFail;          // Notify không đủ tiền

    private int totalPrice = 0;

    void Start()
    {
        // Gán giá hiển thị và reset số lượng khi bắt đầu
        foreach (var item in items)
        {
            if (item.priceText != null)
                item.priceText.text = item.price + " coin";

            item.quantity = 0;
            if (item.quantityText != null)
                item.quantityText.text = "0";

            item.ownedQuantity = 0;
            if (item.hudObject != null)
                item.hudObject.SetActive(false);
        }
        UpdateTotalPrice();

        // Ẩn notify khi bắt đầu
        if (notifySuccess != null) notifySuccess.SetActive(false);
        if (notifyFail != null) notifyFail.SetActive(false);
    }

    // Nút +
    public void AddItem(int index)
    {
        items[index].quantity++;
        items[index].quantityText.text = items[index].quantity.ToString();
        UpdateTotalPrice();
    }

    // Nút -
    public void RemoveItem(int index)
    {
        if (items[index].quantity > 0)
        {
            items[index].quantity--;
            items[index].quantityText.text = items[index].quantity.ToString();
            UpdateTotalPrice();
        }
    }

    // Cập nhật tổng tiền
    void UpdateTotalPrice()
    {
        totalPrice = 0;
        foreach (var item in items)
        {
            totalPrice += item.price * item.quantity;
        }
        totalPriceText.text = "Tổng tiền: " + totalPrice + " coin";
    }

    // Nút Mua
    public void Buy()
    {
        int playerCoin = PlayerPrefs.GetInt("player_coin", 0);

        if (playerCoin >= totalPrice && totalPrice > 0)
        {
            // Trừ coin
            playerCoin -= totalPrice;
            PlayerPrefs.SetInt("player_coin", playerCoin);
            PlayerPrefs.Save();

            // Cập nhật HUD và số lượng sở hữu
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].quantity > 0)
                {
                    items[i].ownedQuantity += items[i].quantity;

                    if (items[i].hudObject != null)
                    {
                        items[i].hudObject.SetActive(true);
                        items[i].hudQuantityText.text = "x" + items[i].ownedQuantity;
                    }

                    // Reset số lượng chọn mua trong shop
                    items[i].quantity = 0;
                    items[i].quantityText.text = "0";
                }
            }

            UpdateTotalPrice();

            // Notify thành công
            if (notifySuccess != null) notifySuccess.SetActive(true);
            if (notifyFail != null) notifyFail.SetActive(false);
        }
        else
        {
            // Notify thất bại
            if (notifyFail != null) notifyFail.SetActive(true);
            if (notifySuccess != null) notifySuccess.SetActive(false);
        }
    }

    // Khi dùng item trong gameplay
    public void UseItem(int index)
    {
        if (items[index].ownedQuantity > 0)
        {
            items[index].ownedQuantity--;

            if (items[index].ownedQuantity > 0)
            {
                items[index].hudQuantityText.text = "x" + items[index].ownedQuantity;
            }
            else
            {
                items[index].hudObject.SetActive(false);
            }
        }
    }
}
