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
    #endregion
    /// <summary>
    /// show popups in field area
    /// </summary>
    /*internal void ShowFieldPopup()
    {
        // Hide all images first
        foreach (var image in UI_Manager.Instance.PopupImg)
            image.SetActive(false);

        // Show the current image based on the step
        UI_Manager.Instance.PopupImg[UI_Manager.Instance.TriggerZoneCallBacks.currentStep].SetActive(true);
        var select = UI_Manager.Instance.PopupImg[UI_Manager.Instance.TriggerZoneCallBacks.currentStep].GetComponent<SelectionFunctionality>();
        select.onClick = null;
        select.onClick += (f) =>
        {
            switch (UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.TriggerZoneCallBacks.currentStep])
            {
                case PlayerAction.Clean:
                    //playerAnimator.SetTrigger("Run");
                    CompleteAction();
                    Debug.Log("Cleaning");
                    break;
                case PlayerAction.Seed:
                    // playerAnimator.SetTrigger("ThrowSeed");
                    CompleteAction();
                    Debug.Log("Seeding");
                    break;
                case PlayerAction.Water:
                    //playerAnimator.SetTrigger("Water");
                    UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 1);
                    CompleteAction();
                    Debug.Log("Water");
                    break;
                case PlayerAction.Harvest:
                    // playerAnimator.SetTrigger("Harvest");
                    UI_Manager.Instance.sickleWeapon.SetActive(true);
                    UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 1);
                    CompleteAction();
                    Debug.Log("Harvest");
                    break;
            }
        };
    }*/

    /*   /// <summary>
       /// field area action
       /// </summary>
       internal void CompleteAction()
       {
           // Move to the next step if available
           if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length - 1)
           {
               UI_Manager.Instance.PopupImg[UI_Manager.Instance.TriggerZoneCallBacks.currentStep].SetActive(false);
               UI_Manager.Instance.TriggerZoneCallBacks.currentStep++;
               ShowFieldPopup();  // Show the next popup if there's another step
           }
       }*/
}
