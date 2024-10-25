using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
 
    // public static ShopManager Instance { get; private set; }

    /* private void Awake()
     {
         if (Instance != null && Instance != this)
         {
             Destroy(gameObject);
         }
         else
         {
             Instance = this;
             DontDestroyOnLoad(gameObject); // If you want the GameManager to persist between scenes
         }
     }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    int ForWheatAdd=0 ;
    int ForCarrotsAdd=0 ;
    int ForStrawberriesAdd = 0;
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
            Debug.Log("You didn't have enough money");
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
            Debug.Log("You didn't have enough money");
        }
    }
    public void ToBuyStrawberries()
    {
        if(UI_Manager.Instance.scoreIn>=12)
        {
            UI_Manager.Instance.scoreIn -=12;
            UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
            ForStrawberriesAdd += 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(2).gameObject.GetComponent<SelectionFunctionality>().productCount.text = ForStrawberriesAdd.ToString();
        }
        else
        {
            Debug.Log("You didn't have enough money");
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