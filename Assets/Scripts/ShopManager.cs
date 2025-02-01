using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;


public class ShopManager : MonoBehaviour
{

    private Dictionary<ItemType, List<ShopItem>> shopItems = new Dictionary<ItemType, List<ShopItem>>();
    Dictionary<string, Action> buttonActions;
    public Dictionary<string,Button>buttons = new Dictionary<string, Button>();
    [SerializeField] private GameObject itemPrefab;

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
        shopItems.Add(ItemType.NFTS, new List<ShopItem>());
        shopItems.Add(ItemType.Sell, new List<ShopItem>());

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
                ///todo
                GameObject itemObject = Instantiate(itemPrefab, UI_Manager.Instance.TabGroup.objectsToSwap[i].transform.GetChild(0).GetChild(0));
                itemObject.GetComponent<ShopItemHolder>().Initialize(item);
                buttons.Add(item.itemName, itemObject.GetComponent<ShopItemHolder>().buyBT);
            }

        }
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
                button.onClick.AddListener(() => buttonActions[itemName]());
            }
            else
            {
                Debug.LogWarning($"No action found for item: {itemName}");
            }
        }


    }

    private void OnLevelChanged(PlayerLevel info)
    {
        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            ItemType key = shopItems.Keys.ToArray()[i];
            for (int j = 0; j < shopItems[key].Count; j++)
            {
                ShopItem item = shopItems[key][j];
                if (item.level == info.CurrentPlayerLevel)
                {
                    ///to do
                    

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
        if (GameManager.Instance.CurrentScore >= 20)
        {
            GameManager.Instance.CurrentScore -= 20;
            isPasticidsBought = true;
            GameManager.Instance.CurrentPasticideCount += 1;

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyWateringTool()
    {
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
        if (GameManager.Instance.CurrentScore >= 20)
        {
            GameManager.Instance.CurrentScore -= 20;
            UI_Manager.Instance.isSuperXpEnable = true;
            UI_Manager.Instance.SliderControls.gameObject.SetActive(true);
            UI_Manager.Instance.SliderControls.StartSliderBehavior();
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyWheatField()
    {
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
    #endregion
}
public enum SeedName
{
    Wheat,
    Carrots,
    Strawberries,
}