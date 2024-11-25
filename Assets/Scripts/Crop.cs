using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
}


[CreateAssetMenu(fileName = "NewCrop", menuName = "Shop/Crop")]
public class Crop : ShopItem
{
    [SerializeField]GameObject plant;
    [SerializeField]GameObject tomato;
    [SerializeField]float _growthTime;  
    [SerializeField]int _harvestYield;
    [SerializeField]int _salePrice;

}



