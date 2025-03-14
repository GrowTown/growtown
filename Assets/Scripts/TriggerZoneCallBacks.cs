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
    public Action<TriggerZoneCallBacks> onPlayerEnter;
    public Action<TriggerZoneCallBacks> onPlayerExit;

    private FieldGrid fieldGrid;
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
                        /*if (fieldID == 2)
                        {
                            other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(1);
                            UI_Manager.Instance.ActiveAllInventory(-232);
                            AudioManager.Instance.hapticFeedbackController.LightFeedback();
                        }
                        else if (fieldID == 1)
                        {
                            other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(3);
                        }
                        else
                        {
                            other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(4);
                           
                        }*/
                        UI_Manager.Instance.ActiveAllInventory(-232);
                        GameManager.Instance.CurrentFieldID = fieldID;
                        UI_Manager.Instance.FieldManager.EnterField(fieldID);
                        UI_Manager.Instance.ToInstantiateLandHealthbar(fieldID);
                        UI_Manager.Instance.ShowLandHealthBar(fieldID);
                        UI_Manager.Instance.ToInstantiateCropTimerbar(fieldID);
                        UI_Manager.Instance.ShowCropTimer(fieldID);
                    }

                    break;
                case ZoneType.Market:
                    onPlayerEnter?.Invoke(this);
                  /* if (!GameManager.Instance.isShowingnewLand)
                    {
                        other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(2);
                        AudioManager.Instance.hapticFeedbackController.LightFeedback();
                    }*/
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

           // other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(0);
            UI_Manager.Instance.ActiveAllInventory(55);
            switch (zoneType)
            {
                case ZoneType.Field:
                    GameManager.Instance.HideFieldPopup();
                    UI_Manager.Instance.HideLandHealthBar();
                    UI_Manager.Instance.HideCropTimerBar();
                    GameManager.Instance.StopCurrentAction();
                    UI_Manager.Instance.FieldManager.SaveFieldStep(fieldID, UI_Manager.Instance.FieldManager.CurrentStepID);
                    UI_Manager.Instance.LandHealthBarImg.SetActive(false);
                 
                    break;

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

    public void CompleteAction()
    {
        GameManager.Instance.HideFieldPopup(); 
        int currentStep = UI_Manager.Instance.FieldManager.CurrentStepID; 

        if (currentStep < actionSequence.Length - 1) 
        {
            GameManager.Instance.isOneWorkingActionCompleted = true; 

            if (currentStep == 1)
            {
                GameManager.Instance.isPlantStartGrowing = true; 
            }

            currentStep++;
            UI_Manager.Instance.FieldManager.SaveFieldStep(fieldID, currentStep); 
            UI_Manager.Instance.FieldManager.CurrentStepID = currentStep; 
            UI_Manager.Instance.oldcurrentStep = currentStep; 

            if (currentStep == 3) // Additional logic for step 3
            {
              
                if (GameManager.Instance.checkPlayerInZone)
                {
                    GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
                }
            }
            else
            {
                GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
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


