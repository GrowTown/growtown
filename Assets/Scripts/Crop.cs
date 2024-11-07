using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int price;
}


[CreateAssetMenu(fileName = "NewCrop", menuName = "Inventory/Crop")]
public class Crop : InventoryItem
{
    [SerializeField]GameObject plant;
    [SerializeField]float _growthTime;  
    [SerializeField] int _harvestYield;
    [SerializeField]int _salePrice;

}

