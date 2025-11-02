using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "NewShopItem", menuName = "Item/ShopItem", order = 0)]
[System.Serializable]
public class ShopItem : ScriptableObject
{
    public string itemName;          // e.g. Salmon
    public ItemType type;            // Seeds, Tools, PowerUps, NFTs, Fish
    [TextArea] public string description;
    public Sprite icon;
    public int price;
    public int level;                // 👈 NEW (fixes missing reference)
}

public enum ItemType
{
    Seeds,
    Tools,
    PowerUps,
    NFTs,
    Fish
}
