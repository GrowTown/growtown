using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    internal ShopItem Item;

    //[SerializeField] private TextMeshProUGUI nameText;
    //[SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] internal Image iconImage;
    [SerializeField] internal TextMeshProUGUI priceText;
    [SerializeField] internal Button buyBT;

    public void Initialize(ShopItem item)
    {
        Item = item; 
        priceText.text=Item.price.ToString()+"GRC";
        if(Item.level== 1) iconImage.sprite = Item.icon;
       
    }


    public void UnlockItem()
    {
        iconImage.sprite = Item.icon;
    }
}
