using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
 
    int ForWheatAdd=0 ;
    int ForCarrotsAdd=0 ;
    int ForStrawberriesAdd = 0;

   /* int ForWeaponAdd = 0;
    int ForWeaponAdd1 = 0;
    int ForWeaponAdd2 = 0;*/

    #region Functions
    public void ToBuyWheat()
    {
        if(UI_Manager.Instance.scoreIn>=5)
        {
            UI_Manager.Instance.scoreIn -=5;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
            ForWheatAdd += 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(0).gameObject.GetComponent<SelectionFunctionality>().productCount.text=ForWheatAdd.ToString();

        }
        else
        {
            UI_Manager.Instance.notEnoughMoneyText.text = "You didn't have enough money";
            //Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyCarrots()
    {
        if(UI_Manager.Instance.scoreIn>=8)
        {
            UI_Manager.Instance.scoreIn -=8;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
            ForCarrotsAdd += 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(1).gameObject.GetComponent<SelectionFunctionality>().productCount.text = ForCarrotsAdd.ToString();
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
            ForStrawberriesAdd += 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(2).gameObject.GetComponent<SelectionFunctionality>().productCount.text = ForStrawberriesAdd.ToString();
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