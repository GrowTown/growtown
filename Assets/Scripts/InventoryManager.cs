using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    internal Dictionary<string, InventoryItem> inventoryItems = new Dictionary<string, InventoryItem>();

    internal List<Transform> inventoryTransforms = new List<Transform>();

    [SerializeField] private Transform seedsParent;
    [SerializeField] private Transform toolsParent;
    [SerializeField] private Transform powerupsParent;
    [SerializeField] private Transform nftsParent;
    [SerializeField] private GameObject inventoryItemPrefab;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        inventoryTransforms.Add(seedsParent);
        inventoryTransforms.Add(toolsParent);
        inventoryTransforms.Add(powerupsParent);
        inventoryTransforms.Add(nftsParent);
    }
    public void AddToInventory(ShopItemHolder item)
    {
        Transform parent = GetParentTransform(item.Item.type);
        if (inventoryItems.ContainsKey(item.Item.itemName))
        {
            if (item.Item.itemName != "TomatoSeed" || item.Item.itemName != "WheatSeed" || item.Item.itemName != "BeansSeed")
                inventoryItems[item.Item.itemName].Itemcount += 1;
        }
        else
        {
            if (item.Item.itemName == "TomatoSeed")
            {

                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, 50);
                inventoryItems.Add(item.Item.itemName, inventoryItem);
            }
            else
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, 1);
                inventoryItems.Add(item.Item.itemName, inventoryItem);
            }
        }
    }

    private Transform GetParentTransform(ItemType type)
    {
        switch (type)
        {
            case ItemType.Seeds: return seedsParent;
            case ItemType.Tools: return toolsParent;
            case ItemType.PowerUps: return powerupsParent;
            case ItemType.NFTs: return nftsParent;
            default: return seedsParent;
        }
    }
    internal InventoryItem FindShopItemHolder(string itemName)
    {
        foreach (var tab in inventoryTransforms)
        {
            Transform content = tab;
            foreach (Transform child in content)
            {
                InventoryItem holder = child.GetComponent<InventoryItem>();
                if (holder != null && inventoryItems.ContainsKey(itemName))
                {
                    return holder;
                }
            }
        }
        return null;
    }


}
