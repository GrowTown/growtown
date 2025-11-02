using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private Transform seedsParent;
    [SerializeField] private Transform toolsParent;
    [SerializeField] private Transform powerUpsParent;
    [SerializeField] private Transform fishParent;

    internal Dictionary<string, InventoryItem> inventoryItems = new Dictionary<string, InventoryItem>();
    internal Dictionary<string, System.Action> inventoryButtonActions = new Dictionary<string, System.Action>();

    private Transform GetParentTransform(ItemType type)
    {
        switch (type)
        {
            case ItemType.Seeds: return seedsParent;
            case ItemType.Tools: return toolsParent;
            case ItemType.PowerUps: return powerUpsParent;
            case ItemType.Fish: return fishParent;
            default: return seedsParent;
        }
    }

    // ==========================
    // Add to Inventory
    // ==========================
    public void AddToInventory(ShopItemHolder item)
    {
        Transform parent = GetParentTransform(item.Item.type);

        if (inventoryItems.ContainsKey(item.Item.itemName))
        {
            // stack items except seeds/points
            if (CanStack(item.Item.itemName))
            {
                inventoryItems[item.Item.itemName].Itemcount += 1;
            }
        }
        else
        {
            if (item.Item.itemName == "TomatoSeed")
            {
                AddCountForInventoryItems(item, parent, 50);
            }
            else if (item.Item.itemName == "SuperXp")
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, 1);
                inventoryItems.Add(item.Item.itemName, inventoryItem);

                if (inventoryButtonActions.ContainsKey(item.Item.itemName))
                    inventoryItem.useBT.onClick.AddListener(() => inventoryButtonActions[item.Item.itemName]());
            }
            else if (item.Item.itemName == "WaterPoints")
            {
                AddCountForInventoryItems(item, parent, 500);
            }
            else if (item.Item.itemName == "EnergyPoints")
            {
                AddCountForInventoryItems(item, parent, 500);
            }
            else
            {
                if (item.Item.itemName != "TomatoSeed" && item.Item.itemName != "WheatSeed" &&
                    item.Item.itemName != "BeansSeed" && item.Item.itemName != "WaterPoints" &&
                    item.Item.itemName != "EnergyPoints")
                {
                    AddCountForInventoryItems(item, parent, 1);
                }
            }
        }
    }

    public void AddToInventory(ShopItemHolder item, int quantity)
    {
        if (item == null || quantity <= 0)
            return;

        AddToInventory(item);

        if (quantity <= 1)
            return;

        if (inventoryItems.TryGetValue(item.Item.itemName, out var inventoryItem) && CanStack(item.Item.itemName))
        {
            inventoryItem.Itemcount += (quantity - 1);
        }
    }

    private bool CanStack(string itemName)
    {
        return itemName != "TomatoSeed" &&
               itemName != "WheatSeed" &&
               itemName != "BeansSeed" &&
               itemName != "WaterPoints" &&
               itemName != "EnergyPoints";
    }

    // ==========================
    // Fish inventory
    // ==========================
    public void AddFishToInventory(Fish fish)
    {
        if (fish == null)
            return;

        string key = fish.fishName;
        if (string.IsNullOrEmpty(key))
            return;

        if (inventoryItems.TryGetValue(key, out var existingItem))
        {
            existingItem.Itemcount += 1;
            existingItem.SetupFish(fish);
            return;
        }

        Transform parent = fishParent != null ? fishParent : GetParentTransform(ItemType.Fish);
        GameObject newItem = Instantiate(inventoryItemPrefab, parent);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        if (inventoryItem == null)
            return;

        inventoryItem.SetupFish(fish);
        inventoryItem.Itemcount = 1;
        inventoryItems.Add(key, inventoryItem);
    }

    // Helper: Instantiate Inventory Item
    // ==========================
    private void AddCountForInventoryItems(ShopItemHolder item, Transform parent, int count)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, parent);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.Initialize(item, count);
        inventoryItems.Add(item.Item.itemName, inventoryItem);

        if (inventoryButtonActions.ContainsKey(item.Item.itemName))
            inventoryItem.useBT.onClick.AddListener(() => inventoryButtonActions[item.Item.itemName]());
    }

    // ==========================
    // Update count from outside (like Shop/Market)
    // ==========================
    internal void UpdatingTheInventoryCountToMarket(string itemName, int newCount)
    {
        if (inventoryItems.ContainsKey(itemName))
        {
            inventoryItems[itemName].Itemcount = newCount;
        }
    }
}







