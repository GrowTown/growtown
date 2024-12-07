using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
 
    #region Functions
    public void ToBuyWheat()
    {
        if(UI_Manager.Instance.scoreIn>=10)
        {
            UI_Manager.Instance.scoreIn -=10;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
            GameManager.Instance.CurrentWheatSeedCount+= 1;
           // UI_Manager.Instance.inventoryPanel.transform.GetChild().gameObject.GetComponent<SelectionFunctionality>().productCount.text = GameManager.Instance.CurrentWheatSeedCount.ToString();

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyTomato()
    {
        if(UI_Manager.Instance.scoreIn>=8)
        {
            UI_Manager.Instance.scoreIn -=8;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
            GameManager.Instance.CurrentTomatoSeedCount += 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(0).gameObject.GetComponent<SelectionFunctionality>().productCount.text = GameManager.Instance.CurrentTomatoSeedCount.ToString();
            GameManager.Instance.HasNotEnoughSeeds = false;
            GameManager.Instance.cropseedingStarted = false;
        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
           // Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyStrawberries()
    {
        if (UI_Manager.Instance.scoreIn >= 12)
        {
            UI_Manager.Instance.scoreIn -= 12;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
            GameManager.Instance.CurrentStrawberriesSeedCount += 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(2).gameObject.GetComponent<SelectionFunctionality>().productCount.text = GameManager.Instance.CurrentStrawberriesSeedCount.ToString();
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
    public void ToBuyCleaningTool()
    {
        if (UI_Manager.Instance.scoreIn >= 5)
        {
            UI_Manager.Instance.scoreIn -= 5;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
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
    public void ToBuyWateringTool()
    {
        if (UI_Manager.Instance.scoreIn >= 5)
        {
            UI_Manager.Instance.scoreIn -= 5;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
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
    public void ToBuyCuttingTool()
    {
        if (UI_Manager.Instance.scoreIn >= 5)
        {
            UI_Manager.Instance.scoreIn -= 5;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
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

    #endregion
}
public enum SeedName
{
    Wheat,
    Carrots,
    Strawberries,
}