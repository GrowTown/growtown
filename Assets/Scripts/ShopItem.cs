using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Item/ShopItem",order =0)]
public class ShopItem : ScriptableObject
{
    public string itemName=string.Empty;
    public Sprite icon;
    public int price;
    public int level;
    public ItemType type;
}



public enum ItemType
{
    Seeds,
    Tools,
    PowerUps,
    NFTs
}



