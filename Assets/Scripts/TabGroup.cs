using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{

    public List<Tab_Button> tabButtons;
    public Button BuyPanelBT;
    public List<GameObject> objectsToSwap;
    public Tab_Button selectedTab;
    public float selectedScaleForBT = 1.5f; 
    public float normalScale = 1f;
    public float scaleDuration = 0.01f; 

    private void Start()
    {
        if (tabButtons.Count > 0)
        {
            OnTabSelected(tabButtons[0]);
        }

        BuyPanelBT.onClick.AddListener(()=> { SellToBuyPanel(); });
    }

    public void SellToBuyPanel()
    {
        if (objectsToSwap[4].activeSelf)
        {
            selectedTab.ScaleButton(selectedScaleForBT, scaleDuration, BuyPanelBT);
            OnTabSelected(tabButtons[0]);
        }
    }

    public void Subscribe(Tab_Button button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<Tab_Button>();
        }
        tabButtons.Add(button);
    }

    public void OnTabSelected(Tab_Button button)
    {
        if (selectedTab == button) return;
        if (selectedTab != null)
        {
            selectedTab.ScaleButton(normalScale, scaleDuration); 
        }
        selectedTab = button;
        selectedTab.ScaleButton(selectedScaleForBT, scaleDuration); 
        int index =  tabButtons.IndexOf(button);
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                if (i == 4)
                {
                    selectedTab.ScaleButton(normalScale, scaleDuration, BuyPanelBT);
                }
                objectsToSwap[i].SetActive(true);
            }
            else
            {
               
                objectsToSwap[i].SetActive(false);
            }
        }
    }
}
