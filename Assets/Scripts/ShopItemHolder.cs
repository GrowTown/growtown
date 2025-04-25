using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    internal ShopItem Item;
    internal string dummyName=string.Empty;
    //[SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] internal TextMeshProUGUI inventoryCountText;
    [SerializeField] internal Image iconImage;
    [SerializeField] internal TextMeshProUGUI priceText;
    [SerializeField] internal Button buyBT;

    public void Initialize(ShopItem item)
    {
        Item = item; 
        priceText.text=Item.price.ToString()+"GRC";
        if(Item.level== 1) iconImage.sprite = Item.icon;
       
    }

    public void InitializetheDummy(string n)
    {
        dummyName = n;
    }


    public void UnlockItem()
    {
        iconImage.sprite = Item.icon;
    }
}
