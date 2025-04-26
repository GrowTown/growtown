
using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZoneCallBacks : MonoBehaviour
{

    public int fieldID;
    public ZoneType zoneType;
    public PlayerAction[] actionSequence;
    int currentStep = 0;
    public bool playerInZone = false;


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
                      var fieldGrid = this.gameObject.GetComponentInParent<FieldGrid>();
                          fieldGrid.checkPlayerInZone = true;
                        UI_Manager.Instance.InstantiateFieldPopUp();
                        UI_Manager.Instance.ActiveAllInventory(-232);
                        GameManager.Instance.CurrentFieldID = fieldID;
                        UI_Manager.Instance.FieldManager.EnterField(fieldGrid);
                        UI_Manager.Instance.fieldCropTimer.text = "00:00";
                        UI_Manager.Instance.fieldCropTimerIcon.sprite = fieldGrid.fieldPlantUIAnimation;
                        fieldGrid.updateValue = true;
                        fieldGrid.ShowCropRemainingTimer();
                        fieldGrid.ShowCropRemainingTimerPanel();
                        UI_Manager.Instance.ToInstantiateLandHealthbar(fieldID, this.gameObject.GetComponentInParent<FieldGrid>());
                        UI_Manager.Instance.ShowLandHealthBar(fieldID);
                        UI_Manager.Instance.ToInstantiateCropTimerbar(fieldID);
                        UI_Manager.Instance.ShowCropTimer(fieldID);
                    }

                    break;
            }
        }
    }

 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var fieldGrid = this.gameObject.GetComponentInParent<FieldGrid>();
            fieldGrid.checkPlayerInZone = false;
            GameManager.Instance.checkPlayerInZone = false;
            playerInZone = false;
            UI_Manager.Instance.isPlayerInField = false;

           // other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(0);
            UI_Manager.Instance.ActiveAllInventory(55);
            switch (zoneType)
            {
                case ZoneType.Field:
                    GameManager.Instance.HideFieldPopup();
                    UI_Manager.Instance.HideLandHealthBar();
                    UI_Manager.Instance.HideCropTimerBar();
                    GameManager.Instance.StopCurrentAction(fieldGrid);
                    fieldGrid.updateValue = false;
                    fieldGrid.HideShowCropRemainingTimer();
                    UI_Manager.Instance.LandHealthBarImg.SetActive(false);
                 
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


