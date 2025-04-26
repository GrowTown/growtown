
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneForMarket : MonoBehaviour
{
    public Action<TriggerZoneForMarket> onPlayerEnter;
    public Action<TriggerZoneForMarket> onPlayerExit;
    public ZoneType zoneType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            UI_Manager.Instance.isPlayerInField = true;
            UI_Manager.Instance.WeaponAttackEvent.ToMakeHammerInactive();

            switch (zoneType)
            {

                case ZoneType.Market:
                    onPlayerEnter?.Invoke(this);
                    
                    break;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            UI_Manager.Instance.isPlayerInField = false;
            switch (zoneType)
            {
                case ZoneType.Market:

                    if (UI_Manager.Instance.ShopManager.isCuttingToolBought &&
                          UI_Manager.Instance.ShopManager.isWateringToolBought &&
                          UI_Manager.Instance.ShopManager.isCleningToolBought)
                    {
                        UI_Manager.Instance.starterPackInfoPopUpPanel.SetActive(false);
                    }
                    else
                    {
                        UI_Manager.Instance.starterPackInfoPopUpPanel.SetActive(true);
                    }
                    onPlayerExit?.Invoke(this);
                    break;
            }
        }
    }
}
