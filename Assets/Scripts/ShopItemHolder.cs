using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    private ShopItem Item;

    //[SerializeField] private TextMeshProUGUI nameText;
    //[SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] internal Button buyBT;

    public void Initialize(ShopItem item)
    {
        Item = item;
        iconImage.sprite=Item.icon;
        priceText.text=Item.price.ToString();
        if (Item.level >= GameManager.Instance.CurrentScore)
        {
            UnlockItem();
        }

    }


    public void UnlockItem()
    {
        ///
    }
}
