using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
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

    public void EnterField(int fieldID)
    {
        UI_Manager.Instance.oldcurrentStep = fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID][fieldSteps[fieldID].Count - 1] : -1;
        if (UI_Manager.Instance.oldcurrentStep == -1 || UI_Manager.Instance.FieldGrid.IsCoverageComplete())
        {
            CurrentFieldID = fieldID;
            if (!fieldSteps.ContainsKey(fieldID))
            {
                fieldSteps[fieldID] = new List<int> { 0 };
            }
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


