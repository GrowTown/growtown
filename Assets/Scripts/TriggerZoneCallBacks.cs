using System;
using UnityEngine;

public class TriggerZoneCallBacks : MonoBehaviour
{

    /*// UnityEvents for trigger enter and exit events
    public Action<TriggerZoneCallBacks> onPlayerEnter;
    public Action<TriggerZoneCallBacks> onPlayerExit;

    // Define the player tag to identify the player
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the correct tag
        if (other.CompareTag(playerTag))
        {
            // Invoke all events associated with the onPlayerEnter event
            onPlayerEnter?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that exited the trigger has the correct tag
        if (other.CompareTag(playerTag))
        {
            // Invoke all events associated with the onPlayerExit event
            onPlayerExit?.Invoke(this);
        }
    }*/
    public ZoneType zoneType;
    public PlayerAction[] actionSequence;
    private int currentStep = 0;
    public bool playerInZone = false;
    public Action<TriggerZoneCallBacks> onPlayerEnter;
    public Action<TriggerZoneCallBacks> onPlayerExit;



    #region Functions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentStep <actionSequence.Length - 1)
        {
            switch (zoneType)
            {
                case ZoneType.Market:
                    onPlayerEnter?.Invoke(this);
                    break;

                case ZoneType.Field:
                    playerInZone = true;
                    ShowPopup();
                    break;
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (zoneType)
            {
                case ZoneType.Market:
                    onPlayerExit?.Invoke(this);
                    break;

                case ZoneType.Field:
                    HidePopup();
                    playerInZone = false;
                    break;
            }
        }
    }

    private void Update()
    {
        // Check for player input if the player is in the zone and the current action isn't complete
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            CompleteAction();
        }
    }

    private void ShowPopup()
    {
        // Hide all images first
        foreach (var image in UI_Manager.Instance.PopupImg)
            image.SetActive(false);

        // Show the current image based on the step
        UI_Manager.Instance.PopupImg[currentStep].SetActive(true);
        var select = UI_Manager.Instance.PopupImg[currentStep].GetComponent<SelectionFunctionality>();
        select.onClick += (f) =>
        {

            switch (actionSequence[currentStep])
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
                    CompleteAction();
                    Debug.Log("Water");
                    break;
                case PlayerAction.Harvest:
                    // playerAnimator.SetTrigger("Harvest");
                    UI_Manager.Instance.sickleWeapon.SetActive(true);
                    UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 1);
                    CompleteAction();
                    Debug.Log("Harvest");
                    break;
            }
        };
    }

    private void HidePopup()
    {
        // Hide the current popup image
        if (currentStep <=UI_Manager.Instance.PopupImg.Length - 1)
        {
            UI_Manager.Instance.PopupImg[currentStep].SetActive(false);
        }
        UI_Manager.Instance.sickleWeapon.SetActive(false);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(1, 0);

    }

    public void CompleteAction()
    {
        // Move to the next step if available
        if (currentStep < actionSequence.Length - 1)
        {
            UI_Manager.Instance.PopupImg[currentStep].SetActive(false);
            currentStep++;
            ShowPopup();  // Show the next popup if there's another step
        }
    }

    #endregion
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


