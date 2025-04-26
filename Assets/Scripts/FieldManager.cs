using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    internal Dictionary<int, int> fieldSteps = new Dictionary<int, int>();
    internal Dictionary<int, int> OldfieldSteps = new Dictionary<int, int>();
   
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

    private void Start()
    {
        InstantiateFields(UI_Manager.Instance.tomatofieldGo);
    }

    public void EnterField(FieldGrid fGrid)
    {
       UI_Manager.Instance.oldcurrentStep = fieldSteps.ContainsKey(fGrid.fieldID) ? fieldSteps[fGrid.fieldID] : -1;
        var trigger = fGrid.GetComponentInChildren<TriggerZoneCallBacks>();
        if (!fieldSteps.ContainsKey(fGrid.fieldID) || fGrid.IsCoverageComplete())
        {
            CurrentFieldID = fGrid.fieldID;
            if (!fieldSteps.ContainsKey(fGrid.fieldID))
            {
                fieldSteps[fGrid.fieldID] = 0;
            }
            CurrentStepID = fieldSteps[fGrid.fieldID];
            if (CurrentStepID < trigger.actionSequence.Length)
            {
                Debug.Log("index :::  " + CurrentStepID);

                  GameManager.Instance.ShowFieldPopup(trigger.actionSequence[CurrentStepID], fGrid);
            }
        }
        else
        {
        
            GameManager.Instance.ShowFieldPopup(trigger.actionSequence[UI_Manager.Instance.oldcurrentStep], fGrid);
        }
    }

    public void SaveFieldStep(int fieldID, int currentStep)
    {
        fieldSteps[fieldID]= currentStep;
    }

    public void InstantiateFields(GameObject fieldGo)
    {
        GameObject field = Instantiate(fieldGo, this.transform);
        field.transform.SetParent(transform);
    }

 /*   public List<int> GetFieldSteps(int fieldID)
    {
        return fieldSteps.ContainsKey(fieldID) ? fieldSteps[fieldID] : ;
    }*/

}


