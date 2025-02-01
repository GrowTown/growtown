using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{

    public List<Tab_Button> tabButtons;
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
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }
}
