using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    // Dictionary to store the current animation step of each field by field ID
    internal Dictionary<int, int> fieldSteps = new Dictionary<int, int>();
    int _currentFieldID;

    public int CurrentFieldID
    {
        get=> _currentFieldID;
        set => _currentFieldID = value;
    }
    public void EnterField(int fieldID)
    {
          UI_Manager.Instance.oldcurrentStep = fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID] : -1;
        if (UI_Manager.Instance.oldcurrentStep == -1 || UI_Manager.Instance.FieldGrid.IsCoverageComplete())
        {
            if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length)
            {
                CurrentFieldID = fieldID;
                Debug.Log("index :::  " + UI_Manager.Instance.TriggerZoneCallBacks.currentStep);
                GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.TriggerZoneCallBacks.currentStep]);
                if(GameManager.Instance.isThroughingseeds)
                {
                    SelectionFunctionality selectionFunctionality = UI_Manager.Instance.inventoryPanel.transform.GetChild(0).gameObject.GetComponent<SelectionFunctionality>();

                    // Parse the current count from text, decrement it, and update the text field
                    int currentCount = int.Parse(selectionFunctionality.productCount.text);
                    currentCount -= 1;
                    selectionFunctionality.productCount.text = currentCount.ToString();
                }
                UI_Manager.Instance.oldcurrentStep = UI_Manager.Instance.TriggerZoneCallBacks.currentStep;
            };
        }
        else
        {
            GameManager.Instance.ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.oldcurrentStep]);

        }
        if (UI_Manager.Instance.isPlanted == true)
        {
            
        }
        // Retrieve or initialize the animation step for the entered field

        // Set the animation to the correct step
       // PlayAnimationAtStep(fieldID, UI_Manager.Instance.oldcurrentStep);
    }

    public void ExitField(int fieldID)
    {
        GameManager.Instance.HideFieldPopup();
        GameManager.Instance.StopCurrentAction();
    }
/*
    public void UpdateFieldStep(int fieldID, int newStep)
    {
        // Update or initialize the animation step for the given field
        if (fieldSteps.ContainsKey(fieldID))
        {
            fieldSteps[fieldID] = newStep;
        }
        else
        {
            fieldSteps.Add(fieldID, newStep);
        }
    }

    private void PlayAnimationAtStep(int fieldID, int step)
    {
        // Logic to set the field animation to the specified step
        Debug.Log($"Setting animation for field {fieldID} to step {step}");
        // Actual animation logic would go here
    }*/
}


