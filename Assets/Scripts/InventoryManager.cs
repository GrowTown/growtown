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
                inventoryItem.useBT.gameObject.SetActive(false);
                inventoryItems.Add(item.Item.itemName, inventoryItem);
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
            }
            else
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, 1);
                inventoryItem.useBT.gameObject.SetActive(false);
                inventoryItems.Add(item.Item.itemName, inventoryItem);
            }
        }
    }
    public void AddToInventory(ShopItemHolder item,int count)

    {
        Transform parent = GetParentTransform(item.Item.type);
        if (inventoryItems.ContainsKey(item.Item.itemName))
        {
            if (item.Item.itemName != "TomatoSeed" || item.Item.itemName != "WheatSeed" || item.Item.itemName != "BeansSeed")
            {

                inventoryItems[item.Item.itemName].Itemcount += 1;
            }
            else
            {
                inventoryItems[item.Item.itemName].Itemcount += count;
            }
        }
        else
        {
            if (item.Item.itemName == "TomatoSeed")
            {

                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, 50);
                inventoryItem.useBT.gameObject.SetActive(false);
                inventoryItems.Add(item.Item.itemName, inventoryItem);
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
            }
            else
            {
                GameObject newItem = Instantiate(inventoryItemPrefab, parent);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.Initialize(item, count);
                inventoryItem.useBT.gameObject.SetActive(false);
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
