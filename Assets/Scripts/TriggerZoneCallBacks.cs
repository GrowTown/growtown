using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneCallBacks : MonoBehaviour
{


    public ZoneType zoneType;
    public PlayerAction[] actionSequence;
    internal int currentStep = 0;
    public bool playerInZone = false;
    public Action<TriggerZoneCallBacks> onPlayerEnter;
    public Action<TriggerZoneCallBacks> onPlayerExit;

    private FieldGrid fieldGrid; // Reference to FieldGrid for tile tracking

    private void Start()
    {
        fieldGrid = FindObjectOfType<FieldGrid>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )

        {
            playerInZone = true;
            switch (zoneType)
            {
                case ZoneType.Field:
                    if(currentStep < actionSequence.Length)
                    GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
                    if (UI_Manager.Instance.isPlanted == true) 
                    { 
                       TimerToolTip.ShowTimerStatic(UI_Manager.Instance.plantHolder);
                    }
                    break;
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
            playerInZone = false;

            switch (zoneType)
            {
                case ZoneType.Field:
                    GameManager.Instance.HideFieldPopup();
                    GameManager.Instance.StopCurrentAction();
                    break;
                case ZoneType.Market:
                    onPlayerExit?.Invoke(this);
                    break;
            }
        }
    }
}



public enum PlayerAction
{
    Clean,
    Seed,
    Water,
    Harvest,
}
public enum ZoneType
{
    Market,
    Field,
}


