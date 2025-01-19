using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    // Dictionary to store the current animation step of each field by field ID
    internal Dictionary<int, List<int>> fieldSteps = new Dictionary<int, List<int>>();
    int _currentFieldID;
    int _currentStepID;

    public int CurrentFieldID
    {
        get => _currentFieldID;
        set => _currentFieldID = value;
    }
    public int CurrentStepID
    {
        get => _currentStepID;
        set => _currentStepID = value;
    }

    /*  public void EnterField(int fieldID)
      {
          UI_Manager.Instance.oldcurrentStep = fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID] : -1;
          if (UI_Manager.Instance.oldcurrentStep == -1 || UI_Manager.Instance.FieldGrid.IsCoverageComplete())
          {
              if (CurrentStepID < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length)
              {
                  CurrentFieldID = fieldID;
                  Debug.Log("index :::  " + CurrentStepID);
                  GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[CurrentStepID]);
                  if (GameManager.Instance.isThroughingseeds)
                  {
                      SelectionFunctionality selectionFunctionality = UI_Manager.Instance.inventoryPanel.transform.GetChild(0).gameObject.GetComponent<SelectionFunctionality>();

                      // Parse the current count from text, decrement it, and update the text field
                      int currentCount = int.Parse(selectionFunctionality.productCount.text);
                      currentCount -= 1;
                      selectionFunctionality.productCount.text = currentCount.ToString();
                  }

              };
          }
          else
          {
              GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.oldcurrentStep]);

          }

          // Retrieve or initialize the animation step for the entered field

          // Set the animation to the correct step
          // PlayAnimationAtStep(fieldID, UI_Manager.Instance.oldcurrentStep);
      }*/

    /* public void EnterField(int fieldID)
     {
         CurrentFieldID = fieldID;

         // Retrieve the current step for this field or initialize to 0 if not visited
         if (!fieldSteps.ContainsKey(fieldID))
         {
             fieldSteps[fieldID] = 0; // Field starts from step 0
         }

         CurrentStepID = fieldSteps[fieldID]; // Set the current step for this field

         // Show the appropriate popup for the current step
         if (CurrentStepID < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length)
         {
             GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[CurrentStepID]);
         }
     }

     public void SaveFieldStep(int fieldID, int currentStep)
     {
         if (fieldSteps.ContainsKey(fieldID))
         {
             fieldSteps[fieldID] = currentStep;
         }
         else
         {
             fieldSteps.Add(fieldID, currentStep);
         }
     }

     public int GetCurrentStep(int fieldID)
     {
         return fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID] : 0;
     }

     public void SetCurrentStep(int fieldID, int step)
     {
         if (fieldSteps.ContainsKey(fieldID))
         {
             fieldSteps[fieldID] = step;
         }
         else
         {
             fieldSteps.Add(fieldID, step);
         }
     }*/


   /* public void EnterField(int fieldID)
    {
        CurrentFieldID = fieldID;

        // Initialize steps for the field if not present
        if (!fieldSteps.ContainsKey(fieldID))
        {
            fieldSteps[fieldID] = new List<int>(); // Initialize with an empty list
        }

        // Retrieve the last step or initialize to 0 if no steps exist
        CurrentStepID = fieldSteps[fieldID].Count > 0 ? fieldSteps[fieldID][fieldSteps[fieldID].Count - 1] : 0;

        // Show the appropriate popup for the current step
        if (CurrentStepID < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length)
        {
            GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[CurrentStepID]);
        }
    }*/

    public void EnterField(int fieldID)
    {
        // Retrieve the last step for the field, or initialize to -1 if it doesn't exist
        UI_Manager.Instance.oldcurrentStep = fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID][fieldSteps[fieldID].Count - 1] : -1;

        // Check if the field is either new or has completed coverage
        if (UI_Manager.Instance.oldcurrentStep == -1 || UI_Manager.Instance.FieldGrid.IsCoverageComplete())
        {
            CurrentFieldID = fieldID;

            // Initialize steps if not already present
            if (!fieldSteps.ContainsKey(fieldID))
            {
                fieldSteps[fieldID] = new List<int> { 0 }; // Start with step 0
            }

            // Retrieve the current step for the field
            CurrentStepID = fieldSteps[fieldID][fieldSteps[fieldID].Count - 1];

            if (CurrentStepID < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length)
            {
                Debug.Log("index :::  " + CurrentStepID);
                GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[CurrentStepID]);

                /*// Handle seed count decrement if the action involves throwing seeds
                if (GameManager.Instance.isThroughingseeds)
                {
                    SelectionFunctionality selectionFunctionality = UI_Manager.Instance.inventoryPanel.transform.GetChild(0)
                        .gameObject.GetComponent<SelectionFunctionality>();

                    int currentCount = int.Parse(selectionFunctionality.productCount.text);
                    currentCount -= 1;
                    selectionFunctionality.productCount.text = currentCount.ToString();
                }*/
            }
        }
        else
        {
            // Show the popup for the last recorded step if the field has been previously entered
            GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.oldcurrentStep]);
        }
    }


    public void SaveFieldStep(int fieldID, int currentStep)
    {
        // Ensure the field exists
        if (!fieldSteps.ContainsKey(fieldID))
        {
            fieldSteps[fieldID] = new List<int>();
        }

        // Add the step to the list if not already present
        if (!fieldSteps[fieldID].Contains(currentStep))
        {
            fieldSteps[fieldID].Add(currentStep);
        }
    }

    public List<int> GetFieldSteps(int fieldID)
    {
        return fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID] : new List<int>();
    }

}


