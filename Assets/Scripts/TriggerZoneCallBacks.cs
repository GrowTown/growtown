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
                        if (fieldID == 2)
                        {
                            //other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(1);
                        }
                        else
                        {
                            other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(3);
                        }
                        GameManager.Instance.CurrentFieldID = fieldID;
                        UI_Manager.Instance.FieldManager.EnterField(fieldID);
                    }
                    UI_Manager.Instance.LandHealthBarImg.SetActive(true);

                    break;
                case ZoneType.Market:
                    onPlayerEnter?.Invoke(this);
                    if (!GameManager.Instance.isShowingnewLand)
                    {
                        other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(2);
                    }
                    break;
            }
        }
    }

    /* private void OnTriggerExit(Collider other)
     {
         if (other.CompareTag("Player"))
         {
             GameManager.Instance.checkPlayerInZone = false;
             playerInZone = false;
             UI_Manager.Instance.isPlayerInField = false;

             other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(0);
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
                     UI_Manager.Instance.LandHealthBarImg.SetActive(false);

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
 */

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
                    UI_Manager.Instance.FieldManager.SaveFieldStep(fieldID, UI_Manager.Instance.FieldManager.CurrentStepID);
                    UI_Manager.Instance.LandHealthBarImg.SetActive(false);
                 
                    break;

                case ZoneType.Market:
                    other.gameObject.GetComponent<CamerasSwitch>().SwitchToCam(0);
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

    /* public void CompleteAction()
     {
         GameManager.Instance.HideFieldPopup();
         int currentStep = UI_Manager.Instance.FieldManager.GetCurrentStep(fieldID);
         if (currentStep < actionSequence.Length - 1)
         {
             GameManager.Instance.isOneWorkingActionCompleted = true;
             if (currentStep == 1)
             {
                 GameManager.Instance.isPlantStartGrowing = true;
             }
             currentStep++;
             UI_Manager.Instance.FieldManager.SetCurrentStep(fieldID, currentStep);
             UI_Manager.Instance.FieldManager.CurrentStepID = currentStep;
             UI_Manager.Instance.oldcurrentStep = currentStep;
             if (currentStep == 3)
             {
                 if (UI_Manager.Instance.FieldManager.fieldSteps.ContainsKey(UI_Manager.Instance.FieldManager.CurrentFieldID))
                 {
                     UI_Manager.Instance.FieldManager.fieldSteps[UI_Manager.Instance.FieldManager.CurrentFieldID] = UI_Manager.Instance.oldcurrentStep;
                 }

                 if (GameManager.Instance.checkPlayerInZone)
                     GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
             }
             else
             {
                 GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
             }
         }
     }
 */
    /* public void CompleteAction()
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

             currentStep++; // Increment the current step
             UI_Manager.Instance.FieldManager.CurrentStepID = currentStep; // Update FieldManager
             UI_Manager.Instance.FieldManager.SetCurrentStep(fieldID, currentStep); // Save step for this field

             if (currentStep == 3)
             {
                 GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
             }
             else
             {
                 GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]);
             }
         }
     }*/


    public void CompleteAction()
    {
        GameManager.Instance.HideFieldPopup(); // Hide the field popup
        int currentStep = UI_Manager.Instance.FieldManager.CurrentStepID; // Retrieve the current step for the field

        if (currentStep < actionSequence.Length - 1) // Check if more steps are available
        {
            GameManager.Instance.isOneWorkingActionCompleted = true; // Flag to indicate action completion

            if (currentStep == 1)
            {
                GameManager.Instance.isPlantStartGrowing = true; // Trigger plant growth if step 1 is completed
            }

            currentStep++; // Move to the next step
            UI_Manager.Instance.FieldManager.SaveFieldStep(fieldID, currentStep); // Save the ste
            UI_Manager.Instance.FieldManager.CurrentStepID = currentStep; // Update the current step ID in FieldManager
            UI_Manager.Instance.oldcurrentStep = currentStep; // Save the current step to an old step tracker

            if (currentStep == 3) // Additional logic for step 3
            {
                if (UI_Manager.Instance.FieldManager.fieldSteps.ContainsKey(UI_Manager.Instance.FieldManager.CurrentFieldID))
                {
                    UI_Manager.Instance.FieldManager.fieldSteps[UI_Manager.Instance.FieldManager.CurrentFieldID] = new List<int> { UI_Manager.Instance.oldcurrentStep };
                }

                if (GameManager.Instance.checkPlayerInZone)
                {
                    GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]); // Show popup if player is in the zone
                }
            }
            else
            {
                GameManager.Instance.ShowFieldPopup(actionSequence[currentStep]); // Show popup for other steps
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


