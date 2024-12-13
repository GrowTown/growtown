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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.checkPlayerInZone = true;
            playerInZone = true;
            UI_Manager.Instance.isPlayerInField = true;
            UI_Manager.Instance.WeaponAttackEvent.ToMakeHammerInactive();
            switch (zoneType)
            {
                case ZoneType.Field:
                    if (!UI_Manager.Instance.starterPackInfoPopUpPanel.activeSelf)
                    {
                        UI_Manager.Instance.FieldManager.EnterField(fieldID);
                        GameManager.Instance.CurrentFieldID = fieldID;

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
            GameManager.Instance.checkPlayerInZone = false;
            playerInZone = false;
            UI_Manager.Instance.isPlayerInField = false;

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
                    if (UI_Manager.Instance.ShopManager.isCuttingToolBought && UI_Manager.Instance.ShopManager.isWateringToolBought && UI_Manager.Instance.ShopManager.isCleningToolBought)
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


