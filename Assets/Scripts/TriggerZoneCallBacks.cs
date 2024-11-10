using System;
using UnityEngine;

public class TriggerZoneCallBacks : MonoBehaviour
{

    public int fieldID;
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
    //int oldcurrentStep = -1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            switch (zoneType)
            {
                case ZoneType.Field:
                      UI_Manager.Instance.FieldManager.EnterField(fieldID);
                    break;
                case ZoneType.Market:
                    UI_Manager.Instance.starterPackInfoPopUpPanel.SetActive(false);
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
                    if (UI_Manager.Instance.FieldManager.fieldSteps.ContainsKey(fieldID))
                    {
                        UI_Manager.Instance.FieldManager.fieldSteps[fieldID] = UI_Manager.Instance.oldcurrentStep;
                    }
                    else
                    {
                        UI_Manager.Instance.FieldManager.fieldSteps.Add(fieldID, UI_Manager.Instance.oldcurrentStep);
                    }
                    
                   // UI_Manager.Instance.oldcurrentStep = currentStep;
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


