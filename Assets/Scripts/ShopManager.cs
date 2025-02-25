using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{

    private Dictionary<ItemType, List<ShopItem>> shopItems = new Dictionary<ItemType, List<ShopItem>>();
    Dictionary<string, Action> buttonActions;
    public Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private RectTransform invetoryBagPos;
    [SerializeField] private Sprite animICON;

    private void Start()
    {
        Load();
        Initialize();
        InitializeButtonActions();
    }
    #region Functions

    private void Load()
    {
        ShopItem[] items = Resources.LoadAll<ShopItem>("Shop");
        shopItems.Add(ItemType.Seeds, new List<ShopItem>());
        shopItems.Add(ItemType.Tools, new List<ShopItem>());
        shopItems.Add(ItemType.PowerUps, new List<ShopItem>());
        shopItems.Add(ItemType.NFTs , new List<ShopItem>());

        foreach (var item in items)
        {
            shopItems[item.type].Add(item);
        }

    }

    private void Initialize()
    {
        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            foreach (var item in shopItems[(ItemType)i])
            {
                GameObject itemObject = Instantiate(itemPrefab, UI_Manager.Instance.TabGroup.objectsToSwap[i].transform.GetChild(0).GetChild(0));
                var shopIH = itemObject.GetComponent<ShopItemHolder>();
                shopIH.Initialize(item);
                if (item.level == 1)
                {
                    buttons.Add(item.itemName, shopIH.buyBT);
                }
                else
                {
                    shopIH.buyBT.gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject itemObject = Instantiate(itemPrefab, UI_Manager.Instance.TabGroup.objectsToSwap[i].transform.GetChild(0).GetChild(0));
                var shopIH = itemObject.GetComponent<ShopItemHolder>();
                shopIH.buyBT.gameObject.SetActive(false);
            }
        }

        if (buttons.ContainsKey("EnergyPoints"))
        {
            UI_Manager.Instance.energyBuyBT = buttons["EnergyPoints"];
        }
        if (buttons.ContainsKey("WaterPoints"))
        {
            UI_Manager.Instance.waterBuyBT = buttons["WaterPoints"];
        } 
        if(buttons.ContainsKey("CleaningTool")) UI_Manager.Instance.cleaningWeaponBT = buttons["CleaningTool"];
        if (buttons.ContainsKey("CuttingTool")) UI_Manager.Instance.sickleWeaponBT = buttons["CuttingTool"];
        if (buttons.ContainsKey("WateringTool")) UI_Manager.Instance.wateringWeaponBT = buttons["WateringTool"];

    }
    private void InitializeButtonActions()
    {
        buttonActions = new Dictionary<string, Action>
        {
            { "TomatoSeed", () => ToBuyTomato() },
            { "BeansLand", () => ToBuyCarrotField()},
            { "BeansSeed", () => ToBuyStrawberries() },
            { "CleaningTool", () => ToBuyCleaningTool() },
            { "CuttingTool", () =>  ToBuyCuttingTool() },
            { "EnergyPoints", () =>GameManager.Instance.ToBuyEnergyPoints() },
            { "Pasticide", () => ToBuyPasticide() },
            { "SuperXp", () =>ToBuySuperXp()},
            { "F", () => Debug.Log("Third Button Pressed") },
            { "WateringTool", () => ToBuyWateringTool() },
            { "WaterPoints", () =>GameManager.Instance.ToBuyWaterPoints() },
            { "WheatLand", () => ToBuyWheatField() },
            { "WheatSeed", () => ToBuyWheat() },
            { "F1", () => Debug.Log("Third Button Pressed") },
            { "F2", () => Debug.Log("Third Button Pressed") },
        };

        foreach (var pair in buttons)
        {
            string itemName = pair.Key;
            Button button = pair.Value;

            if (buttonActions.ContainsKey(itemName))
            {
                button.onClick.AddListener(() =>
                {
                    buttonActions[itemName]();
                    var shopItemHolder = FindShopItemHolder(itemName);
                    UI_Manager.Instance.UIAnimationM.PlayMoveToInventoryAnimation(animICON, shopItemHolder.GetComponent<RectTransform>(), invetoryBagPos);
    
                    if (shopItemHolder != null)
                    {
                        UI_Manager.Instance.InventoryManager.AddToInventory(shopItemHolder);
                    }
                    else
                    {
                        Debug.LogWarning($"No ShopItemHolder found for item: {itemName}");
                    }
                });
            }
            else
            {
                Debug.LogWarning($"No action found for item: {itemName}");
            }
        }
    }

    internal ShopItemHolder FindShopItemHolder(string itemName)
    {
        foreach (var tab in UI_Manager.Instance.TabGroup.objectsToSwap)
        {
            Transform content = tab.transform.GetChild(0).GetChild(0);
            foreach (Transform child in content)
            {
                ShopItemHolder holder = child.GetComponent<ShopItemHolder>();
                if (holder != null && holder.Item!=null)
                {
                     if(holder.Item.itemName == itemName)
                    return holder;
                }
            }
        }
        return null;
    }

    internal void OnLevelChanged(int LVinfo)
    {
        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            var spItemParent = UI_Manager.Instance.TabGroup.objectsToSwap[i].transform.GetChild(0).GetChild(0);
            List<ShopItemHolder> spItemList = new List<ShopItemHolder>();
            ItemType key = shopItems.Keys.ToArray()[i];
            for (int j = 0; j < shopItems[key].Count; j++)
            {
                ShopItem item = shopItems[key][j];
                if (item.level == LVinfo)
                {
                    for (int k = 0; k < spItemParent.childCount; k++)
                    {
                        var shopItem = spItemParent.GetChild(k).gameObject.GetComponent<ShopItemHolder>();
                        if (shopItem != null)
                        {
                            spItemList.Add(shopItem);
                        }
                    }

                    var existingItem = spItemList.Find(x => x.Item.itemName == item.itemName);
                    if (existingItem != null)
                    {
                        existingItem.UnlockItem();
                        existingItem.buyBT.gameObject.SetActive(true);
                        buttons.Add(item.itemName, existingItem.buyBT);
                        if (buttonActions.ContainsKey(item.itemName))
                        {
                            buttons[item.itemName].onClick.AddListener(() =>
                            {
                                buttonActions[item.itemName]();
                                var shopItemHolder = FindShopItemHolder(item.itemName);
                                UI_Manager.Instance.UIAnimationM.PlayMoveToInventoryAnimation(animICON, shopItemHolder.GetComponent<RectTransform>(), invetoryBagPos);

                                if (shopItemHolder != null)
                                {
                                    UI_Manager.Instance.InventoryManager.AddToInventory(shopItemHolder);
                                }
                                else
                                {
                                    Debug.LogWarning($"No ShopItemHolder found for item: {item.itemName}");
                                }
                            });
                        }
                    }

                    spItemList.Clear();
                    if (buttons.ContainsKey("SuperXp") && !UI_Manager.Instance.isButtonsInitialized)
                    {
                        UI_Manager.Instance.superXpBuyBT = buttons["SuperXp"];
                        UI_Manager.Instance.isButtonsInitialized = true;
                    }
                }
            }
        }

    }
    public void ToBuyWheat()
    {
        if (GameManager.Instance.CurrentScore >= 8)
        {
            GameManager.Instance.CurrentScore -= 8;
            GameManager.Instance.CurrentWheatSeedCount += 1;
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
        }
    }
    bool forStarterPack;
    public void ToBuyTomato()
    {
        AudioManager.Instance.PlaySFX();

        if (!forStarterPack)
        {
            GameManager.Instance.CurrentScore -= 250;
            GameManager.Instance.CurrentTomatoSeedCount += 50;
            GameManager.Instance.HasNotEnoughSeeds = false;
            GameManager.Instance.cropseedingStarted = false;
            forStarterPack = true;
        }
        else
        {
            if (GameManager.Instance.CurrentScore >= 5)
            {
                GameManager.Instance.CurrentScore -= 5;
                GameManager.Instance.CurrentTomatoSeedCount += 1;
                GameManager.Instance.HasNotEnoughSeeds = false;
                GameManager.Instance.cropseedingStarted = false;
            }
            else
            {
                UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
                // Debug.Log("You didn't have enough money");
            }
        }
    }
    public void ToBuyStrawberries()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 12)
        {
            GameManager.Instance.CurrentScore -= 12;
            GameManager.Instance.CurrentStrawberriesSeedCount += 1;
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            // Debug.Log("You didn't have enough money");
        }
    }

    internal bool isCleningToolBought;
    internal bool isWateringToolBought;
    internal bool isCuttingToolBought;
    internal bool isPasticidsBought = false;
    public void ToBuyCleaningTool()
    {
        AudioManager.Instance.PlaySFX();

        if (!isCleningToolBought)
        {
            if (GameManager.Instance.CurrentScore >= 5)
            {
                GameManager.Instance.CurrentScore -= 5;
                isCleningToolBought = true;
                //ForWeaponAdd += 1;
                // UI_Manager.Instance.inventoryPanel.transform.GetChild(3).gameObject.GetComponent<SelectionFunctionality>().productCount.text = ForWeaponAdd.ToString();
            }
            else
            {
                UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
                //Debug.Log("You didn't have enough money");
            }
        }
    }

    public void ToBuyCuttingTool()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 5)
        {
            GameManager.Instance.CurrentScore -= 5;
            isCuttingToolBought = true;
            // ForWeaponAdd2 += 1;
            // UI_Manager.Instance.inventoryPanel.transform.GetChild(5).gameObject.GetComponent<SelectionFunctionality>().productCount.text = ForWeaponAdd.ToString();
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
      
    public void ToBuyPasticide()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 20)
        {
            GameManager.Instance.CurrentScore -= 20;
            GameManager.Instance.PesticideboughtCount(UI_Manager.Instance.FieldManager.CurrentFieldID, true);

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyWateringTool()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 5)
        {
            GameManager.Instance.CurrentScore -= 5;
            isWateringToolBought = true;
            // ForWeaponAdd1 += 1;
            //UI_Manager.Instance.inventoryPanel.transform.GetChild(4).gameObject.GetComponent<SelectionFunctionality>().productCount.text = ForWeaponAdd.ToString();
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuySuperXp()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 20)
        {
            GameManager.Instance.CurrentScore -= 20;      
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyWheatField()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 20)
        {
            GameManager.Instance.CurrentScore -= 20;
            UI_Manager.Instance.wheatFieldArea.SetActive(true);
            GameManager.Instance.isShowingnewLand = true;
            StartCoroutine(GameManager.Instance.ShowBoughtLand("wheat"));
            UI_Manager.Instance.wheatlandBuyBT.interactable = false;

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }

    public void ToBuyCarrotField()
    {
        AudioManager.Instance.PlaySFX();

        if (GameManager.Instance.CurrentScore >= 20)
        {
            GameManager.Instance.CurrentScore -= 20;
            UI_Manager.Instance.carrotFieldArea.SetActive(true);
            GameManager.Instance.isShowingnewLand = true;
            StartCoroutine(GameManager.Instance.ShowBoughtLand("carrot"));
            UI_Manager.Instance.carrotlandBuyBT.interactable = false;

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }

    internal void SellHarvest()
    {
        foreach (var item in UI_Manager.Instance.ListOfHarvestCount)
        {
            GameManager.Instance.CounttheHarvest(item.Value.Count);
        }

        UI_Manager.Instance.sellPopupPanel.SetActive(false);
        UI_Manager.Instance.marketPopUp.SetActive(true);
        GameManager.Instance.isHarvestCompleted = false; 
        UI_Manager.Instance.ListOfHarvestCount.Clear();
        GameManager.Instance.HarvestCount = 0;
        UI_Manager.Instance.GrowthStartedPlants.Clear();
    }
    #endregion
}
public enum SeedName
{
    Wheat,
    Carrots,
    Strawberries,
}