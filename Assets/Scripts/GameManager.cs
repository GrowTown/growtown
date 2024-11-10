using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region

    public void StartActionAnimation(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Clean:
                UI_Manager.Instance.cleaningTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 1);
                Debug.Log("Cleanig");
                break;
            case PlayerAction.Seed:
                UI_Manager.Instance.cleaningTool.SetActive(false);
                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 1);
               // Debug.Log("Seeding");
                break;
            case PlayerAction.Water:
                UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed = false;
                UI_Manager.Instance.plantHolder.GetComponent<PlantGrowth>().OnWaterTile();
                UI_Manager.Instance.seedsBag.gameObject.SetActive(false);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                break;
            case PlayerAction.Harvest:
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
                UI_Manager.Instance.sickleWeapon.SetActive(true);
                break;
        }
    }

    public void StopCurrentAnimations()
    {
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 0);
        UI_Manager.Instance.cleaningTool.SetActive(false);

    }
    public void ShowFieldPopup(PlayerAction currentAction)
    {
        UI_Manager.Instance.ShowPopup(currentAction);
    }

    public void HideFieldPopup()
    {
        UI_Manager.Instance.HideFieldPopup();
    }

    internal void StartPlayerAction(PlayerAction action)
    {
        UI_Manager.Instance.TriggerZoneCallBacks.playerInZone = true;
        UI_Manager.Instance.FieldGrid.StartCoverageTracking(action);
    }

    public void StopCurrentAction()
    {
        UI_Manager.Instance.FieldGrid.StopCoverageTracking();
    }

    public void CompleteAction()
    {
        HideFieldPopup();
        if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length - 1)
         {
            UI_Manager.Instance.TriggerZoneCallBacks.currentStep++;
            ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.TriggerZoneCallBacks.currentStep]);
        }
    }

    public void CheckHasCropOrNot()
    {

        int parsedValue;
        if (int.TryParse(UI_Manager.Instance.inventoryPanel.transform.GetChild(0).gameObject.GetComponent<SelectionFunctionality>().productCount.text, out parsedValue))
        {
            if (parsedValue >0)
            {

                // Add your logic here
            }
        }
        else if (int.TryParse(UI_Manager.Instance.inventoryPanel.transform.GetChild(1).gameObject.GetComponent<SelectionFunctionality>().productCount.text, out parsedValue))
        {
            if (parsedValue >0)
            {
                // Add your logic here
            }
        }
        else if (int.TryParse(UI_Manager.Instance.inventoryPanel.transform.GetChild(2).gameObject.GetComponent<SelectionFunctionality>().productCount.text, out parsedValue))
        {
            if (parsedValue >0)
            {
                // Add your logic here
            }
            else
            {
                Debug.Log("You Don't Have Crops");
            }
        }
      
        if (int.TryParse(UI_Manager.Instance.inventoryPanel.transform.GetChild(3).gameObject.GetComponent<SelectionFunctionality>().productCount.text, out parsedValue))
        {
            if (parsedValue >0)
            {
               
            }
            else
            {
                Debug.Log("You Don't Have Cleaning Tool");
            }
        }
        if (int.TryParse(UI_Manager.Instance.inventoryPanel.transform.GetChild(4).gameObject.GetComponent<SelectionFunctionality>().productCount.text, out parsedValue))
        {
            if (parsedValue > 0)
            {
               
            }
            else
            {
                Debug.Log("You Don't Have Watering Tool");
            }
        }
        if (int.TryParse(UI_Manager.Instance.inventoryPanel.transform.GetChild(5).gameObject.GetComponent<SelectionFunctionality>().productCount.text, out parsedValue))
        {
            if (parsedValue >0)
            {
               
            }
            else
            {
                Debug.Log("You Don't Have Cutting Tool");
            }
        }
    }
    #endregion
}
