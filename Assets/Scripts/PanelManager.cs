using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PanelManager
{ 

    private static List<GameObject> panels = new List<GameObject>();

   
    public static  void RegisterPanel(GameObject panel)
    {
        if (!panels.Contains(panel))
        {
            panels.Add(panel);
        }
    }

    public static void HideAllPanels()
    {
        foreach (var panel in panels)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }
    }


}
