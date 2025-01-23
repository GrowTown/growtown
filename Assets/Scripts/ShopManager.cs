using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    #region Functions
    public void ToBuyWheat()
    {
        if (GameManager.Instance.CurrentScore >= 10)
        {
            GameManager.Instance.CurrentScore -= 10;
            GameManager.Instance.CurrentWheatSeedCount += 1;
           

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
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
    internal bool isPasticidsBought=false;
    public void ToBuyCleaningTool()
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
           StartCoroutine (GameManager.Instance.ShowBoughtLand("wheat"));
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