using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    internal Dictionary<string, InventoryItem> inventoryItems = new Dictionary<string, InventoryItem>();
    Dictionary<string, Action> inventoryButtonActions;
    internal List<Transform> inventoryTransforms = new List<Transform>();

    [SerializeField] private Transform seedsParent;
    [SerializeField] private Transform toolsParent;
    [SerializeField] private Transform powerupsParent;
    [SerializeField] private Transform nftsParent;
    [SerializeField] internal GameObject inventoryItemPrefab;

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

        inventoryButtonActions = new Dictionary<string, Action>
        {

            { "SuperXp", () =>GameManager.Instance.SuperXpToUse()},
            { "F", () => Debug.Log("Third Button Pressed") },
            { "F1", () => Debug.Log("Third Button Pressed") },
            { "F2", () => Debug.Log("Third Button Pressed") },
        };
    }
    public void AddToInventory(ShopItemHolder item)
    {
        Transform parent = GetParentTransform(item.Item.type);
        if (inventoryItems.ContainsKey(item.Item.itemName))
        {
            if (item.Item.itemName != "TomatoSeed" && item.Item.itemName != "WheatSeed" && item.Item.itemName != "BeansSeed" && item.Item.itemName != "WaterPoints" && item.Item.itemName != "EnergyPoints")
            {

                inventoryItems[item.Item.itemName].Itemcount += 1;
                item.inventoryCountText.text = inventoryItems[item.Item.itemName].Itemcount.ToString();
            }
        }
        else
        {
            if (item.Item.itemName == "TomatoSeed")
            {

                AddCountForInventoryItems(item, parent, 50); ;
            }
            else if (item.Item.itemName == "SuperXp")
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, 1);
                inventoryItems.Add(item.Item.itemName, inventoryItem);
                if (inventoryButtonActions.ContainsKey(item.Item.itemName))
                {
                    inventoryItem.useBT.onClick.AddListener(() => inventoryButtonActions[item.Item.itemName]());
                }
                item.inventoryCountText.text = inventoryItems[item.Item.itemName].Itemcount.ToString();
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
                if (item.Item.itemName != "TomatoSeed" && item.Item.itemName != "WheatSeed" && item.Item.itemName != "BeansSeed" && item.Item.itemName != "WaterPoints" && item.Item.itemName != "EnergyPoints")
                {

                    AddCountForInventoryItems(item, parent, 1);
                }
            }
        }
    }


    void AddCountForInventoryItems(ShopItemHolder item, Transform parent, int count)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, parent);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.Initialize(item, count);
        inventoryItem.useBT.gameObject.SetActive(false);
        inventoryItems.Add(item.Item.itemName, inventoryItem);
        item.inventoryCountText.text = inventoryItems[item.Item.itemName].Itemcount.ToString();
    }

    internal void UpdatingTheInventoryCountToMarket(string item, int count)
    {
        var shopItemHolder = UI_Manager.Instance.ShopManager.FindShopItemHolder(item);
        if (shopItemHolder != null)
        {
            AddToInventory(shopItemHolder, count);
        }
    }
    public void AddToInventory(ShopItemHolder item, int count)

    {
        Transform parent = GetParentTransform(item.Item.type);
        if (inventoryItems.ContainsKey(item.Item.itemName))
        {

            inventoryItems[item.Item.itemName].Itemcount = count;
            item.inventoryCountText.text = inventoryItems[item.Item.itemName].Itemcount.ToString();
        }
        else
        {
            GameObject newItem = Instantiate(inventoryItemPrefab, parent);
            InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
            inventoryItem.Initialize(item, count);
            inventoryItem.useBT.gameObject.SetActive(false);
            inventoryItems.Add(item.Item.itemName, inventoryItem);
            item.inventoryCountText.text = inventoryItems[item.Item.itemName].Itemcount.ToString();
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
